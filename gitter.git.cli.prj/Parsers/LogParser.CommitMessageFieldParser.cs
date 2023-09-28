namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Text;

#if !NETCOREAPP
using gitter.Framework.CLI;
#endif

partial class LogParser
{
	sealed class CommitMessageFieldParser : ITextFieldParser<CommitMessage>
	{
		sealed class EmptyLineSeparator
		{
			private readonly char[] _buffer;
			private int _length;
			private int _lineEndings;

			public EmptyLineSeparator()
			{
				_buffer = new char[4];
			}

			public int Length => _length;

			public bool Append(char c)
			{
				if(c == '\n')
				{
					if(_length == 0)
					{
						++_lineEndings;
					}
					else
					{
						if(_buffer[_length - 1] != '\r')
						{
							++_lineEndings;
						}
					}
				}
				_buffer[_length++] = c;
				return _lineEndings == 2;
			}

			public void Dump(StringBuilder stringBuilder)
			{
				Assert.IsNotNull(stringBuilder);

				stringBuilder.Append(_buffer, 0, _length);
				Reset();
			}

			public void Reset()
			{
				_length = 0;
				_lineEndings = 0;
			}
		}

		static readonly char[] Separators = new[] { '\0', '\r', '\n' };

		private readonly StringBuilder _subject;
		private readonly StringBuilder _body;
		private readonly EmptyLineSeparator _separator;
		private bool _isSubjectCompleted;
		private bool _isCompleted;

		public CommitMessageFieldParser()
		{
			_subject   = new StringBuilder();
			_body      = new StringBuilder();
			_separator = new EmptyLineSeparator();
		}

		public CommitMessage GetValue() => new(_subject.ToString(), _body.ToString());

		private static void RemoveTrailingWhitespace(StringBuilder stringBuilder)
		{
			Assert.IsNotNull(stringBuilder);

			int offset = stringBuilder.Length - 1;
			while(offset >= 0 && char.IsWhiteSpace(stringBuilder[offset]))
			{
				--offset;
			}
			++offset;
			if(offset < stringBuilder.Length)
			{
				stringBuilder.Remove(offset, stringBuilder.Length - offset);
			}
		}

#if NETCOREAPP

		public bool Parse(ref ReadOnlySpan<char> text)
		{
			Verify.State.IsFalse(_isCompleted, "Field is already completed.");

			while(text.Length > 0)
			{
				int separatorIndex = _isSubjectCompleted ?
					text.IndexOf('\0') :
					text.IndexOfAny(Separators);
				if(separatorIndex == -1)
				{
					if(_isSubjectCompleted)
					{
						_body.Append(text);
					}
					else
					{
						if(_separator.Length != 0)
						{
							_separator.Dump(_subject);
						}
						_subject.Append(text);
					}
					text = ReadOnlySpan<char>.Empty;
					return false;
				}
				else
				{
					if(_isSubjectCompleted)
					{
						_body.Append(text[..separatorIndex]);
					}
					else if(separatorIndex != 0)
					{
						if(_separator.Length != 0)
						{
							_separator.Dump(_subject);
						}
						_subject.Append(text[..separatorIndex]);
					}
					var separatorChar = text[separatorIndex];
					text = text[(separatorIndex + 1)..];
					switch(separatorChar)
					{
						case '\0':
							RemoveTrailingWhitespace(_subject);
							if(_isSubjectCompleted)
							{
								RemoveTrailingWhitespace(_body);
							}
							_isCompleted = true;
							return true;
						case '\r':
						case '\n':
							if(_isSubjectCompleted)
							{
								_body.Append(separatorChar);
							}
							else
							{
								if(_separator.Append(separatorChar))
								{
									_isSubjectCompleted = true;
									_separator.Reset();
								}
							}
							break;
					}
				}
			}
			return false;
		}

#else

		public bool Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment);
			Verify.State.IsFalse(_isCompleted, "Field is already completed.");

			while(textSegment.Length > 0)
			{
				int separatorIndex = _isSubjectCompleted ?
					textSegment.IndexOf('\0') :
					textSegment.IndexOfAny(Separators);
				if(separatorIndex == -1)
				{
					if(_isSubjectCompleted)
					{
						textSegment.MoveTo(_body, textSegment.Length);
					}
					else
					{
						if(_separator.Length != 0)
						{
							_separator.Dump(_subject);
						}
						textSegment.MoveTo(_subject, textSegment.Length);
					}
					return false;
				}
				else
				{
					if(_isSubjectCompleted)
					{
						textSegment.MoveTo(_body, separatorIndex);
					}
					else
					{
						if(separatorIndex != 0)
						{
							if(_separator.Length != 0)
							{
								_separator.Dump(_subject);
							}
							textSegment.MoveTo(_subject, separatorIndex);
						}
					}
					var separatorChar = textSegment.ReadChar();
					switch(separatorChar)
					{
						case '\0':
							RemoveTrailingWhitespace(_subject);
							if(_isSubjectCompleted)
							{
								RemoveTrailingWhitespace(_body);
							}
							_isCompleted = true;
							return true;
						case '\r':
						case '\n':
							if(_isSubjectCompleted)
							{
								_body.Append(separatorChar);
							}
							else
							{
								if(_separator.Append(separatorChar))
								{
									_isSubjectCompleted = true;
									_separator.Reset();
								}
							}
							break;
					}
				}
			}
			return false;
		}

#endif

		public void Reset()
		{
			_subject.Clear();
			_body.Clear();
			_separator.Reset();
			_isSubjectCompleted = false;
			_isCompleted = false;
		}
	}
}
