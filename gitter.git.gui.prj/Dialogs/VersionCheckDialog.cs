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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
internal partial class VersionCheckDialog : GitDialogBase
{
	private const string _downloadUrl = @"https://git-scm.com/download/win";

	private Version _requiredVersion;
	private Version _installedVersion;
	private IGitDownloader _downloader;

	public VersionCheckDialog(
		IGitRepositoryProvider   gitRepositoryProvider,
		IGitDownloaderProvider   gitDownloadDownloader,
		IFactory<GitOptionsPage> gitOptionsPageFactory)
	{
		Verify.Argument.IsNotNull(gitRepositoryProvider);
		Verify.Argument.IsNotNull(gitDownloadDownloader);
		Verify.Argument.IsNotNull(gitOptionsPageFactory);

		GitRepositoryProvider = gitRepositoryProvider;
		GitDownloaderProvider = gitDownloadDownloader;
		GitOptionsPageFactory = gitOptionsPageFactory;

		InitializeComponent();

		Text = Resources.StrGitVersionCheck;

		_lblMessage.Text = Resources.MsgGitIsNotInstalled;
		_lblInstalledVersionValue.Text = Resources.StrlNotInstalled.SurroundWith("<", ">");
		_lblRequiredVersion.Text   = Resources.StrRequiredVersion.AddColon();
		_lblInstalledVersion.Text  = Resources.StrInstalledVersion.AddColon();
		_lblAdditionalMessage.Text = Resources.MsgGitSupportWillBeDisabled;

		_lnkRefresh.Text = Resources.StrlRefresh;
		_lnkGoToDownloadPage.Text = Resources.StrlGoToDownloadPage;
		_lnkConfigure.Text = Resources.StrlConfigure;
		_lnkRefreshLatestVersion.Text = Resources.StrlRefresh;
		_lnkDownload.Text = Resources.StrlDownload;

		_lblLatestVersion.Text = Resources.StrLatestVersion.AddColon();
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 162));

	public Version InstalledVersion
	{
		get => _installedVersion;
		set
		{
			if(_installedVersion != value)
			{
				_installedVersion = value;
				UpdateStatus();
			}
		}
	}

	public Version RequiredVersion
	{
		get => _requiredVersion;
		set
		{
			if(_requiredVersion != value)
			{
				_requiredVersion = value;
				_lblRequiredVersionValue.Text = _requiredVersion.ToString();
			}
			UpdateStatus();
		}
	}

	private IGitRepositoryProvider GitRepositoryProvider { get; }

	private IFactory<GitOptionsPage> GitOptionsPageFactory { get; }

	private IGitDownloaderProvider GitDownloaderProvider { get; }

	private async void RefreshLatestVersion()
	{
		_lnkRefreshLatestVersion.Visible = false;
		_lnkDownload.Visible = false;
		_lblLatestVersionValue.Text = Resources.StrsSearching.AddEllipsis();
		_downloader = await GitDownloaderProvider.CreateAsync();
		UpdateLatestVersion();
	}

	private void UpdateLatestVersion()
	{
		if(_downloader is { IsAvailable: true })
		{
			Version currentVersion = null;
			try
			{
				GitRepositoryProvider.GitAccessor.InvalidateGitVersion();
				currentVersion = GitRepositoryProvider.GitAccessor.GitVersion;
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
			}
			_lnkDownload.Visible =
				(currentVersion is null) ||
				(_downloader.LatestVersion > currentVersion);
			_lblLatestVersionValue.Text = _downloader.LatestVersion.ToString();
		}
		else
		{
			_lblLatestVersionValue.Text = Resources.StrsUnknown.SurroundWith('<', '>');
		}
		_lnkRefreshLatestVersion.Visible = true;
	}

	protected override string ActionVerb => Resources.StrProceed;

	protected override void OnShown()
	{
		base.OnShown();
		SetOkEnabled(_installedVersion is not null && _installedVersion >= _requiredVersion);
		RefreshLatestVersion();
	}

	private void UpdateStatus()
	{
		if(RequiredVersion is null) return;
		if(_installedVersion is null)
		{
			_lblMessage.Text = Resources.MsgGitIsNotInstalled;
			_lblInstalledVersionValue.Text = Resources.StrlNotInstalled.SurroundWith("<", ">");
			_picWarning.Image = CachedResources.ScaledBitmaps[@"warning", 48];
			_lblAdditionalMessage.Visible = true;
			SetOkEnabled(false);
		}
		else
		{
			if(_installedVersion < _requiredVersion)
			{
				_lblMessage.Text = Resources.MsgGitVersionIsOutdated;
				_lblInstalledVersionValue.Text = _installedVersion.ToString();
				_picWarning.Image = CachedResources.ScaledBitmaps["warning", 48];
				_lblAdditionalMessage.Visible = true;
				SetOkEnabled(false);
			}
			else
			{
				_lblMessage.Text = Resources.MsgGitVersionOk;
				_lblInstalledVersionValue.Text = _installedVersion.ToString();
				_picWarning.Image = CachedResources.ScaledBitmaps["info", 48];
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
		catch(Exception exc) when(!exc.IsCritical())
		{
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
		using var dlg = GitOptionsPageFactory.Create();
		if(dlg.Run(this) == DialogResult.OK)
		{
			RefreshVersion();
		}
	}

	private void OnRefreshLatestVersionClick(object sender, LinkLabelLinkClickedEventArgs e)
	{
		RefreshLatestVersion();
	}

	private void OnDownloadClick(object sender, LinkLabelLinkClickedEventArgs e)
	{
		if(_downloader is { IsAvailable: true })
		{
			_downloader.Download();
		}
	}
}
