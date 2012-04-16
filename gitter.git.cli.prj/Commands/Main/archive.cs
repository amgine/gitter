namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	/// <summary>Archive format.</summary>
	public enum ArchiveFormat
	{
		/// <summary>Tar.</summary>
		Tar,
		/// <summary>Zip.</summary>
		Zip,
	}

	/// <summary>Create an archive of files from a named tree.</summary>
	public sealed class ArchiveCommand : Command
	{
		/// <summary>Format of the resulting archive: tar or zip. The default is tar.</summary>
		public static CommandArgument Format(ArchiveFormat format)
		{
			switch(format)
			{
				case ArchiveFormat.Tar:
					return new CommandArgument("--format", "tar");
				case ArchiveFormat.Zip:
					return new CommandArgument("--format", "zip");
				default:
					throw new ArgumentException("format");
			}
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
			return new CommandArgument("--output", file);
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
