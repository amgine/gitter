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

	/// <summary>Show changes between commits, commit and working tree, etc.</summary>
	public sealed class DiffCommand : Command
	{
		public static CommandArgument FullIndex()
		{
			return new CommandArgument("--full-index");
		}

		public static CommandArgument Binary()
		{
			return new CommandArgument("--binary");
		}

		public static CommandArgument NoPrefix()
		{
			return new CommandArgument("--no-prefix");
		}

		public static CommandArgument Cached()
		{
			return new CommandArgument("--cached");
		}

		public static CommandArgument Patch()
		{
			return new CommandArgument("-p");
		}

		public static CommandArgument Raw()
		{
			return new CommandArgument("--raw");
		}

		public static CommandArgument Patience()
		{
			return new CommandArgument("--patience");
		}

		public static CommandArgument Stat()
		{
			return new CommandArgument("--stat");
		}

		public static CommandArgument NumStat()
		{
			return new CommandArgument("--numstat");
		}

		public static CommandArgument ShortStat()
		{
			return new CommandArgument("--shortstat");
		}

		public static CommandArgument NameOnly()
		{
			return new CommandArgument("--name-only");
		}

		public static CommandArgument NameStatus()
		{
			return new CommandArgument("--name-only");
		}

		public static CommandArgument IgnoreSpaceChange()
		{
			return new CommandArgument("--ignore-space-change");
		}

		public static CommandArgument IgnoreSpaceAtEOL()
		{
			return new CommandArgument("--ignore-space-at-eol");
		}

		public static CommandArgument IgnoreAllSpace()
		{
			return new CommandArgument("--ignore-all-space");
		}

		public static CommandArgument NoRenames()
		{
			return new CommandArgument("--no-renames");
		}

		public static CommandArgument PatchWithRaw()
		{
			return new CommandArgument("--patch-with-raw");
		}

		public static CommandArgument Unified(int n)
		{
			return new CommandArgument("--unified", n.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		public static CommandArgument Check()
		{
			return new CommandArgument("--check");
		}

		public static CommandArgument NoColor()
		{
			return new CommandArgument("--no-color");
		}

		public static CommandArgument NoMoreOptions()
		{
			return CommandArgument.NoMoreOptions();
		}

		public static CommandArgument NullTerminate()
		{
			return new CommandArgument("-z");
		}

		public DiffCommand()
			: base("diff")
		{
		}

		public DiffCommand(params CommandArgument[] args)
			: base("diff", args)
		{
		}

		public DiffCommand(IList<CommandArgument> args)
			: base("diff", args)
		{
		}
	}
}
