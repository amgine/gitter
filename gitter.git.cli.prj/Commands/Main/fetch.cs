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

	/// <summary>Download objects and refs from another repository.</summary>
	public sealed class FetchCommand : Command
	{
		public static CommandArgument All()
		{
			return new CommandArgument("--all");
		}

		public static CommandArgument Append()
		{
			return new CommandArgument("--append");
		}

		public static CommandArgument Force()
		{
			return new CommandArgument("--force");
		}

		public static CommandArgument Progress()
		{
			return new CommandArgument("--progress");
		}

		public static CommandArgument Prune()
		{
			return new CommandArgument("--prune");
		}

		public static CommandArgument Depth(int depth)
		{
			return new CommandArgument("--depth", depth.ToString(System.Globalization.CultureInfo.InvariantCulture), '=');
		}

		public static CommandArgument Tags()
		{
			return new CommandArgument("--tags");
		}

		public static CommandArgument NoTags()
		{
			return new CommandArgument("--no-tags");
		}

		/// <summary>
		///	By default git-fetch refuses to update the head which corresponds to the current branch.
		///	This flag disables the check. This is purely for the internal use for git-pull
		///	to communicate with git-fetch, and unless you are implementing your own Porcelain you are
		///	not supposed to use it.
		/// </summary>
		public static CommandArgument UpdateHeadOk()
		{
			return new CommandArgument("--update-head-ok");
		}

		public static CommandArgument UploadPack(string uploadPack)
		{
			return new CommandArgument("--upload-pack", uploadPack, ' ');
		}

		public static CommandArgument Keep()
		{
			return new CommandArgument("--keep");
		}

		public static CommandArgument Quiet()
		{
			return new CommandArgument("--quiet");
		}

		public static CommandArgument Verbose()
		{
			return new CommandArgument("--verbose");
		}

		public FetchCommand()
			: base("fetch")
		{
		}

		public FetchCommand(params CommandArgument[] args)
			: base("fetch", args)
		{
		}

		public FetchCommand(IList<CommandArgument> args)
			: base("fetch", args)
		{
		}
	}
}
