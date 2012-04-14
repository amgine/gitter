namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Text;
	using System.IO;
	using System.Diagnostics;

	internal static class GitProcess
	{
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

		public static string DetectGitExePath()
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

		public static Version CheckVersion(string gitExe)
		{
			GitOutput res;
			using(var p = new Process())
			{
				var psi = new ProcessStartInfo()
				{
					Arguments = "--version",
					WindowStyle = ProcessWindowStyle.Normal,
					UseShellExecute = false,
					StandardOutputEncoding = Encoding.UTF8,
					StandardErrorEncoding = Encoding.UTF8,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					LoadUserProfile = true,
					FileName = _gitExePath,
					ErrorDialog = false,
					CreateNoWindow = true,
				};
				EnsureEnvironmentVariableExists(psi, GitEnvironment.Home, UserProfile);
				p.StartInfo = psi;
				p.Start();
				var stdout = p.StandardOutput.ReadToEnd();
				var stderr = p.StandardError.ReadToEnd();
				p.WaitForExit();
				var code = p.ExitCode;
				res = new GitOutput(stdout, stderr, code);
				p.Close();
			}
			res.ThrowOnBadReturnCode();
			var parser = new GitParser(res.Output);
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

		private static void SetCriticalEnvironmentVariables(ProcessStartInfo psi)
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
			UpdateGitExePath();
			UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			DefaultEncoding = Encoding.UTF8;

			_askPassUtilityPath = Path.Combine(
				Path.GetDirectoryName(System.Windows.Forms.Application.StartupPath),
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

		public static void UpdateGitExePath()
		{
			if(RepositoryProvider.AutodetectGitExePath)
			{
				GitExePath = DetectGitExePath();
			}
			else
			{
				GitExePath = RepositoryProvider.ManualGitExePath;
			}
		}

		public static string GitExePath
		{
			get { return _gitExePath; }
			set
			{
				if(_gitExePath != value)
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

		public static Process ExecNormal(GitInput input)
		{
			if(input == null) throw new ArgumentNullException("input");

			var p = new Process();
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
				psi.WorkingDirectory = input.WorkingDirectory;
			if(input.Environment != null)
			{
				foreach(var opt in input.Environment)
				{
					psi.EnvironmentVariables[opt.Key] = opt.Value;
				}
			}
			SetCriticalEnvironmentVariables(psi);
			p.StartInfo = psi;
			p.Start();
			return p;
		}

		public static Process ExecSh(string repository, string command)
		{
			var p = new Process();
			var psi = new ProcessStartInfo()
			{
				Arguments = command,
				WorkingDirectory = repository,
				WindowStyle = ProcessWindowStyle.Normal,
				LoadUserProfile = true,
				FileName = _shExePath,
				ErrorDialog = false,
			};
			p.StartInfo = psi;
			p.Start();
			return p;
		}

		public static Process ExecGitk(string repository, string command)
		{
			var p = new Process();
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
			p.StartInfo = psi;
			p.Start();
			return p;
		}

		public static GitOutput Exec(GitInput input)
		{
			if(input == null) throw new ArgumentNullException("input");

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
			GitOutput output;
			using(var process = new Process()
			{
				StartInfo = psi,
			})
			{
				process.Start();
				var stdout = process.StandardOutput.ReadToEnd();
				var stderr = process.StandardError.ReadToEnd();
				process.WaitForExit();
				var code = process.ExitCode;
				output = new GitOutput(stdout, stderr, code);
				process.Close();
			}
			return output;
		}

		public static GitAsync ExecAsync(GitInput input)
		{
			if(input == null) throw new ArgumentNullException("input");

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
			if(input == null) throw new ArgumentNullException("input");

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
