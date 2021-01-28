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
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	public sealed class RegexHyperlinkExtractor : IHyperlinkExtractor
	{
		private readonly Regex _regex;
		private readonly string _expression;

		public RegexHyperlinkExtractor(string regexp, string urlPattern)
		{
			_regex      = new Regex(regexp);
			_expression = urlPattern;
		}

		public IReadOnlyList<Hyperlink> ExtractHyperlinks(string text)
		{
			var hyperlinks = default(List<Hyperlink>);
			var names      = _regex.GetGroupNames();
			var numbers    = _regex.GetGroupNumbers();

			foreach(Match match in _regex.Matches(text))
			{
				var index  = match.Index;
				var length = match.Length;

				var linkText = new Substring(text, index, length);
				var linkUrl  = _expression;

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

				hyperlinks ??= new();
				hyperlinks.Add(new Hyperlink(linkText, linkUrl));
			}
			return hyperlinks;
		}
	}
}
