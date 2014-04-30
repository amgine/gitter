#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Framework.CLI;

	abstract class ParserBasedFunctionImpl<TParameters, TOutput> : IGitFunction<TParameters, TOutput>
		where TParameters : class
	{
		#region Data

		private readonly ICommandExecutor _commandExecutor;

		#endregion

		#region .ctor

		protected ParserBasedFunctionImpl(ICommandExecutor commandExecutor)
		{
			Verify.Argument.IsNotNull(commandExecutor, "commandExecutor");

			_commandExecutor = commandExecutor;
		}

		#endregion

		#region Methods

		protected abstract Command CreateCommand(TParameters parameters);

		protected abstract IParser<TOutput> CreateParser();

		protected virtual void HandleNonZeroExitCode(AsyncTextReader stdErrReceiver, int exitCode)
		{
			Assert.IsNotNull(stdErrReceiver);

			throw new GitException(stdErrReceiver.GetText());
		}

		protected virtual CommandExecutionFlags GetExecutionFlags()
		{
			return CommandExecutionFlags.None;
		}

		#endregion

		#region IGitFunction<TParameters, TOutput> Members

		public TOutput Invoke(TParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command        = CreateCommand(parameters);
			var parser         = CreateParser();
			var stdOutReceiver = new AsyncTextParser(parser);
			var stdErrReceiver = new AsyncTextReader();
			var exitCode       = _commandExecutor.ExecuteCommand(
				command, stdOutReceiver, stdErrReceiver, GetExecutionFlags());
			if(exitCode != 0)
			{
				HandleNonZeroExitCode(stdErrReceiver, exitCode);
			}
			return parser.GetResult();
		}

		public Task<TOutput> InvokeAsync(TParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command        = CreateCommand(parameters);
			var parser         = CreateParser();
			var stdOutReceiver = new AsyncTextParser(parser);
			var stdErrReceiver = new AsyncTextReader();

			return _commandExecutor
				.ExecuteCommandAsync(
					command, stdOutReceiver, stdErrReceiver, GetExecutionFlags(), cancellationToken)
				.ContinueWith(task =>
				{
					int exitCode = TaskUtility.UnwrapResult(task);
					if(exitCode != 0)
					{
						HandleNonZeroExitCode(stdErrReceiver, exitCode);
					}
					return parser.GetResult();
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion
	}
}
