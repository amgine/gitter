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
using System.Text;

using gitter.Framework.Services;

/// <summary>Executes commands for specific repository.</summary>
/// <param name="cliOptionsProvider">CLI options provider.</param>
/// <param name="workingDirectory">Repository working directory.</param>
sealed class RepositoryCommandExecutor(ICliOptionsProvider cliOptionsProvider, string workingDirectory)
	: CommandExecutorBase(cliOptionsProvider)
{
	private static LoggingService Log { get; } = new("CLI");

	public string WorkingDirectory { get; } = workingDirectory;

	protected override void OnCommandExecuting(Command command)
	{
		Assert.IsNotNull(command);

		if(CliOptionsProvider.LogCalls) Log.Info("git {0}", command);
	}

	protected override GitInput PrepareInput(Command command, Encoding? encoding)
		=> new(WorkingDirectory, command, encoding ?? CliOptionsProvider.DefaultEncoding);
}
