namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	internal partial class VersionCheckDialog : GitDialogBase
	{
		private const string _downloadUrl = @"http://code.google.com/p/msysgit/downloads/list";
		private readonly Version _requiredVersion;
		private Version _installedVersion;
		private MsysGitDownloader _downloader;

		public VersionCheckDialog(Version requiredVersion, Version installedVersion)
		{
			if(requiredVersion == null) throw new ArgumentNullException("requiredVersion");
			_requiredVersion = requiredVersion;
			_installedVersion = installedVersion;

			InitializeComponent();

			Text = Resources.StrGitVersionCheck;

			if(_installedVersion == null)
			{
				_lblMessage.Text = Resources.MsgGitIsNotInstalled;
				_lblInstalledVersionValue.Text = Resources.StrlNotInstalled.SurroundWith("<", ">");
			}
			else
			{
				_lblMessage.Text = Resources.MsgGitVersionIsOutdated;
				_lblInstalledVersionValue.Text = _installedVersion.ToString();
			}
			_lblRequiredVersion.Text = Resources.StrRequiredVersion.AddColon();
			_lblInstalledVersion.Text = Resources.StrInstalledVersion.AddColon();
			_lblAdditionalMessage.Text = Resources.MsgGitSupportWillBeDisabled;

			_lnkRefresh.Text = Resources.StrlRefresh;
			_lnkGoToDownloadPage.Text = Resources.StrlGoToDownloadPage;
			_lnkConfigure.Text = Resources.StrlConfigure;
			_lnkRefreshLatestVersion.Text = Resources.StrlRefresh;
			_lnkDownload.Text = Resources.StrlDownload;

			_lblRequiredVersionValue.Text = _requiredVersion.ToString();

			_lblLatestVersion.Text = Resources.StrLatestVersion.AddColon();

			UpdateStatus();
		}

		private void RefreshLatestVersion()
		{
			_lnkRefreshLatestVersion.Visible = false;
			_lnkDownload.Visible = false;
			_lblLatestVersionValue.Text = Resources.StrsSearching.AddEllipsis();
			MsysGitDownloader.BeginCreate((ar) =>
			{
				_downloader = MsysGitDownloader.EndCreate(ar);
				if(!Disposing && !IsDisposed)
				{
					try
					{
						BeginInvoke(new MethodInvoker(UpdateLatestVersion));
					}
					catch
					{
					}
				}
			});
		}

		private void UpdateLatestVersion()
		{
			if(_downloader != null && _downloader.IsAvailable)
			{
				_lnkDownload.Visible = (RepositoryProvider.GitVersion == null) ||
					(_downloader.LatestVersion > RepositoryProvider.GitVersion);
				_lblLatestVersionValue.Text = _downloader.LatestVersion.ToString();
			}
			else
			{
				_lblLatestVersionValue.Text = Resources.StrsUnknown.SurroundWith('<', '>');
			}
			_lnkRefreshLatestVersion.Visible = true;
		}

		protected override string ActionVerb
		{
			get { return Resources.StrProceed; }
		}

		public Version InstalledVersion
		{
			get { return _installedVersion; }
		}

		protected override void OnShown()
		{
			base.OnShown();
			SetOkEnabled(_installedVersion != null && _installedVersion >= _requiredVersion);
			RefreshLatestVersion();
		}

		private void UpdateStatus()
		{
			if(_installedVersion == null)
			{
				_lblMessage.Text = Resources.MsgGitIsNotInstalled;
				_lblInstalledVersionValue.Text = Resources.StrlNotInstalled.SurroundWith("<", ">");
				_picWarning.Image = Resources.ImgWarning48;
				_lblAdditionalMessage.Visible = true;
				SetOkEnabled(false);
			}
			else
			{
				if(_installedVersion < _requiredVersion)
				{
					_lblMessage.Text = Resources.MsgGitVersionIsOutdated;
					_lblInstalledVersionValue.Text = _installedVersion.ToString();
					_picWarning.Image = Resources.ImgWarning48;
					_lblAdditionalMessage.Visible = true;
					SetOkEnabled(false);
				}
				else
				{
					_lblMessage.Text = Resources.MsgGitVersionOk;
					_lblInstalledVersionValue.Text = _installedVersion.ToString();
					_picWarning.Image = Resources.ImgInfo48;
					_lblAdditionalMessage.Visible = false;
					SetOkEnabled(true);
				}
			}
		}

		private void RefreshVersion()
		{
			Version v = null;
			try
			{
				v = RepositoryProvider.Git.QueryVersion();
			}
			catch
			{
			}
			_installedVersion = v;
			UpdateStatus();
		}

		private void OnRefreshLoacalVersion(object sender, LinkLabelLinkClickedEventArgs e)
		{
			RefreshVersion();
		}

		private void OnGoToDownloadPage(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Utility.OpenUrl(_downloadUrl);
		}

		private void OnConfigureClick(object sender, LinkLabelLinkClickedEventArgs e)
		{
			using(var dlg = new GitOptionsPage())
			{
				if(dlg.Run(this) == DialogResult.OK)
				{
					RefreshVersion();
				}
			}
		}

		private void OnRefreshLatestVersionClick(object sender, LinkLabelLinkClickedEventArgs e)
		{
			RefreshLatestVersion();
		}

		private void OnDownloadClick(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if(_downloader != null && _downloader.IsAvailable)
				_downloader.Download();
		}
	}
}
