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

	/// <summary>Download objects and refs from another repository.</summary>
	public sealed class FetchCommand : Command
	{
		public static ICommandArgument All()
		{
			return new CommandFlag("--all");
		}

		public static ICommandArgument Append()
		{
			return new CommandFlag("--append");
		}

		public static ICommandArgument Force()
		{
			return new CommandFlag("--force");
		}

		public static ICommandArgument Progress()
		{
			return new CommandFlag("--progress");
		}

		public static ICommandArgument Prune()
		{
			return new CommandFlag("--prune");
		}

		public static ICommandArgument Depth(int depth)
		{
			return new CommandParameterValue("--depth", depth.ToString(CultureInfo.InvariantCulture), '=');
		}

		public static ICommandArgument Tags()
		{
			return new CommandFlag("--tags");
		}

		public static ICommandArgument NoTags()
		{
			return new CommandFlag("--no-tags");
		}

		/// <summary>
		///	By default git-fetch refuses to update the head which corresponds to the current branch.
		///	This flag disables the check. This is purely for the internal use for git-pull
		///	to communicate with git-fetch, and unless you are implementing your own Porcelain you are
		///	not supposed to use it.
		/// </summary>
		public static ICommandArgument UpdateHeadOk()
		{
			return new CommandFlag("--update-head-ok");
		}

		public static ICommandArgument UploadPack(string uploadPack)
		{
			return new CommandParameterValue("--upload-pack", uploadPack, ' ');
		}

		public static ICommandArgument Keep()
		{
			return new CommandFlag("--keep");
		}

		public static ICommandArgument Quiet()
		{
			return new CommandFlag("--quiet");
		}

		public static ICommandArgument Verbose()
		{
			return new CommandFlag("--verbose");
		}

		public FetchCommand()
			: base("fetch")
		{
		}

		public FetchCommand(params ICommandArgument[] args)
			: base("fetch", args)
		{
		}

		public FetchCommand(IList<ICommandArgument> args)
			: base("fetch", args)
		{
		}
	}
}
