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

#nullable enable

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Collections.Generic;
using System.Globalization;

public enum BranchColorWhen
{
	Always,
	Never,
	Auto,
}

/// <summary>List, create, or delete branches.</summary>
public sealed class BranchCommand : Command
{
	const string BranchCommandName = @"branch";

	public static class KnownArguments
	{
		/// <summary>Delete a branch. The branch must be fully merged in HEAD.</summary>
		public static ICommandArgument Delete { get; } = new CommandFlag("-d", "--delete");

		/// <summary>Delete a branch irrespective of its merged status.</summary>
		public static ICommandArgument DeleteForce { get; } = new CommandFlag("-D");

		/// <summary>
		///	Create the branch's reflog. This activates recording of all changes made to the branch ref,
		///	enabling use of date based sha1 expressions such as "branchname@{yesterday}".
		/// </summary>
		public static ICommandArgument CreateReflog { get; } = new CommandFlag("--create-reflog");

		/// <summary>
		///	Create the branch's reflog. This activates recording of all changes made to the branch ref,
		///	enabling use of date based sha1 expressions such as "branchname@{yesterday}".
		/// </summary>
		public static ICommandArgument CreateReflogOld { get; } = new CommandFlag("-l");

		/// <summary>Reset branchname to startpoint if branchname exists already. Without -f git-branch refuses to change an existing branch.</summary>
		public static ICommandArgument Force { get; } = new CommandFlag("-f", "--force");

		/// <summary>Move/rename a branch and the corresponding reflog.</summary>
		public static ICommandArgument Move { get; } = new CommandFlag("-m", "--move");

		/// <summary>Shortcut for --move --force.</summary>
		public static ICommandArgument MoveForce { get; } = new CommandFlag("-M");

		/// <summary>Move/rename a branch and the corresponding reflog.</summary>
		public static ICommandArgument Copy { get; } = new CommandFlag("-c", "--copy");

		/// <summary>Shortcut for --move --force.</summary>
		public static ICommandArgument CopyForce { get; } = new CommandFlag("-C");

		/// <summary>Color branches to highlight current, local, and remote-tracking branches.</summary>
		public static ICommandArgument ColorAlways { get; } = new CommandParameterValue("--color", "always", ' ');

		/// <summary>Color branches to highlight current, local, and remote-tracking branches.</summary>
		public static ICommandArgument ColorNever { get; } = new CommandParameterValue("--color", "never", ' ');

		/// <summary>Color branches to highlight current, local, and remote-tracking branches.</summary>
		public static ICommandArgument ColorAuto { get; } = new CommandParameterValue("--color", "auto", ' ');

		/// <summary>Color branches to highlight current, local, and remote-tracking branches.</summary>
		/// <param name="when">The value must be always (the default), never, or auto.</param>
		public static ICommandArgument Color(BranchColorWhen when)
			=> when switch
			{
				BranchColorWhen.Always => ColorAlways,
				BranchColorWhen.Never  => ColorNever,
				BranchColorWhen.Auto   => ColorAuto,
				_ => throw new ArgumentException(nameof(when)),
			};

		/// <summary>Turn off branch colors, even when the configuration file gives the default to color output. Same as --color=never.</summary>
		public static ICommandArgument NoColor { get; } = new CommandFlag("--no-color");

		/// <summary>Sorting and filtering branches are case insensitive.</summary>
		public static ICommandArgument IgnoreCase { get; } = new CommandFlag("-i", "--ignore-case");

		/// <summary>List or delete (if used with -d) the remote-tracking branches.</summary>
		public static ICommandArgument Remotes { get; } = new CommandFlag("-r", "--remotes");

		/// <summary>
		/// List branches.With optional pattern..., e.g.git branch --list 'maint-*', list only the branches that match the pattern(s).
		/// </summary>
		public static ICommandArgument List { get; } = new CommandFlag("-l", "--list");

		/// <summary>List both remote-tracking branches and local branches.</summary>
		public static ICommandArgument All { get; } = new CommandFlag("-a", "--all");

		/// <summary>
		/// When creating a new branch, set up branch.name.remote and branch.name.merge configuration entries
		/// to mark the start-point branch as "upstream" from the new branch.
		/// </summary>
		public static ICommandArgument Track { get; } = new CommandFlag("-t", "--track");

