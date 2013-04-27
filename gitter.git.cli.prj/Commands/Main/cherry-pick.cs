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
	using System.Globalization;
	using System.Collections.Generic;

	/// <summary>Apply the change introduced by an existing commit.</summary>
	public sealed class CherryPickCommand : Command
	{
		public static CommandArgument Edit()
		{
			return new CommandArgument("--edit");
		}

		public static CommandArgument Mainline(int number)
		{
			return new CommandArgument("--mainline", number.ToString(CultureInfo.InvariantCulture), ' ');
		}

		public static CommandArgument NoCommit()
		{
			return new CommandArgument("--no-commit");
		}

		public static CommandArgument SignOff()
		{
			return new CommandArgument("--signoff");
		}

		public static CommandArgument FastForward()
		{
			return new CommandArgument("--ff");
		}

		public static CommandArgument AllowEmpty()
		{
			return new CommandArgument("--allow-empty");
		}

		public static CommandArgument AllowEmptyMessage()
		{
			return new CommandArgument("--allow-empty-message");
		}

		public static CommandArgument KeepRedundantCommits()
		{
			return new CommandArgument("--keep-redundant-commits");
		}

		public static CommandArgument Continue()
		{
			return new CommandArgument("--continue");
		}

		public static CommandArgument Quit()
		{
			return new CommandArgument("--quit");
		}

		public static CommandArgument Abort()
		{
			return new CommandArgument("--abort");
		}

		public CherryPickCommand()
			: base("cherry-pick")
		{
		}

		public CherryPickCommand(params CommandArgument[] args)
			: base("cherry-pick", args)
		{
		}

		public CherryPickCommand(IList<CommandArgument> args)
			: base("cherry-pick", args)
		{
		}
	}
}
