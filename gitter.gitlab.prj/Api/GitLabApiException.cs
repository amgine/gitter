#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Net;
using System.Net.Http;
using System.Text;

class GitLabApiException : Exception
{
	public static Exception Create(HttpResponseMessage response, string? details)
	{
		Assert.IsNotNull(response);

		var url     = response.RequestMessage?.RequestUri?.ToString();
		var message = FormatMessage(response.StatusCode, response.ReasonPhrase, url, details);

		return response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden
			? new GitLabUnauthorizedException(message, response.StatusCode, url, details)
			: new GitLabApiException(message, response.StatusCode, url, details);
	}

	private static string FormatMessage(HttpStatusCode statusCode, string? reasonPhrase, string? url, string? details)
	{
		var sb = new StringBuilder();
		sb.Append("GitLab API request failed: ");
		sb.Append((int)statusCode);
		if(!string.IsNullOrWhiteSpace(reasonPhrase))
		{
			sb.Append(' ');
			sb.Append(reasonPhrase);
		}
		if(!string.IsNullOrWhiteSpace(details))
		{
			sb.Append(" - ");
			sb.Append(details);
		}
		if(!string.IsNullOrWhiteSpace(url))
		{
			sb.Append(" [");
			sb.Append(url);
			sb.Append(']');
		}
		return sb.ToString();
	}

	public GitLabApiException(string message, HttpStatusCode statusCode, string? url, string? details)
		: base(message)
	{
		StatusCode = statusCode;
		Url        = url;
		Details    = details;
	}

	public HttpStatusCode StatusCode { get; }

	public string? Url { get; }

	public string? Details { get; }
}

sealed class GitLabUnauthorizedException(string message, HttpStatusCode statusCode, string? url, string? details)
	: UnauthorizedAccessException(message)
{
	public HttpStatusCode StatusCode { get; } = statusCode;

	public string? Url { get; } = url;

	public string? Details { get; } = details;
}
