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

namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Text;

	using gitter.Framework.CLI;

	internal static class GitProcess
	{
		private static string GetFullPath(string filename)
		{
			string environmentVariable = Environment.GetEnvironmentVariable("PATH");
			if(!string.IsNullOrWhiteSpace(environmentVariable))
			{
				foreach(string str2 in environmentVariable.Split(
					new char[] { Path.PathSeparator },
					StringSplitOptions.RemoveEmptyEntries))
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

		public static string DetectGitExePath()
		{
			const string GitExe = "git.exe";
			const string GitCmd = "git.cmd";

			string gitExeFullPath = GetFullPath(GitExe);
			if(!string.IsNullOrWhiteSpace(gitExeFullPath))
			{
				string gitWrapperExe = string.Format("{0}cmd{0}{1}", Path.DirectorySeparatorChar, GitExe);
				if(gitExeFullPath.ToLower().EndsWith(gitWrapperExe))
				{
					var realGitExe = Path.Combine(
						gitExeFullPath.Substring(0, gitExeFullPath.Length - gitWrapperExe.Length),
						@"bin", GitExe);
					if(File.Exists(realGitExe))
					{
						return realGitExe;
					}
				}
				return gitExeFullPath;
			}
			string gitCmdFullPath = GetFullPath(GitCmd);
			if(!string.IsNullOrWhiteSpace(gitCmdFullPath))
			{
				string gitWrapperCmd = string.Format("{0}cmd{0}{1}", Path.DirectorySeparatorChar, GitCmd);
				if(gitCmdFullPath.ToLower().EndsWith(gitWrapperCmd))
				{
					var realGitExe = Path.Combine(
						gitCmdFullPath.Substring(0, gitCmdFullPath.Length - gitWrapperCmd.Length),
						@"bin", GitExe);
					if(File.Exists(realGitExe))
					{
						return realGitExe;
					}
				}
				return gitCmdFullPath;
			}
			var programFilesPath = Environment.Is64BitOperatingSystem ?
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) :
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			var defaultInstallationPath = Path.Combine(
				programFilesPath, @"Git\bin", GitExe);
			if(File.Exists(defaultInstallationPath))
			{
				return defaultInstallationPath;
			}
			return null;
		}

		public static Version CheckVersion(string gitExe)
		{
			var stdErrReceiver = new AsyncTextReader();
			var stdOutReceiver = new AsyncTextReader();
			var executor = new GitProcessExecutor(gitExe);
			var exitCode = executor.Execute(new GitInput(new Command("--version")), stdOutReceiver, stdErrReceiver);
			var output = new GitOutput(stdOutReceiver.GetText(), stdErrReceiver.GetText(), exitCode);
			output.ThrowOnBadReturnCode();
			var parser = new GitParser(output.Output);
			return parser.ReadVersion();
		}

		public static void SetCriticalEnvironmentVariables(ProcessStartInfo psi)
		{
			psi.EnsureEnvironmentVariableExists(GitEnvironment.PlinkProtocol, "ssh");
			psi.EnsureEnvironmentVariableExists(GitEnvironment.Home, UserProfile);
			psi.EnsureEnvironmentVariableExists(GitEnvironment.GitAskPass, _askPassUtilityPath);
			psi.EnsureEnvironmentVariableExists(GitEnvironment.SshAskPass, _askPassUtilityPath);
			psi.EnsureEnvironmentVariableExists(GitEnvironment.Display, "localhost:0.0");
		}

		private static Encoding _defaultEncoding = Encoding.UTF8;
		private static bool _enableCodepageFallback;
		private static string _gitInstallationPath;
		private static string _askPassUtilityPath = Path.Combine(
				Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
				"gitter.askpass.exe");
		private static string _gitExePath;
		private static string _shExePath;
		private static string _gitkCmdPath;
		private static readonly string UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

		public static Encoding DefaultEncoding
		{
			get { return _defaultEncoding; }
			private set { _defaultEncoding = value; }
		}

		public static bool EnableAnsiCodepageFallback
		{
			get { return _enableCodepageFallback; }
			set
			{
				if(_enableCodepageFallback != value)
				{
					_enableCodepageFallback = value;
					if(value)
					{
						DefaultEncoding = (Encoding)Encoding.UTF8.Clone();
						DefaultEncoding.DecoderFallback = new UTF8DefaultAnsiCodepageFallback();
					}
					else
					{
						DefaultEncoding = Encoding.UTF8;
					}
				}
			}
		}

		public static void UpdateGitExePath(GitCLI gitCLI)
		{
			Verify.Argument.IsNotNull(gitCLI, "gitCLI");

			if(gitCLI.AutodetectGitExePath)
			{
				GitExePath = DetectGitExePath();
			}
			else
			{
				GitExePath = gitCLI.ManualGitExePath;
			}
		}

		public static string GitExePath
		{
			get { return _gitExePath; }
			set
			{
				if(_gitExePath != value)
				{
					if(string.IsNullOrWhiteSpace(value))
					{
						_gitExePath = string.Empty;
						_gitInstallationPath = string.Empty;
						_shExePath = string.Empty;
						_gitkCmdPath = string.Empty;
					}
					else
					{
						_gitExePath = value;
						_gitInstallationPath = Path.GetFullPath(
							Path.Combine(
								Path.GetDirectoryName(_gitExePath),
								".."));
						_shExePath = Path.Combine(_gitInstallationPath, @"bin\sh.exe");
						_gitkCmdPath = Path.Combine(_gitInstallationPath, @"cmd\gitk.cmd");
					}
				}
			}
		}

		public static Process ExecNormal(GitInput input)
		{
			Verify.Argument.IsNotNull(input, "input");

			var psi = new ProcessStartInfo()
			{
				Arguments = input.GetArguments(),
				WindowStyle = ProcessWindowStyle.Normal,
				UseShellExecute = false,
				LoadUserProfile = true,
				FileName = _gitExePath,
				ErrorDialog = false,
				CreateNoWindow = true,
			};
			if(!string.IsNullOrEmpty(input.WorkingDirectory))
			{
				psi.WorkingDirectory = input.WorkingDirectory;
			}
			if(input.Environment != null)
			{
				foreach(var opt in input.Environment)
				{
					psi.EnvironmentVariables[opt.Key] = opt.Value;
				}
			}
			SetCriticalEnvironmentVariables(psi);
			return Process.Start(psi);
		}

		public static bool CanExecSh
		{
			get
			{
				if(string.IsNullOrWhiteSpace(_shExePath))
				{
					return false;
				}
				try
				{
					return File.Exists(_shExePath);
				}
				catch
				{
					return false;
				}
			}
		}

		public static Process ExecSh(string repository, string command)
		{
			var psi = new ProcessStartInfo()
			{
				Arguments = command,
				WorkingDirectory = repository,
				WindowStyle = ProcessWindowStyle.Normal,
				LoadUserProfile = true,
				FileName = _shExePath,
				ErrorDialog = false,
			};
			return Process.Start(psi);
		}

		public static bool CanExecGitk
		{
			get
			{
				if(string.IsNullOrWhiteSpace(_gitkCmdPath))
				{
					return false;
				}
				try
				{
					return File.Exists(_gitkCmdPath);
				}
				catch
				{
					return false;
				}
			}
		}

		public static Process ExecGitk(string repository, string command)
		{
			var psi = new ProcessStartInfo()
			{
				Arguments = command,
				WorkingDirectory = repository,
				WindowStyle = ProcessWindowStyle.Normal,
				LoadUserProfile = true,
				FileName = _gitkCmdPath,
				ErrorDialog = false,
				UseShellExecute = false,
				CreateNoWindow = true,
			};
			SetCriticalEnvironmentVariables(psi);
			return Process.Start(psi);
		}

		public static GitOutput Execute(GitInput input)
		{
			Verify.Argument.IsNotNull(input, "input");

			var stdOutReader = new AsyncTextReader();
			var stdErrReader = new AsyncTextReader();
			var executor = CreateExecutor();
			var exitCode = executor.Execute(input, stdOutReader, stdErrReader);
			return new GitOutput(
				stdOutReader.GetText(),
				stdErrReader.GetText(),
				exitCode);
		}

		public static GitProcessExecutor CreateExecutor()
		{
			return new GitProcessExecutor(_gitExePath);
		}
	}
}
