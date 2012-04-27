namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	public sealed class RevisionGraphData
	{
		private readonly string _hash;
		private readonly IList<string> _parents;

		public RevisionGraphData(string hash, IList<string> parents)
		{
			_hash = hash;
			_parents = parents;
		}

		public string Hash
		{
			get { return _hash; }
		}

		public IList<string> Parents
		{
			get { return _parents; }
		}
	}
}
