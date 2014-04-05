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

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal partial class VersionCheckDialog : GitDialogBase
	{
		private const string _downloadUrl = @"http://code.google.com/p/msysgit/downloads/list";

		#region Data

		private readonly IWorkingEnvironment _environment;
		private readonly IGitRepositoryProvider _gitRepositoryProvider;
		private Version _requiredVersion;
		private Version _installedVersion;
		private MSysGitDownloader _downloader;

		#endregion

		public VersionCheckDialog(
			IWorkingEnvironment environment,
			IGitRepositoryProvider gitRepositoryProvider,
			Version requiredVersion,
			Version installedVersion)
		{
			Verify.Argument.IsNotNull(environment, "environment");
			Verify.Argument.IsNotNull(gitRepositoryProvider, "gitRepositoryProvider");
			Verify.Argument.IsNotNull(requiredVersion, "requiredVersion");

			_environment = environment;
			_gitRepositoryProvider = gitRepositoryProvider;
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

		private IGitRepositoryProvider GitRepositoryProvider
		{
			get { return _gitRepositoryProvider; }
		}

		private void RefreshLatestVersion()
		{
			_lnkRefreshLatestVersion.Visible = false;
			_lnkDownload.Visible = false;
			_lblLatestVersionValue.Text = Resources.StrsSearching.AddEllipsis();
			MSysGitDownloader.BeginCreate(OnMSysGitDownloaderCreated);
		}

		private void OnMSysGitDownloaderCreated(IAsyncResult ar)
		{
			try
			{
				_downloader = MSysGitDownloader.EndCreate(ar);
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				_downloader = null;
			}
			if(!Disposing && !IsDisposed)
			{
				try
				{
					BeginInvoke(new MethodInvoker(UpdateLatestVersion));
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
				}
			}
		}

		private void UpdateLatestVersion()
		{
			if(_downloader != null && _downloader.IsAvailable)
			{
				Version currentVersion = null;
				try
				{
					GitRepositoryProvider.GitAccessor.InvalidateGitVersion();
					currentVersion = GitRepositoryProvider.GitAccessor.GitVersion;
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
				}
				_lnkDownload.Visible =
					(currentVersion == null) ||
					(_downloader.LatestVersion > currentVersion);
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
			Version gitVersion;
			try
			{
				GitRepositoryProvider.GitAccessor.InvalidateGitVersion();
				gitVersion = GitRepositoryProvider.GitAccessor.GitVersion;
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				gitVersion = null;
			}
			_installedVersion = gitVersion;
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
			using(var dlg = new GitOptionsPage(_gitRepositoryProvider))
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
			{
				_downloader.Download();
			}
		}
	}
}
