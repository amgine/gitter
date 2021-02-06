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

	/// <summary>Reset current HEAD to the specified state.</summary>
	public sealed class ResetCommand : Command
	{
		public static ICommandArgument Mode(ResetMode mode)
			=> mode switch
			{
				ResetMode.Mixed => ResetCommand.Mixed(),
				ResetMode.Soft  => ResetCommand.Soft(),
				ResetMode.Hard  => ResetCommand.Hard(),
				ResetMode.Merge => ResetCommand.Merge(),
				ResetMode.Keep  => ResetCommand.Keep(),
				_ => throw new ArgumentException("Unknown ResetMode", nameof(mode)),
			};

		/// <summary>
		/// Resets the index but not the working tree (i.e., the changed files are preserved but not marked for commit)
		/// and reports what has not been updated. This is the default action.
		/// </summary>
		public static ICommandArgument Mixed()
			=> new CommandFlag("--mixed");

		/// <summary>
		/// Does not touch the index file nor the working tree at all, but requires them to be in a good order.
		/// This leaves all your changed files "Changes to be committed", as git-status would put it
		/// </summary>
		public static ICommandArgument Soft()
			=> new CommandFlag("--soft");

		/// <summary>
		/// Matches the working tree and index to that of the tree being switched to. Any changes to
		/// tracked files in the working tree since "commit" are lost.
		/// </summary>
		public static ICommandArgument Hard()
			=> new CommandFlag("--hard");

		/// <summary>
		/// Resets the index to match the tree recorded by the named commit, and updates the
		/// files that are different between the named commit and the current commit in the working tree.
		/// </summary>
		public static ICommandArgument Merge()
			=> new CommandFlag("--merge");

		public static ICommandArgument Keep()
			=> new CommandFlag("--keep");

		/// <summary>Be quiet, only report errors.</summary>
		public static ICommandArgument Quiet()
			=> new CommandFlag("-q");

		/// <summary>Do not interpret any more arguments as options.</summary>
		public static ICommandArgument NoMoreOptions()
			=> CommandFlag.NoMoreOptions();

		public ResetCommand()
			: base("reset")
		{
		}

		public ResetCommand(params ICommandArgument[] args)
			: base("reset", args)
		{
		}

		public ResetCommand(IList<ICommandArgument> args)
			: base("reset", args)
		{
		}
	}
}
