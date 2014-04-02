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

	/// <summary>Checkout a branch or paths to the working tree.</summary>
	public sealed class CheckoutCommand : Command
	{
		public static CheckoutCommand FormatCreateBranchCommand(string name, string startingRevision, bool specifyTracking, bool track, bool reflog)
		{
			var args = new ICommandArgument[3 + (specifyTracking ? 1 : 0) + (reflog ? 1 : 0)];
			int id = 0;
			if(specifyTracking)
			{
				args[id++] = track ? Track() : NoTrack();
			}
			if(reflog)
			{
				args[id++] = RefLog();
			}
			args[id + 0] = Branch();
			args[id + 1] = new CommandParameter(name);
			args[id + 2] = new CommandParameter(startingRevision);
			return new CheckoutCommand(args);
		}

		/// <summary>
		/// When switching branches, proceed even if the index or the working tree differs from HEAD. This is used to throw away local changes.
		/// When checking out paths from the index, do not fail upon unmerged entries; instead, unmerged entries are ignored.
		/// </summary>
		public static ICommandArgument Force()
		{
			return new CommandFlag("--force");
		}

		/// <summary>
		/// When switching branches, if you have local modifications to one or more files that are different between the current branch
		/// and the branch to which you are switching, the command refuses to switch branches in order to preserve your modifications
		/// in context. However, with this option, a three-way merge between the current branch, your working tree contents, and the
		/// new branch is done, and you will be on the new branch. 
		/// </summary>
		public static ICommandArgument Merge()
		{
			return new CommandFlag("--merge");
		}

		public static ICommandArgument Ours()
		{
			return new CommandFlag("--ours");
		}

		public static ICommandArgument Theirs()
		{
			return new CommandFlag("--theirs");
		}

		/// <summary>
		///	Create the branch's reflog. This activates recording of all changes made to the branch ref,
		///	enabling use of date based sha1 expressions such as "branchname@{yesterday}".
		/// </summary>
		public static ICommandArgument RefLog()
		{
			return new CommandFlag("-l");
		}

		/// <summary>Create a new branch.</summary>
		public static ICommandArgument Branch()
		{
			return new CommandFlag("-b");
		}

		/// <summary>
		/// <para>When creating a new branch, set up configuration to mark the start-point branch as "upstream" from
		/// the new branch. This configuration will tell git to show the relationship between the two branches in git
		/// status and git branch -v. Furthermore, it directs git pull without arguments to pull from the upstream when
		/// the new branch is checked out.</para>
		///	<para>This behavior is the default when the start point is a remote branch. Set the branch.autosetupmerge
		///	configuration variable to false if you want git checkout and git branch to always behave as if --no-track
		///	were given. Set it to always if you want this behavior when the start-point is either a local or 
		///	remote branch.</para>
		/// </summary>
		public static ICommandArgument Track()
		{
			return new CommandFlag("--track");
		}

		/// <summary>Do not set up "upstream" configuration, even if the branch.autosetupmerge configuration variable is true.</summary>
		public static ICommandArgument NoTrack()
		{
			return new CommandFlag("--no-track");
		}

		/// <summary>Create a new orphan branch.</summary>
		public static ICommandArgument Orphan()
		{
			return new CommandFlag("--orphan");
		}

		public static ICommandArgument NoMoreOptions()
		{
			return CommandFlag.NoMoreOptions();
		}

		public CheckoutCommand()
			: base("checkout")
		{
		}

		public CheckoutCommand(params ICommandArgument[] args)
			: base("checkout", args)
		{
		}

		public CheckoutCommand(IList<ICommandArgument> args)
			: base("checkout", args)
		{
		}
	}
}
