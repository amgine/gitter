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

namespace NHunspell;

public class HyphenResult
{
	private readonly int[] cut;
	private readonly byte[] points;
	private readonly int[] pos;
	private readonly string[] rep;
	private readonly string word;

	public HyphenResult(
	  string hyphenatedWord,
	  byte[] hyphenationPoints,
	  string[] hyphenationRep,
	  int[] hyphenationPos,
	  int[] hyphenationCut)
	{
		this.word = hyphenatedWord;
		this.points = hyphenationPoints;
		this.rep = hyphenationRep;
		this.pos = hyphenationPos;
		this.cut = hyphenationCut;
	}

	public string HyphenatedWord => this.word;

	public int[] HyphenationCuts => this.cut;

	public byte[] HyphenationPoints => this.points;

	public int[] HyphenationPositions => this.pos;

	public string[] HyphenationReplacements => this.rep;
}
