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

namespace gitter.Git
{
	using System;
	using System.Text;
	using System.Collections.Generic;

	/// <summary>Represents changes made to a file.</summary>
	public sealed class DiffFile : IEnumerable<DiffHunk>, ICloneable
	{
		#region Data

		private readonly IList<DiffHunk> _hunks;

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
			Verify.Argument.IsNotNull(hunks, nameof(hunks));
			Verify.Argument.IsNotNull(stats, nameof(stats));

			OldIndex   = oldIndex;
			NewIndex   = newIndex;
			OldMode    = oldMode;
			NewMode    = newMode;
			SourceFile = sourceFile;
			TargetFile = targetFile;
			Status     = status;
			_hunks     = hunks;
			IsBinary   = isBinary;
			Stats      = stats;
		}

		#endregion

		#region Properties

		/// <summary>Old file index.</summary>
		public string OldIndex { get; }

		/// <summary>New file index.</summary>
		public string NewIndex { get; }

		/// <summary>Old file mode.</summary>
		public int OldMode { get; }

		/// <summary>New file mode.</summary>
		public int NewMode { get; }

		/// <summary>File is binary.</summary>
		public bool IsBinary { get; }

		/// <summary>Line statistics.</summary>
		public DiffStats Stats { get; }

		/// <summary>Source file name.</summary>
		public string SourceFile { get; }

		/// <summary>Target file name.</summary>
		public string TargetFile { get; }

		/// <summary>File status.</summary>
		public FileStatus Status { get; }

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

		public DiffHunk this[int index] => _hunks[index];

		public int HunkCount => _hunks.Count;

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
				if(c + start > hunk.LineCount)
				{
					c = hunk.LineCount - start;
				}
				var newHunk = hunk.Cut(start, c);
				h.Add(newHunk);
				s.Add(newHunk.Stats);
				count -= c;
				start = 0;
				++hid;
			}

			return new DiffFile(
				OldIndex,
				NewIndex,
				OldMode,
				NewMode,
				SourceFile,
				TargetFile,
				Status,
				h,
				IsBinary,
				s);
		}

		internal void ToString(StringBuilder sb)
		{
			Verify.Argument.IsNotNull(sb, nameof(sb));

			sb.Append("diff --git a/");
			sb.Append(SourceFile);
			sb.Append(" b/");
			sb.Append(TargetFile);
			sb.Append(LineEnding.Lf);

			sb.Append("index ");
			sb.Append(OldIndex);
			sb.Append("..");
			sb.Append(NewIndex);
			sb.Append(' ');
			sb.Append(NewMode);
			sb.Append(LineEnding.Lf);

			sb.Append("--- a/");
			sb.Append(SourceFile);
			sb.Append(LineEnding.Lf);

			sb.Append("+++ b/");
			sb.Append(TargetFile);
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

		#region ICloneable

		public DiffFile Clone()
		{
			var hunks = new DiffHunk[_hunks.Count];
			for(int i = 0; i < _hunks.Count; ++i)
			{
				hunks[i] = _hunks[i].Clone();
			}
			return new DiffFile(
				OldIndex,
				NewIndex,
				OldMode,
				NewMode,
				SourceFile,
				TargetFile,
				Status,
				hunks,
				IsBinary,
				Stats.Clone());

		}

		object ICloneable.Clone() => Clone();

		#endregion
	}
}
