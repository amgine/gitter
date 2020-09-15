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
		private int _charPositions;

		public BlameLine(BlameCommit commit, int number, string text, string ending = LineEnding.Lf)
		{
			Verify.Argument.IsNotNull(commit, nameof(commit));
			Verify.Argument.IsNotNull(text, nameof(text));

			Commit = commit;
			Text = text;
			Ending = ending;
			Number = number;
			_charPositions = -1;
		}

		public int Number { get; }

		public BlameCommit Commit { get; }

		public string Text { get; }

		public string Ending { get; }

		public int GetCharacterPositions(int tabSize)
		{
			if(_charPositions == -1)
			{
				for(int i = 0; i < Text.Length; ++i)
				{
					switch(Text[i])
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

		public override string ToString() => Text;
	}
}
