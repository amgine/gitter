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

	/// <summary>Revert an existing commit.</summary>
	public sealed class RevertCommand : Command
	{
		public static CommandArgument Mainline(int number)
		{
			return new CommandArgument("--mainline", number.ToString(CultureInfo.InvariantCulture), ' ');
		}

		public static CommandArgument Edit()
		{
			return new CommandArgument("--edit");
		}

		public static CommandArgument NoEdit()
		{
			return new CommandArgument("--no-edit");
		}

		public static CommandArgument NoCommit()
		{
			return new CommandArgument("--no-commit");
		}

		public static CommandArgument SignOff()
		{
			return new CommandArgument("--signoff");
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

		public RevertCommand()
			: base("revert")
		{
		}

		public RevertCommand(params CommandArgument[] args)
			: base("revert", args)
		{
		}

		public RevertCommand(IList<CommandArgument> args)
			: base("revert", args)
		{
		}
	}
}
