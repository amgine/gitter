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

namespace gitter.Git;

using System;
using System.Text;
using System.Collections.Generic;

/// <summary>Continuous block of changed lines including context.</summary>
public sealed class DiffHunk : IList<DiffLine>, ICloneable
{
	#region Data

	private readonly DiffColumnHeader[] _headers;
	private readonly IList<DiffLine> _lines;

	#endregion

	#region .ctor

	/// <summary>Create <see cref="DiffHunk"/>.</summary>
	/// <param name="headers">Column headers.</param>
	/// <param name="lines">List of diff lines.</param>
	/// <param name="stats"><see cref="DiffStats"/>.</param>
	public DiffHunk(DiffColumnHeader[] headers, IList<DiffLine> lines, DiffStats stats, bool isBinary)
	{
		Verify.Argument.IsNotNull(headers);
		Verify.Argument.IsNotNull(lines);
		Verify.Argument.IsNotNull(stats);

		_headers = headers;
		_lines   = lines;
		Stats    = stats;
		IsBinary = isBinary;
	}

	/// <summary>Create empty <see cref="DiffHunk"/>.</summary>
	/// <param name="headers">Column headers.</param>
	public DiffHunk(DiffColumnHeader[] headers)
		: this(headers, new List<DiffLine>(), new DiffStats(), false)
	{
	}

	#endregion

	#region Properties

	/// <summary>Hunk is empty.</summary>
	public bool IsEmpty => _lines.Count == 0;

	public bool IsBinary { get; }

	public DiffStats Stats { get; }

	public DiffLine this[int index]
	{
		get => _lines[index];
		set
		{
			Verify.Argument.IsNotNull(value);

			var line = _lines[index];
			Stats.Decrement(line.State);
			_lines[index] = value;
			Stats.Increment(value.State);
		}
	}

	public int LineCount => _lines.Count;

	public int MaxLineNum
	{
		get
		{
			int max = 0;
			for(int j = 0; j < ColumnCount; ++j)
			{
				for(int i = _lines.Count - 1; i >= 0; --i)
				{
					var val = _lines[i].Nums[j];
					if(val > 0)
					{
						if(val > max) max = val;
						break;
					}
				}
			}
			return max;
		}
	}

	public int ColumnCount => _headers.Length;

	int ICollection<DiffLine>.Count => _lines.Count;

	#endregion

	public int IndexOf(DiffLine item) => _lines.IndexOf(item);

	public void Insert(int index, DiffLine line)
	{
		Verify.Argument.IsNotNull(line);

		_lines.Insert(index, line);
		Stats.Increment(line.State);
	}

	public void RemoveAt(int index)
	{
		var line = _lines[index];
		_lines.RemoveAt(index);
		Stats.Decrement(line.State);
	}

	public void Add(DiffLine line)
	{
		Verify.Argument.IsNotNull(line);

		_lines.Add(line);
		Stats.Increment(line.State);
	}

	public void Clear()
	{
		_lines.Clear();
		Stats.Reset();
	}

	public bool Contains(DiffLine item)
	{
		return _lines.Contains(item);
	}

	public void CopyTo(DiffLine[] array, int arrayIndex)
	{
		_lines.CopyTo(array, arrayIndex);
	}

	public bool IsReadOnly => _lines.IsReadOnly;

	public bool Remove(DiffLine line)
	{
		if(_lines.Remove(line))
		{
			Stats.Decrement(line.State);
			return true;
		}
		return false;
	}

	public DiffHunk Cut(int from, int count)
	{
		if((from < 0) || (from == _lines.Count)) throw new ArgumentOutOfRangeException(nameof(from));
		if((count <= 0) || (from + count > _lines.Count)) throw new ArgumentOutOfRangeException(nameof(count));

		int to = from + count - 1;
		// exclude header
		if(from == 0) ++from;
		// expand with context
		while(from > 1 && _lines[from - 1].State == DiffLineState.Context)
		{
			--from;
		}
		while(to < _lines.Count - 1 && _lines[to + 1].State == DiffLineState.Context)
		{
			++to;
		}
		if(from == 1 && to == _lines.Count - 1)
		{
			// copy whole hunk
			var lines = new List<DiffLine>(_lines.Count);
			lines.AddRange(_lines);
			return new DiffHunk(_headers, lines, Stats.Clone(), IsBinary);
		}
		else
		{
			var lines = new List<DiffLine>(to - from + 2);
			// prepare new header
			int rf = _lines[from].Nums[0];
			int af = _lines[from].Nums[1];
			int rc = 0;
			int ac = 0;
			for(int i = from; i <= to; ++i)
			{
				switch(_lines[i].State)
				{
					case DiffLineState.Removed:
						++rc;
						break;
					case DiffLineState.Added:
						++ac;
						break;
					case DiffLineState.Context:
						++rc;
						++ac;
						break;
				}
			}
			var culture = System.Globalization.CultureInfo.InvariantCulture;
			var header = new DiffLine(
				DiffLineState.Header,
				new [] { DiffLineState.Header, DiffLineState.Header },
				new [] { 0, 0 },
				string.Format(culture, "@@ -{0},{1} +{2},{3} @@", rf, rc, af, ac),
				LineEnding.Lf);
			lines.Add(header);
			var stats = new DiffStats();
			stats.Increment(header.State);
			// copy lines
			for(int i = from; i <= to; ++i)
			{
				var line = _lines[i];
				lines.Add(line);
				stats.Increment(line.State);
			}
			return new DiffHunk(new[]
				{
					new DiffColumnHeader(DiffColumnAction.Remove, rf, rc),
					new DiffColumnHeader(DiffColumnAction.Add, af, ac)
				}, lines, stats, IsBinary);
		}
	}

	public IEnumerator<DiffLine> GetEnumerator()
		=> _lines.GetEnumerator();

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		=> _lines.GetEnumerator();

	internal void ToString(StringBuilder sb)
	{
		foreach(var line in _lines)
		{
			line.ToString(sb);
			sb.Append(line.Ending);
		}
	}

	public override string ToString()
	{
		var sb = new StringBuilder();
		ToString(sb);
		return sb.ToString();
	}

	#region ICloneable

	public DiffHunk Clone()
	{
		var lines = new DiffLine[_lines.Count];
		for(int i = 0; i < _lines.Count; ++i)
		{
			lines[i] = _lines[i].Clone();
		}
		return new DiffHunk(
			(DiffColumnHeader[])_headers.Clone(),
			lines,
			Stats.Clone(),
			IsBinary);
	}

	object ICloneable.Clone() => Clone();

	#endregion
}
