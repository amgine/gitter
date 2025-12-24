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

namespace gitter.Framework.Services;

using System.Collections.Generic;

public sealed class AbsoluteUrlHyperlinkExtractor : IHyperlinkExtractor
{
	private static readonly string[] _protocols =
		[
			@"http://",
			@"https://",
			@"ftp://",
			@"mailto://",
		];

	private static bool ShouldTrim(char c)
		=> c is ',' or '.' or '(' or ')' or '[' or ']' or '{' or '}' or '<' or '>';

	private static void TrimEnd(string text, int start, ref int end)
	{
		while(end > start && ShouldTrim(text[end])) --end;
	}

	private static void AddHyperlink(ref List<Hyperlink>? hyperlinks, string text, int start, int end)
	{
		TrimEnd(text, start, ref end);
		var href = new Substring(text, start, end - start + 1);
		hyperlinks ??= [];
		hyperlinks.Add(new Hyperlink(href, href));
	}

	public IReadOnlyList<Hyperlink> ExtractHyperlinks(string text)
	{
		var hyperlinks = default(List<Hyperlink>);
		var parser = new Parser(text);
		int start = -1;

		while(!parser.IsAtEndOfString)
		{
			if(start == -1)
			{
				bool isLinkStart = false;
				foreach(var protocol in _protocols)
				{
					if(!parser.CheckValue(protocol)) continue;

					isLinkStart = true;
					start = parser.Position;
					parser.Skip(protocol.Length);
					break;
				}
				if(!isLinkStart) parser.Skip();
			}
			else
			{
				if(start != -1 && char.IsWhiteSpace(parser.CurrentChar))
				{
					var end = parser.Position - 1;
					AddHyperlink(ref hyperlinks, text, start, end);
					start = -1;
				}
				parser.Skip();
			}
		}
		if(start != -1)
		{
			var end = text.Length - 1;
			AddHyperlink(ref hyperlinks, text, start, end);
		}
		return hyperlinks is not null
			? hyperlinks
			: Preallocated<Hyperlink>.EmptyArray;
	}
}
