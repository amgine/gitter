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

namespace gitter.GitLab.Api
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Threading.Tasks;

	using Newtonsoft.Json;

	using gitter.Framework;
	using System.Text;
	using System.Globalization;

	class ApiEndpoint
	{
		private static string GetNextPageUrl(HttpResponseHeaders headers)
		{
			Assert.IsNotNull(headers);

			if(headers.TryGetValues("Link", out var links))
			{
				foreach(var link in links)
				{
					if(string.IsNullOrWhiteSpace(link)) continue;
					foreach(var part in link.Split(','))
					{
						if(part.EndsWith("rel=\"next\""))
						{
							var s = part.IndexOf('<');
							var e = part.IndexOf('>');
							if(s >= 0 && e > s)
							{
								return part.Substring(s + 1, e - s - 1);
							}
						}
					}
				}
			}
			return default;
		}

		private static async Task<T> ReadReponseAsync<T>(HttpResponseMessage response)
		{
			Assert.IsNotNull(response);

			using var stream     = await response.Content.ReadAsStreamAsync().ConfigureAwait(continueOnCapturedContext: false);

			using var ms = new MemoryStream();
			await stream.CopyToAsync(ms);
			ms.Seek(0, SeekOrigin.Begin);

			using var textReader = new StreamReader(ms, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024*4, leaveOpen: true);

			var resp = await textReader.ReadToEndAsync();
			ms.Seek(0, SeekOrigin.Begin);

			var jsonReader = new JsonTextReader(textReader);
			var serializer = JsonSerializer.CreateDefault();

			return serializer.Deserialize<T>(jsonReader);
		}

		static async Task<IReadOnlyList<T>> ReadPagedResultAsync<T>(HttpClient http, string url)
		{
			Assert.IsNotNull(http);
			Assert.IsNeitherNullNorWhitespace(url);

			var result = default(List<T>);
			while(true)
			{
				using var response = await http
					.GetAsync(url)
					.ConfigureAwait(continueOnCapturedContext: false);

				response.EnsureSuccessStatusCode();

				var page = await ReadReponseAsync<T[]>(response)
					.ConfigureAwait(continueOnCapturedContext: false);

				if(page == null || page.Length == 0) break;

				url = GetNextPageUrl(response.Headers);
				if(url == null)
				{
					if(result == null) return page;
				}

				result ??= new List<T>();
				result.AddRange(page);

				if(url == null) break;
			}
			return result ?? (IReadOnlyList<T>)Preallocated<T>.EmptyArray;
		}

		static async Task<T> ReadResultAsync<T>(HttpClient http, string url)
		{
			Assert.IsNotNull(http);
			Assert.IsNeitherNullNorWhitespace(url);

			using var response = await http
				.GetAsync(url)
				.ConfigureAwait(continueOnCapturedContext: false);

			if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				throw new UnauthorizedAccessException();
			}
			response.EnsureSuccessStatusCode();

			return await ReadReponseAsync<T>(response)
				.ConfigureAwait(continueOnCapturedContext: false);
		}

		public ApiEndpoint(Uri serviceUri, string accessToken)
		{
			Verify.Argument.IsNotNull(serviceUri, nameof(serviceUri));

			ServiceUri  = serviceUri;
			AccessToken = accessToken;
		}

		private Uri ServiceUri { get; }

		private string AccessToken { get; }

		private HttpClient CreateHttpClient()
		{
			var http = new HttpClient { BaseAddress = ServiceUri };

			if(!string.IsNullOrEmpty(AccessToken))
			{
				http.DefaultRequestHeaders.Add("PRIVATE-TOKEN", AccessToken);
			}

			return http;
		}

		public async Task<GitLabVersion> GetVersionAsync()
		{
			using var http = CreateHttpClient();
			return await ReadResultAsync<GitLabVersion>(http, "/api/v4/version")
				.ConfigureAwait(continueOnCapturedContext: false);
		}

		public async Task<IReadOnlyList<Project>> GetProjectsAsync()
		{
			using var http = CreateHttpClient();

			var urlBuilder = new StringBuilder();
			urlBuilder.Append("/api/v4/projects/");

			return await ReadPagedResultAsync<Project>(http, urlBuilder.ToString())
				.ConfigureAwait(continueOnCapturedContext: false);
		}

		public async Task<IReadOnlyList<Issue>> GetProjectIssuesAsync(string projectId, IssueState state = IssueState.All)
		{
			using var http = CreateHttpClient();

			var urlBuilder = new StringBuilder();
			urlBuilder.Append("/api/v4/projects/");
			urlBuilder.Append(Uri.EscapeDataString(projectId));
			urlBuilder.Append("/issues");

			if(state != IssueState.All)
			{
				urlBuilder.Append('?');
				switch(state)
				{
					case IssueState.Closed:
						urlBuilder.Append("state=closed");
						break;
					case IssueState.Opened:
						urlBuilder.Append("state=opened");
						break;
				}
			}

			return await ReadPagedResultAsync<Issue>(http, urlBuilder.ToString())
				.ConfigureAwait(continueOnCapturedContext: false);
		}

		public async Task<IReadOnlyList<Pipeline>> GetPipelinesAsync(string path, string sha = default)
		{
			using var http = CreateHttpClient();

			var urlBuilder = new StringBuilder();
			urlBuilder.Append("/api/v4/projects/");
			urlBuilder.Append(Uri.EscapeDataString(path));
			urlBuilder.Append("/pipelines");

			if(sha != null)
			{
				urlBuilder.Append("?sha=");
				urlBuilder.Append(sha);
			}

			return await ReadPagedResultAsync<Pipeline>(http, urlBuilder.ToString())
				.ConfigureAwait(continueOnCapturedContext: false);
		}

		public async Task<IReadOnlyList<Job>> GetJobsAsync(string path, long pipelineId)
		{
			using var http = CreateHttpClient();

			var urlBuilder = new StringBuilder();
			urlBuilder.Append("/api/v4/projects/");
			urlBuilder.Append(Uri.EscapeDataString(path));
			urlBuilder.Append("/pipelines/");
			urlBuilder.Append(pipelineId.ToString(CultureInfo.InvariantCulture));
			urlBuilder.Append("/jobs");

			return await ReadPagedResultAsync<Job>(http, urlBuilder.ToString())
				.ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
