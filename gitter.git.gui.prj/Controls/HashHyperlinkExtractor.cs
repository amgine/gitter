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

namespace gitter.Git.Gui;

using System.Collections.Generic;

using gitter.Framework;
using gitter.Framework.Services;

sealed class HashHyperlinkExtractor : IHyperlinkExtractor
{
	const int MinLength = Hash.HexStringLength;

	static bool IsHashChar(char value)
		=> (value is >= '0' and <= '9') || (value is >= 'a' and <= 'f');

	ref struct State
	{
		private readonly string Text;

		public State(string text)
		{
			Text       = text;
			Hyperlinks = null;
			Start      = -1;
			NonHashSeq = false;
		}

		public List<Hyperlink> Hyperlinks;

		public int Start;

		public bool NonHashSeq;

		public void Reset()
		{
			Start      = -1;
			NonHashSeq = false;
		}

		public void Commit(int position)
		{
			if(Start >= 0 && !NonHashSeq)
			{
				var len = position - Start;
				if(len >= MinLength && len <= Hash.HexStringLength)
				{
					var hash = Text.Substring(Start, len);
					Hyperlinks ??= new();
					Hyperlinks.Add(new Hyperlink(new Substring(Text, Start, len), "gitter://history/" + hash));
				}
			}
			Reset();
		}

		public void Append(char c, int position)
		{
			if(NonHashSeq) return;
			if(Start == -1)
			{
				if(IsHashChar(c))
				{
					Start = position;
				}
				else
				{
					NonHashSeq = true;
				}
			}
			else
			{
				if(!IsHashChar(c))
				{
					Start = -1;
					NonHashSeq = true;
				}
			}
		}
	}

	public IReadOnlyList<Hyperlink> ExtractHyperlinks(string text)
	{
		var state = new State(text);
		for(int i = 0; i < text.Length; ++i)
		{
			var c = text[i];
			if(char.IsWhiteSpace(c) || c is ')' or ',' or '.' or ':' or ';')
			{
				state.Commit(i);
			}
			else if(c == '(')
			{
				state.Reset();
			}
			else
			{
				state.Append(c, i);
			}
		}
		state.Commit(text.Length);
		return state.Hyperlinks is not null
			? state.Hyperlinks
			: Preallocated<Hyperlink>.EmptyArray;
	}
}
