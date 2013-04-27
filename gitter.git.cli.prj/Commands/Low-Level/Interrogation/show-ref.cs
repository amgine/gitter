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
		public static CommandArgument Head()
		{
			return new CommandArgument("--head");
		}

		public static CommandArgument Dereference()
		{
			return new CommandArgument("--dereference");
		}

		public static CommandArgument Tags()
		{
			return new CommandArgument("--tags");
		}

		public static CommandArgument Hash()
		{
			return new CommandArgument("--hash");
		}

		public static CommandArgument Hash(int n)
		{
			return new CommandArgument("--hash", n.ToString());
		}

		public static CommandArgument Heads()
		{
			return new CommandArgument("--heads");
		}

		public static CommandArgument Quiet()
		{
			return new CommandArgument("--quiet");
		}

		public static CommandArgument Verify()
		{
			return new CommandArgument("--verify");
		}

		public static CommandArgument NoMoreOptions()
		{
			return CommandArgument.NoMoreOptions();
		}

		public ShowRefCommand()
			: base("show-ref")
		{
		}

		public ShowRefCommand(params CommandArgument[] args)
			: base("show-ref", args)
		{
		}

		public ShowRefCommand(IList<CommandArgument> args)
			: base("show-ref", args)
		{
		}
	}
}
