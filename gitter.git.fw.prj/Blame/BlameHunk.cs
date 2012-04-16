namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public sealed class BlameHunk : IEnumerable<BlameLine>
	{
		private readonly BlameCommit _commit;
		private readonly List<BlameLine> _lines;

		public BlameHunk(BlameCommit commit, IEnumerable<BlameLine> lines)
		{
			if(commit == null) throw new ArgumentNullException("commit");
			if(lines == null) throw new ArgumentNullException("lines");
			
			_commit = commit;
			_lines = new List<BlameLine>(lines);
		}

		public BlameLine this[int index]
		{
			get { return _lines[index]; }
		}

		public int Count
		{
			get { return _lines.Count; }
		}

		public BlameCommit Commit
		{
			get { return _commit; }
		}

		#region IEnumerable<BlameLine>

		public IEnumerator<BlameLine> GetEnumerator()
		{
			return _lines.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _lines.GetEnumerator();
		}

		#endregion
	}
}
