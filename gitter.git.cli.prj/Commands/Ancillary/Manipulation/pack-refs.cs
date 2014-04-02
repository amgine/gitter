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
	using System;
	using System.Collections.Generic;

	/// <summary>Pack heads and tags for efficient repository access.</summary>
	sealed class PackRefsCommand : Command
	{
		/// <summary>
		/// The command by default packs all tags and refs that are already packed, and leaves other refs alone.
		/// This is because branches are expected to be actively developed and packing their tips does not help performance.
		/// This option causes branch tips to be packed as well. Useful for a repository with many branches of historical interests.
		/// </summary>
		public static ICommandArgument All()
		{
			return new CommandFlag("--all");
		}

		/// <summary>
		/// The command usually removes loose refs under $GIT_DIR/refs hierarchy after packing them.
		/// This option tells it not to.
		/// </summary>
		public static ICommandArgument NoPrune()
		{
			return new CommandFlag("--no-prune");
		}

		public PackRefsCommand()
			: base("pack-refs")
		{
		}

		public PackRefsCommand(params ICommandArgument[] args)
			: base("pack-refs", args)
		{
		}

		public PackRefsCommand(IEnumerable<ICommandArgument> args)
			: base("pack-refs", args)
		{
		}
	}
}
