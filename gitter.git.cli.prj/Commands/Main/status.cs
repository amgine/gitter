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

	/// <summary>Show the working tree status.</summary>
	public sealed class StatusCommand : Command
	{
		/// <summary>
		/// Terminate entries with NUL, instead of LF.
		/// This implies the --porcelain output format if no other format is given.
		/// </summary>
		public static CommandArgument NullTerminate()
		{
			return new CommandArgument("-z");
		}

		/// <summary>Give the output in the short-format.</summary>
		public static CommandArgument Short()
		{
			return new CommandArgument("--short");
		}

		/// <summary>
		/// Give the output in a stable, easy-to-parse format for scripts. Currently this is identical to --short output,
		/// but is guaranteed not to change in the future, making it safe for scripts.
		/// </summary>
		public static CommandArgument Porcelain()
		{
			return new CommandArgument("--porcelain");
		}

		/// <summary>Show the branch and tracking info even in short-format.</summary>
		public static CommandArgument Branch()
		{
			return new CommandArgument("--branch");
		}

		/// <summary>Show untracked files (Default: all).</summary>
		public static CommandArgument UntrackedFiles(StatusUntrackedFilesMode mode)
		{
			switch(mode)
			{
				case StatusUntrackedFilesMode.No:
					return new CommandArgument("--untracked-files", "no", '=');
				case StatusUntrackedFilesMode.Normal:
					return new CommandArgument("--untracked-files", "normal", '=');
				case StatusUntrackedFilesMode.All:
					return new CommandArgument("--untracked-files", "all", '=');
				default:
					throw new ArgumentException("mode");
			}
		}

		/// <summary>Ignore changes to submodules when looking for changes.</summary>
		public static CommandArgument IgnoreSubmodules()
		{
			return new CommandArgument("--ignore-submodules");
		}

		/// <summary>Ignore changes to submodules when looking for changes.</summary>
		public static CommandArgument IgnoreSubmodules(StatusIgnoreSubmodulesMode mode)
		{
			switch(mode)
			{
				case StatusIgnoreSubmodulesMode.All:
					return new CommandArgument("--ignore-submodules", "all", '=');
				case StatusIgnoreSubmodulesMode.Dirty:
					return new CommandArgument("--ignore-submodules", "dirty", '=');
				case StatusIgnoreSubmodulesMode.Untracked:
					return new CommandArgument("--ignore-submodules", "untracked", '=');
				default:
					throw new ArgumentException("mode");
			}
		}

		public static CommandArgument NoMoreOptions()
		{
			return CommandArgument.NoMoreOptions();
		}

		public static CommandArgument Verbose()
		{
			return CommandArgument.Verbose();
		}

		public StatusCommand()
			: base("status")
		{
		}

		public StatusCommand(params CommandArgument[] args)
			: base("status", args)
		{
		}

		public StatusCommand(IList<CommandArgument> args)
			: base("status", args)
		{
		}
	}
}
