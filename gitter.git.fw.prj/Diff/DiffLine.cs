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
	using System;
	using System.Text;
	using System.Collections.Generic;

	/// <summary>Represents a single diff line.</summary>
	public sealed class DiffLine : ICloneable
	{
		#region Data

		private readonly string _text;
		private readonly DiffLineState _state;
		private readonly DiffLineState[] _states;
		private readonly int[] _nums;
		private readonly string _ending;
		private int _charPositions;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="DiffLine"/>.</summary>
		/// <param name="state">Line state.</param>
		/// <param name="states">Line states.</param>
		/// <param name="nums">Line numbers.</param>
		/// <param name="text">Line text.</param>
		public DiffLine(DiffLineState state, DiffLineState[] states, int[] nums, string text, string ending = LineEnding.Lf)
		{
			Verify.Argument.IsNotNull(states, "states");
			Verify.Argument.IsNotNull(nums, "nums");

			_charPositions = -1;
			_text	= text;
			_state	= state;
			_states	= states;
			_nums	= nums;
			_ending	= ending;
		}

		#endregion

		public DiffLineState State
		{
			get { return _state; }
		}

		public DiffLineState[] States
		{
			get { return _states; }
		}

		public string Text
		{
			get { return _text; }
		}

		public string Ending
		{
			get { return _ending; }
		}

		public int GetCharacterPositions(int tabSize)
		{
			if(_charPositions == -1)
			{
				for(int i = 0; i < _text.Length; ++i)
				{
					switch(_text[i])
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

		public int[] Nums
		{
			get { return _nums; }
		}

		public int MaxLineNum
		{
			get
			{
				int max = 0;
				for(int i = 0; i < _nums.Length; ++i)
				{
					if(_nums[i] > max) max = _nums[i];
				}
				return max;
			}
		}

		internal void ToString(StringBuilder sb)
		{
			sb.Append(ToString());
		}

		public override string ToString()
		{
			switch(_state)
			{
				case DiffLineState.Added:
					return "+" + _text;
				case DiffLineState.Removed:
					return "-" + _text;
				case DiffLineState.Context:
					return " " + _text;
				default:
					return _text;
			}
		}

		#region ICloneable

		public DiffLine Clone()
		{
			return new DiffLine(
				_state,
				(DiffLineState[])_states.Clone(),
				(int[])_nums.Clone(),
				_text,
				_ending);
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion
	}
}
