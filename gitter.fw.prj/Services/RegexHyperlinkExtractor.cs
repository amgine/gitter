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

	public sealed class RegexHyperlinkExtractor : IHyperlinkExtractor
	{
		private readonly Regex _regex;
		private readonly IReadOnlyList<IToken> _tokens;

		interface IToken
		{
			string FetchValue(Match match);
		}

		sealed class ConstantToken : IToken
		{
			public ConstantToken(string value) => Value = value;

			public string Value { get; }

			public string FetchValue(Match match) => Value;

			public override string ToString() => Value;
		}

		sealed class MatchGroupValueToken : IToken
		{
			public MatchGroupValueToken(int groupNumber) => GroupNumber = groupNumber;

			private int GroupNumber { get; }

			public string FetchValue(Match match)
			{
				var group = match.Groups[GroupNumber];
				return group != null && group.Success
					? group.Value
					: string.Empty;
			}

			public override string ToString() => $"<group: {GroupNumber}>";
		}

		private static IReadOnlyList<IToken> Tokenize(Regex regex, string pattern)
		{
			Assert.IsNotNull(regex);
			Assert.IsNotNull(pattern);

			var names   = regex.GetGroupNames();
			var numbers = regex.GetGroupNumbers();

			if(names == null || numbers == null || names.Length == 0 || numbers.Length == 0 || names.Length != numbers.Length || pattern.Length == 0)
			{
				return new[] { new ConstantToken(pattern) };
			}

			var tokens = new List<IToken>();
			int start  = 0;
			for(int i = 0; i < pattern.Length; ++i)
			{
				switch(pattern[i])
				{
					case '%':
						int end = i;
						++i;
						while(i < pattern.Length)
						{
							if(pattern[i] == '%')
							{
								var nameLength = i - end - 1;
								if(nameLength > 0)
								{
									for(int j = 0; j < names.Length; ++j)
									{
										if(names[j] != null && names[j].Length == nameLength
											&& string.Compare(pattern, end + 1, names[j], 0, nameLength, ignoreCase: false) == 0)
										{
											var len = end - start;
											if(len > 0)
											{
												tokens.Add(new ConstantToken(pattern.Substring(start, len)));
											}
											tokens.Add(new MatchGroupValueToken(numbers[j]));
											start = i + 1;
										}
									}
								}
								break;
							}
							++i;
						}
						break;
				}
			}
			if(start < pattern.Length)
			{
				var len = pattern.Length - start;
				if(len > 0)
				{
					tokens.Add(new ConstantToken(pattern.Substring(start, len)));
				}
			}
			return tokens;
		}

		public RegexHyperlinkExtractor(string regexp, string urlPattern)
		{
			Verify.Argument.IsNotNull(regexp, nameof(regexp));
			Verify.Argument.IsNotNull(urlPattern, nameof(urlPattern));

			_regex  = new Regex(regexp);
			_tokens = Tokenize(_regex, urlPattern);
		}

		public IReadOnlyList<Hyperlink> ExtractHyperlinks(string text)
		{
			if(text == null || text.Length == 0) return default;

			var hyperlinks = default(List<Hyperlink>);
			var values     = default(string[]);

			foreach(Match match in _regex.Matches(text))
			{
				var index  = match.Index;
				var length = match.Length;

				var linkText = new Substring(text, index, length);

				values ??= new string[_tokens.Count];
				for(int i = 0; i < values.Length; ++i)
				{
					values[i] = _tokens[i].FetchValue(match);
				}

				var linkUrl = string.Concat(values);

				hyperlinks ??= new();
				hyperlinks.Add(new Hyperlink(linkText, linkUrl));
			}
			return hyperlinks;
		}
	}
}
