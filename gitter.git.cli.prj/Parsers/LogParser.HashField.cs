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

#if !NETCOREAPP
using gitter.Framework.CLI;
#endif

partial class LogParser
{
	sealed class HashField : RevisionFieldParser<Sha1Hash>
	{
		public HashField() : base(Sha1Hash.HexStringLength) { }

		protected override bool ParseInitial(
#if NETCOREAPP
			ref ReadOnlySpan<char> text
#else
			ITextSegment text
#endif
			)
		{
			if(text.Length >= Sha1Hash.HexStringLength)
			{
#if NETCOREAPP
				_value = Sha1Hash.Parse(text);
#else
				text.MoveTo(_buffer, 0, Sha1Hash.HexStringLength);
				_value = Sha1Hash.Parse(_buffer);
#endif
				if(text.Length > Sha1Hash.HexStringLength)
				{
#if NETCOREAPP
					text = text[(Sha1Hash.HexStringLength + 1)..];
#else
					text.Skip();
#endif
					_state = FieldParserState.Completed;
					return true;
				}
				else
				{
#if NETCOREAPP
					text = default;
#endif
					_state = FieldParserState.WaitingTerminator;
					return false;
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
#if NETCOREAPP
			if(!FillBufferExcludeLastChar(Sha1Hash.HexStringLength, ref text)) return false;
#else
			if(!FillBufferExcludeLastChar(Sha1Hash.HexStringLength, text)) return false;
#endif
			_value = Sha1Hash.Parse(_buffer);
			_state = FieldParserState.Completed;
			return true;
		}
	}
}
