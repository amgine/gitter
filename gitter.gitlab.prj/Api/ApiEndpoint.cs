#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Api;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if SYSTEM_TEXT_JSON
using System.Text.Json;
#elif NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

using gitter.Framework;

partial class ApiEndpoint
{
	private static string? GetNextPageUrl(HttpResponseHeaders headers)
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

	private static async Task<T?> ReadReponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
	{
		Assert.IsNotNull(response);

		using var stream = await response.Content
			.ReadAsStreamAsync(cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);

#if SYSTEM_TEXT_JSON
		return await JsonSerializer
			.DeserializeAsync<T>(stream, cancellationToken: cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
#elif NEWTONSOFT_JSON
		using var textReader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024*4, leaveOpen: true);
		var jsonReader = new JsonTextReader(textReader);
		var serializer = JsonSerializer.CreateDefault();
		return serializer.Deserialize<T>(jsonReader);
#else
#error Undefined serializer library
#endif
	}

	private async Task<IReadOnlyList<T>> ReadPagedResultAsync<T>(string url, CancellationToken cancellationToken = default)
	{
		Assert.IsNeitherNullNorWhitespace(url);

		var result = default(List<T>);
		string? next = url;
		while(true)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using var message = CreateRequest(HttpMethod.Get, next);

			using var response = await HttpMessageInvoker
				.SendAsync(message, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);

			response.EnsureSuccessStatusCode();

			var page = await ReadReponseAsync<T[]>(response, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);

			if(page is not { Length: not 0 }) break;

			next = GetNextPageUrl(response.Headers);
			if(next is null)
			{
				if(result is null) return page;
			}

			result ??= [];
			result.AddRange(page);

			if(next is null) break;
		}
		return result ?? (IReadOnlyList<T>)Preallocated<T>.EmptyArray;
	}

	private static void CheckResponse(HttpResponseMessage response)
	{
		if(response.IsSuccessStatusCode) return;

		if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
		{
			throw new UnauthorizedAccessException();
		}
		response.EnsureSuccessStatusCode();
	}

	private async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
	{
		Assert.IsNeitherNullNorWhitespace(url);

		using var message = CreateRequest(HttpMethod.Get, url);

		using var response = await HttpMessageInvoker
			.SendAsync(message, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);

		CheckResponse(response);

		return await ReadReponseAsync<T>(response, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
	}

	private async Task<T?> PostAsync<T>(string url, CancellationToken cancellationToken = default)
	{
		Assert.IsNeitherNullNorWhitespace(url);

		using var message = CreateRequest(HttpMethod.Post, url);

		using var response = await HttpMessageInvoker
			.SendAsync(message, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);

		CheckResponse(response);

		return await ReadReponseAsync<T>(response, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
	}

	private async Task DeleteAsync(string url, CancellationToken cancellationToken = default)
	{
		using var request = CreateRequest(HttpMethod.Delete, url);
		using var response = await HttpMessageInvoker
			.SendAsync(request, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		response.EnsureSuccessStatusCode();
	}

	public ApiEndpoint(HttpMessageInvoker httpMessageInvoker, ServerInfo server)
	{
		Verify.Argument.IsNotNull(httpMessageInvoker);
		Verify.Argument.IsNotNull(server);

		HttpMessageInvoker = httpMessageInvoker;
		Server             = server;
	}

	private HttpMessageInvoker HttpMessageInvoker { get; }

	private ServerInfo Server { get; }

	private Uri ServiceUri => Server.ServiceUri;

	private string AccessToken => Server.ApiKey;

	private HttpRequestMessage CreateRequest(HttpMethod method, string relativeUri)
	{
		var message = new HttpRequestMessage(method, new Uri(ServiceUri, relativeUri));
		message.Headers.Add("PRIVATE-TOKEN", AccessToken);
		return message;
	}

	public Task<GitLabVersion?> GetVersionAsync(CancellationToken cancellationToken = default)
	{
		return GetAsync<GitLabVersion>("/api/v4/version", cancellationToken);
	}

	public Task<IReadOnlyList<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
	{
		var urlBuilder = new StringBuilder();
		urlBuilder.Append("/api/v4/projects/");

		return ReadPagedResultAsync<Project>(urlBuilder.ToString(), cancellationToken);
	}

	private static void AppendProjectUrl(StringBuilder urlBuilder, NameOrNumericId projectId, string? path = null)
	{
		urlBuilder.Append(@"/api/v4/projects/");
		projectId.AppendTo(urlBuilder);
		if(path is not null)
		{
			urlBuilder.Append('/');
			urlBuilder.Append(path);
		}
	}

	private static string SortToString(SortOrder order)
		=> order switch
		{
			SortOrder.Descending => @"desc",
			SortOrder.Ascending  => @"asc",
			_ => throw new ArgumentException("Unknown order.", nameof(order)),
		};

	private static void AppendParameter(StringBuilder query, ref char sep, string name, string value)
	{
		query.Append(sep);
		query.Append(name);
		query.Append('=');
		query.Append(value);
		sep = '&';
	}

	public Task<IReadOnlyList<Milestone>> GetMilestonesAsync(NameOrNumericId projectId,
		CancellationToken cancellationToken)
	{
		var urlBuilder = new StringBuilder();
		AppendProjectUrl(urlBuilder, projectId, @"milestones");

		return ReadPagedResultAsync<Milestone>(urlBuilder.ToString(), cancellationToken);
	}
}
