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

namespace gitter.Git.AccessLayer.CLI;

using System.Collections.Generic;

/// <summary>Checkout a branch or paths to the working tree.</summary>
public sealed class CheckoutCommand : Command
{
	public static class KnownArguments
	{
		public static ICommandArgument Quiet => CommonArguments.Quiet;

		public static ICommandArgument Progress { get; } = new CommandFlag("--progress");

		public static ICommandArgument NoProgress { get; } = new CommandFlag("--no-progress");

		public static ICommandArgument Force { get; } = new CommandFlag("-f", "--force");

		public static ICommandArgument Ours { get; } = new CommandFlag("--ours");

		public static ICommandArgument Theirs { get; } = new CommandFlag("--theirs");

		public static ICommandArgument Branch { get; } = new CommandFlag("-b");

		public static ICommandArgument BranchForce { get; } = new CommandFlag("-B");

		/// <summary>
		/// When creating a new branch, set up branch.name.remote and branch.name.merge configuration entries
		/// to mark the start-point branch as "upstream" from the new branch.
		/// </summary>
		public static ICommandArgument Track { get; } = new CommandFlag("-t", "--track");

		/// <summary>Do not set up "upstream" configuration, even if the branch.autoSetupMerge configuration variable is true.</summary>
		public static ICommandArgument NoTrack { get; } = new CommandFlag("--no-track");

		/// <summary>
		///	Create the branch's reflog. This activates recording of all changes made to the branch ref,
		///	enabling use of date based sha1 expressions such as "branchname@{yesterday}".
		/// </summary>
		public static ICommandArgument CreateReflog { get; } = new CommandFlag("-l");

		public static ICommandArgument Detach { get; } = new CommandFlag("--detach");

		public static ICommandArgument Orphan { get; } = new CommandFlag("--orphan");

		public static ICommandArgument IgnoreSkipWorktreeBits { get; } = new CommandFlag("--ignore-skip-worktree-bits");

		public static ICommandArgument Merge { get; } = new CommandFlag("-m", "--merge");

		public static ICommandArgument Patch { get; } = new CommandFlag("-p", "--patch");

		public static ICommandArgument IgnoreOtherWorktrees { get; } = new CommandFlag("--ignore-other-worktrees");

		public static ICommandArgument RecurseSubmodules { get; } = new CommandFlag("--recurse-submodules");

		public static ICommandArgument NoRecurseSubmodules { get; } = new CommandFlag("--no-recurse-submodules");

		public static ICommandArgument NoMoreOptions => CommonArguments.NoMoreOptions;
	}

	public class Builder : CommandBuilderBase
	{
		public Builder()
			: base("checkout")
		{
		}

		public void Quiet() => AddArgument(KnownArguments.Quiet);

		public void Progress() => AddArgument(KnownArguments.Progress);

		public void NoProgress() => AddArgument(KnownArguments.NoProgress);

		public void Force() => AddArgument(KnownArguments.Force);

		public void Ours() => AddArgument(KnownArguments.Ours);

		public void Theirs() => AddArgument(KnownArguments.Theirs);

		public void Branch(bool force = false) => AddArgument(force ? KnownArguments.Branch : KnownArguments.BranchForce);

		public void Track() => AddArgument(KnownArguments.Track);

		public void NoTrack() => AddArgument(KnownArguments.NoTrack);

		public void CreateReflog() => AddArgument(KnownArguments.CreateReflog);

		public void Detach() => AddArgument(KnownArguments.Detach);

		public void Orphan() => AddArgument(KnownArguments.Orphan);

		public void Merge() => AddArgument(KnownArguments.Merge);

		public void NoMoreOptions() => AddArgument(KnownArguments.NoMoreOptions);
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
