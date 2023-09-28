namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Text;

#if !NETCOREAPP
using gitter.Framework.CLI;
#endif

partial class LogParser
{
	sealed class StringLineFieldParser : ITextFieldParser<string>
	{
		private const char Terminator = '\n';
		private readonly StringBuilder _line;
		private FieldParserState _state;
		private string _value;

		public StringLineFieldParser()
		{
			_line = new StringBuilder();
			_state = FieldParserState.Initial;
			_value = string.Empty;
		}

		public string GetValue() => _value;

		public bool Parse(
#if NETCOREAPP
			ref ReadOnlySpan<char> text
#else
			ITextSegment text
#endif
			)
		{
			switch(_state)
			{
				case FieldParserState.Initial:
					{
						if(text.Length == 0) return false;
						int eol = text.IndexOf(Terminator);
						if(eol == -1)
						{
#if NETCOREAPP
							_line.Append(text);
							text = default;
#else
							text.MoveTo(_line, text.Length);
#endif
							_state = FieldParserState.Buffering;
							return false;
						}
						else
						{
#if NETCOREAPP
							_value = eol != 0
								? new string(text[..eol])
								: string.Empty;
							text = text[(eol + 1)..];
#else
							_value = text.ReadString(eol);
							text.Skip();
#endif
							_state = FieldParserState.Completed;
							return true;
						}
					}
				case FieldParserState.Buffering:
					{
						if(text.Length == 0) return false;
						int eol = text.IndexOf(Terminator);
						if(eol == -1)
						{
#if NETCOREAPP
							_line.Append(text);
							text = default;
#else
							text.MoveTo(_line, text.Length);
#endif
							return false;
						}
						else
						{
							if(eol != 0)
							{
#if NETCOREAPP
								_line.Append(text[..eol]);
#else
								text.MoveTo(_line, eol);
#endif
							}
#if NETCOREAPP
							text = text[(eol + 1)..];
#else
							text.Skip();
#endif
							_value = _line.ToString();
							_line.Clear();
							return true;
						}
					}
				case FieldParserState.Completed:
					throw new InvalidOperationException("Field is already completed.");
				default:
					throw new ApplicationException($"Invalid state: {_state}");
			}
		}

		public void Reset()
		{
			_line.Clear();
			_value = string.Empty;
			_state = FieldParserState.Initial;
		}
	}
}
