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
using System.Runtime.CompilerServices;

#if !NETCOREAPP
using gitter.Framework.CLI;
#endif

partial class LogParser
{
	sealed class StrictISO8601TimestampField : RevisionFieldParser<DateTimeOffset>
	{
		const int FieldSizeZ = 20;
		const int FieldSize = 25;

		public StrictISO8601TimestampField() : base(FieldSize)
		{
			_state = FieldParserState.Initial;
		}

#if NETCOREAPP

		private bool InitialParseFull(ref ReadOnlySpan<char> text, int fieldSize)
		{
			_value = StrictISO8601TimestampParser.Parse(text);
			if(text.Length > fieldSize)
			{
				text = text[(fieldSize + 1)..];
				_state = FieldParserState.Completed;
				return true;
			}
			text = default;
			_state = FieldParserState.WaitingTerminator;
			return false;
		}

#else

		private bool InitialParseFull(ITextSegment text, int fieldSize)
		{
			text.MoveTo(_buffer, 0, fieldSize);
			_value = StrictISO8601TimestampParser.Parse(_buffer);
			if(text.Length > 0)
			{
				text.Skip();
				_state = FieldParserState.Completed;
				return true;
			}
			_state = FieldParserState.WaitingTerminator;
			return false;
		}

#endif

		private void BufferedParseFull()
		{
			_value = StrictISO8601TimestampParser.Parse(_buffer);
			_state = FieldParserState.Completed;
		}

		protected override bool ParseInitial(
#if NETCOREAPP
			ref ReadOnlySpan<char> text
#else
			ITextSegment text
#endif
			)
		{
			if(text.Length >= FieldSizeZ)
			{
				if(text[StrictISO8601TimestampParser.OffsetOf.UtcOffsetSign] == 'Z')
				{
#if NETCOREAPP
					return InitialParseFull(ref text, FieldSizeZ);
#else
					return InitialParseFull(text, FieldSizeZ);
#endif
				}
				if(text.Length >= FieldSize)
				{
#if NETCOREAPP
					return InitialParseFull(ref text, FieldSize);
#else
					return InitialParseFull(text, FieldSize);
#endif
				}
			}
#if NETCOREAPP
			InitialBuffer(ref text);
#else
			InitialBuffer(text);
#endif
			return false;
		}

		protected override bool ParseBuffering(
#if NETCOREAPP
			ref ReadOnlySpan<char> text
#else
			ITextSegment text
#endif
			)
		{
			if(_offset < FieldSizeZ)
			{
#if NETCOREAPP
				if(!FillBuffer(FieldSizeZ, ref text)) return false;
#else
				if(!FillBuffer(FieldSizeZ, text)) return false;
#endif
				if(_buffer[StrictISO8601TimestampParser.OffsetOf.UtcOffsetSign] == 'Z')
				{
					_value = StrictISO8601TimestampParser.Parse(_buffer);
#if NETCOREAPP
					if(ParseTerminator(ref text)) return true;
#else
					if(ParseTerminator(text)) return true;
#endif
					_state = FieldParserState.WaitingTerminator;
					return false;
				}
				if(text.Length <= 0) return false;
			}
#if NETCOREAPP
			if(!FillBufferExcludeLastChar(FieldSize, ref text)) return false;
#else
			if(!FillBufferExcludeLastChar(FieldSize, text)) return false;
#endif
			BufferedParseFull();
			return true;
		}
	}
}
