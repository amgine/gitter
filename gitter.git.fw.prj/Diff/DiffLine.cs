namespace gitter.Git
{
	using System;
	using System.Text;
	using System.Collections.Generic;

	/// <summary>Represents a single diff line.</summary>
	public sealed class DiffLine
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
			if(states == null) throw new ArgumentNullException("states");
			if(nums == null) throw new ArgumentNullException("nums");

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
	}
}
