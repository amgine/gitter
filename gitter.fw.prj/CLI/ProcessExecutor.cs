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

namespace gitter.Framework.CLI;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public static class ProcessExecutor
{
	public static class CancellationMethods
	{
		public static Action<Process> KillProcess { get; } = static process =>
		{
			Verify.Argument.IsNotNull(process);

			try
			{
				process.Kill();
			}
			catch(Exception exc) when(!exc.IsCritical)
			{
			}
		};

		public static Action<Process> AllowToExecute { get; } = static process =>
		{
			Verify.Argument.IsNotNull(process);
		};
	}
}

/// <summary>Executes process with output redirecting.</summary>
/// <typeparam name="TInput">Process input type.</typeparam>
public abstract class ProcessExecutor<TInput>
	where TInput : class
{
	/// <summary>Initializes a new instance of the <see cref="ProcessExecutor{TInput}"/> class.</summary>
	/// <param name="exeFileName">Path to executable file.</param>
	public ProcessExecutor(string exeFileName)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(exeFileName);

		ExeFileName = exeFileName;
	}

	public string ExeFileName { get; }

	protected virtual ProcessStartInfo InitializeStartInfo(TInput input)
		=> new(ExeFileName)
		{
			WindowStyle            = ProcessWindowStyle.Hidden,
			UseShellExecute        = false,
			RedirectStandardInput  = true,
			RedirectStandardOutput = true,
			RedirectStandardError  = true,
			LoadUserProfile        = true,
			ErrorDialog            = false,
			CreateNoWindow         = true,
		};

	protected virtual Process CreateProcess(TInput input)
		=> new() { StartInfo = InitializeStartInfo(input) };

	public int Execute(TInput input, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver)
	{
		Verify.Argument.IsNotNull(input);

		using var process = CreateProcess(input);
		process.Start();
		stdOutReceiver?.Initialize(process, process.StandardOutput);
		stdErrReceiver?.Initialize(process, process.StandardError);
		try
		{
			process.WaitForExit();
		}
		finally
		{
			stdErrReceiver?.WaitForEndOfStream();
			stdOutReceiver?.WaitForEndOfStream();
		}
		return process.ExitCode;
	}

	private static async Task<int> ExecuteAsyncCore(Process process, IOutputReceiver? stdOutReceiver, IOutputReceiver? stdErrReceiver,
		Action<Process> cancellationMethod, CancellationToken cancellationToken)
	{
		Assert.IsNotNull(process);
		Assert.IsNotNull(cancellationMethod);

		var task = process.StartAsync();
		using(cancellationToken.Register(() =>
		{
			stdErrReceiver?.NotifyCanceled();
			stdOutReceiver?.NotifyCanceled();
			cancellationMethod(process);
		}))
		{
			stdOutReceiver?.Initialize(process, process.StandardOutput);
			stdErrReceiver?.Initialize(process, process.StandardError);
			try
			{
				return await task.ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				stdErrReceiver?.WaitForEndOfStream();
				stdOutReceiver?.WaitForEndOfStream();
			}
		}
	}

	private static async Task<int> ExecuteAsyncCore(Process process, IOutputReceiver? stdOutReceiver, IOutputReceiver? stdErrReceiver)
	{
		Assert.IsNotNull(process);

		var task = process.StartAsync();
		stdOutReceiver?.Initialize(process, process.StandardOutput);
		stdErrReceiver?.Initialize(process, process.StandardError);
		try
		{
			return await task.ConfigureAwait(continueOnCapturedContext: false);
		}
		finally
		{
			stdErrReceiver?.WaitForEndOfStream();
			stdOutReceiver?.WaitForEndOfStream();
		}
	}

	public async Task<int> ExecuteAsync(TInput input, IOutputReceiver? stdOutReceiver, IOutputReceiver? stdErrReceiver,
		Action<Process>? cancellationMethod = null, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(input);

		cancellationToken.ThrowIfCancellationRequested();
		using var process = CreateProcess(input);
		var canCancel = cancellationToken.CanBeCanceled
			&& cancellationMethod is not null
			&& cancellationMethod != ProcessExecutor.CancellationMethods.AllowToExecute;
		var task = canCancel
			? ExecuteAsyncCore(process, stdOutReceiver, stdErrReceiver, cancellationMethod!, cancellationToken)
			: ExecuteAsyncCore(process, stdOutReceiver, stdErrReceiver);
		return await task.ConfigureAwait(continueOnCapturedContext: false);
	}
}
