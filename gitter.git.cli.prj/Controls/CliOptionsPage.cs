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
	using System.ComponentModel;
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
		public static readonly new Guid Guid = new Guid("EF9DFCAC-FDD1-4B69-AD0E-3783035FA13C");
		private string _versionPath;

		private MSysGitDownloader _downloader;
		private readonly GitCLI _gitCLI;

		public CliOptionsPage(IGitAccessor gitAccessor)
			: base(Guid)
		{
			Verify.Argument.IsNotNull(gitAccessor, "gitAccessor");

			_gitCLI = (GitCLI)gitAccessor;

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

			var version = TryGetVersion();
			if(version != null)
			{
				_lblVersion.Text = version.ToString();
			}
			else
			{
				_lblVersion.Text = Resources.StrlUnavailable.SurroundWith("<", ">");
			}

			GitterApplication.FontManager.InputFont.Apply(_txtmSysGitPath);
		}

		private Version TryGetVersion()
		{
			Version version = null;
			try
			{
				_gitCLI.InvalidateGitVersion();
				version = _gitCLI.GitVersion;
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
			}
			return version;
		}

		protected override void OnShown()
		{
			base.OnShown();

			if(_downloader == null)
			{
				RefreshLatestVersion();
			}
		}

		private void RefreshLatestVersion()
		{
			_btnDownload.Enabled = false;
			_btnRefreshLatestVersion.Enabled = false;
			_lblLatestVersionValue.Text = Resources.StrsSearching.AddEllipsis();
			MSysGitDownloader.BeginCreate((ar) =>
				{
					_downloader = MSysGitDownloader.EndCreate(ar);
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
				});
		}

		private void UpdateLatestVersion()
		{
			if(_downloader != null)
			{
				if(_downloader.IsAvailable)
				{
					var version = TryGetVersion();
					_btnDownload.Enabled =
						(version == null) ||
						(_downloader.LatestVersion > version);
					_lblLatestVersionValue.Text = _downloader.LatestVersion.ToString();
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
			_txtmSysGitPath.Text = GitProcess.DetectGitExePath();
		}

		private void _cmdBrowse_Click(object sender, EventArgs e)
		{
			_openFileDialog.Filter = "Git Executable|git.exe";
			_openFileDialog.FileName = "git.exe";
			var path = GitProcess.DetectGitExePath();
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
						catch(Exception exc)
						{
							if(exc.IsCritical())
							{
								throw;
							}
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
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
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
				ProgressForm.MonitorTaskAsModalWindow(this, "MSysGit Installation", _downloader.DownloadAndInstallAsync);
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				GitterApplication.MessageBoxService.Show(
					this, exc.Message, "MSysGit Installation Failed", MessageBoxButton.Close, MessageBoxIcon.Error);
				return;
			}
			var version = TryGetVersion();
			if(version != null)
			{
				_lblVersion.Text = version.ToString();
				_versionPath = _gitCLI.GitExecutablePath;
				if(string.IsNullOrWhiteSpace(_txtmSysGitPath.Text))
				{
					_txtmSysGitPath.Text = _versionPath;
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
}
