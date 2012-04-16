namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Checkout a branch or paths to the working tree.</summary>
	public sealed class CheckoutCommand : Command
	{
		public static CheckoutCommand FormatCreateBranchCommand(string name, string startingRevision, bool specifyTracking, bool track, bool reflog)
		{
			var args = new CommandArgument[3 + (specifyTracking ? 1 : 0) + (reflog ? 1 : 0)];
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
			args[id + 1] = new CommandArgument(name);
			args[id + 2] = new CommandArgument(startingRevision);
			return new CheckoutCommand(args);
		}

		/// <summary>
		/// When switching branches, proceed even if the index or the working tree differs from HEAD. This is used to throw away local changes.
		/// When checking out paths from the index, do not fail upon unmerged entries; instead, unmerged entries are ignored.
		/// </summary>
		public static CommandArgument Force()
		{
			return new CommandArgument("--force");
		}

		/// <summary>
		/// When switching branches, if you have local modifications to one or more files that are different between the current branch
		/// and the branch to which you are switching, the command refuses to switch branches in order to preserve your modifications
		/// in context. However, with this option, a three-way merge between the current branch, your working tree contents, and the
		/// new branch is done, and you will be on the new branch. 
		/// </summary>
		public static CommandArgument Merge()
		{
			return new CommandArgument("--merge");
		}

		public static CommandArgument Ours()
		{
			return new CommandArgument("--ours");
		}

		public static CommandArgument Theirs()
		{
			return new CommandArgument("--theirs");
		}

		/// <summary>
		///	Create the branch's reflog. This activates recording of all changes made to the branch ref,
		///	enabling use of date based sha1 expressions such as "branchname@{yesterday}".
		/// </summary>
		public static CommandArgument RefLog()
		{
			return new CommandArgument("-l");
		}

		/// <summary>Create a new branch.</summary>
		public static CommandArgument Branch()
		{
			return new CommandArgument("-b");
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

		/// <summary>Create a new orphan branch.</summary>
		public static CommandArgument Orphan()
		{
			return new CommandArgument("--orphan");
		}

		public static CommandArgument NoMoreOptions()
		{
			return CommandArgument.NoMoreOptions();
		}

		public CheckoutCommand()
			: base("checkout")
		{
		}

		public CheckoutCommand(params CommandArgument[] args)
			: base("checkout", args)
		{
		}

		public CheckoutCommand(IList<CommandArgument> args)
			: base("checkout", args)
		{
		}
	}
}
