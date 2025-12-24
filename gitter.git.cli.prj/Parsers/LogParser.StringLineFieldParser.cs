#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Text;

#if !NETCOREAPP
using gitter.Framework.CLI;
#endif

partial class LogParser
{
	sealed class StringLineFieldParser : IRevisionFieldParser<string>
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
