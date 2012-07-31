namespace gitter.Updater
{
	using System;
	using System.IO;
	using System.Text;
	using System.Diagnostics;
	using System.Security;
	using System.Runtime.InteropServices;

	using Resources = gitter.Updater.Properties.Resources;

	class GitUpdateDriver : IUpdateDriver
	{
		public string Name
		{
			get { return "git"; }
		}

		private static string GetFullPath(string filename)
		{
			string environmentVariable = Environment.GetEnvironmentVariable("PATH");
			if(!string.IsNullOrEmpty(environmentVariable))
			{
				foreach(string str2 in environmentVariable.Split(new char[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries))
				{
					try
					{
						string path = Path.Combine(str2, filename);
						if(File.Exists(path))
						{
							return path;
						}
					}
					catch
					{
					}
				}
			}
			return null;
		}

		private static string DetectGitExePath()
		{
			string fullPath = GetFullPath("git.exe");
			if(!string.IsNullOrEmpty(fullPath))
			{
				return fullPath;
			}
			string str2 = GetFullPath("git.cmd");
			if(string.IsNullOrEmpty(str2))
			{
				return null;
			}
			int length = str2.ToLower().LastIndexOf(string.Format("{0}cmd{0}", Path.DirectorySeparatorChar));
			if(length == -1)
			{
				return null;
			}
			return Path.Combine(str2.Substring(0, length), @"bin\git.exe");
		}

		public IUpdateProcess CreateProcess(CommandLine cmdline)
		{
			Version ver;
			var version = cmdline["version"];
			if(string.IsNullOrEmpty(version))
			{
				ver = new Version(0, 0, 0, 0);
			}
			else
			{
				if(!Version.TryParse(version, out ver))
				{
					ver = new Version(0, 0, 0, 0);
				}
			}
			var git = cmdline["git"];
			if(string.IsNullOrEmpty(git))
			{
				git = DetectGitExePath();
				if(string.IsNullOrEmpty(git)) return null;
			}
			var url = cmdline["url"];
			if(string.IsNullOrEmpty(url)) return null;
			var target = cmdline["target"];
			if(string.IsNullOrEmpty(target)) return null;
			bool skipVersionCheck = cmdline.IsDefined("skipversioncheck");
			return new UpdateFromGitRepositoryProcess(ver, git, url, target, skipVersionCheck);
		}
	}

	/// <summary>Updates gitter directly from git repository.</summary>
	class UpdateFromGitRepositoryProcess : IUpdateProcess
	{
		#region Const

		const string msbuildExePathPart2 = @"Microsoft.NET\Framework\v4.0.30319\msbuild.exe";
		const string buildFileName = @"master.build";
		const string buildFileTaskName = @"BuildRelease";
		const string buildOutputPath = @"Output\Release";
		const string mainBinaryName = "gitter.exe";

		#endregion

		private readonly Version _currentVersion;
		private readonly string _repoDownloadPath;
		private readonly string _repoUrl;
		private readonly string _targetDirectory;
		private readonly string _gitExePath;
		private readonly bool _skipVersionCheck;

		private IAsyncResult _currentProcess;
		private UpdateProcessMonitor _monitor;

		[DllImport("user32.dll")]
		[SuppressUnmanagedCodeSecurity]
		private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		private Version GetRemoteMasterVersion()
		{
			Version result = null;
			using(var git = new Process())
			{
				var args = "ls-remote --heads --tags " + _repoUrl;
				var psi = new ProcessStartInfo()
				{
					FileName = _gitExePath,
					Arguments = args,
					WindowStyle = ProcessWindowStyle.Normal,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					RedirectStandardInput = true,
					StandardOutputEncoding = Encoding.UTF8,
					StandardErrorEncoding = Encoding.UTF8,
					LoadUserProfile = true,
					ErrorDialog = false,
					CreateNoWindow = true,
				};
				if(!psi.EnvironmentVariables.ContainsKey("PLINK_PROTOCOL"))
				{
					psi.EnvironmentVariables.Add("PLINK_PROTOCOL", "ssh");
				}
				if(!psi.EnvironmentVariables.ContainsKey("HOME"))
				{
					var UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
					psi.EnvironmentVariables.Add("HOME", UserProfile);
				}
				git.StartInfo = psi;
				git.Start();
				var stdout = git.StandardOutput.ReadToEnd();
				var stderr = git.StandardError.ReadToEnd();
				git.WaitForExit();
				if(git.ExitCode == 0)
				{
					string masterSHA1 = null;
					var lines = stdout.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
					for(int i = 0; i < lines.Length; ++i)
					{
						if(lines[i].Length > 41)
						{
							var refname = lines[i].Substring(41).Trim();
							if(masterSHA1 == null)
							{
								if(refname == "refs/heads/master")
								{
									masterSHA1 = lines[i].Substring(0, 40);
								}
							}
							else
							{
								if(lines[i].Substring(0, 40) == masterSHA1)
								{
									int s = 0;
									int e = refname.Length - 1;
									while(s < refname.Length && !char.IsDigit(refname[s])) ++s;
									while(e > 0 && !char.IsDigit(refname[e])) --e;
									if(e > s && s > 9 && (refname[s - 1] == 'v' || (refname[s - 1] == '-' && refname[s - 2] == 'v')))
									{
										if(Version.TryParse(refname.Substring(s, e - s + 1), out result))
										{
											break;
										}
										else
										{
											result = null;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private bool DownloadSourceCode()
		{
			Utility.EnsureDirectoryExists(_repoDownloadPath);
			Utility.EnsureDirectoryIsEmpty(_repoDownloadPath);
			var args = "clone --depth 1 -- " + "\"" + _repoUrl + "\" " + _repoDownloadPath;
			using(var git = new Process())
			{
				var psi = new ProcessStartInfo()
				{
					FileName = _gitExePath,
					Arguments = args,
					WindowStyle = ProcessWindowStyle.Normal,
					UseShellExecute = false,
					LoadUserProfile = true,
					ErrorDialog = false,
					CreateNoWindow = true,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
				};
				if(!psi.EnvironmentVariables.ContainsKey("PLINK_PROTOCOL"))
				{
					psi.EnvironmentVariables.Add("PLINK_PROTOCOL", "ssh");
				}
				if(!psi.EnvironmentVariables.ContainsKey("HOME"))
				{
					var UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
					psi.EnvironmentVariables.Add("HOME", UserProfile);
				}
				git.StartInfo = psi;
				git.Start();
				var stderr = git.StandardError.ReadToEnd();
				var stdout = git.StandardOutput.ReadToEnd();
				git.WaitForExit();
				return git.ExitCode == 0;
			}
		}

		private bool BuildSourceCode()
		{
			var msbuildPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), msbuildExePathPart2);
			var args = @"/verbosity:minimal /nologo " + buildFileName + @" /t:" + buildFileTaskName;
			using(var msbuild = new Process())
			{
				msbuild.StartInfo = new ProcessStartInfo()
				{
					FileName = msbuildPath,
					WorkingDirectory = _repoDownloadPath,
					Arguments = args,
					WindowStyle = ProcessWindowStyle.Normal,
					UseShellExecute = false,
					LoadUserProfile = true,
					ErrorDialog = false,
					CreateNoWindow = true,
				};
				msbuild.Start();
				msbuild.WaitForExit();
				return msbuild.ExitCode == 0;
			}
		}

		private void KillAllGitterProcesses()
		{
			Utility.KillAllGitterProcesses(_targetDirectory);
		}

		private bool DeployBuildResults()
		{
			var source = new DirectoryInfo(Path.Combine(_repoDownloadPath, buildOutputPath));
			if(Utility.IsRunningWithAdministratorRights)
			{
				Utility.CopyDirectoryContent(source, _targetDirectory);
				return true;
			}
			else
			{
				var cmdline = new StringBuilder();
				cmdline.Append("/forcenewinstance");
				cmdline.Append(' ');
				cmdline.Append("/hidden");
				cmdline.Append(' ');
				cmdline.Append('\"');
				cmdline.Append("/source:" + source);
				cmdline.Append('\"');
				cmdline.Append(' ');
				cmdline.Append('\"');
				cmdline.Append("/target:" + _targetDirectory);
				cmdline.Append('\"');
				string exeFileName;
				using(var p = Process.GetCurrentProcess())
				{
					exeFileName = p.MainModule.FileName;
				}
				using(var p = new Process()
					{
						StartInfo = new ProcessStartInfo(exeFileName, cmdline.ToString())
						{
							CreateNoWindow = true,
							Verb = "runas",
						},
					})
				{
					p.Start();
					p.WaitForExit();
					return p.ExitCode == 0;
				}
			}
		}

		private void CleanUp()
		{
			try
			{
				Utility.EnsureDirectoryDoesNotExist(_repoDownloadPath);
			}
			catch
			{
			}
		}

		private void StartApplication()
		{
			try
			{
				var exeName = Path.Combine(_targetDirectory, mainBinaryName);
				Utility.StartApplication(exeName);
			}
			catch
			{
			}
		}

		public UpdateFromGitRepositoryProcess(Version currentVersion, string gitExePath, string repoUrl, string targetDirectory, bool skipVersionCheck = false)
		{
			_currentVersion = currentVersion;
			_repoDownloadPath = Path.Combine(Path.GetTempPath(), "gitter-updater", "src-download"); /*Path.Combine(typeof(UpdateFromGitRepositoryProcess).Assembly.Location, "src-download");*/
			_gitExePath = gitExePath;
			_repoUrl = repoUrl;
			_targetDirectory = targetDirectory;
			_skipVersionCheck = skipVersionCheck;
		}

		public void BeginUpdate(UpdateProcessMonitor monitor)
		{
			if(_currentProcess != null) throw new InvalidOperationException();

			monitor.Stage = Resources.StrInitializing + "...";
			monitor.MaximumProgress = 10;
			Action proc = UpdateProc;

			_monitor = monitor;
			_currentProcess = proc.BeginInvoke(UpdateProcCallback, proc);
		}

		public void Update(UpdateProcessMonitor monitor)
		{
			_monitor = monitor;
			UpdateProc();
			_monitor = null;
		}

		private void UpdateProc()
		{
			try
			{
				if(_monitor.CancelRequested)
				{
					_monitor.ReportCancelled();
					return;
				}
				if(!_skipVersionCheck)
				{
					_monitor.Stage = "Checking for new version...";
					var ver = GetRemoteMasterVersion();
					if(ver == null)
					{
						_monitor.ReportFailure("Failed to check for new version");
						return;
					}
					else if(ver <= _currentVersion)
					{
						_monitor.Stage = "Your version is up to date";
						_monitor.CurrentProgress = _monitor.MaximumProgress;
						_monitor.ReportSuccess();
						return;
					}
					if(_monitor.CancelRequested)
					{
						_monitor.ReportCancelled();
						return;
					}
				}
				_monitor.Stage = "Downloading source code from " + _repoUrl + "...";
				_monitor.CurrentProgress = 1;
				if(DownloadSourceCode())
				{
					if(_monitor.CancelRequested)
					{
						_monitor.ReportCancelled();
						return;
					}
					_monitor.Stage = "Compiling program...";
					_monitor.CurrentProgress = 4;
					if(BuildSourceCode())
					{
						if(_monitor.CancelRequested)
						{
							_monitor.ReportCancelled();
							return;
						}
						_monitor.Stage = "Installing program...";
						_monitor.CurrentProgress = 8;
						KillAllGitterProcesses();
						if(_monitor.CancelRequested)
						{
							_monitor.ReportCancelled();
							return;
						}
						_monitor.CanCancel = false;
						if(DeployBuildResults())
						{
							_monitor.Stage = "Cleaning up temporary files...";
							_monitor.CurrentProgress = 9;
							CleanUp();
							_monitor.CurrentProgress = 10;
							_monitor.Stage = "Launching program...";
							StartApplication();
							_monitor.ReportSuccess();
						}
						else
						{
							_monitor.ReportFailure("Failed to deploy build results.");
						}
					}
					else
					{
						_monitor.ReportFailure("Failed to build source code.");
					}
				}
				else
				{
					_monitor.ReportFailure("Failed to download source code.");
				}
			}
			catch(Exception exc)
			{
				if(_monitor.CancelRequested)
				{
					_monitor.ReportCancelled();
				}
				else
				{
					_monitor.ReportFailure("Unexpected error:\n" + exc.Message);
				}
			}
			finally
			{
				CleanUp();
			}
		}

		public bool IsUpdating
		{
			get { return _currentProcess != null; }
		}

		private void UpdateProcCallback(IAsyncResult ar)
		{
			var proc = (Action)ar.AsyncState;
			try
			{
				proc.EndInvoke(ar);
			}
			catch(Exception exc)
			{
				_monitor.ReportFailure("Unexpected error:\n" + exc.Message);
			}
			_monitor = null;
			_currentProcess = null;
		}
	}
}
