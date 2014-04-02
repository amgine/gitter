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

	/// <summary>List references in a local repository.</summary>
	public sealed class ShowRefCommand : Command
	{
		public static ICommandArgument Head()
		{
			return new CommandFlag("--head");
		}

		public static ICommandArgument Dereference()
		{
			return new CommandFlag("--dereference");
		}

		public static ICommandArgument Tags()
		{
			return new CommandFlag("--tags");
		}

		public static ICommandArgument Hash()
		{
			return new CommandFlag("--hash");
		}

		public static ICommandArgument Hash(int n)
		{
			return new CommandParameterValue("--hash", n.ToString());
		}

		public static ICommandArgument Heads()
		{
			return new CommandFlag("--heads");
		}

		public static ICommandArgument Quiet()
		{
			return new CommandFlag("--quiet");
		}

		public static ICommandArgument Verify()
		{
			return new CommandFlag("--verify");
		}

		public static ICommandArgument NoMoreOptions()
		{
			return CommandFlag.NoMoreOptions();
		}

		public ShowRefCommand()
			: base("show-ref")
		{
		}

		public ShowRefCommand(params ICommandArgument[] args)
			: base("show-ref", args)
		{
		}

		public ShowRefCommand(IList<ICommandArgument> args)
			: base("show-ref", args)
		{
		}
	}
}
