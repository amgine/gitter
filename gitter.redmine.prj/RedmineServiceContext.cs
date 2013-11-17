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

namespace gitter.Redmine
{
	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Xml;
	using System.Net;

	public sealed class RedmineServiceContext
	{
		#region Data

		private readonly Uri _serviceUri;
		private readonly string _apiKey;

		private readonly NewsCollection _news;
		private readonly ProjectsCollection _projects;
		private readonly IssuesCollection _issues;
		private readonly UsersCollection _users;
		private readonly UserRolesCollection _userRoles;
		private readonly IssueRelationsCollection _relations;
		private readonly IssueTrackersCollection _trackers;
		private readonly IssueStatusesCollection _statuses;
		private readonly IssuePrioritiesCollection _priorities;
		private readonly IssueCategoriesCollection _categories;
		private readonly ProjectVersionsCollection _versions;
		private readonly AttachmentsCollection _attachments;
		private readonly CustomFieldsCollection _customFields;
		private readonly QueriesCollection _queries;

		private readonly object _syncRoot;

		#endregion

		#region Constants

		const string apiKeyHeader = "X-Redmine-API-Key";

		const string projectsQuery = "projects";
		const string usersQuery = "users";
		const string issuesQuery = "issues";

		#endregion

		public RedmineServiceContext(Uri serviceUri, string apiKey)
		{
			_serviceUri		= serviceUri;
			_apiKey			= apiKey;

			_news			= new NewsCollection(this);
			_projects		= new ProjectsCollection(this);
			_issues			= new IssuesCollection(this);
			_users			= new UsersCollection(this);
			_userRoles		= new UserRolesCollection(this);
			_relations		= new IssueRelationsCollection(this);
			_trackers		= new IssueTrackersCollection(this);
			_statuses		= new IssueStatusesCollection(this);
			_priorities		= new IssuePrioritiesCollection(this);
			_categories		= new IssueCategoriesCollection(this);
			_versions		= new ProjectVersionsCollection(this);
			_attachments	= new AttachmentsCollection(this);
			_customFields	= new CustomFieldsCollection(this);
			_queries		= new QueriesCollection(this);

			_syncRoot		= new object();
		}

		public Uri ServiceUri
		{
			get { return _serviceUri; }
		}

		public string DefaultProjectId
		{
			get;
			set;
		}

		public object SyncRoot
		{
			get { return _syncRoot; }
		}

		internal XmlDocument GetXml(string url)
		{
			var request = WebRequest.Create(_serviceUri + url);
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
			var request = WebRequest.Create(_serviceUri + relativeUrl);
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

		public NewsCollection News
		{
			get { return _news; }
		}

		public ProjectsCollection Projects
		{
			get { return _projects; }
		}

		public ProjectVersionsCollection ProjectVersions
		{
			get { return _versions; }
		}

		public UsersCollection Users
		{
			get { return _users; }
		}

		public UserRolesCollection UserRoles
		{
			get { return _userRoles; }
		}

		public IssuesCollection Issues
		{
			get { return _issues; }
		}

		public AttachmentsCollection Attachments
		{
			get { return _attachments; }
		}

		public IssueRelationsCollection IssueRelations
		{
			get { return _relations; }
		}

		public IssueTrackersCollection Trackers
		{
			get { return _trackers; }
		}

		public IssueStatusesCollection IssueStatuses
		{
			get { return _statuses; }
		}

		public IssueCategoriesCollection IssueCategories
		{
			get { return _categories; }
		}

		public IssuePrioritiesCollection IssuePriorities
		{
			get { return _priorities; }
		}

		public CustomFieldsCollection CustomFields
		{
			get { return _customFields; }
		}

		public QueriesCollection Queries
		{
			get { return _queries; }
		}
	}
}
