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

namespace gitter.Framework.CLI
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using System.Threading.Tasks;

	public static class ProcessExecutor
	{
		public static class CancellationMethods
		{
			public static void KillProcess(Process process)
			{
				Verify.Argument.IsNotNull(process, nameof(process));

				try
				{
					process.Kill();
				}
				catch(Exception exc) when(!exc.IsCritical())
				{
				}
			}

			public static void AllowToExecute(Process process)
			{
				Verify.Argument.IsNotNull(process, nameof(process));

			}
		}
	}

	/// <summary>Executes process with output redirecting.</summary>
	/// <typeparam name="TInput">Process input type.</typeparam>
	public abstract class ProcessExecutor<TInput>
		where TInput : class
	{
		/// <summary>Initializes a new instance of the <see cref="ProcessExecutor&lt;TInput&gt;"/> class.</summary>
		/// <param name="exeFileName">Path to exe file.</param>
		/// <param name="stdOutReceiver">STDOUT receiver (can be null).</param>
		/// <param name="stdErrReceiver">STDERR receiver (can be null).</param>
		public ProcessExecutor(string exeFileName)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(exeFileName, nameof(exeFileName));

			ExeFileName = exeFileName;
		}

		public string ExeFileName { get; }

		protected virtual ProcessStartInfo InitializeStartInfo(TInput input)
			=> new ProcessStartInfo(ExeFileName);

		protected virtual Process CreateProcess(TInput input)
			=> new Process { StartInfo = InitializeStartInfo(input) };

		public int Execute(TInput input, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver)
		{
			Verify.Argument.IsNotNull(input, nameof(input));

			using var process = CreateProcess(input);
			process.Start();
			stdOutReceiver?.Initialize(process, process.StandardOutput);
			stdErrReceiver?.Initialize(process, process.StandardError);
			process.WaitForExit();
			stdErrReceiver?.WaitForEndOfStream();
			stdOutReceiver?.WaitForEndOfStream();
			return process.ExitCode;
		}

		public async Task<int> ExecuteAsync(TInput input, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver, Action<Process> cancellationMethod, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(input, nameof(input));

			var tcs = new TaskCompletionSource<int>();
			if(cancellationToken.IsCancellationRequested)
			{
				tcs.SetCanceled();
				return await tcs.Task
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			using var process = CreateProcess(input);
			process.EnableRaisingEvents = true;
			process.Exited += (s, e) =>
				{
					if(!tcs.Task.IsCanceled)
					{
						int exitCode;
						try
						{
							exitCode = ((Process)s).ExitCode;
						}
						catch(Exception exc) when(!exc.IsCritical())
						{
							tcs.TrySetException(exc);
							return;
						}
						tcs.TrySetResult(exitCode);
					}
				};
			process.Start();
			stdOutReceiver?.Initialize(process, process.StandardOutput);
			stdErrReceiver?.Initialize(process, process.StandardError);
			using var cancellationRegistration = cancellationToken.CanBeCanceled
				? cancellationToken.Register(() => tcs.TrySetCanceled())
				: default;
			var processExitCode = await tcs.Task
				.ConfigureAwait(continueOnCapturedContext: false);
			if(tcs.Task.IsCanceled)
			{
				stdErrReceiver?.NotifyCanceled();
				stdOutReceiver?.NotifyCanceled();
				cancellationMethod?.Invoke(process);
			}
			else
			{
				stdErrReceiver?.WaitForEndOfStream();
				stdOutReceiver?.WaitForEndOfStream();
			}
			return processExitCode;
		}
	}
}
