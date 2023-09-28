namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Collections.Generic;

#if !NETCOREAPP
using gitter.Framework.CLI;
#endif

partial class LogParser
{
	sealed class MultiHashFieldParser : ITextFieldParser<List<Hash>>
	{
		private readonly char[] _buffer;
		private readonly List<Hash> _hashes;
		private int _offset;
		private bool _isCompleted;

		public MultiHashFieldParser()
		{
			_buffer = new char[Hash.HexStringLength];
			_hashes = new List<Hash>();
		}

		public void Reset()
		{
			_isCompleted = false;
			_offset = 0;
			_hashes.Clear();
		}

#if NETCOREAPP

		public bool Parse(ref ReadOnlySpan<char> text)
		{
			Verify.State.IsFalse(_isCompleted, "Field is already completed.");

			if(_offset == 0)
			{
				if(text.Length > 0 && text[0] == '\n')
				{
					text = text[1..];
					_isCompleted = true;
					return true;
				}
				if(text.Length == Hash.HexStringLength)
				{
					_hashes.Add(new Hash(text));
					text = default;
					return false;
				}
				else
				{
					while(text.Length > Hash.HexStringLength)
					{
						_hashes.Add(new Hash(text));
						var terminator = text[Hash.HexStringLength];
						text = text[(Hash.HexStringLength + 1)..];
						if(terminator == '\n')
						{
							_isCompleted = true;
							return true;
						}
						else
						{
							return false;
						}
					}
				}
			}
			if(_offset < Hash.HexStringLength && text.Length > 0)
			{
				int c = Math.Min(text.Length, Hash.HexStringLength - _offset);
				text[..c].CopyTo(new(_buffer, _offset, c));
				text = text[c..];
				_offset += c;
			}
			if(_offset == Hash.HexStringLength && text.Length > 0)
			{
				_offset = 0;
				_hashes.Add(new Hash(_buffer));
				var separator = text[0];
				text = text[1..];
				if(separator == '\n')
				{
					_isCompleted = true;
					return true;
				}
			}
			return false;
		}

#else

		public bool Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment);
			Verify.State.IsFalse(_isCompleted, "Field is already completed.");

			if(_offset == 0 && textSegment.Length > 0 && textSegment.PeekChar() == '\n')
			{
				textSegment.Skip(1);
				_isCompleted = true;
				return true;
			}
			if(_offset < Hash.HexStringLength && textSegment.Length > 0)
			{
				int c = Math.Min(textSegment.Length, Hash.HexStringLength - _offset);
				textSegment.MoveTo(_buffer, _offset, c);
				_offset += c;
			}
			if(_offset == Hash.HexStringLength && textSegment.Length > 0)
			{
				_offset = 0;
				_hashes.Add(new Hash(_buffer));
				var separator = textSegment.ReadChar();
				if(separator == '\n')
				{
					_isCompleted = true;
					return true;
				}
			}
			return false;
		}

#endif

		public List<Hash> GetValue() => _hashes;
	}
}
