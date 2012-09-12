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
			_btnDownload.Text = Resources.StrDownload;
			_btnRefreshLatestVersion.Text = Resources.StrRefresh;

			if(_gitCLI.AutodetectGitExePath)
			{
				_versionPath = GitProcess.GitExePath;
				_radAlwaysAutodetect.Checked = true;
			}
			else
			{
				_versionPath = _gitCLI.ManualGitExePath;
				_radSpecifyManually.Checked = true;
			}
			_txtmSysGitPath.Text = _versionPath;

			_chkLogCLICalls.Checked = _gitCLI.LogCLICalls;
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
				_gitCLI.RefreshGitVersion();
				version = _gitCLI.GitVersion;
			}
			catch { }
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
						catch { }
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
			{
				_downloader.Download();
			}
		}

		#region IExecutableDialog Members

		public bool Execute()
		{
			_gitCLI.ManualGitExePath = Path.GetFullPath(_txtmSysGitPath.Text.Trim());
			_gitCLI.AutodetectGitExePath = _radAlwaysAutodetect.Checked;
			_gitCLI.LogCLICalls = _chkLogCLICalls.Checked;
			_gitCLI.EnableAnsiCodepageFallback = _chkFallbackToAnsi.Checked;
			return true;
		}

		#endregion
	}
}
