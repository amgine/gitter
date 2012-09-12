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
			Verify.Argument.IsNotNull(commit, "commit");
			Verify.Argument.IsNotNull(lines, "lines");
			
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
