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

	/// <summary>Summarize 'git-log' output.</summary>
	public sealed class ShortLogCommand : Command
	{
		internal static ICommandArgument All()
		{
			return new CommandFlag("--all");
		}

		/// <summary>Suppress commit description and provide a commit count summary only.</summary>
		public static ICommandArgument Summary()
		{
			return new CommandFlag("--summary");
		}

		/// <summary>Sort output according to the number of commits per author instead of author alphabetic order.</summary>
		public static ICommandArgument Numbered()
		{
			return new CommandFlag("--numbered");
		}

		/// <summary>Show the email address of each author.</summary>
		public static ICommandArgument Email()
		{
			return new CommandFlag("--email");
		}

		public ShortLogCommand()
			: base("shortlog")
		{
		}

		public ShortLogCommand(params ICommandArgument[] args)
			: base("shortlog", args)
		{
		}

		public ShortLogCommand(IList<ICommandArgument> args)
			: base("shortlog", args)
		{
		}
	}
}
