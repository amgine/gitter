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

namespace gitter.Git
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Net.Http;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	public sealed class GitDownloader
	{
		private const string VersionPageUrl  = @"https://git-scm.com/";
		private const string DownloadPageUrl = @"https://git-scm.com/download/win";

		private static readonly Regex _regexVersion = new Regex("\\<span\\s+class\\s*\\=\\s*\"version\"\\s*\\>\\s*(?<version>\\d+\\.\\d+\\.\\d+)\\s*\\<\\/span\\>");
		private static readonly Regex _regexUrl     = new Regex("\\<a\\s+id\\s*\\=\\s*\"auto\\-download\\-link\"\\s+href\\s*\\=\\s*\"(?<url>[\\w\\.\\:\\/\\-]+)\"\\s*\\>");

		public static async Task<GitDownloader> CreateAsync()
		{
			var version     = default(Version);
			var downloadUrl = default(string);

			using var http = new HttpClient();
			try
			{
				var versionPage = await http
					.GetStringAsync(VersionPageUrl)
					.ConfigureAwait(continueOnCapturedContext: false);
				var versionMatch = _regexVersion.Match(versionPage);
				if(versionMatch != null && versionMatch.Success
					&& Version.TryParse(versionMatch.Groups["version"].Value, out version))
				{
					var downloadPage = await http
						.GetStringAsync(DownloadPageUrl)
						.ConfigureAwait(continueOnCapturedContext: false);
					var urlMatch = _regexUrl.Match(downloadPage);
					if(urlMatch != null && urlMatch.Success)
					{
						downloadUrl = urlMatch.Groups["url"].Value;
					}
				}
			}
			catch
			{
			}
			return new GitDownloader(version, downloadUrl);
		}

		private GitDownloader(Version version, string downloadUrl)
		{
			LatestVersion = version;
			DownloadUrl   = downloadUrl;
			IsAvailable   = version != null && DownloadUrl != null;
		}

		public Version LatestVersion { get; }

		public bool IsAvailable { get; }

		public string DownloadUrl { get; }

		public void Download()
		{
			Utility.OpenUrl(DownloadUrl);
		}

		private sealed class DownloadAndInstallProcess : IDisposable
		{
			public DownloadAndInstallProcess(EventWaitHandle completionWaitHandle)
			{
				_waitHandle = completionWaitHandle;
			}

			public IProgress<OperationProgress> Monitor { get; set; }
			public WebRequest WebRequest { get; set; }
			public WebResponse WebResponse { get; set; }
			public Stream ResponseStream { get; set; }
			public byte[] Buffer { get; set; }
			public Stream InstallerFileStream { get; set; }
			public string InstallerFileName { get; set; }
			public Process InstallerProcess { get; set; }
			public long DownloadedBytes { get; set; }
			public Exception Exception { get; set; }
			public int InstallerExitCode { get; set; }
			public bool IsDisposed { get; private set; }

			private EventWaitHandle _waitHandle;

			public void Dispose()
			{
				if(!IsDisposed)
				{
					if(WebResponse != null)
					{
						WebResponse.Close();
						WebResponse = null;
					}
					if(ResponseStream != null)
					{
						ResponseStream.Dispose();
						ResponseStream = null;
					}
					Buffer = null;
					if(InstallerFileStream != null)
					{
						InstallerFileStream.Dispose();
						InstallerFileStream = null;
					}
					if(InstallerProcess != null)
					{
						InstallerProcess.Dispose();
					}
					if(_waitHandle != null)
					{
						_waitHandle.Set();
						_waitHandle = null;
					}
					IsDisposed = true;
				}
			}

			public void OnInstallerProcessExited(object sender, EventArgs e)
			{
				InstallerExitCode = InstallerProcess.ExitCode;
				InstallerProcess.Dispose();
				InstallerProcess = null;
				Monitor?.Report(new OperationProgress("Completed.") { IsCompleted = true });
				try
				{
					File.Delete(InstallerFileName);
				}
				catch
				{
				}
				Dispose();
			}
		}

		public Task DownloadAndInstallAsync(IProgress<OperationProgress> progress = default)
		{
			return Task.Run(
				() =>
				{
					progress?.Report(new OperationProgress("Initializing..."));
					using var evt = new ManualResetEvent(false);
					var process = new DownloadAndInstallProcess(evt)
					{
						Monitor = progress,
						WebRequest = WebRequest.Create(DownloadUrl),
					};
					progress?.Report(new OperationProgress("Connecting to Git download server..."));
					process.WebRequest.BeginGetResponse(OnGotResponse, process);
					evt.WaitOne();
					if(process.Exception != null)
					{
						throw process.Exception;
					}
					if(process.InstallerExitCode != 0)
					{
						throw new ApplicationException("Installer returned exit code " + process.InstallerExitCode);
					}
				});
		}

		private static void OnGotResponse(IAsyncResult ar)
		{
			var process = (DownloadAndInstallProcess)ar.AsyncState;
			var state = new OperationProgress
			{
				CurrentProgress = 0,
				MaxProgress     = 0,
				IsIndeterminate = true,
				ActionName      = "Downloading Git...",
			};
			process.Monitor?.Report(state);
			process.WebResponse = process.WebRequest.EndGetResponse(ar);
			process.ResponseStream = process.WebResponse.GetResponseStream();
			if(process.WebResponse.ContentLength > 0)
			{
				state.IsIndeterminate = false;
				state.MaxProgress = (int)process.WebResponse.ContentLength;
				process.Monitor?.Report(state);
			}
			process.Buffer = new byte[1024*4];
			process.ResponseStream.BeginRead(
				process.Buffer,
				0,
				process.Buffer.Length,
				OnResponseStreamRead,
				process);
		}

		private static void OnResponseStreamRead(IAsyncResult ar)
		{
			var process = (DownloadAndInstallProcess)ar.AsyncState;
			int bytesRead = 0;
			try
			{
				bytesRead = process.ResponseStream.EndRead(ar);
			}
			catch(Exception exc)
			{
				process.Exception = exc;
				process.Dispose();
				return;
			}
			if(bytesRead == 0)
			{
				try
				{
					process.InstallerFileStream.Close();
					process.InstallerFileStream.Dispose();
					process.InstallerFileStream = null;
				}
				catch(Exception exc)
				{
					process.Exception = exc;
					process.Dispose();
					return;
				}
				process.ResponseStream.Dispose();
				process.ResponseStream = null;
				process.WebResponse.Close();
				process.WebResponse = null;
				process.Buffer = null;
				RunInstaller(process);
			}
			else
			{
				if(!EnsureOutputFileStreamExists(process))
				{
					return;
				}
				UpdateDownloadProgress(process, bytesRead);
				try
				{
					process.InstallerFileStream.Write(
						process.Buffer,
						0,
						bytesRead);
				}
				catch(Exception exc)
				{
					process.Exception = exc;
					process.Dispose();
					return;
				}
				process.ResponseStream.BeginRead(
					process.Buffer,
					0,
					process.Buffer.Length,
					OnResponseStreamRead,
					process);
			}
		}

		private static void UpdateDownloadProgress(DownloadAndInstallProcess process, int downloadedBytes)
		{
			process.DownloadedBytes += downloadedBytes;
			if(process.Monitor != null && process.WebResponse.ContentLength > 0 && process.DownloadedBytes <= process.WebResponse.ContentLength)
			{
				var state = new OperationProgress
				{
					CurrentProgress = (int)process.DownloadedBytes,
					MaxProgress     = (int)process.WebResponse.ContentLength,
					IsIndeterminate = false,
					ActionName      = "Downloading Git...",
				};
				process.Monitor.Report(state);
			}
		}

		private static bool EnsureOutputFileStreamExists(DownloadAndInstallProcess process)
		{
			if(process.InstallerFileStream == null)
			{
				try
				{
					process.InstallerFileName = Path.Combine(
						Path.GetTempPath(), "git-installer.exe");
					process.InstallerFileStream = new FileStream(
						process.InstallerFileName,
						FileMode.Create,
						FileAccess.Write,
						FileShare.None);
				}
				catch(Exception exc)
				{
					process.Exception = exc;
					process.Dispose();
					return false;
				}
			}
			return true;
		}

		private static void RunInstaller(DownloadAndInstallProcess process)
		{
			var state = new OperationProgress
			{
				CurrentProgress = 0,
				MaxProgress     = 0,
				IsIndeterminate = true,
				ActionName      = "Installing Git...",
			};
			process.Monitor?.Report(state);
			try
			{
				process.InstallerProcess = new Process()
				{
					StartInfo = new ProcessStartInfo()
					{
						FileName = process.InstallerFileName,
						Arguments = @"/verysilent",
					},
					EnableRaisingEvents = true,
				};
				process.InstallerProcess.Exited += process.OnInstallerProcessExited;
				process.InstallerProcess.Start();
			}
			catch(Exception exc)
			{
				process.Exception = exc;
				process.Dispose();
			}
		}

		public override string ToString() => DownloadUrl;
	}
}
