#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.CLI;

using System;
using System.Diagnostics.CodeAnalysis;

public interface ITextParser
{
#if NETCOREAPP
	void Parse(ReadOnlySpan<char> text);
#else
	void Parse(ITextSegment text);
#endif

	void Complete();

	void Reset();
}

public interface ITextParser<out T> : ITextParser
{
	T GetResult();
}

#if UNUSED

abstract class LineByLineParser : ITextParser, IDisposable
{
	struct Buffer
	{
		private char[]? _buffer;
		private int _length;

		public readonly int Length => _length;

		public Buffer()
		{
			_length = -1;
		}

		[MemberNotNull(nameof(_buffer))]
		private void Allocate(int minSize)
		{
			if(_buffer is null)
			{
				_buffer = System.Buffers.ArrayPool<char>.Shared.Rent(minSize);
			}
			else if(_buffer.Length < minSize)
			{
				System.Buffers.ArrayPool<char>.Shared.Return(_buffer);
				_buffer = System.Buffers.ArrayPool<char>.Shared.Rent(minSize);
			}
		}

		[MemberNotNull(nameof(_buffer))]
		private void Reallocate(int minSize)
		{
			if(_buffer is null)
			{
				_buffer = System.Buffers.ArrayPool<char>.Shared.Rent(minSize);
			}
			else if(_buffer.Length < minSize)
			{
				var temp = System.Buffers.ArrayPool<char>.Shared.Rent(minSize);
				Array.Copy(_buffer, temp, _length);
				System.Buffers.ArrayPool<char>.Shared.Return(_buffer);
				_buffer = temp;
			}
		}

		public void Reset()
		{
			_length = -1;
		}

		public void Release()
		{
			if(_buffer is not null)
			{
				System.Buffers.ArrayPool<char>.Shared.Return(_buffer);
				_buffer = null;
				_length = -1;
			}
		}

		private static int GetBufferSize(int size)
		{
			const int MinBufferSize = 1024;

			return Math.Min(size, MinBufferSize);
		}

		public void AppendText(ReadOnlySpan<char> text)
		{
			if(_length < 0)
			{
				Allocate(GetBufferSize(text.Length));
				text.CopyTo(_buffer);
				_length = text.Length;
			}
			else
			{
				Reallocate(GetBufferSize(_length + text.Length));
				text.CopyTo(new Span<char>(_buffer, _length, _buffer.Length - _length));
				_length += text.Length;
			}
		}

		public readonly ReadOnlySpan<char> GetText()
			=> new(_buffer, 0, _length);
	}

	private Buffer _buffer;
	private bool _waitingForN;

	private void InvokeParseLine(string? ending)
	{
		if(_buffer.Length <= 0)
		{
			ParseLine(default, ending);
		}
		else
		{
			ParseLine(_buffer.GetText(), ending);
			_buffer.Reset();
		}
	}

	private void InvokeParseLine(in ReadOnlySpan<char> text, string? ending)
	{
		if(_buffer.Length <= 0)
		{
			ParseLine(text, ending);
		}
		else
		{
			_buffer.AppendText(text);
			ParseLine(_buffer.GetText(), ending);
			_buffer.Reset();
		}
	}

	public void Parse(ReadOnlySpan<char> text)
	{
		while(text.Length != 0)
		{
			var eol = text.IndexOfAny('\r', '\n');
			if(eol < 0)
			{
				if(_waitingForN)
				{
					InvokeParseLine(LineEnding.Cr);
					_waitingForN = false;
				}
				_buffer.AppendText(text);
				break;
			}
			if(text[eol] == '\n')
			{
				if(eol == 0)
				{
					if(_waitingForN)
					{
						InvokeParseLine(LineEnding.CrLf);
						_waitingForN = false;
					}
					else
					{
						InvokeParseLine(LineEnding.Lf);
					}
					text = text[1..];
				}
				else
				{
					if(_waitingForN)
					{
						InvokeParseLine(LineEnding.Cr);
						_waitingForN = false;
					}
					InvokeParseLine(text[..eol], LineEnding.Lf);
					text = text[(eol + 1)..];
				}
			}
			else
			{
				if(eol < text.Length - 1)
				{
					if(text[eol + 1] == '\n')
					{
						InvokeParseLine(text[..eol], LineEnding.CrLf);
						text = text[(eol + 2)..];
					}
					else
					{
						InvokeParseLine(text[..eol], LineEnding.Cr);
						text = text[(eol + 1)..];
					}
					_waitingForN = false;
				}
				else
				{
					if(_waitingForN)
					{
						InvokeParseLine(LineEnding.Cr);
					}
					_buffer.AppendText(text[..eol]);
					_waitingForN = true;
					break;
				}
			}
		}
	}

	protected virtual void CompleteCore()
	{
	}

	protected virtual void ResetCore()
	{
	}

	protected virtual void DisposeCore()
	{
	}

	protected abstract void ParseLine(ReadOnlySpan<char> text, string? ending);

	public void Reset()
	{
		ResetCore();
	}

	public void Complete()
	{
		if(_waitingForN)
		{
			InvokeParseLine(LineEnding.Cr);
			_waitingForN = false;
		}
		else if(_buffer.Length > 0)
		{
			InvokeParseLine(null);
		}
		CompleteCore();
	}

	public void Dispose()
	{
		_buffer.Release();
		DisposeCore();
	}
}

#endif
