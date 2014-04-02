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
	using System.Collections.Generic;

	/// <summary>Move or rename a file, a directory, or a symlink.</summary>
	public sealed class MvCommand : Command
	{
		/// <summary>Force renaming or moving of a file even if the target exists.</summary>
		public static ICommandArgument Force()
		{
			return new CommandFlag("-f");
		}

		/// <summary>
		/// Skip move or rename actions which would lead to an error condition. An error happens when
		/// a source is neither existing nor controlled by GIT, or when it would overwrite an existing
		/// file unless -f is given. 
		/// </summary>
		public static ICommandArgument SkipErrors()
		{
			return new CommandFlag("-k");
		}

		/// <summary>Do nothing; only show what would happen.</summary>
		public static ICommandArgument DryRun()
		{
			return CommandFlag.DryRun();
		}

		public MvCommand()
			: base("mv")
		{
		}

		public MvCommand(params ICommandArgument[] args)
			: base("mv", args)
		{
		}

		public MvCommand(IList<ICommandArgument> args)
			: base("mv", args)
		{
		}
	}
}
