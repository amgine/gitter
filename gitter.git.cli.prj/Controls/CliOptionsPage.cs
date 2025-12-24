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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Options;
using gitter.Framework.Services;

using gitter.Git.AccessLayer;
using gitter.Git.AccessLayer.CLI;

using Resources = gitter.Git.AccessLayer.CLI.Properties.Resources;

[ToolboxItem(false)]
public partial class CliOptionsPage : PropertyPage, IExecutableDialog
{
	public static readonly new Guid Guid = new("EF9DFCAC-FDD1-4B69-AD0E-3783035FA13C");

	readonly struct DialogControls
	{
		public readonly LabelControl _lblPathToGitExe;
		public readonly TextBox _txtGitPath;
		public readonly IButtonWidget _cmdAutoDetect;
		public readonly LabelControl _lblFoundVersion;
		public readonly IButtonWidget _cmdBrowse;
		public readonly LabelControl _lblVersion;
		public readonly IRadioButtonWidget _radAlwaysAutodetect;
		public readonly IRadioButtonWidget _radSpecifyManually;
		public readonly OpenFileDialog _openFileDialog;
		public readonly ICheckBoxWidget _chkLogCLICalls;
		public readonly LabelControl _lblLatestVersionValue;
		public readonly LabelControl _lblLatestVersion;
		public readonly IButtonWidget _btnDownload;
		public readonly IButtonWidget _btnRefreshLatestVersion;
		public readonly ICheckBoxWidget _chkFallbackToAnsi;

		public DialogControls(IGitterStyle? style)
		{
			style ??= GitterApplication.Style;

			_lblPathToGitExe         = new();
			_txtGitPath              = new();
			_cmdAutoDetect           = style.ButtonFactory.Create();
			_lblFoundVersion         = new();
			_cmdBrowse               = style.ButtonFactory.Create();
			_lblVersion              = new();
			_radAlwaysAutodetect     = style.RadioButtonFactory.Create();
			_radSpecifyManually      = style.RadioButtonFactory.Create();
			_lblLatestVersionValue   = new();
			_lblLatestVersion        = new();
			_btnDownload             = style.ButtonFactory.Create();
			_btnRefreshLatestVersion = style.ButtonFactory.Create();
			_chkFallbackToAnsi       = style.CheckBoxFactory.Create();
			_chkLogCLICalls          = style.CheckBoxFactory.Create();
			_openFileDialog          = new();

			GitterApplication.FontManager.InputFont.Apply(_txtGitPath);
		}

		public void Localize()
		{
			_lblPathToGitExe.Text = Resources.StrPathToGitExe.AddColon();
			_lblFoundVersion.Text = Resources.StrFoundVersion.AddColon();

			_radAlwaysAutodetect.Text = Resources.StrsAlwaysAutodetect;
			_radSpecifyManually.Text = Resources.StrsSpecifyManually;

			_cmdAutoDetect.Text = Resources.StrAutodetect;
			_cmdBrowse.Text = Resources.StrBrowse.AddEllipsis();

			_chkLogCLICalls.Text = Resources.StrsLogCLICalls;
			_chkFallbackToAnsi.Text = Resources.StrsFallbackToAnsi + ' ' + Resources.StrlRestartRequired.SurroundWithBraces();

			_lblLatestVersionValue.Text = Resources.StrsSearching.AddEllipsis();
			_lblLatestVersion.Text = Resources.StrLatestVersion.AddColon();
			_btnDownload.Text = Resources.StrInstall;
			_btnRefreshLatestVersion.Text = Resources.StrRefresh;
		}

		public void Layout(Control parent)
		{
			var pathDec = new TextBoxDecorator(_txtGitPath);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(117),
						SizeSpec.Everything(),
					],
					rows:
					[
						/* 0 */ LayoutConstants.RadioButtonRowHeight,
						/* 1 */ LayoutConstants.TextInputRowHeight,
						/* 2 */ LayoutConstants.RowSpacing,
						/* 3 */ LayoutConstants.ButtonRowHeight,
						/* 4 */ LayoutConstants.RowSpacing,
						/* 5 */ LayoutConstants.ButtonRowHeight,
						/* 6 */ LayoutConstants.RowSpacing,
						/* 7 */ LayoutConstants.CheckBoxRowHeight,
						/* 8 */ LayoutConstants.CheckBoxRowHeight,
					],
					content:
					[
						new GridContent(new ControlContent(_lblPathToGitExe, marginOverride: LayoutConstants.NoMargin), column: 0, row: 0),
						new GridContent(new Grid(
							columns:
							[
								SizeSpec.Absolute(150),
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new WidgetContent(_radAlwaysAutodetect, marginOverride: LayoutConstants.NoMargin), column: 0),
								new GridContent(new WidgetContent(_radSpecifyManually,  marginOverride: LayoutConstants.NoMargin), column: 1),
							]), column: 1, row: 0),
						new GridContent(new ControlContent(pathDec, marginOverride: LayoutConstants.TextBoxMargin), columnSpan: 2, row: 1),

						new GridContent(new Grid(
							columns:
							[
								/* 0 */ SizeSpec.Absolute(117),
								/* 1 */ SizeSpec.Everything(),
								/* 2 */ SizeSpec.Absolute(100),
								/* 3 */ SizeSpec.Absolute(6),
								/* 4 */ SizeSpec.Absolute(100),
							],
							rows:
							[
								/* 0 */ LayoutConstants.ButtonRowHeight,
								/* 1 */ LayoutConstants.RowSpacing,
								/* 2 */ LayoutConstants.ButtonRowHeight,
							],
							content:
							[
								new GridContent(new ControlContent(_lblFoundVersion,         marginOverride: LayoutConstants.NoMargin), row: 0),
								new GridContent(new ControlContent(_lblVersion,              marginOverride: LayoutConstants.NoMargin), row: 0, column: 1),
								new GridContent(new WidgetContent (_cmdAutoDetect,           marginOverride: LayoutConstants.NoMargin), row: 0, column: 2),
								new GridContent(new WidgetContent (_cmdBrowse,               marginOverride: LayoutConstants.NoMargin), row: 0, column: 4),
								new GridContent(new ControlContent(_lblLatestVersion,        marginOverride: LayoutConstants.NoMargin), row: 2),
								new GridContent(new ControlContent(_lblLatestVersionValue,   marginOverride: LayoutConstants.NoMargin), row: 2, column: 1),
								new GridContent(new WidgetContent (_btnRefreshLatestVersion, marginOverride: LayoutConstants.NoMargin), row: 2, column: 2),
								new GridContent(new WidgetContent (_btnDownload,             marginOverride: LayoutConstants.NoMargin), row: 2, column: 4),
							]), row: 3, columnSpan: 2, rowSpan: 3),

						new GridContent(new WidgetContent(_chkLogCLICalls,    marginOverride: LayoutConstants.NoMargin), columnSpan: 2, row: 7),
						new GridContent(new WidgetContent(_chkFallbackToAnsi, marginOverride: LayoutConstants.NoMargin), columnSpan: 2, row: 8),
					]),
			};

			_lblPathToGitExe.Parent     = parent;
			_radAlwaysAutodetect.Parent = parent;
			_radSpecifyManually.Parent  = parent;
			pathDec.Parent              = parent;

			_lblFoundVersion.Parent = parent;
			_lblVersion.Parent = parent;
			_cmdAutoDetect.Parent = parent;
			_cmdBrowse.Parent = parent;
			_lblLatestVersion.Parent = parent;
			_lblLatestVersionValue.Parent = parent;
			_btnRefreshLatestVersion.Parent = parent;
			_btnDownload.Parent = parent;

			_chkLogCLICalls.Parent = parent;
			_chkFallbackToAnsi.Parent = parent;
		}
	}

	private string? _versionPath;

	private readonly DialogControls _controls;
	private readonly IGitDownloaderProvider _gitDownloaderProvider;
	private IGitDownloader? _gitDownloader;
	private readonly GitCLI _gitCLI;

	public CliOptionsPage(IGitAccessor gitAccessor, IGitDownloaderProvider gitDownloaderProvider)
		: base(Guid)
	{
		Verify.Argument.IsNotNull(gitAccessor);
		Verify.Argument.IsNotNull(gitDownloaderProvider);

		_gitCLI = (GitCLI)gitAccessor;
		_gitDownloaderProvider = gitDownloaderProvider;

		Name = nameof(CliOptionsPage);
		Text = Resources.StrCommandLineInterface;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode = AutoScaleMode.Dpi;
		Size = new(479, 158);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_controls._txtGitPath.TextChanged += OnGitPathTextChanged;
		_controls._cmdAutoDetect.Click += OnAutoDetectClick;
		_controls._cmdBrowse.Click += OnBrowseClick;
		_controls._radAlwaysAutodetect.IsCheckedChanged += _radAlwaysAutodetect_CheckedChanged;
		_controls._btnDownload.Click += OnDownloadClick;
		_controls._btnRefreshLatestVersion.Click += _btnRefreshLatestVersion_Click;

		if(_gitCLI.AutodetectGitExePath)
		{
			_controls._radAlwaysAutodetect.IsChecked = true;
		}
		else
		{
			_controls._radSpecifyManually.IsChecked = true;
		}
		_versionPath = _gitCLI.GitExecutablePath;
		_controls._txtGitPath.Text = _versionPath;

		_controls._chkLogCLICalls.IsChecked = _gitCLI.LogCalls;
		_controls._chkFallbackToAnsi.IsChecked = GitProcess.EnableAnsiCodepageFallback;
		_controls._lblVersion.Text = TryGetVersion()?.ToString() ?? Resources.StrlUnavailable.SurroundWith("<", ">");
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_controls._openFileDialog.Dispose();
		}
		base.Dispose(disposing);
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(479, 158));

	private Version? TryGetVersion()
	{
		var version = default(Version);
		try
		{
			_gitCLI.InvalidateGitVersion();
			version = _gitCLI.GitVersion;
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
		}
		return version;
	}

	/// <inheritdoc/>
	protected override void OnShown()
	{
		base.OnShown();

		if(_gitDownloader is null)
		{
			RefreshLatestVersion();
		}
	}

	private async void RefreshLatestVersion()
	{
		_controls._btnDownload.Enabled = false;
		_controls._btnRefreshLatestVersion.Enabled = false;
		_controls._lblLatestVersionValue.Text = Resources.StrsSearching.AddEllipsis();
		_gitDownloader = await _gitDownloaderProvider.CreateAsync();
		if(IsDisposed) return;
		UpdateLatestVersion();
	}

	private void UpdateLatestVersion()
	{
		if(_gitDownloader is { IsAvailable: true })
		{
			var version = TryGetVersion();
			_controls._btnDownload.Enabled =
				(version is null) ||
				(_gitDownloader.LatestVersion > version);
			_controls._lblLatestVersionValue.Text = _gitDownloader.LatestVersion?.ToString();
		}
		else
		{
			_controls._lblLatestVersionValue.Text = Resources.StrsUnknown.SurroundWith('<', '>');
		}
		_controls._btnRefreshLatestVersion.Enabled = true;
	}

	private void _radAlwaysAutodetect_CheckedChanged(object? sender, EventArgs e)
	{
		if(_controls._radAlwaysAutodetect.IsChecked)
		{
			_controls._txtGitPath.Enabled = false;
			_controls._cmdBrowse.Enabled = false;
			_controls._cmdAutoDetect.Enabled = false;
		}
		else
		{
			_controls._txtGitPath.Enabled = true;
			_controls._cmdBrowse.Enabled = true;
			_controls._cmdAutoDetect.Enabled = true;
		}
	}

	private void OnAutoDetectClick(object? sender, EventArgs e)
	{
		_controls._txtGitPath.Text = GitFinder.DetectGitExePath();
	}

	private void OnBrowseClick(object? sender, EventArgs e)
	{
		_controls._openFileDialog.Filter = "Git Executable|git.exe";
		_controls._openFileDialog.FileName = "git.exe";
		var path = GitFinder.DetectGitExePath();
		if(!string.IsNullOrEmpty(path))
		{
			path = Path.GetDirectoryName(path);
			_controls._openFileDialog.InitialDirectory = path;
		}
		if(_controls._openFileDialog.ShowDialog(this) == DialogResult.OK)
		{
			_controls._txtGitPath.Text = _controls._openFileDialog.FileName;
		}
	}

	private void OnGitPathTextChanged(object? sender, EventArgs e)
	{
		static bool CanBeGitExePath(string? path)
		{
			if(string.IsNullOrEmpty(path)) return false;
			var fileName = Path.GetFileName(path);
			var isGitExecutable =
				string.Equals(fileName, "git.exe", StringComparison.OrdinalIgnoreCase) ||
				string.Equals(fileName, "git.cmd", StringComparison.OrdinalIgnoreCase);
			return isGitExecutable && File.Exists(path);
		}

		if(_controls._txtGitPath.Text == _versionPath) return;

		_versionPath = _controls._txtGitPath.Text;
		var version = default(Version);
		try
		{
			var path = _controls._txtGitPath.Text.Trim();
			path = Path.GetFullPath(path);
			if(CanBeGitExePath(path))
			{
				version = GitProcess.CheckVersion(path);
			}
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
		}
		_controls._lblVersion.Text = version is not null
			? version.ToString()
			: Resources.StrlUnavailable.SurroundWith("<", ">");
	}

	private void _btnRefreshLatestVersion_Click(object? sender, EventArgs e)
	{
		RefreshLatestVersion();
	}

	private void OnDownloadClick(object? sender, EventArgs e)
	{
		if(_gitDownloader is not { IsAvailable: true } downloader) return;
		try
		{
			ProgressForm.MonitorTaskAsModalWindow(this, "Git Installation", downloader.DownloadAndInstallAsync);
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			GitterApplication.MessageBoxService.Show(
				this, exc.Message, "Git Installation Failed", MessageBoxButton.Close, MessageBoxIcon.Error);
			return;
		}
		var version = TryGetVersion();
		if(version is not null)
		{
			_controls._lblVersion.Text = version.ToString();
			_versionPath = _gitCLI.GitExecutablePath;
			if(string.IsNullOrWhiteSpace(_controls._txtGitPath.Text))
			{
				_controls._txtGitPath.Text = _versionPath;
			}
			if(_gitDownloader.LatestVersion == version)
			{
				_controls._btnDownload.Enabled = false;
			}
		}
		else
		{
			_controls._lblVersion.Text = Resources.StrlUnavailable.SurroundWith("<", ">");
		}
	}

	#region IExecutableDialog Members

	public bool Execute()
	{
		_gitCLI.ManualGitExePath = Path.GetFullPath(_controls._txtGitPath.Text.Trim());
		_gitCLI.AutodetectGitExePath = _controls._radAlwaysAutodetect.IsChecked;
		_gitCLI.LogCalls = _controls._chkLogCLICalls.IsChecked;
		_gitCLI.EnableAnsiCodepageFallback = _controls._chkFallbackToAnsi.IsChecked;
		return true;
	}

	#endregion
}
