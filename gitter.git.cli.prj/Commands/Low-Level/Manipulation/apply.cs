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
		public static ICommandArgument Cached()
		{
			return new CommandFlag("--cached");
		}

		public static ICommandArgument Index()
		{
			return new CommandFlag("--index");
		}

		public static ICommandArgument Check()
		{
			return new CommandFlag("--check");
		}

		public static ICommandArgument Reverse()
		{
			return new CommandFlag("--reverse");
		}

		public static ICommandArgument FromStdin()
		{
			return new CommandFlag("-");
		}

		public static ICommandArgument Whitespace(string action)
		{
			return new CommandParameterValue("--whitespace", action, '=');
		}

		public static ICommandArgument UnidiffZero()
		{
			return new CommandFlag("--unidiff-zero");
		}

		public ApplyCommand()
			: base("apply")
		{
		}

		public ApplyCommand(params ICommandArgument[] args)
			: base("apply", args)
		{
		}

		public ApplyCommand(IList<ICommandArgument> args)
			: base("apply", args)
		{
		}
	}
}
