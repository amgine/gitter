namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public sealed class BlameLine
	{
		#region Data

		private readonly int _num;
		private readonly string _text;
		private readonly BlameCommit _commit;
		private readonly string _ending;
		private int _charPositions;

		#endregion

		public BlameLine(BlameCommit commit, int num, string text, string ending = LineEnding.Lf)
		{
			Verify.Argument.IsNotNull(commit, "commit");
			Verify.Argument.IsNotNull(text, "text");

			_commit = commit;
			_text = text;
			_ending = ending;
			_num = num;
			_charPositions = -1;
		}

		public int Number
		{
			get { return _num; }
		}

		public BlameCommit Commit
		{
			get { return _commit; }
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
						case '\r':
							break;
						default:
							++_charPositions;
							break;
					}
				}
			}
			return _charPositions;
		}

		public override string ToString()
		{
			return _text;
		}
	}
}
