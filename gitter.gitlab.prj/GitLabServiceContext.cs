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

	using Newtonsoft.Json;

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

		public string FormatProjectUrl()
			=> ServiceUri + $@"{DefaultProjectId}";

		public string FormatCommitUrl(string sha)
			=> ServiceUri + $@"{DefaultProjectId}/-/commit/{sha}";

		public Uri ServiceUri { get; }

		private string ApiKey { get; }

		public string DefaultProjectId { get; set; }

		public async Task<IReadOnlyList<Api.Project>> GetProjects()
		{
			using var http = new HttpClient { BaseAddress = ServiceUri };

			http.DefaultRequestHeaders.Add("PRIVATE-TOKEN", ApiKey);

			var result = new List<Api.Project>();

			var url = "/api/v4/projects?";//?pagination=keyset&per_page=100";

			while(true)
			{
				using var response   = await http.GetAsync(url).ConfigureAwait(continueOnCapturedContext: false);
				using var stream     = await response.Content.ReadAsStreamAsync().ConfigureAwait(continueOnCapturedContext: false);
				using var textReader = new StreamReader(stream, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false);

				if(response.StatusCode != System.Net.HttpStatusCode.OK)
				{
					var error = textReader.ReadToEnd();
				}

				url = default;
				if(response.Headers.TryGetValues("Link", out var links))
				{
					var link = links.FirstOrDefault();
					if(link != null)
					{
						var parts = link.Split(',');
						foreach(var part in parts)
						{
							if(part.EndsWith("rel=\"next\""))
							{
								var s = part.IndexOf('<');
								var e = part.IndexOf('>');
								if(s >= 0 && e > s)
								{
									url = part.Substring(s + 1, e - s - 1);
								}
							}
						}
					}

				}

				var jsonReader = new JsonTextReader(textReader);
				var serializer = JsonSerializer.CreateDefault();
				var projects   = serializer.Deserialize<Api.Project[]>(jsonReader);

				if(projects == null || projects.Length == 0) break;

				result.AddRange(projects);

				if(url == null) break;
			}

			return result;
		}
	}
}
