namespace gitter.Git
{
	using System;
	using System.Net;
	using System.Xml;

	using gitter.Framework;

	using gitter.Git.AccessLayer.CLI;

	public sealed class MsysGitDownloader
	{
		private Version _latestVersion;
		private bool _isAvailable;
		private string _downloadUrl;

		private const string MsysgitFeedUrl = @"http://code.google.com/feeds/p/msysgit/downloads/basic";
		private const string DownloadURL = @"http://msysgit.googlecode.com/files/";

		public static MsysGitDownloader Create()
		{
			return new MsysGitDownloader();
		}

		public static IAsyncResult BeginCreate(AsyncCallback callback)
		{
			var func = new Func<MsysGitDownloader>(Create);
			return func.BeginInvoke(callback, func);
		}

		public static MsysGitDownloader EndCreate(IAsyncResult ar)
		{
			var func = (Func<MsysGitDownloader>)ar.AsyncState;
			return func.EndInvoke(ar);
		}

		private MsysGitDownloader()
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
							if(fileName.StartsWith("Git-") && !fileName.EndsWith("-unicode.exe"))
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

		public override string ToString()
		{
			return _downloadUrl;
		}
	}
}
