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
		internal static CommandArgument All()
		{
			return new CommandArgument("--all");
		}

		/// <summary>Suppress commit description and provide a commit count summary only.</summary>
		public static CommandArgument Summary()
		{
			return new CommandArgument("--summary");
		}

		/// <summary>Sort output according to the number of commits per author instead of author alphabetic order.</summary>
		public static CommandArgument Numbered()
		{
			return new CommandArgument("--numbered");
		}

		/// <summary>Show the email address of each author.</summary>
		public static CommandArgument Email()
		{
			return new CommandArgument("--email");
		}

		public ShortLogCommand()
			: base("shortlog")
		{
		}

		public ShortLogCommand(params CommandArgument[] args)
			: base("shortlog", args)
		{
		}

		public ShortLogCommand(IList<CommandArgument> args)
			: base("shortlog", args)
		{
		}
	}
}
