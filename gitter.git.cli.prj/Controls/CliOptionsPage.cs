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
using gitter.Framework.Options;
using gitter.Framework.Services;

using gitter.Git.AccessLayer;
using gitter.Git.AccessLayer.CLI;

using Resources = gitter.Git.AccessLayer.CLI.Properties.Resources;

[ToolboxItem(false)]
public partial class CliOptionsPage : PropertyPage, IExecutableDialog
{
	public static readonly new Guid Guid = new("EF9DFCAC-FDD1-4B69-AD0E-3783035FA13C");

	private string _versionPath;

	private readonly IGitDownloaderProvider _gitDownloaderProvider;
	private IGitDownloader _gitDownloader;
	private readonly GitCLI _gitCLI;

	public CliOptionsPage(IGitAccessor gitAccessor, IGitDownloaderProvider gitDownloaderProvider)
		: base(Guid)
	{
		Verify.Argument.IsNotNull(gitAccessor);
		Verify.Argument.IsNotNull(gitDownloaderProvider);

		_gitCLI = (GitCLI)gitAccessor;
		_gitDownloaderProvider = gitDownloaderProvider;

		InitializeComponent();

		Text = Resources.StrCommandLineInterface;

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

		if(_gitCLI.AutodetectGitExePath)
		{
			_radAlwaysAutodetect.Checked = true;
		}
		else
		{
			_radSpecifyManually.Checked = true;
		}
		_versionPath = _gitCLI.GitExecutablePath;
		_txtmSysGitPath.Text = _versionPath;

		_chkLogCLICalls.Checked = _gitCLI.LogCalls;
		_chkFallbackToAnsi.Checked = GitProcess.EnableAnsiCodepageFallback;
		_lblVersion.Text = TryGetVersion()?.ToString() ?? Resources.StrlUnavailable.SurroundWith("<", ">");
	}

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		GitterApplication.FontManager.InputFont.Apply(_txtmSysGitPath);
		base.OnLoad(e);
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(479, 158));

	private Version TryGetVersion()
	{
		Version version = null;
		try
		{
			_gitCLI.InvalidateGitVersion();
			version = _gitCLI.GitVersion;
		}
		catch(Exception exc) when(!exc.IsCritical())
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
		_btnDownload.Enabled = false;
		_btnRefreshLatestVersion.Enabled = false;
		_lblLatestVersionValue.Text = Resources.StrsSearching.AddEllipsis();
		_gitDownloader = await _gitDownloaderProvider.CreateAsync();
		UpdateLatestVersion();
	}

	private void UpdateLatestVersion()
	{
		if(_gitDownloader is not null)
		{
			if(_gitDownloader.IsAvailable)
			{
				var version = TryGetVersion();
				_btnDownload.Enabled =
					(version == null) ||
					(_gitDownloader.LatestVersion > version);
				_lblLatestVersionValue.Text = _gitDownloader.LatestVersion.ToString();
			}
			else
			{
				_lblLatestVersionValue.Text = Resources.StrsUnknown.SurroundWith('<', '>');
			}
		}
		else
		{
			_lblLatestVersionValue.Text = Resources.StrsUnknown.SurroundWith('<', '>');
		}
		_btnRefreshLatestVersion.Enabled = true;
	}

	private void _radAlwaysAutodetect_CheckedChanged(object sender, EventArgs e)
	{
		if(_radAlwaysAutodetect.Checked)
		{
			_txtmSysGitPath.Enabled = false;
			_cmdBrowse.Enabled = false;
			_cmdAutoDetect.Enabled = false;
		}
		else
		{
			_txtmSysGitPath.Enabled = true;
			_cmdBrowse.Enabled = true;
			_cmdAutoDetect.Enabled = true;
		}
	}

	private void _cmdAutoDetect_Click(object sender, EventArgs e)
	{
		_txtmSysGitPath.Text = GitFinder.DetectGitExePath();
	}

	private void _cmdBrowse_Click(object sender, EventArgs e)
	{
		_openFileDialog.Filter = "Git Executable|git.exe";
		_openFileDialog.FileName = "git.exe";
		var path = GitFinder.DetectGitExePath();
		if(!string.IsNullOrEmpty(path))
		{
			path = Path.GetDirectoryName(path);
			_openFileDialog.InitialDirectory = path;
		}
		if(_openFileDialog.ShowDialog(this) == DialogResult.OK)
		{
			_txtmSysGitPath.Text = _openFileDialog.FileName;
		}
	}

	private void _txtmSysGitPath_TextChanged(object sender, EventArgs e)
	{
		if(_txtmSysGitPath.Text != _versionPath)
		{
			_versionPath = _txtmSysGitPath.Text;
			try
			{
				var path = _txtmSysGitPath.Text.Trim();
				path = Path.GetFullPath(path);
				var fileName = Path.GetFileName(path).ToLower();
				if((fileName == "git.exe" || fileName == "git.cmd") && File.Exists(path))
				{
					Version version = null;
					try
					{
						version = GitProcess.CheckVersion(path);
					}
					catch(Exception exc) when (!exc.IsCritical())
					{
					}
					if(version != null)
					{
						_lblVersion.Text = version.ToString();
					}
					else
					{
						_lblVersion.Text = Resources.StrlUnavailable.SurroundWith("<", ">");
					}
				}
				else
				{
					_lblVersion.Text = Resources.StrlUnavailable.SurroundWith("<", ">");
				}
			}
			catch(Exception exc) when (!exc.IsCritical())
			{
				_lblVersion.Text = Resources.StrlUnavailable.SurroundWith("<", ">");
			}
		}
	}

	private void _btnRefreshLatestVersion_Click(object sender, EventArgs e)
	{
		RefreshLatestVersion();
	}

	private void _btnDownload_Click(object sender, EventArgs e)
	{
		try
		{
			ProgressForm.MonitorTaskAsModalWindow(this, "Git Installation", _gitDownloader.DownloadAndInstallAsync);
		}
		catch(Exception exc) when(!exc.IsCritical())
		{
			GitterApplication.MessageBoxService.Show(
				this, exc.Message, "Git Installation Failed", MessageBoxButton.Close, MessageBoxIcon.Error);
			return;
		}
		var version = TryGetVersion();
		if(version is not null)
		{
			_lblVersion.Text = version.ToString();
			_versionPath = _gitCLI.GitExecutablePath;
			if(string.IsNullOrWhiteSpace(_txtmSysGitPath.Text))
			{
				_txtmSysGitPath.Text = _versionPath;
			}
			if(_gitDownloader.LatestVersion == version)
			{
				_btnDownload.Enabled = false;
			}
		}
		else
		{
			_lblVersion.Text = Resources.StrlUnavailable.SurroundWith("<", ">");
		}
	}

	#region IExecutableDialog Members

	public bool Execute()
	{
		_gitCLI.ManualGitExePath = Path.GetFullPath(_txtmSysGitPath.Text.Trim());
		_gitCLI.AutodetectGitExePath = _radAlwaysAutodetect.Checked;
		_gitCLI.LogCalls = _chkLogCLICalls.Checked;
		_gitCLI.EnableAnsiCodepageFallback = _chkFallbackToAnsi.Checked;
		return true;
	}

	#endregion
}
