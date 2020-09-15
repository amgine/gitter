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

namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Working tree status information.</summary>
	public sealed class StatusData
	{
		public StatusData(
			IDictionary<string, TreeFileData> stagedFiles,
			IDictionary<string, TreeFileData> unstagedFiles,
			int unstagedUntrackedCount,
			int unstagedRemovedCount,
			int unstagedModifiedCount,
			int unmergedCount,
			int stagedAddedCount,
			int stagedModifiedCount,
			int stagedRemovedCount)
		{
			StagedFiles            = stagedFiles;
			UnstagedFiles          = unstagedFiles;
			UnstagedUntrackedCount = unstagedUntrackedCount;
			UnstagedRemovedCount   = unstagedRemovedCount;
			UnstagedModifiedCount  = unstagedModifiedCount;
			UnmergedCount          = unmergedCount;
			StagedAddedCount       = stagedAddedCount;
			StagedModifiedCount    = stagedModifiedCount;
			StagedRemovedCount     = stagedRemovedCount;
		}

		public IDictionary<string, TreeFileData> StagedFiles { get; }

		public IDictionary<string, TreeFileData> UnstagedFiles { get; }

		public int UnstagedUntrackedCount { get; }

		public int UnstagedModifiedCount { get; }

		public int UnstagedRemovedCount { get; }

		public int UnmergedCount { get; }

		public int StagedAddedCount { get; }

		public int StagedModifiedCount { get; }

		public int StagedRemovedCount { get; }
	}
}
