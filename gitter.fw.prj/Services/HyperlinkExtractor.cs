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

	/// <summary>Extracts hyperlinks from plain text.</summary>
	public sealed class HyperlinkExtractor : IHyperlinkExtractor
	{
		private readonly IEnumerable<IHyperlinkExtractor> _extractors;

		public HyperlinkExtractor(IEnumerable<IHyperlinkExtractor> extractors)
		{
			_extractors = extractors;
		}

		public IReadOnlyList<Hyperlink> ExtractHyperlinks(string text)
		{
			var hyperlinks = default(List<Hyperlink>);

			if(_extractors != null)
			{
				foreach(var extractor in _extractors)
				{
					if(extractor == null) continue;
					var variants = extractor.ExtractHyperlinks(text);
					if(variants == null || variants.Count == 0) continue;

					if(hyperlinks == null)
					{
						hyperlinks = new(variants);
					}
					else
					{
						bool added = false;
						foreach(var b in variants)
						{
							for(int i = 0; i < hyperlinks.Count; ++i)
							{
								var a = hyperlinks[i];
								if(a.Text.Start >= b.Text.Start && a.Text.End <= b.Text.End)
								{
									hyperlinks[i] = b;
									added = true;
									break;
								}
								if(b.Text.Start >= a.Text.Start && b.Text.End <= a.Text.End)
								{
									added = true;
									break;
								}
								if(b.Text.Start >= a.Text.Start && b.Text.Start <= a.Text.End)
								{
									added = true;
									break;
								}
								if(b.Text.End >= a.Text.Start && b.Text.End <= a.Text.End)
								{
									added = true;
									break;
								}
							}
							if(!added)
							{
								hyperlinks.Add(b);
							}
						}
					}
				}
			}

			return hyperlinks != null
				? hyperlinks
				: Preallocated<Hyperlink>.EmptyArray;
		}
	}
}
