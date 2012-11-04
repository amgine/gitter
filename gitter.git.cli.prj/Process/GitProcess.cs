namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Specialized;
	using System.Text;
	using System.IO;
	using System.Diagnostics;

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
			var output = Exec(new GitInput(new Command("--version")));
			output.ThrowOnBadReturnCode();
			var parser = new GitParser(output.Output);
			return parser.ReadVersion();
		}

		private static void EnsureEnvironmentVariableExists(ProcessStartInfo psi, string variable, string value, bool resetIfExists = false)
		{
			EnsureEnvironmentVariableExists(psi.EnvironmentVariables, variable, value, resetIfExists);
		}

		private static void EnsureEnvironmentVariableExists(StringDictionary dictionary, string variable, string value, bool resetIfExists = false)
		{
			if(!dictionary.ContainsKey(variable))
			{
				dictionary.Add(variable, value);
			}
			else if(resetIfExists)
			{
				dictionary[variable] = value;
			}
		}

		public static void SetCriticalEnvironmentVariables(ProcessStartInfo psi)
		{
			EnsureEnvironmentVariableExists(psi, GitEnvironment.PlinkProtocol, "ssh");
			EnsureEnvironmentVariableExists(psi, GitEnvironment.Home, UserProfile);
			EnsureEnvironmentVariableExists(psi, GitEnvironment.GitAskPass, _askPassUtilityPath);
			EnsureEnvironmentVariableExists(psi, GitEnvironment.SshAskPass, _askPassUtilityPath);
			EnsureEnvironmentVariableExists(psi, GitEnvironment.Display, "localhost:0.0");
		}

		private static Encoding _defaultEncoding;
		private static bool _enableCodepageFallback;
		private static string _gitInstallationPath;
		private static string _askPassUtilityPath;
		private static string _gitExePath;
		private static string _shExePath;
		private static string _gitkCmdPath;
		private static readonly string UserProfile;

		static GitProcess()
		{
			UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			DefaultEncoding = Encoding.UTF8;

			_askPassUtilityPath = Path.Combine(
				Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]),
				"gitter.askpass.exe");
		}

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

		public static GitOutput Exec(GitInput input)
		{
			Verify.Argument.IsNotNull(input, "input");

			var stdOutReader = new AsyncTextReader();
			var stdErrReader = new AsyncTextReader();
			var executor = CreateExecutor(stdOutReader, stdErrReader);
			var exitCode = executor.Execute(input);
			return new GitOutput(
				stdOutReader.GetText(),
				stdErrReader.GetText(),
				exitCode);
		}

		public static ProcessExecutor CreateExecutor(IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver)
		{
			return new ProcessExecutor(_gitExePath, stdOutReceiver, stdErrReceiver);
		}

		public static GitAsync ExecAsync(GitInput input)
		{
			Verify.Argument.IsNotNull(input, "input");

			var psi = new ProcessStartInfo()
			{
				Arguments = input.GetArguments(),
				WindowStyle = ProcessWindowStyle.Normal,
				UseShellExecute = false,
				StandardOutputEncoding = input.Encoding,
				StandardErrorEncoding = input.Encoding,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				LoadUserProfile = true,
				FileName = _gitExePath,
				ErrorDialog = false,
				CreateNoWindow = true,
			};
			if(!string.IsNullOrEmpty(input.WorkingDirectory))
			{
				psi.WorkingDirectory = input.WorkingDirectory;
			}
			SetCriticalEnvironmentVariables(psi);
			if(input.Environment != null)
			{
				foreach(var opt in input.Environment)
				{
					psi.EnvironmentVariables[opt.Key] = opt.Value;
				}
			}
			return new GitAsync(new Process()
			{
				StartInfo = psi,
				EnableRaisingEvents = true,
			});
		}

		public static GitInteractive ExecInteractive(GitInput input)
		{
			Verify.Argument.IsNotNull(input, "input");

			var p = new Process();
			var psi = new ProcessStartInfo()
			{
				Arguments = input.GetArguments(),
				WindowStyle = ProcessWindowStyle.Normal,
				UseShellExecute = false,
				StandardOutputEncoding = input.Encoding,
				StandardErrorEncoding = input.Encoding,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				LoadUserProfile = true,
				FileName = _gitExePath,
				ErrorDialog = false,
				CreateNoWindow = true,
			};
			if(!string.IsNullOrEmpty(input.WorkingDirectory))
			{
				psi.WorkingDirectory = input.WorkingDirectory;
			}
			SetCriticalEnvironmentVariables(psi);
			if(input.Environment != null)
			{
				foreach(var opt in input.Environment)
				{
					psi.EnvironmentVariables[opt.Key] = opt.Value;
				}
			}
			p.StartInfo = psi;
			p.Start();
			return new GitInteractive(p);
		}
	}
}
