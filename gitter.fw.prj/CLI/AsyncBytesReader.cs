#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

/// <summary>Reads bytes from stdio/stderr.</summary>
public class AsyncBytesReader : OutputReceiverBase, IOutputReceiver
{
	private static byte[] AllocateArray(int length)
	{
#if NET5_0_OR_GREATER
		return GC.AllocateUninitializedArray<byte>(length);
#else
		return new byte[length];
#endif
	}

	private readonly LinkedList<byte[]> _bufferChain;
	private readonly int _bufferSize;
	private Stream? _stream;
	private int _offset;
	private int _length;

	/// <summary>Initializes a new instance of the <see cref="AsyncBytesReader"/> class.</summary>
	/// <param name="bufferSize">Size of the internal buffer.</param>
	public AsyncBytesReader(int bufferSize = 0x400)
	{
		Verify.Argument.IsPositive(bufferSize);

		_bufferSize  = bufferSize;
		_bufferChain = new LinkedList<byte[]>();
	}

	/// <inheritdoc/>
	public override bool IsInitialized => _stream is not null;

	/// <inheritdoc/>
	public void Initialize(Process process, StreamReader reader)
	{
		Verify.Argument.IsNotNull(process);
		Verify.Argument.IsNotNull(reader);
		Verify.State.IsFalse(IsInitialized);

		_stream = reader.BaseStream;
		_offset = 0;
		_length = 0;

		_bufferChain.Clear();
		_bufferChain.AddLast(AllocateArray(_bufferSize));

		BeginReadAsync();
	}

	/// <inheritdoc/>
	public void WaitForEndOfStream()
	{
		Verify.State.IsTrue(IsInitialized);

		WaitForCompleted();

		_stream = null;
	}

	/// <summary>Returns the number of collected bytes.</summary>
	public int Length => _length;

	/// <summary>Returns collected bytes.</summary>
	/// <returns>Collected bytes.</returns>
	public byte[] GetBytes()
	{
		if(_length == 0) return Preallocated<byte>.EmptyArray;

		var length = _length;
		var res    = AllocateArray(length);
		int offset = 0;
		foreach(var buffer in _bufferChain)
		{
			var partLength = Math.Min(length, _bufferSize);
			Array.Copy(buffer, 0, res, offset, partLength);
			offset += partLength;
			length -= partLength;
		}
		return res;
	}

	/// <summary>Removes all collected byte buffers.</summary>
	public void Clear()
	{
		Verify.State.IsFalse(IsInitialized);

		_bufferChain.Clear();
		_length = 0;
	}

	private void OnStreamRead(IAsyncResult ar)
	{
		int bytesCount;
		try
		{
			bytesCount = _stream!.EndRead(ar);
		}
		catch(IOException)
		{
			bytesCount = 0;
		}
		catch(OperationCanceledException)
		{
			bytesCount = 0;
		}
		if(bytesCount != 0)
		{
			if(!IsCanceled)
			{
				_offset += bytesCount;
				_length += bytesCount;
				if(_offset >= _bufferSize)
				{
					_offset = 0;
					_bufferChain.AddLast(AllocateArray(_bufferSize));
				}
			}
			BeginReadAsync();
		}
		else
		{
			NotifyCompleted();
		}
	}

	private void BeginReadAsync()
	{
		bool isReading;
		var buffer = _bufferChain.Last!.Value;
		try
		{
			if(IsCanceled)
			{
				_stream!.BeginRead(buffer, 0, buffer.Length, OnStreamRead, this);
			}
			else
			{
				_stream!.BeginRead(buffer, _offset, buffer.Length - _offset, OnStreamRead, this);
			}
			isReading = true;
		}
		catch(IOException)
		{
			isReading = false;
		}
		catch(ObjectDisposedException)
		{
			isReading = false;
		}
		if(!isReading)
		{
			NotifyCompleted();
		}
	}
}
