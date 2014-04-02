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
	using System.Collections.Generic;

	/// <summary>Run merge conflict resolution tools to resolve merge conflicts.</summary>
	public sealed class MergeToolCommand : Command
	{
		public static ICommandArgument Tool(string tool)
		{
			return new CommandParameterValue("--tool", tool, '=');
		}

		public static ICommandArgument Prompt()
		{
			return new CommandFlag("--prompt");
		}

		public static ICommandArgument NoPrompt()
		{
			return new CommandFlag("--no-prompt");
		}

		public MergeToolCommand()
			: base("mergetool")
		{
		}

		public MergeToolCommand(params ICommandArgument[] args)
			: base("mergetool", args)
		{
		}

		public MergeToolCommand(IList<ICommandArgument> args)
			: base("mergetool", args)
		{
		}
	}
}
