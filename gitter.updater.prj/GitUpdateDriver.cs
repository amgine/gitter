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

namespace gitter.Updater;

using System;
using System.IO;
using System.Text;
using System.Diagnostics;

using Resources = gitter.Updater.Properties.Resources;

class GitUpdateDriver : IUpdateDriver
{
	public string Name => "git";

	private static string? GetFullPath(string filename)
	{
		string environmentVariable = Environment.GetEnvironmentVariable("PATH") ?? "";
		if(!string.IsNullOrEmpty(environmentVariable))
		{
			foreach(string str2 in environmentVariable.Split([Path.PathSeparator], StringSplitOptions.RemoveEmptyEntries))
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

	private static string? DetectGitExePath()
	{
		var fullPath = GetFullPath("git.exe");
		if(!string.IsNullOrEmpty(fullPath))
		{
			return fullPath;
		}
		var str2 = GetFullPath("git.cmd") ?? "";
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

	public IUpdateProcess? CreateProcess(CommandLine cmdline)
	{
		Version? ver;
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
		return new UpdateFromGitRepositoryProcess(ver, git!, url!, target!, skipVersionCheck);
	}
}

/// <summary>Updates gitter directly from git repository.</summary>
class UpdateFromGitRepositoryProcess : UpdateProcessBase
{
	#region Const

	const string msbuildExePathPart2 = @"Microsoft.NET\Framework\v4.0.30319\msbuild.exe";
	const string buildFileName = @"master.build";
	const string buildFileTaskName = @"BuildRelease";
	const string buildOutputPath = @"Output\Release";

	#endregion

	private readonly Version _currentVersion;
	private readonly string _repoDownloadPath;
	private readonly string _repoUrl;
	private readonly string _gitExePath;
	private readonly bool _skipVersionCheck;

	private Version? GetRemoteMasterVersion()
	{
		Version? result = null;
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
				var masterSHA1 = default(string);
				var lines = stdout.Split(['\n'], StringSplitOptions.RemoveEmptyEntries);
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
			_ = git.StandardError.ReadToEnd();
			_ = git.StandardOutput.ReadToEnd();
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

	protected override void Cleanup()
	{
		try
		{
			Utility.EnsureDirectoryDoesNotExist(_repoDownloadPath);
		}
		catch
		{
		}
	}

	public UpdateFromGitRepositoryProcess(Version currentVersion, string gitExePath, string repoUrl, string targetDirectory, bool skipVersionCheck = false)
		: base(targetDirectory)
	{
		_currentVersion = currentVersion;
		_repoDownloadPath = Path.Combine(Path.GetTempPath(), "gitter-updater", "src-download"); /*Path.Combine(typeof(UpdateFromGitRepositoryProcess).Assembly.Location, "src-download");*/
		_gitExePath = gitExePath;
		_repoUrl = repoUrl;
		_skipVersionCheck = skipVersionCheck;
	}

	protected override void NotifyInitializing(UpdateProcessMonitor monitor)
	{
		base.NotifyInitializing(monitor);
		monitor.MaximumProgress = 10;
	}

	protected override void UpdateProc()
	{
		if(Monitor.CancelRequested)
		{
			Monitor.ReportCancelled();
			return;
		}
		if(!_skipVersionCheck)
		{
			Monitor.Stage = "Checking for new version...";
			var ver = GetRemoteMasterVersion();
			if(ver == null)
			{
				Monitor.ReportFailure("Failed to check for new version");
				return;
			}
			else if(ver <= _currentVersion)
			{
				Monitor.Stage = "Your version is up to date";
				Monitor.CurrentProgress = Monitor.MaximumProgress;
				Monitor.ReportSuccess();
				return;
			}
			if(Monitor.CancelRequested)
			{
				Monitor.ReportCancelled();
				return;
			}
		}
		Monitor.Stage = "Downloading source code from " + _repoUrl + "...";
		Monitor.CurrentProgress = 1;
		if(!DownloadSourceCode())
		{
			Monitor.ReportFailure("Failed to download source code.");
			return;
		}
		if(Monitor.CancelRequested)
		{
			Monitor.ReportCancelled();
			return;
		}
		Monitor.Stage = "Compiling program...";
		Monitor.CurrentProgress = 4;
		if(!BuildSourceCode())
		{
			Monitor.ReportFailure("Failed to build source code.");
			return;
		}
		if(Monitor.CancelRequested)
		{
			Monitor.ReportCancelled();
			return;
		}
		InstallApplication(from: Path.Combine(_repoDownloadPath, buildOutputPath));
	}
}
