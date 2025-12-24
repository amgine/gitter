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

namespace gitter.TeamCity;

using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Text.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public sealed class TeamCityServiceContext
{
	#region Data

	private string _serviceUri;
	private string _passphrase;

	private readonly ProjectsCollection _projects;
	private readonly BuildTypesCollection _buildTypes;
	private readonly BuildsCollection _builds;

	#endregion

	private enum ResponseContentType
	{
		Default,
		PlainText,
		Xml,
		Json,
	}

	private static string GetPassphrase(string username, string password)
	{
		return Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
	}

	#region .ctor

	public TeamCityServiceContext(HttpMessageInvoker httpMessageInvoker, ServerInfo server)
	{
		Verify.Argument.IsNotNull(httpMessageInvoker);

		HttpMessageInvoker = httpMessageInvoker;

		_serviceUri = server.ServiceUri.ToString();
		_passphrase = server.ApiKey;

		_projects	= new ProjectsCollection(this);
		_buildTypes	= new BuildTypesCollection(this);
		_builds		= new BuildsCollection(this);
	}

	#endregion

	#region Properties

	private HttpMessageInvoker HttpMessageInvoker { get; }

	internal LockType SyncRoot { get; } = new();

	public ProjectsCollection Projects => _projects;

	public BuildTypesCollection BuildTypes => _buildTypes;

	public BuildsCollection Builds => _builds;

	public string? DefaultProjectId { get; set; }

	#endregion

	public string ServiceUri => _serviceUri;

	private Uri GetUri(string relativeUri)
		=> _serviceUri.EndsWith('/')
			? new(_serviceUri + @"app/rest/" + relativeUri)
			: new(_serviceUri + @"/app/rest/" + relativeUri);

	private void SetupHttpAuth(HttpRequestMessage request)
	{
		request.Headers.Authorization = new("Bearer", _passphrase);
	}

	private static void SetupAcceptContentType(HttpRequestMessage request, ResponseContentType contentType)
	{
		if(contentType == ResponseContentType.Default) return;
		var header = contentType switch
		{
			ResponseContentType.PlainText => @"text/plain",
			ResponseContentType.Xml       => @"application/xml",
			ResponseContentType.Json      => @"application/json",
			_ => throw new ArgumentException($"Unknown content type: {contentType}", nameof(contentType)),
		};
		request.Headers.Add(@"Accept", header);
	}

	private Task<HttpResponseMessage> GetResponseAsync(string relativeUri, ResponseContentType contentType, CancellationToken cancellationToken)
	{
		var uri = GetUri(relativeUri);
		using var request = new HttpRequestMessage(HttpMethod.Get, uri);
		SetupHttpAuth(request);
		SetupAcceptContentType(request, contentType);
		return HttpMessageInvoker.SendAsync(request, cancellationToken);
	}

	internal async Task<XmlDocument> GetXmlAsync(string relativeUri, CancellationToken cancellationToken = default)
	{
		using var response = await GetResponseAsync(relativeUri, ResponseContentType.Xml, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		using var stream = await response.Content.ReadAsStreamAsync(cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		var xml = new XmlDocument();
		xml.Load(stream);
		return xml;
	}

	internal async Task<JsonDocument> GetJsonAsync(string relativeUri, CancellationToken cancellationToken = default)
	{
		using var response = await GetResponseAsync(relativeUri, ResponseContentType.Json, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		using var stream = await response.Content.ReadAsStreamAsync(cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		return await JsonDocument
			.ParseAsync(stream, cancellationToken: cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
	}

	internal async Task<string> GetPlainTextAsync(string relativeUri, CancellationToken cancellationToken = default)
	{
		using var response = await GetResponseAsync(relativeUri, ResponseContentType.Json, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		using var stream = await response.Content.ReadAsStreamAsync(cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		using var streamReader = new StreamReader(stream);
		return await streamReader
#if NET9_0_OR_GREATER
			.ReadToEndAsync(cancellationToken)
#else
			.ReadToEndAsync()
#endif
			.ConfigureAwait(continueOnCapturedContext: false);
	}
}
