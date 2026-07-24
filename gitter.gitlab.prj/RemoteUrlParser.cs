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

namespace gitter.GitLab;

using System;
using System.Diagnostics.CodeAnalysis;

static class RemoteUrlParser
{
	private const string DotGit = @".git";

	public static bool TryGetProjectPath(Uri? serviceUri, string? remoteFetchUrl,
		[MaybeNullWhen(returnValue: false)] out string projectPath)
	{
		projectPath = default;

		if(serviceUri is null) return false;
		if(string.IsNullOrWhiteSpace(remoteFetchUrl)) return false;

		var remote = remoteFetchUrl!.Trim();

		if(TryParseScpLike(remote, out var scpHost, out var scpPath))
		{
			if(!HostMatches(serviceUri, scpHost, port: -1)) return false;
			return TryNormalize(scpPath, out projectPath);
		}

		if(!Uri.TryCreate(remote, UriKind.Absolute, out var uri)) return false;

		var isWeb = uri.Scheme is @"http" or @"https";
		if(!isWeb && uri.Scheme is not (@"ssh" or @"git")) return false;
		if(!HostMatches(serviceUri, uri.Host, isWeb ? uri.Port : -1)) return false;

		var path = uri.AbsolutePath;
		if(isWeb) path = StripBasePath(serviceUri, path);

		return TryNormalize(path, out projectPath);
	}

	private static bool TryParseScpLike(string url,
		[MaybeNullWhen(returnValue: false)] out string host,
		[MaybeNullWhen(returnValue: false)] out string path)
	{
		host = default;
		path = default;

		if(url.IndexOf(@"://", StringComparison.Ordinal) >= 0) return false;

		var colon = url.IndexOf(':');
		if(colon <= 0) return false;

		var slash = url.IndexOfAny(['/', '\\']);
		if(slash >= 0 && slash < colon) return false;

		var authority = url.Substring(0, colon);
		var at = authority.LastIndexOf('@');
		host = at >= 0 ? authority.Substring(at + 1) : authority;
		if(host.Length == 0) return false;

		path = url.Substring(colon + 1);
		return path.Length != 0;
	}

	private static bool HostMatches(Uri serviceUri, string host, int port)
	{
		if(!string.Equals(serviceUri.Host, host, StringComparison.OrdinalIgnoreCase)) return false;

		return port < 0 || port == serviceUri.Port;
	}

	private static string StripBasePath(Uri serviceUri, string path)
	{
		var basePath = serviceUri.AbsolutePath.Trim('/');
		if(basePath.Length == 0) return path;

		var trimmed = path.TrimStart('/');
		if(trimmed.Length > basePath.Length
			&& trimmed.StartsWith(basePath, StringComparison.OrdinalIgnoreCase)
			&& trimmed[basePath.Length] == '/')
		{
			return trimmed.Substring(basePath.Length + 1);
		}
		return path;
	}

	private static bool TryNormalize(string path,
		[MaybeNullWhen(returnValue: false)] out string projectPath)
	{
		projectPath = default;

		var value = Uri.UnescapeDataString(path).Trim().Trim('/');
		if(value.EndsWith(DotGit, StringComparison.OrdinalIgnoreCase))
		{
			value = value.Substring(0, value.Length - DotGit.Length).TrimEnd('/');
		}
		if(value.Length == 0) return false;

		projectPath = value;
		return true;
	}
}
