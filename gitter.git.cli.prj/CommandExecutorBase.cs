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
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Framework.CLI;

	abstract class CommandExecutorBase : ICommandExecutor
	{
		protected CommandExecutorBase()
		{
		}

		protected abstract GitInput PrepareInput(Command command, Encoding encoding);

		protected virtual ProcessExecutor<GitInput> CreateProcessExecutor()
		{
			return GitProcess.CreateExecutor();
		}

		protected virtual void OnCommandExecuting(Command command)
		{
		}

		public GitOutput ExecuteCommand(Command command)
		{
			OnCommandExecuting(command);

			var stdOutReceiver = new AsyncTextReader();
			var stdErrReceiver = new AsyncTextReader();
			var executor = CreateProcessExecutor();
			var exitCode = executor.Execute(PrepareInput(command, GitProcess.DefaultEncoding), stdOutReceiver, stdErrReceiver);
			return new GitOutput(stdOutReceiver.GetText(), stdErrReceiver.GetText(), exitCode);
		}

		public GitOutput ExecuteCommand(Command command, Encoding encoding)
		{
			OnCommandExecuting(command);

			var stdOutReceiver = new AsyncTextReader();
			var stdErrReceiver = new AsyncTextReader();
			var executor = CreateProcessExecutor();
			var exitCode = executor.Execute(PrepareInput(command, encoding), stdOutReceiver, stdErrReceiver);
			return new GitOutput(stdOutReceiver.GetText(), stdErrReceiver.GetText(), exitCode);
		}

		public int ExecuteCommand(Command command, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver)
		{
			OnCommandExecuting(command);

			var executor = CreateProcessExecutor();
			var exitCode = executor.Execute(PrepareInput(command, GitProcess.DefaultEncoding), stdOutReceiver, stdErrReceiver);
			return exitCode;
		}

		public int ExecuteCommand(Command command, Encoding encoding, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver)
		{
			OnCommandExecuting(command);

			var executor = CreateProcessExecutor();
			var exitCode = executor.Execute(PrepareInput(command, encoding), stdOutReceiver, stdErrReceiver);
			return exitCode;
		}

		private Task<int> ExecuteCommandAsyncCore(Command command, Encoding encoding, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver, CancellationToken cancellationToken)
		{
			OnCommandExecuting(command);

			var processExecutor = CreateProcessExecutor();
			return processExecutor.ExecuteAsync(
				PrepareInput(command, encoding),
				stdOutReceiver,
				stdErrReceiver,
				cancellationToken);
		}

		private Task<GitOutput> ExecuteCommandAsyncCore(Command command, Encoding encoding, CancellationToken cancellationToken)
		{
			OnCommandExecuting(command);

			var stdOutReceiver = new AsyncTextReader();
			var stdErrReceiver = new AsyncTextReader();
			var executor = CreateProcessExecutor();
			return executor.ExecuteAsync(PrepareInput(command, encoding), stdOutReceiver, stdErrReceiver, cancellationToken)
						   .ContinueWith(
								task =>
								{
									int exitCode = TaskUtility.UnwrapResult(task);
									return new GitOutput(stdOutReceiver.GetText(), stdErrReceiver.GetText(), exitCode);
								},
								cancellationToken,
								TaskContinuationOptions.ExecuteSynchronously,
								TaskScheduler.Default);
		}

		public Task<GitOutput> ExecuteCommandAsync(Command command, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(command, "command");

			return ExecuteCommandAsyncCore(command, GitProcess.DefaultEncoding, cancellationToken);
		}

		public Task<GitOutput> ExecuteCommandAsync(Command command, Encoding encoding, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(command, "command");
			Verify.Argument.IsNotNull(encoding, "encoding");

			return ExecuteCommandAsyncCore(command, encoding, cancellationToken);
		}

		public Task<int> ExecuteCommandAsync(Command command, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(command, "command");

			return ExecuteCommandAsyncCore(command, GitProcess.DefaultEncoding, stdOutReceiver, stdErrReceiver, cancellationToken);
		}

		public Task<int> ExecuteCommandAsync(Command command, Encoding encoding, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(command, "command");
			Verify.Argument.IsNotNull(encoding, "encoding");

			return ExecuteCommandAsyncCore(command, encoding, stdOutReceiver, stdErrReceiver, cancellationToken);
		}
	}
}
