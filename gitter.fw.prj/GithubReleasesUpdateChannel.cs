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

namespace gitter;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Framework.Services;

/// <summary>Downloads updates from <c>github</c>.</summary>
public sealed class GithubReleasesUpdateChannel(HttpMessageInvoker httpMessageInvoker) : IUpdateChannel
{
	const string ServiceUrl = @"https://github.com";

	static readonly Regex _assetsLinkRegex = new("src\\=\\\"(?<url>https\\:\\/\\/github\\.com\\/amgine\\/gitter\\/releases\\/expanded_assets\\/v(?<version>\\d+(\\.\\d+){0,3}))\\\"");

#if NET6_0_OR_GREATER
	static readonly Regex _downloadLinkRegex = new("\\<a\\s+href\\=\\\"(?<url>\\/amgine\\/gitter\\/releases\\/download\\/v(?<version>\\d+(\\.\\d+){0,3})\\/gitter\\-net6\\.0\\-binaries\\.zip)\\\"");
#else
	static readonly Regex _downloadLinkRegex = new("\\<a\\s+href\\=\\\"(?<url>\\/amgine\\/gitter\\/releases\\/download\\/v(?<version>\\d+(\\.\\d+){0,3})\\/gitter\\-binaries\\.zip)\\\"");
#endif

	private sealed class UpdateVersion : IUpdateVersion
	{
		public UpdateVersion(string downloadUrl, Version version)
		{
			DownloadUrl = downloadUrl;
			Version = version;
		}

		public string DownloadUrl { get; }

		public Version Version { get; }

		private string FormatUpdaterCommand()
		{
			var sb = new StringBuilder();
			// update driver
			sb.Append('"');
			sb.Append(@"/driver:download+unzip");
			sb.Append('"');
			sb.Append(' ');
			// remote url
			sb.Append('"');
			sb.Append(@"/source:");
			sb.Append(DownloadUrl);
			sb.Append('"');
			sb.Append(' ');
			// install directory
			sb.Append('"');
			sb.Append(@"/target:");
			sb.Append(AppContext.BaseDirectory);
			sb.Append('"');
			return sb.ToString();
		}

		public void Update()
		{
			var command = FormatUpdaterCommand();
			HelperExecutables.LaunchUpdater(command);
		}
	}

	private static string MakeAbsoluteUrl(string url)
	{
		Assert.IsNotNull(url);

		if(url.StartsWith(@"http://") || url.StartsWith(@"https://"))
		{
			return url;
		}
		return url.StartsWith(@"/")
			? ServiceUrl + url
			: ServiceUrl + @"/" + url;
	}

	private static bool TryFindBestUrl(string src, Regex regex,
		[MaybeNullWhen(returnValue: false)] out string url,
		[MaybeNullWhen(returnValue: false)] out Version version)
	{
		version = default!;
		url     = default!;

		foreach(Match match in regex.Matches(src))
		{
			if(Version.TryParse(match.Groups[@"version"].Value, out var v))
			{
				if(version is null || version < v)
				{
					version = v;
					url     = match.Groups[@"url"].Value;
				}
			}
		}

		return url is not null && version is not null;
	}

	private async Task<string> GetStringAsync(string url, CancellationToken cancellationToken)
	{
		using var message = new HttpRequestMessage(HttpMethod.Get, url);

		using var response = await httpMessageInvoker
			.SendAsync(message, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);

		response.EnsureSuccessStatusCode();

		return await response.Content
#if NET5_0_OR_GREATER
			.ReadAsStringAsync(cancellationToken)
#else
			.ReadAsStringAsync()
#endif
			.ConfigureAwait(continueOnCapturedContext: false);
	}

	private async Task<string?> TryFindAssetsUrlAsync(CancellationToken cancellationToken)
	{
		var page = await GetStringAsync($@"{ServiceUrl}/amgine/gitter/releases", cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);

		return TryFindBestUrl(page, _assetsLinkRegex, out var url, out _)
			? url
			: default;
	}

	private async Task<IUpdateVersion?> TryFindDownloadUrlAsync(string assetsUrl, CancellationToken cancellationToken)
	{
		var page = await GetStringAsync(assetsUrl, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);

		return TryFindBestUrl(page, _downloadLinkRegex, out var url, out var version)
			? new UpdateVersion(MakeAbsoluteUrl(url), version)
			: default;
	}

	/// <summary>Check latest <c>gitter</c> version on this channel.</summary>
	/// <returns>Latest <c>gitter</c> version.</returns>
	public async Task<IUpdateVersion?> GetLatestVersionAsync(CancellationToken cancellationToken = default)
	{
		var assetsUrl = await TryFindAssetsUrlAsync(cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);

		if(assetsUrl is null) return default;

		cancellationToken.ThrowIfCancellationRequested();

		return await TryFindDownloadUrlAsync(assetsUrl, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
	}
}
