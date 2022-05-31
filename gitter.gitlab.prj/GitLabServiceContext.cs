#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2020  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using gitter.GitLab.Api;

class GitLabServiceContext
{
	private readonly ApiEndpoint _api;

	public GitLabServiceContext(HttpMessageInvoker httpMessageInvoker, Uri serviceUri, string apiKey)
	{
		Verify.Argument.IsNotNull(httpMessageInvoker);
		Verify.Argument.IsNotNull(serviceUri);
		Verify.Argument.IsNeitherNullNorWhitespace(apiKey);

		ServiceUri = serviceUri;

		_api = new Api.ApiEndpoint(httpMessageInvoker, serviceUri, apiKey);
	}

	public Task<GitLabVersion> GetVersionAsync()
		=> _api.GetVersionAsync();

	public Task<IReadOnlyList<Pipeline>> GetPipelinesAsync(
		string          sha           = default,
		string          reference     = default,
		PipelineScope?  scope         = default,
		PipelineStatus? status        = default,
		PipelineSource? source        = default,
		string          username      = default,
		DateTimeOffset? updatedBefore = default,
		DateTimeOffset? updatedAfter  = default,
		PipelineOrder?  orderBy       = default,
		SortOrder?      sort          = default,
		CancellationToken cancellationToken = default)
		=> _api.GetPipelinesAsync(
			DefaultProjectId,
			sha, reference, scope, status, source, username, updatedBefore, updatedAfter, orderBy, sort,
			cancellationToken: cancellationToken);

	public Task DeletePipelineAsync(long pipelineId)
		=> _api.DeletePipelineAsync(DefaultProjectId, pipelineId);

	public Task<TestReport> GetTestReportAsync(long pipelineId, CancellationToken cancellationToken = default)
		=> _api.GetTestReportAsync(DefaultProjectId, pipelineId, cancellationToken);

	public Task<TestReportSummary> GetTestReportSummaryAsync(long pipelineId, CancellationToken cancellationToken = default)
		=> _api.GetTestReportSummaryAsync(DefaultProjectId, pipelineId, cancellationToken);

	public Task<IReadOnlyList<Issue>> GetIssuesAsync(
		IssueState? state = default,
		CancellationToken cancellationToken = default)
		=> _api.GetProjectIssuesAsync(DefaultProjectId, state,
			cancellationToken: cancellationToken);

	public Task<IReadOnlyList<Project>> GetProjectsAsync()
		=> _api.GetProjectsAsync();

	public string FormatProjectUrl()
		=> ServiceUri + $@"{DefaultProjectId}";

	public string FormatCommitUrl(string sha)
		=> ServiceUri + $@"{DefaultProjectId}/-/commit/{sha}";

	public string FormatIssueUrl(int id)
		=> ServiceUri + $@"{DefaultProjectId}/-/issues/{id}";

	public Uri ServiceUri { get; }

	public NameOrNumericId DefaultProjectId { get; set; }
}
