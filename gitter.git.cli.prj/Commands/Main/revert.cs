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
		public static ICommandArgument Mainline(int number)
		{
			return new CommandParameterValue("--mainline", number.ToString(CultureInfo.InvariantCulture), ' ');
		}

		public static ICommandArgument Edit()
		{
			return new CommandFlag("--edit");
		}

		public static ICommandArgument NoEdit()
		{
			return new CommandFlag("--no-edit");
		}

		public static ICommandArgument NoCommit()
		{
			return new CommandFlag("--no-commit");
		}

		public static ICommandArgument SignOff()
		{
			return new CommandFlag("--signoff");
		}

		public static ICommandArgument Continue()
		{
			return new CommandFlag("--continue");
		}

		public static ICommandArgument Quit()
		{
			return new CommandFlag("--quit");
		}

		public static ICommandArgument Abort()
		{
			return new CommandFlag("--abort");
		}

		public RevertCommand()
			: base("revert")
		{
		}

		public RevertCommand(params ICommandArgument[] args)
			: base("revert", args)
		{
		}

		public RevertCommand(IList<ICommandArgument> args)
			: base("revert", args)
		{
		}
	}
}
