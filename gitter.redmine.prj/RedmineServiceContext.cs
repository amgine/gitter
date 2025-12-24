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

#nullable enable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Http;

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

	public RedmineServiceContext(HttpMessageInvoker httpMessageInvoker, Uri serviceUri, string apiKey)
	{
		Verify.Argument.IsNotNull(httpMessageInvoker);

		HttpMessageInvoker = httpMessageInvoker;
		ServiceUri         = serviceUri;
		_apiKey            = apiKey;

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
	}

	private HttpMessageInvoker HttpMessageInvoker { get; }

	public Uri ServiceUri { get; }

	public string? DefaultProjectId { get; set; }

	public LockType SyncRoot { get; } = new();

	internal async Task<XmlDocument?> GetXmlAsync(string url, CancellationToken cancellationToken = default)
	{
		using var request = new HttpRequestMessage(HttpMethod.Get, ServiceUri + url);
		request.Headers.Add(apiKeyHeader, _apiKey);

		try
		{
			using var response = await HttpMessageInvoker
				.SendAsync(request, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);

			response.EnsureSuccessStatusCode();

			using var stream = await response.Content
				.ReadAsStreamAsync(cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);

			var xmldoc = new XmlDocument();
			xmldoc.Load(stream);
			return xmldoc;
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			return null;
		}
	}

	private async Task SendDataAsync(string relativeUrl, HttpMethod httpMethod, Action<Stream> send, CancellationToken cancellationToken = default)
	{
		using var ms = new MemoryStream();
		send(ms);
		ms.Seek(0, SeekOrigin.Begin);

		using var request = new HttpRequestMessage(httpMethod, ServiceUri + relativeUrl);
		request.Headers.Add(apiKeyHeader, _apiKey);
		request.Content = new StreamContent(ms);

		using var response = await HttpMessageInvoker
			.SendAsync(request, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);

		response.EnsureSuccessStatusCode();
	}

	internal Task PostXmlAsync(string url, XmlDocument doc)
		=> SendDataAsync(url, HttpMethod.Post, doc.Save);

	internal Task PutXmlAsync(string url, XmlDocument doc)
		=> SendDataAsync(url, HttpMethod.Put, doc.Save);

	internal async Task GetAllDataPagesAsync(string url, Action<XmlDocument> processing, CancellationToken cancellationToken = default)
	{
		const int maxlimit = 100;

		string suff = "?limit=" + maxlimit;
		while(true)
		{
			var xml = await GetXmlAsync(url + suff, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
			if(xml is null) break;

			var e = xml.DocumentElement;
			if(e is null) break;

			int total_count = 0;
			int offset = 0;
			int count = e.ChildNodes.Count;
			var totalCountAttr = e.Attributes["total_count"];
			if(totalCountAttr != null)
			{
				total_count = int.Parse(totalCountAttr.Value);
			}
			var offsetAttr = e.Attributes["offset"];
			if(offsetAttr is not null)
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
