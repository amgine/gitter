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

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework.CLI;

#nullable enable

internal static class GitProcess
{
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
		Assert.IsNotNull(psi);

		psi.EnsureEnvironmentVariableExists(GitEnvironment.PlinkProtocol, @"ssh");
		psi.EnsureEnvironmentVariableExists(GitEnvironment.Home, UserProfile);
		psi.EnsureEnvironmentVariableExists(GitEnvironment.GitAskPass, _askPassUtilityPath);
		psi.EnsureEnvironmentVariableExists(GitEnvironment.SshAskPass, _askPassUtilityPath);
		psi.EnsureEnvironmentVariableExists(GitEnvironment.Display, @"localhost:0.0");
	}

	private static bool _enableCodepageFallback;
	private static string? _gitInstallationPath;
	private static readonly string _askPassUtilityPath = Path.Combine(
		AppContext.BaseDirectory, @"gitter.askpass.exe");
	private static string? _gitExePath;
	private static string? _shExePath;
	private static string? _gitkCmdPath;
	private static readonly string UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

	public static Encoding DefaultEncoding { get; private set; } = Encoding.UTF8;

	public static bool EnableAnsiCodepageFallback
	{
		get => _enableCodepageFallback;
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

	public static string? GitExePath
	{
		get => _gitExePath;
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

	public static Process? ExecNormal(GitInput input)
	{
		Verify.Argument.IsNotNull(input);
		Verify.State.IsTrue(_gitExePath is not null, "Git executable path is not set.");

		var psi = new ProcessStartInfo(_gitExePath)
		{
			Arguments       = input.GetArguments(),
			WindowStyle     = ProcessWindowStyle.Normal,
			UseShellExecute = false,
			LoadUserProfile = true,
			ErrorDialog     = false,
			CreateNoWindow  = true,
		};
		if(!string.IsNullOrEmpty(input.WorkingDirectory))
		{
			psi.WorkingDirectory = input.WorkingDirectory;
		}
		if(input.Environment is { Count: not 0 })
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
			catch(Exception exc) when(!exc.IsCritical())
			{
				return false;
			}
		}
	}

	public static Process? ExecSh(string repository, string command)
	{
		Verify.State.IsTrue(_shExePath is not null, "Git bash executable path is not set.");

		var psi = new ProcessStartInfo(_shExePath)
		{
			Arguments        = command,
			WorkingDirectory = repository,
			WindowStyle      = ProcessWindowStyle.Normal,
			LoadUserProfile  = true,
			ErrorDialog      = false,
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
			catch(Exception exc) when(!exc.IsCritical())
			{
				return false;
			}
		}
	}

	public static Process? ExecGitk(string repository, string command)
	{
		Verify.State.IsTrue(_gitkCmdPath is not null, "Gitk executable path is not set.");

		var psi = new ProcessStartInfo(_gitkCmdPath)
		{
			Arguments        = command,
			WorkingDirectory = repository,
			WindowStyle      = ProcessWindowStyle.Normal,
			LoadUserProfile  = true,
			ErrorDialog      = false,
			UseShellExecute  = false,
			CreateNoWindow   = true,
		};
		SetCriticalEnvironmentVariables(psi);
		return Process.Start(psi);
	}

	public static GitOutput Execute(GitInput input)
	{
		Verify.Argument.IsNotNull(input);

		var stdOutReader = new AsyncTextReader();
		var stdErrReader = new AsyncTextReader();
		var executor = new GitProcessExecutor(GitExePath);
		var exitCode = executor.Execute(input, stdOutReader, stdErrReader);
		return new GitOutput(
			stdOutReader.GetText(),
			stdErrReader.GetText(),
			exitCode);
	}

	public static async Task<GitOutput> ExecuteAsync(GitInput input, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(input);

		var stdOutReader = new AsyncTextReader();
		var stdErrReader = new AsyncTextReader();
		var executor = new GitProcessExecutor(GitExePath);
		var exitCode = await executor
			.ExecuteAsync(input, stdOutReader, stdErrReader, ProcessExecutor.CancellationMethods.KillProcess, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		return new GitOutput(
			stdOutReader.GetText(),
			stdErrReader.GetText(),
			exitCode);
	}
}
