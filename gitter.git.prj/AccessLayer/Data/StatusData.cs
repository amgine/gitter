namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Working tree status information.</summary>
	public sealed class StatusData
	{
		private readonly IDictionary<string, TreeFileData> _stagedFiles;
		private readonly IDictionary<string, TreeFileData> _unstagedFiles;

		private readonly int _unstagedUntrackedCount;
		private readonly int _unstagedRemovedCount;
		private readonly int _unstagedModifiedCount;
		private readonly int _unmergedCount;
		private readonly int _stagedAddedCount;
		private readonly int _stagedModifiedCount;
		private readonly int _stagedRemovedCount;

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
			_stagedFiles = stagedFiles;
			_unstagedFiles = unstagedFiles;

			_unstagedUntrackedCount = unstagedUntrackedCount;
			_unstagedRemovedCount = unstagedRemovedCount;
			_unstagedModifiedCount = unstagedModifiedCount;
			_unmergedCount = unmergedCount;
			_stagedAddedCount = stagedAddedCount;
			_stagedModifiedCount = stagedModifiedCount;
			_stagedRemovedCount = stagedRemovedCount;
		}

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
	}
}
