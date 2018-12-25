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

namespace gitter.Git
{
	using System;

	/// <summary>Helper class for determining available extra features.</summary>
	public static class GitFeatures
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
