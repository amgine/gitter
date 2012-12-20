namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Text;

	sealed class UTF8DefaultAnsiCodepageFallback : DecoderFallback
	{
		private static readonly Decoder _decoder = Encoding.Default.GetDecoder();

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

			public override int Remaining
			{
				get { return _length - _pos; }
			}

			public override bool MovePrevious()
			{
				if(_pos != 0)
				{
					--_pos;
					return true;
				}
				else
				{
					return false;
				}
			}

			public override void Reset()
			{
				_chars = null;
				_length = 0;
				_pos = 0;
			}
		}

		public override int MaxCharCount
		{
			get { return 1; }
		}

		public override DecoderFallbackBuffer CreateFallbackBuffer()
		{
			return new FallBackBuffer();
		}
	}
}
