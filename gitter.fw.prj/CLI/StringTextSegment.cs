﻿#region Copyright Notice
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
using System.Text;

public sealed class StringTextSegment : ITextSegment
{
	#region Data

	private string _buffer;
	private int _offset;
	private int _length;

	#endregion

	#region .ctor

	public StringTextSegment()
	{
	}

	#endregion

	#region Methods

	public void SetBuffer(string buffer, int offset, int length)
	{
		_buffer = buffer;
		_offset = offset;
		_length = length;
	}

	public override string ToString()
	{
		return _buffer.Substring(_offset, _length);
	}

	#endregion

	#region ITextSegment Members

	public char this[int index]
	{
		get
		{
			Verify.Argument.IsValidIndex(0, index, _length, nameof(index));

			return _buffer[_offset + index];
		}
	}

	public int Length => _length;

	public char PeekChar()
	{
		Verify.State.IsTrue(_length != 0, "Text segment is empty.");

		return _buffer[_offset];
	}

	public char ReadChar()
	{
		Verify.State.IsTrue(_length != 0, "Text segment is empty.");

		var value = _buffer[_offset];
		++_offset;
		--_length;
		return value;
	}

	public int IndexOf(char c)
	{
		int index = _buffer.IndexOf(c);
		if(index != -1)
		{
			index -= _offset;
		}
		return index;
	}

	public int IndexOfAny(char[] chars)
	{
		int index = _buffer.IndexOfAny(chars);
		if(index != -1)
		{
			index -= _offset;
		}
		return index;
	}

	public void Skip(int count)
	{
		Verify.Argument.IsNotNegative(count);
		Verify.State.IsTrue(_length != 0, "Text segment is empty.");

		_offset += count;
		_length -= count;
	}

	public void MoveTo(char[] buffer, int bufferOffset, int count)
	{
		_buffer.CopyTo(_offset, buffer, bufferOffset, count);
		_offset += count;
		_length -= count;
	}

	public void MoveTo(StringBuilder stringBuilder, int count)
	{
		stringBuilder.Append(_buffer, _offset, count);
		_offset += count;
		_length -= count;
	}

	public string ReadString(int length)
	{
		var value = _buffer.Substring(_offset, length);
		_offset += length;
		_length -= length;
		return value;
	}

	#endregion
}
