namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	public sealed class BranchesData
	{
		#region Data

		private readonly IList<BranchData> _heads;
		private readonly IList<BranchData> _remotes;

		#endregion

		#region .ctor

		public BranchesData(IList<BranchData> heads, IList<BranchData> remotes)
		{
			_heads = heads;
			_remotes = remotes;
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

		#endregion
	}
}
