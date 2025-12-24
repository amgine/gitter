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
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;

sealed class InitAction(ICliOptionsProvider cliOptionsProvider, Func<InitRepositoryRequest, Command> commandFactory)
	: IGitAction<InitRepositoryRequest>
{
	public void Invoke(InitRepositoryRequest parameters)
	{
		Verify.Argument.IsNotNull(parameters);

		var command  = commandFactory(parameters);
		var executor = new RepositoryCommandExecutor(cliOptionsProvider, parameters.Path);
		var output   = executor.ExecuteCommand(command, CommandExecutionFlags.None);
		output.ThrowOnBadReturnCode();
	}

	public async Task InvokeAsync(InitRepositoryRequest parameters,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(parameters);

		var command  = commandFactory(parameters);
		var executor = new RepositoryCommandExecutor(cliOptionsProvider, parameters.Path);
		var output   = await executor
			.ExecuteCommandAsync(command, CommandExecutionFlags.None, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		output.ThrowOnBadReturnCode();
	}
}
