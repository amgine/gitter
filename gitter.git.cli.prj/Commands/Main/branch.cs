namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	/// <summary>List, create, or delete branches.</summary>
	public sealed class BranchCommand : Command
	{
		public static BranchCommand FormatCreateBranchCommand(string name, string startingRevision, bool specifyTracking, bool track, bool reflog)
		{
			var args = new CommandArgument[2+(specifyTracking?1:0)+(reflog?1:0)];
			int id = 0;
			if(specifyTracking)
			{
				args[id++] = track ? Track() : NoTrack();
			}
			if(reflog)
			{
				args[id++] = RefLog();
			}
			args[id + 0] = new CommandArgument(name);
			args[id + 1] = new CommandArgument(startingRevision);
			return new BranchCommand(args);
		}

		/// <summary>Delete a branch. The branch must be fully merged in HEAD.</summary>
		public static CommandArgument Delete()
		{
			return new CommandArgument("-d");
		}

		/// <summary>Delete a branch irrespective of its merged status.</summary>
		public static CommandArgument DeleteForce()
		{
			return new CommandArgument("-D");
		}

		/// <summary>
		///	Create the branch's reflog. This activates recording of all changes made to the branch ref,
		///	enabling use of date based sha1 expressions such as "branchname@{yesterday}".
		/// </summary>
		public static CommandArgument RefLog()
		{
			return new CommandArgument("-l");
		}

		/// <summary>Reset branchname to startpoint if branchname exists already. Without -f git-branch refuses to change an existing branch.</summary>
		public static CommandArgument Reset()
		{
			return new CommandArgument("-f");
		}

		/// <summary>Move/rename a branch and the corresponding reflog.</summary>
		public static CommandArgument Move()
		{
			return new CommandArgument("-m");
		}

		/// <summary>Move/rename a branch even if the new branch name already exists.</summary>
		public static CommandArgument MoveForce()
		{
			return new CommandArgument("-M");
		}

		/// <summary>Color branches to highlight current, local, and remote branches.</summary>
		public static CommandArgument Color()
		{
			return new CommandArgument("--color");
		}

		/// <summary>Turn off branch colors, even when the configuration file gives the default to color output.</summary>
		public static CommandArgument NoColor()
		{
			return new CommandArgument("--no-color");
		}

		/// <summary>List or delete (if used with -d) the remote-tracking branches.</summary>
		public static CommandArgument Remote()
		{
			return new CommandArgument("-r");
		}

		/// <summary>List both remote-tracking branches and local branches.</summary>
		public static CommandArgument All()
		{
			return new CommandArgument("-a");
		}

		/// <summary>
		/// Show sha1 and commit subject line for each head, along with relationship to upstream branch (if any).
		/// If given twice, print the name of the upstream branch, as well. 
		/// </summary>
		public static CommandArgument Verbose()
		{
			return CommandArgument.Verbose();
		}
		
		/// <summary>Alter the sha1's minimum display length in the output listing. The default value is 7.</summary>
		public static CommandArgument Abbrev(int length)
		{
			return new CommandArgument("--abbrev", length.ToString());
		}

		/// <summary>Display the full sha1s in the output listing rather than abbreviating them.</summary>
		public static CommandArgument NoAbbrev()
		{
			return new CommandArgument("--no-abbrev");
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
		public static CommandArgument Track()
		{
			return new CommandArgument("--track");
		}

		/// <summary>Do not set up "upstream" configuration, even if the branch.autosetupmerge configuration variable is true.</summary>
		public static CommandArgument NoTrack()
		{
			return new CommandArgument("--no-track");
		}

		/// <summary>Only list branches which contain the specified commit.</summary>
		public static CommandArgument Contains()
		{
			return new CommandArgument("--contains");
		}

		/// <summary>Only list branches which contain the specified commit.</summary>
		public static CommandArgument Contains(string commit)
		{
			return new CommandArgument("--contains", commit, ' ');
		}

		/// <summary>Only list branches which are fully contained by HEAD.</summary>
		public static CommandArgument Merged()
		{
			return new CommandArgument("--merged");
		}

		/// <summary>Do not list branches which are fully contained by HEAD.</summary>
		public static CommandArgument NoMerged()
		{
			return new CommandArgument("--no-merged");
		}

		public BranchCommand()
			: base("branch")
		{
		}

		public BranchCommand(params CommandArgument[] args)
			: base("branch", args)
		{
		}

		public BranchCommand(IList<CommandArgument> args)
			: base("branch", args)
		{
		}
	}
}
