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

namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	/// <summary>Extracts hyperlinks from plain text.</summary>
	public sealed class HyperlinkExtractor
	{
		private readonly Regex _regex;
		private readonly string _bugtrackerUrl;

		private static readonly string[] Protocols = new string[]
			{
				@"http://",
				@"https://",
				@"ftp://",
				@"mailto://",
			};

		public HyperlinkExtractor()
		{
		}

		public HyperlinkExtractor(string issueIdRegexp, string bugtrackerUrlPattern)
		{
			_regex = new Regex(issueIdRegexp);
			_bugtrackerUrl = bugtrackerUrlPattern;
		}

		public IList<Hyperlink> ExtractHyperlinks(string text)
		{
			var hyperlinks = new List<Hyperlink>();
			var parser = new Parser(text);
			int start = -1;

			// serach inline links
			while(!parser.IsAtEndOfString)
			{
				if(start == -1)
				{
					bool isLinkStart = false;
					for(int i = 0; i < Protocols.Length; ++i)
					{
						if(parser.CheckValue(Protocols[i]))
						{
							isLinkStart = true;
							start = parser.Position;
							parser.Skip(Protocols[i].Length);
							break;
						}
					}
					if(!isLinkStart)
					{
						parser.Skip();
					}
				}
				else
				{
					if(start != -1)
					{
						if(char.IsWhiteSpace(parser.CurrentChar))
						{
							var href = new Substring(text, start, parser.Position - start);
							hyperlinks.Add(new Hyperlink(href, href));
							start = -1;
						}
					}
					parser.Skip();
				}
			}
			if(start != -1)
			{
				var href = new Substring(text, start);
				hyperlinks.Add(new Hyperlink(href, href));
			}
			// search regexp matches which do not intersect already found links
			if(_regex != null)
			{
				int inline_links = hyperlinks.Count;

				var names = _regex.GetGroupNames();
				var numbers = _regex.GetGroupNumbers();

				foreach(Match match in _regex.Matches(text))
				{
					var index = match.Index;
					var length = match.Length;
					bool intersects = false;

					for(int i = 0; i < inline_links; ++i)
					{
						var link = hyperlinks[i].Text;
						if(link.Start == index)
						{
							intersects = true;
							break;
						}
						if(link.Start > index && link.Start - index < length)
						{
							intersects = true;
							break;
						}
						if(link.Start < index && index - link.Start < link.Length)
						{
							intersects = true;
							break;
						}
					}

					if(intersects)
					{
						continue;
					}

					var linkText = new Substring(text, index, length);
					var linkUrl = _bugtrackerUrl;

					if(numbers != null)
					{
						// replace url template variables with corresponding regexp group values
						for(int i = 0; i < numbers.Length; ++i)
						{
							var group = match.Groups[numbers[i]];
							if(group != null && group.Success)
							{
								var name = names[i].SurroundWith('%');
								var value = group.Value;

								linkUrl = linkUrl.Replace(name, value);
							}
						}
					}

					hyperlinks.Add(new Hyperlink(linkText, linkUrl));
				}
			}
			return hyperlinks;
		}
	}
}
