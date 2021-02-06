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

namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Text;

	sealed class UTF8DefaultAnsiCodepageFallback : DecoderFallback
	{
		private sealed class FallBackBuffer : DecoderFallbackBuffer
		{
			private char[] _chars;
			private int _length;
			private int _pos;

			public override bool Fallback(byte[] bytesUnknown, int index)
			{
				if(bytesUnknown == null || bytesUnknown.Length == 0) return false;
				var encoding = Encoding.Default;
				var decoder = encoding.GetDecoder();
				_chars	= new char[encoding.GetMaxCharCount(bytesUnknown.Length)];
				_length	= decoder.GetChars(bytesUnknown, 0, bytesUnknown.Length, _chars, 0, true);
				_pos	= 0;
				return _length != 0;
			}

			public override char GetNextChar()
			{
				if(_pos >= _length)
				{
					return '\0';
				}
				char replacement = _chars[_pos++];
				if(replacement == '\0')
				{
					return (char)0xFFFD;
				}
				else
				{
					return replacement;
				}
			}

			public override int Remaining => _length - _pos;

			public override bool MovePrevious()
			{
				if(_pos != 0)
				{
					--_pos;
					return true;
				}
				return false;
			}

			public override void Reset()
			{
				_chars = null;
				_length = 0;
				_pos = 0;
			}
		}

		public override int MaxCharCount => 1;

		public override DecoderFallbackBuffer CreateFallbackBuffer()
			=> new FallBackBuffer();
	}
}
