namespace gitter.Git
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Xml;
	using System.Threading;

	using gitter.Framework;

	using gitter.Git.AccessLayer.CLI;

	public sealed class MSysGitDownloader
	{
		private Version _latestVersion;
		private bool _isAvailable;
		private string _downloadUrl;

		private const string MsysgitFeedUrl = @"http://code.google.com/feeds/p/msysgit/downloads/basic";
		private const string DownloadURL = @"http://msysgit.googlecode.com/files/";

		public static MSysGitDownloader Create()
		{
			return new MSysGitDownloader();
		}

		public static IAsyncResult BeginCreate(AsyncCallback callback)
		{
			var func = new Func<MSysGitDownloader>(Create);
			return func.BeginInvoke(callback, func);
		}

		public static MSysGitDownloader EndCreate(IAsyncResult ar)
		{
			var func = (Func<MSysGitDownloader>)ar.AsyncState;
			return func.EndInvoke(ar);
		}

		private MSysGitDownloader()
		{
			var feed = DownloadFeed();
			if(feed != null)
			{
				try
				{
					ExtractFeedData(feed);
				}
				catch
				{
				}
			}
		}

		private void ExtractFeedData(XmlDocument feed)
		{
			Version version = null;
			var rootNode = feed.DocumentElement;
			foreach(XmlNode node in rootNode.ChildNodes)
			{
				if(node.Name == "entry")
				{
					var id = node["id"].InnerText;
					if(id.EndsWith(".exe"))
					{
						var pos = id.LastIndexOf("/");
						if(pos != -1)
						{
							var fileName = id.Substring(pos + 1);
							if(fileName.StartsWith("Git-"))
							{
								var parser = new Parser(fileName);
								try
								{
									var v = parser.ReadVersion();
									if(version == null || v > version)
									{
										version = v;
										_downloadUrl = DownloadURL + fileName;
										_isAvailable = true;
									}
								}
								catch
								{
									continue;
								}
							}
						}
					}
				}
			}
			_latestVersion = version;
		}

		private XmlDocument DownloadFeed()
		{
			var doc = new XmlDocument();
			try
			{
				var request = HttpWebRequest.Create(MsysgitFeedUrl);
				using(var response = request.GetResponse())
				{
					using(var stream = response.GetResponseStream())
					{
						doc.Load(stream);
					}
				}
				return doc;
			}
			catch
			{
				return null;
			}
		}

		public Version LatestVersion
		{
			get { return _latestVersion; }
		}

		public bool IsAvailable
		{
			get { return _isAvailable; }
		}

		public string DownloadUrl
		{
			get { return _downloadUrl; }
		}

		public void Download()
		{
			Utility.OpenUrl(_downloadUrl);
		}

		private sealed class DownloadAndInstallProcess : IDisposable
		{
			public DownloadAndInstallProcess(EventWaitHandle completionWaitHandle)
			{
				_waitHandle = completionWaitHandle;
			}

			public IAsyncProgressMonitor Monitor { get; set; }
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
				Monitor.ProcessCompleted();
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

		public IAsyncFunc<Exception> DownloadAndInstallAsync()
		{
			return AsyncFunc.Create<object, Exception>(
				null,
				(data, monitor) =>
				{
					using(var evt = new ManualResetEvent(false))
					{
						var process = new DownloadAndInstallProcess(evt)
						{
							Monitor = monitor,
							WebRequest = WebRequest.Create(DownloadUrl),
						};
						process.Monitor.SetProgressIndeterminate();
						process.Monitor.SetAction("Connecting to MSysGit download server...");
						process.WebRequest.BeginGetResponse(OnGotResponse, process);
						evt.WaitOne();
						if(process.Exception != null)
						{
							return process.Exception;
						}
						if(process.InstallerExitCode != 0)
						{
							return new ApplicationException("Installer returned exit code " + process.InstallerExitCode);
						}
						return null;
					}
				},
				"MSysGit Installation",
				"Initializing...");
		}

		private static void OnGotResponse(IAsyncResult ar)
		{
			var process = (DownloadAndInstallProcess)ar.AsyncState;
			process.Monitor.SetAction("Downloading MSysGit...");
			process.WebResponse = process.WebRequest.EndGetResponse(ar);
			process.ResponseStream = process.WebResponse.GetResponseStream();
			if(process.WebResponse.ContentLength > 0)
			{
				process.Monitor.SetProgressRange(0, (int)process.WebResponse.ContentLength);
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
				process.Monitor.ProcessCompleted();
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
					process.Monitor.ProcessCompleted();
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
					process.Monitor.ProcessCompleted();
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
			if(process.WebResponse.ContentLength > 0 && process.DownloadedBytes <= process.WebResponse.ContentLength)
			{
				process.Monitor.SetProgress((int)process.DownloadedBytes);
			}
		}

		private static bool EnsureOutputFileStreamExists(DownloadAndInstallProcess process)
		{
			if(process.InstallerFileStream == null)
			{
				try
				{
					process.InstallerFileName = Path.Combine(
						Path.GetTempPath(), "msysgit-installer.exe");
					process.InstallerFileStream = new FileStream(
						process.InstallerFileName,
						FileMode.Create,
						FileAccess.Write,
						FileShare.None);
				}
				catch(Exception exc)
				{
					process.Exception = exc;
					process.Monitor.ProcessCompleted();
					process.Dispose();
					return false;
				}
			}
			return true;
		}

		private static void RunInstaller(DownloadAndInstallProcess process)
		{
			process.Monitor.SetProgressIndeterminate();
			process.Monitor.SetAction("Installing MSysGit...");
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
				process.Monitor.ProcessCompleted();
				process.Dispose();
			}
		}

		public override string ToString()
		{
			return _downloadUrl;
		}
	}
}
