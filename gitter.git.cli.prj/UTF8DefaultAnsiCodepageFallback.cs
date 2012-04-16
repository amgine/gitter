namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Text;

	sealed class UTF8DefaultAnsiCodepageFallback : DecoderFallback
	{
		private static readonly Decoder _decoder = Encoding.Default.GetDecoder();

		private sealed class FallBackBuffer : DecoderFallbackBuffer
		{
			private byte[] _bytes;
			private int _pos;

			public override bool Fallback(byte[] bytesUnknown, int index)
			{
				_bytes = bytesUnknown;
				_pos = 0;
				return _pos < _bytes.Length;
			}

			public override char GetNextChar()
			{
				if(_pos >= _bytes.Length)
				{
					return '\0';
				}
				var ansi = new char[1];
				_decoder.GetChars(_bytes, _pos, 1, ansi, 0);
				++_pos;
				return ansi[0];
			}

			public override int Remaining
			{
				get { return _bytes != null ? _bytes.Length - _pos : 0; }
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
				_bytes = null;
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
