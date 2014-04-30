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
		#region Data

		private readonly IDictionary<string, TreeFileData> _stagedFiles;
		private readonly IDictionary<string, TreeFileData> _unstagedFiles;
		private readonly int _unstagedUntrackedCount;
		private readonly int _unstagedRemovedCount;
		private readonly int _unstagedModifiedCount;
		private readonly int _unmergedCount;
		private readonly int _stagedAddedCount;
		private readonly int _stagedModifiedCount;
		private readonly int _stagedRemovedCount;

		#endregion

		#region .ctor

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
			_stagedFiles            = stagedFiles;
			_unstagedFiles          = unstagedFiles;
			_unstagedUntrackedCount = unstagedUntrackedCount;
			_unstagedRemovedCount   = unstagedRemovedCount;
			_unstagedModifiedCount  = unstagedModifiedCount;
			_unmergedCount          = unmergedCount;
			_stagedAddedCount       = stagedAddedCount;
			_stagedModifiedCount    = stagedModifiedCount;
			_stagedRemovedCount     = stagedRemovedCount;
		}

		#endregion

		#region Properties

		public IDictionary<string, TreeFileData> StagedFiles
		{
			get { return _stagedFiles; }
		}

		public IDictionary<string, TreeFileData> UnstagedFiles
		{
			get { return _unstagedFiles; }
		}

		public int UnstagedUntrackedCount
		{
			get { return _unstagedUntrackedCount; }
		}

		public int UnstagedModifiedCount
		{
			get { return _unstagedModifiedCount; }
		}

		public int UnstagedRemovedCount
		{
			get { return _unstagedRemovedCount; }
		}

		public int UnmergedCount
		{
			get { return _unmergedCount; }
		}

		public int StagedAddedCount
		{
			get { return _stagedAddedCount; }
		}

		public int StagedModifiedCount
		{
			get { return _stagedModifiedCount; }
		}

		public int StagedRemovedCount
		{
			get { return _stagedRemovedCount; }
		}

		#endregion
	}
}
