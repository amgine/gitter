#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Redmine;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Net;

public sealed class RedmineServiceContext
{
	#region Data

	private readonly string _apiKey;

	#endregion

	#region Constants

	const string apiKeyHeader = "X-Redmine-API-Key";

	const string projectsQuery = "projects";
	const string usersQuery = "users";
	const string issuesQuery = "issues";

	#endregion

	public RedmineServiceContext(Uri serviceUri, string apiKey)
	{
		ServiceUri		= serviceUri;
		_apiKey			= apiKey;

		News            = new NewsCollection(this);
		Projects        = new ProjectsCollection(this);
		Issues          = new IssuesCollection(this);
		Users           = new UsersCollection(this);
		UserRoles       = new UserRolesCollection(this);
		IssueRelations  = new IssueRelationsCollection(this);
		Trackers        = new IssueTrackersCollection(this);
		IssueStatuses   = new IssueStatusesCollection(this);
		IssuePriorities = new IssuePrioritiesCollection(this);
		IssueCategories = new IssueCategoriesCollection(this);
		ProjectVersions = new ProjectVersionsCollection(this);
		Attachments     = new AttachmentsCollection(this);
		CustomFields    = new CustomFieldsCollection(this);
		Queries         = new QueriesCollection(this);

		SyncRoot        = new object();
	}

	public Uri ServiceUri { get; }

	public string DefaultProjectId { get; set; }

	public object SyncRoot { get; }

	internal XmlDocument GetXml(string url)
	{
		var request = WebRequest.Create(ServiceUri + url);
		request.Headers.Add(apiKeyHeader, _apiKey);
		request.Timeout = 10000;
		string xml = string.Empty;
		try
		{
			using(var response = request.GetResponse())
			using(var stream = response.GetResponseStream())
			using(var reader = new StreamReader(stream))
			{
				xml = reader.ReadToEnd();
			}
		}
		catch(WebException)
		{
			return null;
		}
		var xmldoc = new XmlDocument();
		xmldoc.LoadXml(xml);
		return xmldoc;
	}

	private void SendData(string relativeUrl, string httpMethod, Action<Stream> send)
	{
		var request = WebRequest.Create(ServiceUri + relativeUrl);
		request.Headers.Add(apiKeyHeader, _apiKey);
		request.Method = httpMethod;
		using(var stream = request.GetRequestStream())
		{
			send(stream);
		}
	}

	internal void PostXml(string url, XmlDocument doc)
	{
		SendData(url, "POST", doc.Save);
	}

	internal void PutXml(string url, XmlDocument doc)
	{
		SendData(url, "PUT", doc.Save);
	}

	internal void GetAllDataPages(string url, Action<XmlDocument> processing)
	{
		const int maxlimit = 100;

		string suff = "?limit=" + maxlimit;
		while(true)
		{
			var xml = GetXml(url + suff);
			if(xml != null)
			{
				var e = xml.DocumentElement;
				if(e != null)
				{
					int total_count = 0;
					int offset = 0;
					int count = e.ChildNodes.Count;
					var totalCountAttr = e.Attributes["total_count"];
					if(totalCountAttr != null)
					{
						total_count = int.Parse(totalCountAttr.Value);
					}
					var offsetAttr = e.Attributes["offset"];
					if(offsetAttr != null)
					{
						offset = int.Parse(offsetAttr.Value);
					}
					if(count != 0)
					{
						processing(xml);
					}
					if(offset + count < total_count)
					{
						suff = "?limit=" + maxlimit + "&offset=" + (offset + count);
					}
					else
					{
						break;
					}
				}
				else
				{
					break;
				}
			}
			else
			{
				break;
			}
		}
	}

	internal Task GetAllDataPagesAsync(string url, Action<XmlDocument> processing, CancellationToken cancellationToken)
	{
		return Task.Factory.StartNew(
			() => GetAllDataPages(url, processing),
			cancellationToken,
			TaskCreationOptions.None,
			TaskScheduler.Default);
	}

	public NewsCollection News { get; }

	public ProjectsCollection Projects { get; }

	public ProjectVersionsCollection ProjectVersions { get; }

	public UsersCollection Users { get; }

	public UserRolesCollection UserRoles { get; }

	public IssuesCollection Issues { get; }

	public AttachmentsCollection Attachments { get; }

	public IssueRelationsCollection IssueRelations { get; }

	public IssueTrackersCollection Trackers { get; }

	public IssueStatusesCollection IssueStatuses { get; }

	public IssueCategoriesCollection IssueCategories { get; }

	public IssuePrioritiesCollection IssuePriorities { get; }

	public CustomFieldsCollection CustomFields { get; }

	public QueriesCollection Queries { get; }
}
