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

	/// <summary>Create an archive of files from a named tree.</summary>
	public sealed class ArchiveCommand : Command
	{
		/// <summary>Format of the resulting archive.</summary>
		public static CommandArgument Format(string format)
		{
			return new CommandArgument("--format", format);
		}

		/// <summary>Show all available formats.</summary>
		public static CommandArgument List()
		{
			return new CommandArgument("--list");
		}

		/// <summary>Report progress to stderr.</summary>
		public static CommandArgument Verbose()
		{
			return CommandArgument.Verbose();
		}

		/// <summary>Prepend prefix to each filename in the archive.</summary>
		public static CommandArgument Prefix(string prefix)
		{
			return new CommandArgument("--prefix", prefix);
		}

		/// <summary>Write the archive to file instead of stdout.</summary>
		public static CommandArgument Output(string file)
		{
			return new CommandArgument("--output", file.AssureDoubleQuotes());
		}

		/// <summary>Look for attributes in .gitattributes in working directory too.</summary>
		public static CommandArgument WorktreeAttributes()
		{
			return new CommandArgument("--worktree-attributes");
		}

		/// <summary>Instead of making a tar archive from the local repository, retrieve a tar archive from a remote repository.</summary>
		public static CommandArgument Remote(string repo)
		{
			return new CommandArgument("--remote", repo);
		}

		/// <summary>Used with --remote to specify the path to the git-upload-archive on the remote side.</summary>
		public static CommandArgument Exec(string gitUploadArchive)
		{
			return new CommandArgument("--exec", gitUploadArchive);
		}

		public ArchiveCommand()
			: base("archive")
		{
		}

		public ArchiveCommand(params CommandArgument[] args)
			: base("archive", args)
		{
		}

		public ArchiveCommand(IList<CommandArgument> args)
			: base("archive", args)
		{
		}
	}
}
