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

	/// <summary>List, create, or delete branches.</summary>
	public sealed class BranchCommand : Command
	{
		public static BranchCommand FormatCreateBranchCommand(string name, string startingRevision, bool specifyTracking, bool track, bool reflog)
		{
			var args = new ICommandArgument[2+(specifyTracking?1:0)+(reflog?1:0)];
			int id = 0;
			if(specifyTracking)
			{
				args[id++] = track ? Track() : NoTrack();
			}
			if(reflog)
			{
				args[id++] = RefLog();
			}
			args[id + 0] = new CommandParameter(name);
			args[id + 1] = new CommandParameter(startingRevision);
			return new BranchCommand(args);
		}

		/// <summary>Delete a branch. The branch must be fully merged in HEAD.</summary>
		public static ICommandArgument Delete()
		{
			return new CommandFlag("-d");
		}

		/// <summary>Delete a branch irrespective of its merged status.</summary>
		public static ICommandArgument DeleteForce()
		{
			return new CommandFlag("-D");
		}

		/// <summary>
		///	Create the branch's reflog. This activates recording of all changes made to the branch ref,
		///	enabling use of date based sha1 expressions such as "branchname@{yesterday}".
		/// </summary>
		public static ICommandArgument RefLog()
		{
			return new CommandFlag("-l");
		}

		/// <summary>Reset branchname to startpoint if branchname exists already. Without -f git-branch refuses to change an existing branch.</summary>
		public static ICommandArgument Reset()
		{
			return new CommandFlag("-f");
		}

		/// <summary>Move/rename a branch and the corresponding reflog.</summary>
		public static ICommandArgument Move()
		{
			return new CommandFlag("-m");
		}

		/// <summary>Move/rename a branch even if the new branch name already exists.</summary>
		public static ICommandArgument MoveForce()
		{
			return new CommandFlag("-M");
		}

		/// <summary>Color branches to highlight current, local, and remote branches.</summary>
		public static ICommandArgument Color()
		{
			return new CommandFlag("--color");
		}

		/// <summary>Turn off branch colors, even when the configuration file gives the default to color output.</summary>
		public static ICommandArgument NoColor()
		{
			return new CommandFlag("--no-color");
		}

		/// <summary>List or delete (if used with -d) the remote-tracking branches.</summary>
		public static ICommandArgument Remote()
		{
			return new CommandFlag("-r");
		}

		/// <summary>List both remote-tracking branches and local branches.</summary>
		public static ICommandArgument All()
		{
			return new CommandFlag("-a");
		}

		/// <summary>
		/// Show sha1 and commit subject line for each head, along with relationship to upstream branch (if any).
		/// If given twice, print the name of the upstream branch, as well. 
		/// </summary>
		public static ICommandArgument Verbose()
		{
			return CommandFlag.Verbose();
		}
		
		/// <summary>Alter the sha1's minimum display length in the output listing. The default value is 7.</summary>
		public static ICommandArgument Abbrev(int length)
		{
			return new CommandParameterValue("--abbrev", length.ToString());
		}

		/// <summary>Display the full sha1s in the output listing rather than abbreviating them.</summary>
		public static ICommandArgument NoAbbrev()
		{
			return new CommandFlag("--no-abbrev");
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

		/// <summary>Only list branches which contain the specified commit.</summary>
		public static ICommandArgument Contains()
		{
			return new CommandFlag("--contains");
		}

		/// <summary>Only list branches which contain the specified commit.</summary>
		public static ICommandArgument Contains(string commit)
		{
			return new CommandParameterValue("--contains", commit, ' ');
		}

		/// <summary>Only list branches which are fully contained by HEAD.</summary>
		public static ICommandArgument Merged()
		{
			return new CommandFlag("--merged");
		}

		/// <summary>Do not list branches which are fully contained by HEAD.</summary>
		public static ICommandArgument NoMerged()
		{
			return new CommandFlag("--no-merged");
		}

		public BranchCommand()
			: base("branch")
		{
		}

		public BranchCommand(params ICommandArgument[] args)
			: base("branch", args)
		{
		}

		public BranchCommand(IList<ICommandArgument> args)
			: base("branch", args)
		{
		}
	}
}
