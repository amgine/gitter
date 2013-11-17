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

	/// <summary>Executes process with output redirecting.</summary>
	/// <typeparam name="TInput">Process input type.</typeparam>
	public abstract class ProcessExecutor<TInput>
		where TInput : class
	{
		#region Data

		private readonly string _exeFileName;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="ProcessExecutor&lt;TInput&gt;"/> class.</summary>
		/// <param name="exeFileName">Path to exe file.</param>
		/// <param name="stdOutReceiver">STDOUT receiver (can be null).</param>
		/// <param name="stdErrReceiver">STDERR receiver (can be null).</param>
		public ProcessExecutor(string exeFileName)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(exeFileName, "exeFileName");

			_exeFileName = exeFileName;
		}

		#endregion

		#region Properties

		public string ExeFileName
		{
			get { return _exeFileName; }
		}

		#endregion

		#region Methods

		protected virtual ProcessStartInfo InitializeStartInfo(TInput input)
		{
			return new ProcessStartInfo { FileName = ExeFileName };
		}

		protected virtual Process CreateProcess(TInput input)
		{
			return new Process { StartInfo = InitializeStartInfo(input) };
		}

		public int Execute(TInput input, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver)
		{
			Verify.Argument.IsNotNull(input, "input");

			using(var process = CreateProcess(input))
			{
				process.Start();
				if(stdOutReceiver != null)
				{
					stdOutReceiver.Initialize(process, process.StandardOutput);
				}
				if(stdErrReceiver != null)
				{
					stdErrReceiver.Initialize(process, process.StandardError);
				}
				process.WaitForExit();
				if(stdErrReceiver != null)
				{
					stdErrReceiver.WaitForEndOfStream();
				}
				if(stdOutReceiver != null)
				{
					stdOutReceiver.WaitForEndOfStream();
				}
				return process.ExitCode;
			}
		}

		public Task<int> ExecuteAsync(TInput input, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(input, "input");

			var tcs = new TaskCompletionSource<int>();
			if(cancellationToken.IsCancellationRequested)
			{
				tcs.SetCanceled();
				return tcs.Task;
			}
			var process = CreateProcess(input);
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
						catch(Exception exc)
						{
							tcs.TrySetException(exc);
							return;
						}
						tcs.TrySetResult(exitCode);
					}
				};
			process.Start();
			if(stdOutReceiver != null)
			{
				stdOutReceiver.Initialize(process, process.StandardOutput);
			}
			if(stdErrReceiver != null)
			{
				stdErrReceiver.Initialize(process, process.StandardError);
			}
			if(cancellationToken.CanBeCanceled)
			{
				cancellationToken.Register(() => tcs.TrySetCanceled());
			}
			return tcs.Task.ContinueWith(
				task =>
				{
					if(task.IsCanceled)
					{
						try
						{
							process.Kill();
						}
						catch
						{
						}
					}
					else
					{
						if(stdErrReceiver != null)
						{
							stdErrReceiver.WaitForEndOfStream();
						}
						if(stdOutReceiver != null)
						{
							stdOutReceiver.WaitForEndOfStream();
						}
					}
					process.Dispose();
					return TaskUtility.UnwrapResult(task);
				},
				CancellationToken.None,
				TaskContinuationOptions.None,
				TaskScheduler.Default);
		}

		#endregion
	}
}
