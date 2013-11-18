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
		#region Data

		private readonly ICliOptionsProvider _cliOptionsProvider;
		private readonly Func<InitRepositoryParameters, Command> _commandFactory;

		#endregion

		#region .ctor

		public InitImpl(ICliOptionsProvider cliOptionsProvider, Func<InitRepositoryParameters, Command> commandFactory)
		{
			_cliOptionsProvider = cliOptionsProvider;
			_commandFactory     = commandFactory;
		}

		#endregion

		#region Methods

		public void Invoke(InitRepositoryParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command  = _commandFactory(parameters);
			var executor = new RepositoryCommandExecutor(_cliOptionsProvider, parameters.Path);
			var output   = executor.ExecuteCommand(command, CommandExecutionFlags.None);
			output.ThrowOnBadReturnCode();
		}

		public Task InvokeAsync(InitRepositoryParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command  = _commandFactory(parameters);
			var executor = new RepositoryCommandExecutor(_cliOptionsProvider, parameters.Path);
			return executor
				.ExecuteCommandAsync(command, CommandExecutionFlags.None, cancellationToken)
				.ContinueWith(
				t =>
				{
					TaskUtility.UnwrapResult(t).ThrowOnBadReturnCode();
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion
	}
}
