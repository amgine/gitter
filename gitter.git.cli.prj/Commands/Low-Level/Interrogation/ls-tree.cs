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

	/// <summary>List the contents of a tree object.</summary>
	public sealed class LsTreeCommand : Command
	{
		public static CommandArgument Directories()
		{
			return new CommandArgument("-d");
		}

		public static CommandArgument Recurse()
		{
			return new CommandArgument("-r");
		}

		public static CommandArgument Tree()
		{
			return new CommandArgument("-t");
		}

		public static CommandArgument Long()
		{
			return new CommandArgument("--long");
		}

		public static CommandArgument NameOnly()
		{
			return new CommandArgument("--name-only");
		}

		public static CommandArgument FullName()
		{
			return new CommandArgument("--full-name");
		}

		public static CommandArgument FullTree()
		{
			return new CommandArgument("--full-tree");
		}

		public static CommandArgument Abbrev()
		{
			return new CommandArgument("--abbrev");
		}

		public static CommandArgument Abbrev(int n)
		{
			return new CommandArgument("--abbrev", n.ToString(System.Globalization.CultureInfo.InvariantCulture), '=');
		}

		public static CommandArgument NullTerminate()
		{
			return new CommandArgument("-z");
		}

		public LsTreeCommand()
			: base("ls-tree")
		{
		}

		public LsTreeCommand(params CommandArgument[] args)
			: base("ls-tree", args)
		{
		}

		public LsTreeCommand(IList<CommandArgument> args)
			: base("ls-tree", args)
		{
		}
	}
}
