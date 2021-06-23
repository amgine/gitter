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

namespace gitter
{
	using System;
	using System.IO;
	using System.Net.Http;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using System.Reflection;

	using gitter.Framework;
	using gitter.Framework.Services;

	public sealed class GithubReleasesUpdateChannel : IUpdateChannel
	{
		const string ServiceUrl = @"https://github.com";

		static readonly Regex _downloadLinkRegex = new Regex("\\<a\\s+href\\=\\\"(?<url>\\/amgine\\/gitter\\/releases\\/download\\/v(?<version>\\d+(\\.\\d+){0,3})\\/gitter\\-binaries\\.zip)\\\"");

		private sealed class UpdateVersion : IUpdateVersion
		{
			public UpdateVersion(string downloadUrl, Version version)
			{
				DownloadUrl = downloadUrl;
				Version     = version;
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
				sb.Append(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
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

		public GithubReleasesUpdateChannel()
		{
		}

		/// <summary>Check latest gitter version on this channel.</summary>
		/// <returns>Latest gitter version.</returns>
		public async Task<IUpdateVersion> GetLatestVersionAsync()
		{
			using var http = new HttpClient();

			var page = await http
				.GetStringAsync(@"https://github.com/amgine/gitter/releases")
				.ConfigureAwait(continueOnCapturedContext: false);

			var version = default(Version);
			var url     = default(string);

			foreach(Match match in _downloadLinkRegex.Matches(page))
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

			return version is not null
				? new UpdateVersion(MakeAbsoluteUrl(url), version)
				: default;
		}
	}
}
