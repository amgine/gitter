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

namespace gitter.Git;

using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Framework.CLI;

sealed class GitDownloader(HttpMessageInvoker httpMessageInvoker, Version version, string downloadUrl) : IGitDownloader
{
	sealed class UnavailableImpl : IGitDownloader
	{
		public Version? LatestVersion => default;

		public bool IsAvailable => false;

		public string? DownloadUrl => default;

		public void Download() => throw new InvalidOperationException();

		public Task DownloadAndInstallAsync(IProgress<OperationProgress>? progress = null)
			=> throw new InvalidOperationException();
	}

	public static IGitDownloader Unavailable { get; } = new UnavailableImpl();

	public Version LatestVersion { get; } = version;

	public bool IsAvailable => true;

	public string DownloadUrl { get; } = downloadUrl;

	public void Download()
	{
		if(DownloadUrl is not { Length: not 0 } url)
		{
			throw new InvalidOperationException("URL is not available.");
		}
		Utility.OpenUrl(url);
	}

	public async Task DownloadAndInstallAsync(IProgress<OperationProgress>? progress = default)
	{
		var cancellationToken = CancellationToken.None;

		progress?.Report(new OperationProgress("Connecting to Git download server..."));

		using var request  = new HttpRequestMessage(HttpMethod.Get, DownloadUrl);
		using var response = httpMessageInvoker is HttpClient http
			? await http
				.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false)
			: await httpMessageInvoker
				.SendAsync(request, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);

		response.EnsureSuccessStatusCode();

		var length = response.Content.Headers.ContentLength;

		progress?.Report(new()
		{
			ActionName      = "Downloading Git...",
			MaxProgress     = (int)length.GetValueOrDefault(),
			IsIndeterminate = !length.HasValue,
		});

		var downloadedExe = Path.Combine(Path.GetTempPath(), "git-installer.exe");

		using(var stream = await response.Content
			.ReadAsStreamAsync(cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false))
		{
			using var dst = new FileStream(downloadedExe, FileMode.Create, FileAccess.Write, FileShare.Read);
			var downloadedBytes = 0;
			var buffer = ArrayPool<byte>.Shared.Rent(512 * 1024);
			try
			{
				while(true)
				{
#if NETCOREAPP
					var count = await stream
						.ReadAsync(buffer, cancellationToken)
						.ConfigureAwait(continueOnCapturedContext: false);
#else
					var count = await stream
						.ReadAsync(buffer, 0, buffer.Length)
						.ConfigureAwait(continueOnCapturedContext: false);
#endif

					if(count == 0) break;

#if NETCOREAPP
					await dst
						.WriteAsync(new Memory<byte>(buffer, 0, count), cancellationToken)
						.ConfigureAwait(continueOnCapturedContext: false);
#else
					await dst
						.WriteAsync(buffer, 0, count, cancellationToken)
						.ConfigureAwait(continueOnCapturedContext: false);
#endif

					downloadedBytes += count;
					if(length.HasValue)
					{
						var p = new OperationProgress("Downloading Git...", (int)length.Value) { CurrentProgress = downloadedBytes };
						progress?.Report(p);
					}
				}
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(buffer);
			}
		}

		progress?.Report(OperationProgress.Indeterminate("Installing Git..."));

		using(var process = new Process()
		{
			StartInfo = new ProcessStartInfo()
			{
				FileName  = downloadedExe,
				Arguments = "/VERYSILENT /SUPPRESSMSGBOXES /FORCECLOSEAPPLICATIONS /NORESTART",
			},
		})
		{
			await process
				.StartAsync()
				.ConfigureAwait(continueOnCapturedContext: false);
		}

		progress?.Report(OperationProgress.Indeterminate("Cleanup..."));

		try
		{
			File.Delete(downloadedExe);
		}
		catch
		{
		}

		progress?.Report(OperationProgress.Completed);
	}

	/// <inheritdoc/>
	public override string? ToString() => DownloadUrl;
}
