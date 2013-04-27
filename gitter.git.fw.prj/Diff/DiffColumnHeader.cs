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

	public struct DiffColumnHeader
	{
		private readonly DiffColumnAction _action;
		private readonly int _startLine;
		private readonly int _lineCount;

		public DiffColumnHeader(DiffColumnAction action, int startLine, int lineCount)
		{
			_action = action;
			_startLine = startLine;
			_lineCount = lineCount;
		}

		public DiffColumnAction Action
		{
			get { return _action; }
		}

		public int StartLine
		{
			get { return _startLine; }
		}

		public int LineCount
		{
			get { return _lineCount; }
		}

		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}{1},{2}", Action == DiffColumnAction.Add ? '+' : '-', _startLine, _lineCount);
		}
	}
}
