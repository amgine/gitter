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

	using gitter.Framework.Services;

	sealed class GitCommandExecutor : CommandExecutorBase
	{
		#region Static

		private static readonly LoggingService Log = new LoggingService("Global CLI");

		#endregion

		#region Data

		private readonly GitCLI _gitCLI;

		#endregion

		#region .ctor

		public GitCommandExecutor(GitCLI gitCLI)
		{
			Verify.Argument.IsNotNull(gitCLI, "gitCLI");

			_gitCLI = gitCLI;
		}

		#endregion

		#region Overrides

		protected override void OnCommandExecuting(Command command)
		{
			if(_gitCLI.LogCLICalls) Log.Info("git {0}", command);
		}

		protected override GitInput PrepareInput(Command command, Encoding encoding)
		{
			return new GitInput(command, encoding);
		}

		#endregion
	}
}
