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

	/// <summary>Remove untracked files from the working tree.</summary>
	public sealed class CleanCommand : Command
	{
		/// <summary>
		/// If the git configuration variable clean.requireForce is not set to false, git clean will refuse to run unless given -f or -n.
		/// </summary>
		public static CommandArgument Force()
		{
			return new CommandArgument("--force");
		}

		/// <summary>
		/// Remove untracked directories in addition to untracked files. If an untracked directory is managed by a different git
		/// repository, it is not removed by default. Use -f option twice if you really want to remove such a directory.
		/// </summary>
		public static CommandArgument Directories()
		{
			return new CommandArgument("-d");
		}

		/// <summary>Be quiet, only report errors, but not the files that are successfully removed.</summary>
		public static CommandArgument Quiet()
		{
			return new CommandArgument("--quiet");
		}

		/// <summary>Don't actually remove anything, just show what would be done.</summary>
		public static CommandArgument DryRun()
		{
			return new CommandArgument("--dry-run");
		}

		/// <summary>
		/// Specify special exceptions to not be cleaned. Each <pattern> is the same form as in $GIT_DIR/info/excludes
		/// and this option can be given multiple times.
		/// </summary>
		public static CommandArgument Exclude(string pattern)
		{
			return new CommandArgument("--exclude", pattern, '=');
		}

		/// <summary>
		/// Don't use the ignore rules. This allows removing all untracked files, including build products.
		/// This can be used (possibly in conjunction with git reset) to create a pristine working directory to test a clean build.
		/// </summary>
		public static CommandArgument IncludeIgnored()
		{
			return new CommandArgument("-x");
		}

		/// <summary>
		/// Remove only files ignored by git. This may be useful to rebuild everything from scratch, but keep manually created files.
		/// </summary>
		public static CommandArgument ExcludeUntracked()
		{
			return new CommandArgument("-X");
		}

		public static CommandArgument NoMoreOptions()
		{
			return CommandArgument.NoMoreOptions();
		}

		public CleanCommand()
			: base("clean")
		{
		}

		public CleanCommand(params CommandArgument[] args)
			: base("clean", args)
		{
		}

		public CleanCommand(IList<CommandArgument> args)
			: base("clean", args)
		{
		}
	}
}
