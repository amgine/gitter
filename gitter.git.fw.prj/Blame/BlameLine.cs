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
	public sealed class BlameLine
	{
		#region Data

		private readonly int _number;
		private readonly string _text;
		private readonly BlameCommit _commit;
		private readonly string _ending;
		private int _charPositions;

		#endregion

		public BlameLine(BlameCommit commit, int number, string text, string ending = LineEnding.Lf)
		{
			Verify.Argument.IsNotNull(commit, "commit");
			Verify.Argument.IsNotNull(text, "text");

			_commit = commit;
			_text = text;
			_ending = ending;
			_number = number;
			_charPositions = -1;
		}

		public int Number
		{
			get { return _number; }
		}

		public BlameCommit Commit
		{
			get { return _commit; }
		}

		public string Text
		{
			get { return _text; }
		}

		public string Ending
		{
			get { return _ending; }
		}

		public int GetCharacterPositions(int tabSize)
		{
			if(_charPositions == -1)
			{
				for(int i = 0; i < _text.Length; ++i)
				{
					switch(_text[i])
					{
						case '\t':
							_charPositions += tabSize - (_charPositions % tabSize);
							break;
						case '\r':
							break;
						default:
							++_charPositions;
							break;
					}
				}
			}
			return _charPositions;
		}

		public override string ToString()
		{
			return _text;
		}
	}
}
