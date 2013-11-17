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
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	sealed class InitImpl : IGitAction<InitRepositoryParameters>
	{
		private readonly GitCLI _gitCLI;
		private readonly Func<InitRepositoryParameters, Command> _commandFactory;

		public InitImpl(GitCLI gitCLI, Func<InitRepositoryParameters, Command> commandFactory)
		{
			_gitCLI = gitCLI;
			_commandFactory = commandFactory;
		}

		public void Invoke(InitRepositoryParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command  = _commandFactory(parameters);
			var executor = new RepositoryCommandExecutor(_gitCLI, parameters.Path);
			var output   = executor.ExecuteCommand(command);
			output.ThrowOnBadReturnCode();
		}

		public Task InvokeAsync(InitRepositoryParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command  = _commandFactory(parameters);
			var executor = new RepositoryCommandExecutor(_gitCLI, parameters.Path);
			return executor
				.ExecuteCommandAsync(command, cancellationToken)
				.ContinueWith(
				t =>
				{
					TaskUtility.UnwrapResult(t).ThrowOnBadReturnCode();
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}
	}
}
