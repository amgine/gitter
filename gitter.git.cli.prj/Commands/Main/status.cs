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

using System;
using System.Collections.Generic;

/// <summary>Show the working tree status.</summary>
public sealed class StatusCommand : Command
{
	/// <summary>
	/// Terminate entries with NUL, instead of LF.
	/// This implies the --porcelain output format if no other format is given.
	/// </summary>
	public static ICommandArgument NullTerminate()
		=> new CommandFlag("-z");

	/// <summary>Give the output in the short-format.</summary>
	public static ICommandArgument Short()
		=> new CommandFlag("--short");

	/// <summary>
	/// Give the output in a stable, easy-to-parse format for scripts. Currently this is identical to --short output,
	/// but is guaranteed not to change in the future, making it safe for scripts.
	/// </summary>
	public static ICommandArgument Porcelain()
		=> new CommandFlag("--porcelain");

	/// <summary>Show the branch and tracking info even in short-format.</summary>
	public static ICommandArgument Branch()
		=> new CommandFlag("--branch");

	/// <summary>Show untracked files (Default: all).</summary>
	public static ICommandArgument UntrackedFiles(StatusUntrackedFilesMode mode)
		=> mode switch
		{
			StatusUntrackedFilesMode.No     => new CommandParameterValue("--untracked-files", "no", '='),
			StatusUntrackedFilesMode.Normal => new CommandParameterValue("--untracked-files", "normal", '='),
			StatusUntrackedFilesMode.All    => new CommandParameterValue("--untracked-files", "all", '='),
			_ => throw new ArgumentException($"Unknown {nameof(StatusUntrackedFilesMode)}", nameof(mode)),
		};

	/// <summary>Ignore changes to submodules when looking for changes.</summary>
	public static ICommandArgument IgnoreSubmodules()
		=> new CommandFlag("--ignore-submodules");

	/// <summary>Ignore changes to submodules when looking for changes.</summary>
	public static ICommandArgument IgnoreSubmodules(StatusIgnoreSubmodulesMode mode)
		=> mode switch
		{
			StatusIgnoreSubmodulesMode.All       => new CommandParameterValue("--ignore-submodules", "all", '='),
			StatusIgnoreSubmodulesMode.Dirty     => new CommandParameterValue("--ignore-submodules", "dirty", '='),
			StatusIgnoreSubmodulesMode.Untracked => new CommandParameterValue("--ignore-submodules", "untracked", '='),
			_ => throw new ArgumentException($"Unknown {nameof(StatusIgnoreSubmodulesMode)}", nameof(mode)),
		};

	public static ICommandArgument NoMoreOptions()
		=> CommandFlag.NoMoreOptions;

	public static ICommandArgument Verbose()
		=> CommandFlag.Verbose;

	public StatusCommand()
		: base("status")
	{
	}

	public StatusCommand(params ICommandArgument[] args)
		: base("status", args)
	{
	}

	public StatusCommand(IList<ICommandArgument> args)
		: base("status", args)
	{
	}
}
