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

namespace gitter.Updater
{
	using System;
	using System.Net.Http;
	using System.Threading.Tasks;
	using System.IO;
	using System.IO.Compression;

	class DownloadAndUnzipDriver : IUpdateDriver
	{
		public string Name => "download+unzip";

		public IUpdateProcess CreateProcess(CommandLine cmdline)
		{
			return new DownloadAndUnzipProcess(
				downloadUrl:     cmdline[@"source"],
				targetDirectory: cmdline[@"target"]);
		}
	}

	sealed class DownloadAndUnzipProcess : UpdateProcessBase
	{
		public DownloadAndUnzipProcess(string downloadUrl, string targetDirectory)
			: base(targetDirectory)
		{
			DownloadUrl     = downloadUrl;
			UpdaterPath     = Path.Combine(Path.GetTempPath(), @"gitter-updater");
			SourceDirectory = Path.Combine(UpdaterPath, @"gitter");
		}

		private string DownloadUrl { get; }

		private string UpdaterPath { get; }

		private string SourceDirectory { get; }

		private async Task DownloadAndUnzipAsync()
		{
			using var http = new HttpClient();

			Utility.EnsureDirectoryExists(UpdaterPath);
			Utility.EnsureDirectoryDoesNotExist(SourceDirectory);

			using var stream = await http
				.GetStreamAsync(DownloadUrl)
				.ConfigureAwait(continueOnCapturedContext: false);

			using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
			zip.ExtractToDirectory(UpdaterPath);
		}

		protected override void NotifyInitializing(UpdateProcessMonitor monitor)
		{
			Assert.IsNotNull(monitor);

			base.NotifyInitializing(monitor);
			monitor.MaximumProgress = 4;
		}

		protected override void Cleanup()
		{
			try
			{
				Utility.EnsureDirectoryDoesNotExist(SourceDirectory);
			}
			catch
			{
			}
		}

		protected override void UpdateProc()
		{
			Monitor.Stage = "Downloading...";
			Monitor.CurrentProgress = 1;
			DownloadAndUnzipAsync().Wait();

			InstallApplication(from: SourceDirectory);
		}
	}
}
