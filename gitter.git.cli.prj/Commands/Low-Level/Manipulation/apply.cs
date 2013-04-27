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
	public sealed class ApplyCommand : Command
	{
		public static CommandArgument Cached()
		{
			return new CommandArgument("--cached");
		}

		public static CommandArgument Index()
		{
			return new CommandArgument("--index");
		}

		public static CommandArgument Check()
		{
			return new CommandArgument("--check");
		}

		public static CommandArgument Reverse()
		{
			return new CommandArgument("--reverse");
		}

		public static CommandArgument FromStdin()
		{
			return new CommandArgument("-");
		}

		public static CommandArgument Whitespace(string action)
		{
			return new CommandArgument("--whitespace", action, '=');
		}

		public static CommandArgument UnidiffZero()
		{
			return new CommandArgument("--unidiff-zero");
		}

		public ApplyCommand()
			: base("apply")
		{
		}

		public ApplyCommand(params CommandArgument[] args)
			: base("apply", args)
		{
		}

		public ApplyCommand(IList<CommandArgument> args)
			: base("apply", args)
		{
		}
	}
}
