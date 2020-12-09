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

namespace gitter.GitLab
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net.Http;
	using System.Threading.Tasks;

	using gitter.GitLab.Api;

	class GitLabServiceContext
	{
		private readonly Api.ApiEndpoint _api;

		public GitLabServiceContext(Uri serviceUri, string apiKey)
		{
			Verify.Argument.IsNotNull(serviceUri, nameof(serviceUri));
			Verify.Argument.IsNeitherNullNorWhitespace(apiKey, nameof(apiKey));

			ServiceUri = serviceUri;
			ApiKey     = apiKey;

			_api = new Api.ApiEndpoint(serviceUri, apiKey);
		}

		public Task<Api.GitLabVersion> GetVersionAsync()
			=> _api.GetVersionAsync();

		public Task<IReadOnlyList<Api.Pipeline>> GetPipelinesAsync(string sha = default)
			=> _api.GetPipelinesAsync(DefaultProjectId, sha);

		public Task<IReadOnlyList<Api.Issue>> GetIssuesAsync(IssueState state = IssueState.All)
			=> _api.GetProjectIssuesAsync(DefaultProjectId, state);

		public Task<IReadOnlyList<Api.Project>> GetProjectsAsync()
			=> _api.GetProjectsAsync();

		public string FormatProjectUrl()
			=> ServiceUri + $@"{DefaultProjectId}";

		public string FormatCommitUrl(string sha)
			=> ServiceUri + $@"{DefaultProjectId}/-/commit/{sha}";

		public string FormatIssueUrl(int id)
			=> ServiceUri + $@"{DefaultProjectId}/-/issues/{id}";

		public Uri ServiceUri { get; }

		private string ApiKey { get; }

		public string DefaultProjectId { get; set; }
	}
}
