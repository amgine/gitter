namespace gitter.Git
{
	using System;
	using System.Text;
	using System.Collections.Generic;

	/// <summary>Represents changes made to a file.</summary>
	public sealed class DiffFile : IEnumerable<DiffHunk>
	{
		#region Data

		private readonly string _sourceFile;
		private readonly string _targetFile;
		private readonly FileStatus _status;
		private readonly IList<DiffHunk> _hunks;
		private readonly string _oldIndex;
		private readonly string _newIndex;
		private readonly int _oldMode;
		private readonly int _newMode;
		private readonly bool _isBinary;
		private readonly DiffStats _stats;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="DiffFile"/>.</summary>
		/// <param name="oldIndex">Old index value.</param>
		/// <param name="newIndex">New index value.</param>
		/// <param name="oldMode">Old mode value.</param>
		/// <param name="newMode">New mode value.</param>
		/// <param name="sourceFile">Source file name.</param>
		/// <param name="targetFile">Target file name.</param>
		/// <param name="status">File status.</param>
		/// <param name="hunks">List of <see cref="DiffHunk"/>.</param>
		/// <param name="isBinary">File is binary.</param>
		/// <param name="stats"><see cref="DiffStats"/>.</param>
		public DiffFile(
			string oldIndex, string newIndex, int oldMode, int newMode,
			string sourceFile, string targetFile,
			FileStatus status, IList<DiffHunk> hunks, bool isBinary, DiffStats stats)
		{
			if(hunks == null) throw new ArgumentNullException("hunks");
			if(stats == null) throw new ArgumentNullException("stats");

			_oldIndex = oldIndex;
			_newIndex = newIndex;
			_oldMode = oldMode;
			_newMode = newMode;
			_sourceFile = sourceFile;
			_targetFile = targetFile;
			_status = status;
			_hunks = hunks;
			_isBinary = isBinary;
			_stats = stats;
		}

		#endregion

		#region Properties

		/// <summary>Old file index.</summary>
		public string OldIndex
		{
			get { return _oldIndex; }
		}

		/// <summary>New file index.</summary>
		public string NewIndex
		{
			get { return _newIndex; }
		}

		/// <summary>Old file mode.</summary>
		public int OldMode
		{
			get { return _oldMode; }
		}

		/// <summary>New file mode.</summary>
		public int NewMode
		{
			get { return _newMode; }
		}

		/// <summary>File is binary.</summary>
		public bool IsBinary
		{
			get { return _isBinary; }
		}

		/// <summary>Line statistics.</summary>
		public DiffStats Stats
		{
			get { return _stats; }
		}

		/// <summary>Source file name.</summary>
		public string SourceFile
		{
			get { return _sourceFile; }
		}

		/// <summary>Target file name.</summary>
		public string TargetFile
		{
			get { return _targetFile; }
		}

		/// <summary>File status.</summary>
		public FileStatus Status
		{
			get { return _status; }
		}

		public int MaxLineNum
		{
			get
			{
				int max = 0;
				foreach(var hunk in _hunks)
				{
					var val = hunk.MaxLineNum;
					if(val > max) max = val;
				}
				return max;
			}
		}

		public DiffHunk this[int index]
		{
			get { return _hunks[index]; }
		}

		public int HunkCount
		{
			get { return _hunks.Count; }
		}

		public int LineCount
		{
			get
			{
				int lines = 0;
				foreach(var h in _hunks)
				{
					lines += h.LineCount;
				}
				return lines;
			}
		}

		#endregion

		#region IEnumerable<DiffHunk>

		public IEnumerator<DiffHunk> GetEnumerator()
		{
			return _hunks.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _hunks.GetEnumerator();
		}

		#endregion

		public DiffFile Cut(int from, int count)
		{
			var h = new List<DiffHunk>();
			var s = new DiffStats();

			int sl = 0;
			int hid = 0;
			while(sl + _hunks[hid].LineCount <= from)
			{
				sl += _hunks[hid].LineCount;
				++hid;
			}

			int start = from - sl;
			while(count > 0)
			{
				var hunk = _hunks[hid];
				var c = count;
				if(c > hunk.LineCount)
				{
					c = hunk.LineCount;
				}
				var newHunk = hunk.Cut(start, c);
				h.Add(newHunk);
				s.Add(newHunk.Stats);
				count -= c;
				start = 0;
				++hid;
			}

			return new DiffFile(
				_oldIndex,
				_newIndex,
				_oldMode,
				_newMode,
				_sourceFile,
				_targetFile,
				_status,
				h,
				_isBinary,
				s);
		}

		internal void ToString(StringBuilder sb)
		{
			if(sb == null) throw new ArgumentNullException("sb");

			sb.Append("diff --git a/");
			sb.Append(_sourceFile);
			sb.Append(" b/");
			sb.Append(_targetFile);
			sb.Append(LineEnding.Lf);

			sb.Append("index ");
			sb.Append(_oldIndex);
			sb.Append("..");
			sb.Append(_newIndex);
			sb.Append(' ');
			sb.Append(_newMode);
			sb.Append(LineEnding.Lf);

			sb.Append("--- a/");
			sb.Append(_sourceFile);
			sb.Append(LineEnding.Lf);

			sb.Append("+++ b/");
			sb.Append(_targetFile);
			sb.Append(LineEnding.Lf);

			foreach(var hunk in _hunks)
			{
				hunk.ToString(sb);
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			ToString(sb);
			return sb.ToString();
		}
	}
}
