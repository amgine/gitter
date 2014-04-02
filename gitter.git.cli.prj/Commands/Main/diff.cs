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
	using System.Globalization;

	/// <summary>Show changes between commits, commit and working tree, etc.</summary>
	public sealed class DiffCommand : Command
	{
		public static ICommandArgument FullIndex()
		{
			return new CommandFlag("--full-index");
		}

		public static ICommandArgument Binary()
		{
			return new CommandFlag("--binary");
		}

		public static ICommandArgument NoPrefix()
		{
			return new CommandFlag("--no-prefix");
		}

		public static ICommandArgument Cached()
		{
			return new CommandFlag("--cached");
		}

		public static ICommandArgument Patch()
		{
			return new CommandFlag("-p");
		}

		public static ICommandArgument Raw()
		{
			return new CommandFlag("--raw");
		}

		public static ICommandArgument Patience()
		{
			return new CommandFlag("--patience");
		}

		public static ICommandArgument Stat()
		{
			return new CommandFlag("--stat");
		}

		public static ICommandArgument NumStat()
		{
			return new CommandFlag("--numstat");
		}

		public static ICommandArgument ShortStat()
		{
			return new CommandFlag("--shortstat");
		}

		public static ICommandArgument NameOnly()
		{
			return new CommandFlag("--name-only");
		}

		public static ICommandArgument NameStatus()
		{
			return new CommandFlag("--name-status");
		}

		public static ICommandArgument IgnoreSpaceChange()
		{
			return new CommandFlag("--ignore-space-change");
		}

		public static ICommandArgument IgnoreSpaceAtEOL()
		{
			return new CommandFlag("--ignore-space-at-eol");
		}

		public static ICommandArgument IgnoreAllSpace()
		{
			return new CommandFlag("--ignore-all-space");
		}

		public static ICommandArgument NoRenames()
		{
			return new CommandFlag("--no-renames");
		}

		public static ICommandArgument PatchWithRaw()
		{
			return new CommandFlag("--patch-with-raw");
		}

		public static ICommandArgument Unified(int n)
		{
			return new CommandParameterValue("--unified", n.ToString(CultureInfo.InvariantCulture));
		}

		public static ICommandArgument Check()
		{
			return new CommandFlag("--check");
		}

		public static ICommandArgument NoColor()
		{
			return new CommandFlag("--no-color");
		}

		public static ICommandArgument SwapInputs()
		{
			return new CommandFlag("-R");
		}

		public static ICommandArgument TextConv()
		{
			return new CommandFlag("--textconv");
		}

		public static ICommandArgument NoTextConv()
		{
			return new CommandFlag("--no-textconv");
		}

		public static ICommandArgument ExtDiff()
		{
			return new CommandFlag("--ext-diff");
		}

		public static ICommandArgument NoExtDiff()
		{
			return new CommandFlag("--no-ext-diff");
		}

		public static ICommandArgument Text()
		{
			return new CommandFlag("--text");
		}

		public static ICommandArgument FindRenames()
		{
			return new CommandFlag("--find-renames");
		}

		public static ICommandArgument FindRenames(double similarity)
		{
			return new CommandParameterValue("--find-renames", similarity.ToString("G", CultureInfo.InvariantCulture).Substring(2), '=');
		}

		public static ICommandArgument FindCopies()
		{
			return new CommandFlag("--find-copies");
		}

		public static ICommandArgument FindCopies(double similarity)
		{
			return new CommandParameterValue("--find-copies", similarity.ToString("G", CultureInfo.InvariantCulture).Substring(2), '=');
		}

		public static ICommandArgument NoMoreOptions()
		{
			return CommandFlag.NoMoreOptions();
		}

		public static ICommandArgument NullTerminate()
		{
			return new CommandFlag("-z");
		}

		public DiffCommand()
			: base("diff")
		{
		}

		public DiffCommand(params ICommandArgument[] args)
			: base("diff", args)
		{
		}

		public DiffCommand(IList<ICommandArgument> args)
			: base("diff", args)
		{
		}
	}
}
