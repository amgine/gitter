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
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework.CLI;
	using gitter.Framework.Services;

	/// <summary>Executes commands for specific repository.</summary>
	sealed class RepositoryCommandExecutor : CommandExecutorBase
	{
		#region Static

		private static readonly LoggingService Log = new LoggingService("CLI");

		#endregion

		#region Data

		private readonly GitCLI _gitCLI;
		private readonly string _workingDirectory;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="RepositoryCommandExecutor"/> class.</summary>
		/// <param name="workingDirectory">Repository working directory.</param>
		public RepositoryCommandExecutor(GitCLI gitCLI, string workingDirectory)
		{
			Verify.Argument.IsNotNull(gitCLI, "gitCLI");
			Verify.Argument.IsNotNull(workingDirectory, "workingDirectory");

			_gitCLI = gitCLI;
			_workingDirectory = workingDirectory;
		}

		#endregion

		#region Overrides

		protected override void OnCommandExecuting(Command command)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
		}

		protected override GitInput PrepareInput(Command command, Encoding encoding)
		{
			return new GitInput(_workingDirectory, command, encoding);
		}

		#endregion
	}
}
