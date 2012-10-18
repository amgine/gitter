namespace gitter.Git
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Xml;

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

			public void Dispose()
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
			}

			public void OnInstallerProcessExited(object sender, EventArgs e)
			{
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
			}
		}

		public void DownloadAndInstall()
		{
			var process = new DownloadAndInstallProcess()
			{
				Monitor = new NullMonitor(),
				WebRequest = WebRequest.Create(DownloadUrl),
			};
			process.Monitor.SetProgressIntermediate();
			process.Monitor.SetAction("Connecting to MSysGit download server...");
			process.WebRequest.BeginGetResponse(OnGotResponse, process);
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
			var bytesRead = process.ResponseStream.EndRead(ar);
			if(bytesRead == 0)
			{
				process.InstallerFileStream.Close();
				process.InstallerFileStream.Dispose();
				process.InstallerFileStream = null;
				process.ResponseStream.Dispose();
				process.ResponseStream = null;
				process.WebResponse.Close();
				process.WebResponse = null;
				process.Buffer = null;
				process.Monitor.SetProgressIntermediate();
				process.Monitor.SetAction("Installing MSysGit...");
				process.InstallerProcess = new Process()
				{
					StartInfo = new ProcessStartInfo()
					{
						FileName = process.InstallerFileName,
						Arguments = @"/silent",
					},
					EnableRaisingEvents = true,
				};
				process.InstallerProcess.Exited += process.OnInstallerProcessExited;
				process.InstallerProcess.Start();
			}
			else
			{
				if(process.InstallerFileStream == null)
				{
					process.InstallerFileName = Path.Combine(
						Path.GetTempPath(), "msysgit-installer.exe");
					process.InstallerFileStream = new FileStream(
						process.InstallerFileName,
						FileMode.Create,
						FileAccess.Write,
						FileShare.None);
				}
				process.DownloadedBytes += bytesRead;
				if(process.WebResponse.ContentLength > 0 && process.DownloadedBytes <= process.WebResponse.ContentLength)
				{
					process.Monitor.SetProgress((int)process.DownloadedBytes);
				}
				process.InstallerFileStream.Write(
					process.Buffer,
					0,
					bytesRead);
				process.ResponseStream.BeginRead(
					process.Buffer,
					0,
					process.Buffer.Length,
					OnResponseStreamRead,
					process);
			}
		}

		public override string ToString()
		{
			return _downloadUrl;
		}
	}
}
