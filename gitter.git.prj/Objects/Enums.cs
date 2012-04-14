namespace gitter.Git
{
	using System;

	[Flags]
	public enum StagedStatus
	{
		None		= 0,

		Committed	= (1 << 0),
		Unstaged	= (1 << 1),
		Staged		= (1 << 2),
	}

	public enum DateFormat
	{
		SystemDefault,
		Relative,
		UnixTimestamp,
		/// <summary>2010-06-16 14:30:52 +0400</summary>
		ISO8601,
		/// <summary>Wed, 16 Jun 2010 11:14:30 +0400</summary>
		RFC2822,
	}

	public enum FileStatus
	{
		Unknown		= 0,
		Cached		= (int)'H',
		Unmerged	= (int)'M',
		Removed		= (int)'R',
		Modified	= (int)'C',
		Killed		= (int)'K',
		Added		= (int)'?',
		Renamed		= (int)'r',
		Copied		= (int)'c',
		ModeChanged	= (int)'x',
	}

	public enum TagFetchMode
	{
		Default,
		NoTags,
		AllTags,
	}

	public enum ConfigFile
	{
		Repository,
		User,
		System,
		Other,
	}

	public enum SubmoduleUpdateMode
	{
		Checkout,
		Rebase,
		Merge,
	}
}
