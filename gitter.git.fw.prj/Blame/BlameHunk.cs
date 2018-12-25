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
	using System.Text;

	public sealed class BlameHunk : IEnumerable<BlameLine>
	{
		private readonly BlameCommit _commit;
		private readonly List<BlameLine> _lines;

		public BlameHunk(BlameCommit commit, IEnumerable<BlameLine> lines)
		{
			Verify.Argument.IsNotNull(commit, nameof(commit));
			Verify.Argument.IsNotNull(lines, nameof(lines));
			
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
