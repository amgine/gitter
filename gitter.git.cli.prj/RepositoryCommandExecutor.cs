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
	using System.Text;

	using gitter.Framework.Services;

	/// <summary>Executes commands for specific repository.</summary>
	sealed class RepositoryCommandExecutor : ICommandExecutor
	{
		private static readonly LoggingService Log = new LoggingService("CLI");

		private readonly GitCLI _gitCLI;
		private readonly string _workingDirectory;

		/// <summary>Initializes a new instance of the <see cref="RepositoryCommandExecutor"/> class.</summary>
		/// <param name="workingDirectory">Repository working directory.</param>
		public RepositoryCommandExecutor(GitCLI gitCLI, string workingDirectory)
		{
			Verify.Argument.IsNotNull(gitCLI, "gitCLI");
			Verify.Argument.IsNotNull(workingDirectory, "workingDirectory");

			_gitCLI = gitCLI;
			_workingDirectory = workingDirectory;
		}

		#region ICommandExecutor

		public GitOutput ExecCommand(Command command)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
			return GitProcess.Exec(
				new GitInput(_workingDirectory, command, GitProcess.DefaultEncoding));
		}

		public GitOutput ExecCommand(Command command, Encoding encoding)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
			return GitProcess.Exec(
				new GitInput(_workingDirectory, command, encoding));
		}

		public GitAsync ExecAsync(Command command)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
			return GitProcess.ExecAsync(
				new GitInput(_workingDirectory, command, GitProcess.DefaultEncoding));
		}

		public GitAsync ExecAsync(Command command, Encoding encoding)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
			return GitProcess.ExecAsync(
				new GitInput(_workingDirectory, command, encoding));
		}

		#endregion
	}
}
