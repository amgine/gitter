#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using System.Collections.Generic;

namespace NHunspell;

public class ThesResult(List<ThesMeaning> meanings, bool generatedByStem)
{
	public bool IsGenerated { get; } = generatedByStem;

	public List<ThesMeaning> Meanings { get; } = meanings;

	public Dictionary<string, List<ThesMeaning>> GetSynonyms()
	{
		var synonyms = new Dictionary<string, List<ThesMeaning>>();
		foreach(var meaning in Meanings)
		{
			foreach(string synonym in meaning.Synonyms)
			{
				if(!synonyms.TryGetValue(synonym, out var thesMeaningList))
				{
					synonyms.Add(synonym, thesMeaningList = new(capacity: 1));
				}
				thesMeaningList.Add(meaning);
			}
		}
		return synonyms;
	}
}
