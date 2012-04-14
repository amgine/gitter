namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	public sealed class BlameFile : IEnumerable<BlameHunk>
	{
		private readonly string _fileName;
		private readonly List<BlameHunk> _hunks;
		private readonly int _lineCount;

		public BlameFile(string fileName, List<BlameHunk> hunks)
		{
			if(fileName == null)
				throw new ArgumentNullException("fileName");
			if(hunks == null)
				throw new ArgumentNullException("hunks");

			_fileName = fileName;
			_hunks = hunks;

			foreach(var hunk in _hunks) _lineCount += hunk.Count;
		}

		public string Name
		{
			get { return _fileName; }
		}

		public BlameHunk this[int index]
		{
			get { return _hunks[index]; }
		}

		public int Count
		{
			get { return _hunks.Count; }
		}

		public int LineCount
		{
			get { return _lineCount; }
		}

		public override string ToString()
		{
			return _fileName;
		}

		#region IEnumerable<BlameHunk>

		public IEnumerator<BlameHunk> GetEnumerator()
		{
			return _hunks.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _hunks.GetEnumerator();
		}

		#endregion
	}
}
