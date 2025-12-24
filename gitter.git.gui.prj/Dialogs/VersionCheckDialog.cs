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
using gitter.Framework.Controls;
using gitter.Framework.Layout;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
internal partial class VersionCheckDialog : GitDialogBase
{
	private const string _downloadUrl = @"https://git-scm.com/downloads/win";

	readonly struct DialogControls
	{
		public readonly LabelControl _lblInstalledVersion;
		public readonly LabelControl _lblInstalledVersionValue;
		public readonly LabelControl _lblRequiredVersionValue;
		public readonly LabelControl _lblRequiredVersion;
		public readonly LinkLabel _lnkRefresh;
		public readonly LinkLabel _lnkGoToDownloadPage;
		public readonly PictureBox _picWarning;
		public readonly LabelControl _lblMessage;
		public readonly LabelControl _lblAdditionalMessage;
		public readonly LinkLabel _lnkConfigure;
		public readonly LabelControl _lblLatestVersion;
		public readonly LabelControl _lblLatestVersionValue;
		public readonly LinkLabel _lnkRefreshLatestVersion;
		public readonly LinkLabel _lnkDownload;

		public DialogControls(IGitterStyle style)
		{
			_lblInstalledVersion = new();
			_lblInstalledVersionValue = new();
			_lblRequiredVersionValue = new();
			_lblRequiredVersion = new();
			_lnkRefresh = new();
			_lnkGoToDownloadPage = new();
			_picWarning = new() { SizeMode = PictureBoxSizeMode.Zoom };
			_lblMessage = new() { WordBreak = true };
			_lblAdditionalMessage = new() { WordBreak = true };
			_lnkConfigure = new();
			_lblLatestVersion = new();
			_lblLatestVersionValue = new();
			_lnkRefreshLatestVersion = new();
			_lnkDownload = new();
			ApplyStyle(style, _lnkRefresh);
			ApplyStyle(style, _lnkGoToDownloadPage);
			ApplyStyle(style, _lnkConfigure);
			ApplyStyle(style, _lnkRefreshLatestVersion);
			ApplyStyle(style, _lnkDownload);
		}

		static void ApplyStyle(IGitterStyle style, LinkLabel link)
		{
			link.LinkColor       = style.Colors.HyperlinkText;
			link.ActiveLinkColor = style.Colors.HyperlinkTextHotTrack;
		}

		public void Localize()
		{
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

		public void Layout(Control parent)
		{
			var labelColumn   = SizeSpec.Absolute(125);
			var versionColumn = SizeSpec.Absolute(65);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						/* 0 */ SizeSpec.Absolute(48),
						/* 1 */ LayoutConstants.LabelRowSpacing,
						/* 2 */ LayoutConstants.LabelRowHeight,
						/* 3 */ LayoutConstants.LabelRowSpacing,
						/* 4 */ LayoutConstants.LabelRowHeight,
						/* 5 */ LayoutConstants.LabelRowSpacing,
						/* 6 */ LayoutConstants.LabelRowHeight,
						/* 7 */ LayoutConstants.LabelRowSpacing,
						/* 8 */ LayoutConstants.LabelRowHeight,
					],
					content:
					[
						new GridContent(new Grid(
							columns:
							[
								SizeSpec.Absolute(48),
								SizeSpec.Absolute(10),
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new ControlContent(_picWarning, marginOverride: LayoutConstants.NoMargin), column: 0),
								new GridContent(new ControlContent(_lblMessage, marginOverride: LayoutConstants.NoMargin), column: 2),
							]), row: 0),
						new GridContent(new Grid(
							columns:
							[
								labelColumn,
								versionColumn,
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new ControlContent(_lblInstalledVersion,      marginOverride: LayoutConstants.NoMargin), column: 0),
								new GridContent(new ControlContent(_lblInstalledVersionValue, marginOverride: LayoutConstants.NoMargin), column: 1),
								new GridContent(new ControlContent(_lnkRefresh,               marginOverride: LayoutConstants.NoMargin), column: 2),
							]), row: 2),
						new GridContent(new Grid(
							columns:
							[
								labelColumn,
								versionColumn,
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new ControlContent(_lblRequiredVersion,      marginOverride: LayoutConstants.NoMargin), column: 0),
								new GridContent(new ControlContent(_lblRequiredVersionValue, marginOverride: LayoutConstants.NoMargin), column: 1),
								new GridContent(new ControlContent(_lnkGoToDownloadPage,     marginOverride: LayoutConstants.NoMargin), column: 2),
							]), row: 4),
						new GridContent(new Grid(
							columns:
							[
								labelColumn,
								versionColumn,
								SizeSpec.Absolute(60),
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new ControlContent(_lblLatestVersion,        marginOverride: LayoutConstants.NoMargin), column: 0),
								new GridContent(new ControlContent(_lblLatestVersionValue,   marginOverride: LayoutConstants.NoMargin), column: 1),
								new GridContent(new ControlContent(_lnkRefreshLatestVersion, marginOverride: LayoutConstants.NoMargin), column: 2),
								new GridContent(new ControlContent(_lnkDownload,             marginOverride: LayoutConstants.NoMargin), column: 3),
							]), row: 6),
						new GridContent(new ControlContent(_lblAdditionalMessage, marginOverride: LayoutConstants.NoMargin), row: 8),
					]),
			};

			_picWarning.Parent = parent;
			_lblMessage.Parent = parent;
			_lblInstalledVersion.Parent = parent;
			_lblInstalledVersionValue.Parent = parent;
			_lnkRefresh.Parent = parent;
			_lblRequiredVersion.Parent = parent;
			_lblRequiredVersionValue.Parent = parent;
			_lnkGoToDownloadPage.Parent = parent;
			_lblLatestVersion.Parent = parent;
			_lblLatestVersionValue.Parent = parent;
			_lnkRefreshLatestVersion.Parent = parent;
			_lnkDownload.Parent = parent;
			_lblAdditionalMessage.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private Version? _requiredVersion;
	private Version? _installedVersion;
	private IGitDownloader? _downloader;

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

		Name = nameof(VersionCheckDialog);
		Text = Resources.StrGitVersionCheck;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_controls._lnkRefresh.LinkClicked              += OnRefreshLoacalVersion;
		_controls._lnkGoToDownloadPage.LinkClicked     += OnGoToDownloadPage;
		_controls._lnkConfigure.LinkClicked            += OnConfigureClick;
		_controls._lnkRefreshLatestVersion.LinkClicked += OnRefreshLatestVersionClick;
		_controls._lnkDownload.LinkClicked             += OnDownloadClick;
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 134));

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	public Version? InstalledVersion
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

	public Version? RequiredVersion
	{
		get => _requiredVersion;
		set
		{
			if(_requiredVersion != value)
			{
				_requiredVersion = value;
				_controls._lblRequiredVersionValue.Text = _requiredVersion?.ToString();
			}
			UpdateStatus();
		}
	}

	private IGitRepositoryProvider GitRepositoryProvider { get; }

	private IFactory<GitOptionsPage> GitOptionsPageFactory { get; }

	private IGitDownloaderProvider GitDownloaderProvider { get; }

	private async void RefreshLatestVersion()
	{
		_controls._lnkRefreshLatestVersion.Visible = false;
		_controls._lnkDownload.Visible = false;
		_controls._lblLatestVersionValue.Text = Resources.StrsSearching.AddEllipsis();
		_downloader = await GitDownloaderProvider.CreateAsync();
		UpdateLatestVersion();
	}

	private void UpdateLatestVersion()
	{
		if(_downloader is { IsAvailable: true })
		{
			var currentVersion = default(Version);
			var accessor = GitRepositoryProvider.GitAccessor;
			if(accessor is not null)
			{
				try
				{
					accessor.InvalidateGitVersion();
					currentVersion = accessor.GitVersion;
				}
				catch(Exception exc) when(!exc.IsCritical)
				{
				}
			}
			_controls._lnkDownload.Visible =
				(currentVersion is null) ||
				(_downloader.LatestVersion > currentVersion);
			_controls._lblLatestVersionValue.Text = _downloader.LatestVersion?.ToString();
		}
		else
		{
			_controls._lblLatestVersionValue.Text = Resources.StrsUnknown.SurroundWith('<', '>');
		}
		_controls._lnkRefreshLatestVersion.Visible = true;
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
			_controls._lblMessage.Text = Resources.MsgGitIsNotInstalled;
			_controls._lblInstalledVersionValue.Text = Resources.StrlNotInstalled.SurroundWith("<", ">");
			_controls._picWarning.Image = CachedResources.ScaledBitmaps[@"warning", 48];
			_controls._lblAdditionalMessage.Visible = true;
			SetOkEnabled(false);
		}
		else if(_installedVersion < _requiredVersion)
		{
			_controls._lblMessage.Text = Resources.MsgGitVersionIsOutdated;
			_controls._lblInstalledVersionValue.Text = _installedVersion.ToString();
			_controls._picWarning.Image = CachedResources.ScaledBitmaps["warning", 48];
			_controls._lblAdditionalMessage.Visible = true;
			SetOkEnabled(false);
		}
		else
		{
			_controls._lblMessage.Text = Resources.MsgGitVersionOk;
			_controls._lblInstalledVersionValue.Text = _installedVersion.ToString();
			_controls._picWarning.Image = CachedResources.ScaledBitmaps["info", 48];
			_controls._lblAdditionalMessage.Visible = false;
			SetOkEnabled(true);
		}
	}

	private void RefreshVersion()
	{
		var gitVersion = default(Version);
		var accessor = GitRepositoryProvider.GitAccessor;
		if(accessor is not null)
		{
			try
			{
				accessor.InvalidateGitVersion();
				gitVersion = accessor.GitVersion;
			}
			catch(Exception exc) when(!exc.IsCritical)
			{
				gitVersion = default;
			}
		}
		_installedVersion = gitVersion;
		UpdateStatus();
	}

	private void OnRefreshLoacalVersion(object? sender, LinkLabelLinkClickedEventArgs e)
	{
		RefreshVersion();
	}

	private void OnGoToDownloadPage(object? sender, LinkLabelLinkClickedEventArgs e)
	{
		Utility.OpenUrl(_downloadUrl);
	}

	private void OnConfigureClick(object? sender, LinkLabelLinkClickedEventArgs e)
	{
		using var dialog = GitOptionsPageFactory.Create();
		if(dialog.Run(this) == DialogResult.OK)
		{
			RefreshVersion();
		}
	}

	private void OnRefreshLatestVersionClick(object? sender, LinkLabelLinkClickedEventArgs e)
	{
		RefreshLatestVersion();
	}

	private void OnDownloadClick(object? sender, LinkLabelLinkClickedEventArgs e)
	{
		if(_downloader is { IsAvailable: true })
		{
			_downloader.Download();
		}
	}
}
