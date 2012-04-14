namespace gitter.Git
{
	using System;

	/// <summary>Helper class for determining available extra fetures.</summary>
	internal static class GitFeatures
	{
		/// <summary>Ability to create orphan branches.</summary>
		public static readonly VersionFeature CheckoutOrphan =
			new VersionFeature("checkout --orphan", new Version(1, 7, 2, 3));

		/// <summary>Ability to remove submodules from git status output.</summary>
		public static readonly VersionFeature StatusIgnoreSubmodules =
			new VersionFeature("status --ignore-submodules", new Version(1, 7, 2, 3));

		/// <summary>Ability to output subject + body as is in log --format output.</summary>
		public static readonly VersionFeature LogFormatBTag =
			new VersionFeature("log --format=format:%B", new Version(1, 7, 1, 0));

		/// <summary>Advanced git notes commands.</summary>
		public static readonly VersionFeature AdvancedNotesCommands =
			new VersionFeature("notes list", new Version(1, 7, 1, 0));

		/// <summary>Ability to output progress for clone/fetch/pull/push.</summary>
		public static readonly VersionFeature ProgressFlag =
			new VersionFeature("--progress", new Version(1, 7, 1, 2));

		/// <summary>Ability to exclude patterns from git clean.</summary>
		public static readonly VersionFeature CleanExcludeOption =
			new VersionFeature("clean --exclude", new Version(1, 7, 3, 0));

		/// <summary>Ability to include untracked files in stash.</summary>
		public static readonly VersionFeature StashIncludeUntrackedOption =
			new VersionFeature("stash --include-untracked", new Version(1, 7, 7, 0));
	}
}