		/// <summary>Do not set up "upstream" configuration, even if the branch.autoSetupMerge configuration variable is true.</summary>
		public static ICommandArgument NoTrack { get; } = new CommandFlag("--no-track");

		/// <summary>
		/// When in list mode, show sha1 and commit subject line for each head, along with relationship to upstream
		/// branch (if any). If given twice, print the name of the upstream branch, as well.
		/// </summary>
		public static ICommandArgument Verbose => CommonArguments.Verbose;

		/// <summary>
		/// Be more quiet when creating or deleting a branch, suppressing non-error messages.
		/// </summary>
		public static ICommandArgument Quiet => CommonArguments.Quiet;

		/// <summary>Display the full sha1s in the output listing rather than abbreviating them.</summary>
		public static ICommandArgument Abbrev(int length = 7) => new CommandParameterValue("--abbrev", length.ToString(CultureInfo.InvariantCulture), '=');

		/// <summary>Display the full sha1s in the output listing rather than abbreviating them.</summary>
		public static ICommandArgument NoAbbrev { get; } = new CommandFlag("--no-abbrev");

		private static ICommandArgument ContainsHEAD { get; } = new CommandFlag("--contains");

		private static ICommandArgument NoContainsHEAD { get; } = new CommandFlag("--no-contains");

		private static ICommandArgument MergedHEAD { get; } = new CommandFlag("--merged");

		private static ICommandArgument NoMergedHEAD { get; } = new CommandFlag("--no-merged");

		public static ICommandArgument Contains(string? commit) => commit is null ? ContainsHEAD : new CommandParameterValue("--contains", commit, ' ');

		public static ICommandArgument NoContains(string? commit) => commit is null ? NoContainsHEAD : new CommandParameterValue("--no-contains", commit, ' ');

		public static ICommandArgument Merged(string? commit) => commit is null ? MergedHEAD : new CommandParameterValue("--merged", commit, ' ');

		public static ICommandArgument NoMerged(string? commit) => commit is null ? NoMergedHEAD : new CommandParameterValue("--no-merged", commit, ' ');
	}

	public sealed class Builder(Version gitVersion) : CommandBuilderBase(BranchCommandName)
	{
		static readonly Version NewReflogArgVersion = new(2, 20, 0);

		private Version GitVersion { get; } = gitVersion;

		public void Delete(bool force = false)
			=> AddArgument(force
				? KnownArguments.Delete
				: KnownArguments.DeleteForce);

		public void Move(bool force = false)
			=> AddArgument(force
				? KnownArguments.Move
				: KnownArguments.MoveForce);

		public void Copy(bool force = false)
			=> AddArgument(force
				? KnownArguments.Copy
				: KnownArguments.CopyForce);

		public void Force() => AddArgument(KnownArguments.Force);

		public void CreateReflog()
			=> AddArgument(GitVersion >= NewReflogArgVersion
				? KnownArguments.CreateReflog
				: KnownArguments.CreateReflogOld);

		public void Color(BranchColorWhen when = BranchColorWhen.Always) => AddArgument(KnownArguments.Color(when));

		public void NoColor() => AddArgument(KnownArguments.NoColor);

		public void List() => AddArgument(KnownArguments.List);

		public void Remotes() => AddArgument(KnownArguments.Remotes);

		public void All() => AddArgument(KnownArguments.All);

		public void Track() => AddArgument(KnownArguments.Track);

		public void NoTrack() => AddArgument(KnownArguments.NoTrack);

		public void Abbrev(int length = 7) => AddArgument(KnownArguments.Abbrev(length));

		public void NoAbbrev() => AddArgument(KnownArguments.NoAbbrev);

		public void Verbose() => AddArgument(KnownArguments.Verbose);

		public void Quiet() => AddArgument(KnownArguments.Quiet);

		public void Contains(string? commit = null) => AddArgument(KnownArguments.Contains(commit));

		public void NoContains(string? commit = null) => AddArgument(KnownArguments.NoContains(commit));

		public void Merged(string? commit = null) => AddArgument(KnownArguments.Merged(commit));

		public void NoMerged(string? commit = null) => AddArgument(KnownArguments.NoMerged(commit));
	}

	public BranchCommand()
		: base(BranchCommandName)
	{
	}

	public BranchCommand(params ICommandArgument[] args)
		: base(BranchCommandName, args)
	{
	}

	public BranchCommand(IList<ICommandArgument> args)
		: base(BranchCommandName, args)
	{
	}
}
