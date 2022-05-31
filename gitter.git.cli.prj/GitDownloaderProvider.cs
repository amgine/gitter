﻿#region Copyright Notice
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

namespace gitter.Git;

using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

sealed class GitDownloaderProvider : IGitDownloaderProvider
{
	private const string VersionPageUrl  = @"https://git-scm.com/";
	private const string DownloadPageUrl = @"https://git-scm.com/download/win";

	private static readonly Regex _regexVersion = new("\\<span\\s+class\\s*\\=\\s*\"version\"\\s*\\>\\s*(?<version>\\d+\\.\\d+\\.\\d+)\\s*\\<\\/span\\>");
	private static readonly Regex _regexUrl     = new("\\<a\\s+href\\s*\\=\\s*\"(?<url>[\\w\\.\\:\\/\\-]+\\/Git-(?<version>\\d+\\.\\d+\\.\\d+)\\-(?<arch>\\d{2})\\-bit\\.exe)\"\\>");

	public GitDownloaderProvider(HttpMessageInvoker httpMessageInvoker)
	{
		Verify.Argument.IsNotNull(httpMessageInvoker);

		HttpMessageInvoker = httpMessageInvoker;
	}

	private HttpMessageInvoker HttpMessageInvoker { get; }

	private async Task<string> DownloadPageSourceAsync(string url, CancellationToken cancellationToken)
	{
		using var request = new HttpRequestMessage(HttpMethod.Get, DownloadPageUrl);
		using var response = await HttpMessageInvoker
			.SendAsync(request, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);

#if NET5_0_OR_GREATER
		var task = response.Content.ReadAsStringAsync(cancellationToken);
#else
		var task = response.Content.ReadAsStringAsync();
#endif

		return await task.ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<IGitDownloader> CreateAsync(CancellationToken cancellationToken = default)
	{
		var version     = default(Version);
		var downloadUrl = default(string);

		try
		{
			var downloadPage = await DownloadPageSourceAsync(DownloadPageUrl, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
			var arch = Environment.Is64BitOperatingSystem ? @"64" : @"32";
			foreach(Match match in _regexUrl.Matches(downloadPage))
			{
				if(match.Groups[@"arch"]?.Value == arch && Version.TryParse(match.Groups[@"version"]?.Value, out version))
				{
					return new GitDownloader(version, match.Groups[@"url"].Value);
				}
			}
		}
		catch
		{
		}
		return new GitDownloader(version, downloadUrl);
	}
}
