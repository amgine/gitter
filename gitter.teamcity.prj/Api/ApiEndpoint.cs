#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.TeamCity.Api;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

partial class ApiEndpoint
{
	public ApiEndpoint(HttpMessageInvoker httpMessageInvoker, ServerInfo server)
	{
		Verify.Argument.IsNotNull(httpMessageInvoker);
		Verify.Argument.IsNotNull(server);

		HttpMessageInvoker = httpMessageInvoker;
		Server = server;
	}

	private HttpMessageInvoker HttpMessageInvoker { get; }

	private ServerInfo Server { get; }

	private Uri ServiceUri => Server.ServiceUri;

	private string AccessToken => Server.ApiKey;

	private HttpRequestMessage CreateRequest(HttpMethod method, string relativeUri)
	{
		var request = new HttpRequestMessage(method, new Uri(ServiceUri, relativeUri));
		request.Headers.Authorization = new("Bearer", AccessToken);
		return request;
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

	private async Task<XDocument> GetXDocumentAsync(string url, CancellationToken cancellationToken = default)
	{
		Assert.IsNeitherNullNorWhitespace(url);

		using var message = CreateRequest(HttpMethod.Get, url);

		using var response = await HttpMessageInvoker
			.SendAsync(message, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);

		CheckResponse(response);

		return await ReadXDocumentAsync(response, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
	}

	private static async Task<XDocument> ReadXDocumentAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
	{
		Assert.IsNotNull(response);

		using var stream = await response.Content
			.ReadAsStreamAsync(cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);

#if NETCOREAPP
		return await XDocument
			.LoadAsync(stream, LoadOptions.None, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
#else
		return XDocument.Load(stream);
#endif
	}

	public async Task<string> GetVersionAsync(CancellationToken cancellationToken = default)
	{
		var xml = await GetXDocumentAsync("app/rest/server", cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		var version = (string?)xml.Element("server")?.Attribute("version");
		return version ?? throw new Exception("Could not extract server version.");
	}

	public async Task<AccessToken[]> GetAccessTokensAsync(CancellationToken cancellationToken = default)
	{
		var xml = await GetXDocumentAsync("app/rest/users/current/tokens?fields=count,token(name,creationTime,value,expirationTime)", cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);

		var tokens = xml.Element("tokens") ?? throw new Exception();
		var count = (int)tokens.Attribute("count")!;
		var result = new AccessToken[count];
		var index = 0;
		foreach(var e in tokens.Elements())
		{
			var name = (string)e.Attribute("name")!;
			var createdAt = (DateTimeOffset)e.Attribute("creationTime")!;
			var expiresAt = (DateTimeOffset?)e.Attribute("expirationTime");
			result[index++] = new(name, createdAt, expiresAt);
		}
		return result;
	}
}
