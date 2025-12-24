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

namespace gitter.Git;

using System;
using System.Text;

/// <summary>Represents a single diff line.</summary>
public sealed class DiffLine : ICloneable
{
	private int _charPositions;

	/// <summary>Create <see cref="DiffLine"/>.</summary>
	/// <param name="state">Line state.</param>
	/// <param name="states">Line states.</param>
	/// <param name="nums">Line numbers.</param>
	/// <param name="text">Line text.</param>
	/// <param name="ending">Line ending.</param>
	public DiffLine(DiffLineState state, DiffLineState[] states, int[] nums, string text, string ending = LineEnding.Lf)
	{
		Verify.Argument.IsNotNull(states);
		Verify.Argument.IsNotNull(nums);

		_charPositions = -1;
		Text	= text;
		State	= state;
		States	= states;
		Nums	= nums;
		Ending	= ending;
	}

	public DiffLineState State { get; }

	public DiffLineState[] States { get; }

	public string Text { get; }

	public string Ending { get; }

	public int[] Nums { get; }

	public int MaxLineNum
	{
		get
		{
			int max = 0;
			for(int i = 0; i < Nums.Length; ++i)
			{
				if(Nums[i] > max) max = Nums[i];
			}
			return max;
		}
	}

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
					default:
						++_charPositions;
						break;
				}
			}
		}
		return _charPositions;
	}

	internal void ToString(StringBuilder sb) => sb.Append(ToString());

	/// <inheritdoc/>
	public override string ToString()
		=> State switch
		{
			DiffLineState.Added   => "+" + Text,
			DiffLineState.Removed => "-" + Text,
			DiffLineState.Context => " " + Text,
			_ => Text,
		};

	public DiffLine Clone() => new(
		State,
		(DiffLineState[])States.Clone(),
		(int[])Nums.Clone(),
		Text,
		Ending);

	/// <inheritdoc/>
	object ICloneable.Clone() => Clone();
}
