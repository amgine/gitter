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
		public static ICommandArgument Format(string format)
		{
			return new CommandParameterValue("--format", format);
		}

		/// <summary>Show all available formats.</summary>
		public static ICommandArgument List()
		{
			return new CommandFlag("--list");
		}

		/// <summary>Report progress to stderr.</summary>
		public static ICommandArgument Verbose()
		{
			return CommandFlag.Verbose();
		}

		/// <summary>Prepend prefix to each filename in the archive.</summary>
		public static ICommandArgument Prefix(string prefix)
		{
			return new CommandParameterValue("--prefix", prefix);
		}

		/// <summary>Write the archive to file instead of stdout.</summary>
		public static ICommandArgument Output(string file)
		{
			return new CommandParameterValue("--output", file.AssureDoubleQuotes());
		}

		/// <summary>Look for attributes in .gitattributes in working directory too.</summary>
		public static ICommandArgument WorktreeAttributes()
		{
			return new CommandFlag("--worktree-attributes");
		}

		/// <summary>Instead of making a tar archive from the local repository, retrieve a tar archive from a remote repository.</summary>
		public static ICommandArgument Remote(string repo)
		{
			return new CommandParameterValue("--remote", repo);
		}

		/// <summary>Used with --remote to specify the path to the git-upload-archive on the remote side.</summary>
		public static ICommandArgument Exec(string gitUploadArchive)
		{
			return new CommandParameterValue("--exec", gitUploadArchive);
		}

		public ArchiveCommand()
			: base("archive")
		{
		}

		public ArchiveCommand(params ICommandArgument[] args)
			: base("archive", args)
		{
		}

		public ArchiveCommand(IList<ICommandArgument> args)
			: base("archive", args)
		{
		}
	}
}
