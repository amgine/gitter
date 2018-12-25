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
	using System.Collections.Generic;

	public sealed class BlameFile : IEnumerable<BlameHunk>
	{
		private readonly string _fileName;
		private readonly List<BlameHunk> _hunks;
		private readonly int _lineCount;

		public BlameFile(string fileName, List<BlameHunk> hunks)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(fileName, nameof(fileName));
			Verify.Argument.IsNotNull(hunks, nameof(hunks));

			_fileName = fileName;
			_hunks = hunks;

			foreach(var hunk in _hunks)
			{
				_lineCount += hunk.Count;
			}
		}

		public string Name
		{
			get { return _fileName; }
		}

		public BlameHunk this[int index]
		{
			get { return _hunks[index]; }
		}

		public BlameLine GetLine(int lineIndex)
		{
			int c = 0;
			int hunkIndex = 0;
			while(c + _hunks[hunkIndex].Count <= lineIndex)
			{
				c += _hunks[hunkIndex].Count;
				++hunkIndex;
			}
			return _hunks[hunkIndex][lineIndex - c];
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
