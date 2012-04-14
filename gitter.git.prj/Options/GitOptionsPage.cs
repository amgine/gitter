namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.IO;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;

	using gitter.Git.AccessLayer.CLI;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public partial class GitOptionsPage : PropertyPage, IExecutableDialog
	{
		public static readonly new Guid Guid = new Guid("22102F21-350D-426A-AE8A-685928EAABE5");
		private string _versionPath;

		private MsysGitDownloader _downloader;

		public GitOptionsPage()
			: base(Guid)
		{
			InitializeComponent();

			Text = Resources.StrGit;

			_cmbAccessMethod.SelectedIndex = 0;

			_grpRepositoryAccessor.Text = Resources.StrsRepositoryAccessMethod;

			_lblPathToGitExe.Text = Resources.StrPathToGitExe.AddColon();
			_lblAccessmethod.Text = Resources.StrAccessMethod.AddColon();
			_lblFoundVersion.Text = Resources.StrFoundVersion.AddColon();

			_radAlwaysAutodetect.Text = Resources.StrsAlwaysAutodetect;
			_radSpecifyManually.Text = Resources.StrsSpecifyManually;

			_cmdAutoDetect.Text = Resources.StrAutodetect;
			_cmdBrowse.Text = Resources.StrBrowse.AddEllipsis();

			_chkLogCLICalls.Text = Resources.StrsLogCLICalls;
			_chkFallbackToAnsi.Text = Resources.StrsFallbackToAnsi + ' ' + Resources.StrlRestartRequired.SurroundWithBraces();

			_lblLatestVersionValue.Text = Resources.StrsSearching.AddEllipsis();
			_lblLatestVersion.Text = Resources.StrLatestVersion.AddColon();
			_btnDownload.Text = Resources.StrDownload;
			_btnRefreshLatestVersion.Text = Resources.StrRefresh;

			if(RepositoryProvider.AutodetectGitExePath)
			{
				_versionPath = GitProcess.GitExePath;
				_radAlwaysAutodetect.Checked = true;
			}
			else
			{
				_versionPath = RepositoryProvider.ManualGitExePath;
				_radSpecifyManually.Checked = true;
			}
			_txtmSysGitPath.Text = _versionPath;

			_chkLogCLICalls.Checked = RepositoryProvider.LogCLICalls;
			_chkFallbackToAnsi.Checked = GitProcess.EnableAnsiCodepageFallback;

			var version = RepositoryProvider.GitVersion;
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

		protected override void OnShown()
		{
			base.OnShown();

			if(_downloader == null)
				RefreshLatestVersion();
		}

		private void RefreshLatestVersion()
		{
			_btnDownload.Enabled = false;
			_btnRefreshLatestVersion.Enabled = false;
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
			if(_downloader != null)
			{
				if(_downloader.IsAvailable)
				{
					_btnDownload.Enabled = (RepositoryProvider.GitVersion == null) ||
						(_downloader.LatestVersion > RepositoryProvider.GitVersion);
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
					if(File.Exists(path))
					{
						var version = GitProcess.CheckVersion(path);
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
				catch
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
			if(_downloader != null && _downloader.IsAvailable)
				_downloader.Download();
		}

		#region IExecutableDialog Members

		public bool Execute()
		{
			RepositoryProvider.AutodetectGitExePath = _radAlwaysAutodetect.Checked;
			RepositoryProvider.ManualGitExePath = Path.GetFullPath(_txtmSysGitPath.Text.Trim());
			RepositoryProvider.LogCLICalls = _chkLogCLICalls.Checked;
			GitProcess.EnableAnsiCodepageFallback = _chkFallbackToAnsi.Checked;
			GitProcess.UpdateGitExePath();
			return true;
		}

		#endregion
	}
}
