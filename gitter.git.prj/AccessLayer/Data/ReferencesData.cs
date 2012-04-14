namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	public sealed class ReferencesData
	{
		#region Data

		private readonly IList<BranchData> _heads;
		private readonly IList<BranchData> _remotes;
		private readonly IList<TagData> _tags;
		private readonly RevisionData _stash;

		#endregion

		#region .ctor

		public ReferencesData(IList<BranchData> heads, IList<BranchData> remotes, IList<TagData> tags, RevisionData stash)
		{
			_heads = heads;
			_remotes = remotes;
			_tags = tags;
			_stash = stash;
		}

		#endregion

		#region Properties

		public IList<BranchData> Heads
		{
			get { return _heads; }
		}

		public IList<BranchData> Remotes
		{
			get { return _remotes; }
		}

		public IList<TagData> Tags
		{
			get { return _tags; }
		}

		public RevisionData Stash
		{
			get { return _stash; }
		}

		#endregion
	}
}
