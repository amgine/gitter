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
using System.Buffers;
using System.Diagnostics;
using System.IO;

public abstract class AsyncOutputReceiverBase : OutputReceiverBase, IOutputReceiver
{
#if !NETCOREAPP
	private static readonly AsyncCallback _onStreamRead = OnStreamRead;
#endif

	private readonly int _bufferSize;
	private Stream? _stream;
	private ArraySegment<byte> _buffer;

	protected AsyncOutputReceiverBase(int bufferSize = 0x400)
	{
		Verify.Argument.IsPositive(bufferSize);

		_bufferSize = bufferSize;
	}

	protected int BufferSize => _bufferSize;

	/// <inheritdoc/>
	public sealed override bool IsInitialized => _stream is not null;

	protected virtual ArraySegment<byte> AllocateBuffer()
		=> new(ArrayPool<byte>.Shared.Rent(_bufferSize));

	protected virtual void FreeBuffer(ArraySegment<byte> buffer)
		=> ArrayPool<byte>.Shared.Return(buffer.Array!);

	protected virtual void InitializeCore(Process process, StreamReader reader, ArraySegment<byte> buffer)
	{
	}

	protected virtual void DeinitializeCore()
	{
	}

	/// <inheritdoc/>
	public void Initialize(Process process, StreamReader reader)
	{
		Verify.Argument.IsNotNull(process);
		Verify.Argument.IsNotNull(reader);
		Verify.State.IsFalse(IsInitialized);

		_stream = reader.BaseStream;
		_buffer = AllocateBuffer();

		InitializeCore(process, reader, _buffer);

#if NETCOREAPP
		ReadLoop();
#else
		BeginReadAsync();
#endif
	}

	/// <inheritdoc/>
	public void WaitForEndOfStream()
	{
		Verify.State.IsTrue(IsInitialized);

		WaitForCompleted();

		FreeBuffer(_buffer);

		DeinitializeCore();

		_stream = null;
		_buffer = default;
	}

	protected abstract void Process(ArraySegment<byte> buffer);

	protected virtual void Complete()
	{
	}

#if NETCOREAPP

	private async void ReadLoop()
	{
		var memory = new Memory<byte>(_buffer.Array, _buffer.Offset, _buffer.Count);
		while(true)
		{
			int bytesCount;
			try
			{
				bytesCount = await _stream!
					.ReadAsync(memory)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			catch(IOException)
			{
				NotifyCompleted();
				break;
			}
			catch(ObjectDisposedException)
			{
				NotifyCompleted();
				break;
			}
			if(bytesCount != 0)
			{
				if(!IsCanceled)
				{
					Process(new(_buffer.Array!, _buffer.Offset, bytesCount));
				}
			}
			else
			{
				try
				{
					if(!IsCanceled)
					{
						Complete();
					}
				}
				finally
				{
					NotifyCompleted();
				}
				break;
			}
		}
	}

#else

	private int EndRead(IAsyncResult ar)
	{
		try
		{
			return _stream!.EndRead(ar);
		}
		catch(IOException)
		{
			return 0;
		}
		catch(OperationCanceledException)
		{
			return 0;
		}
	}

	private static void OnStreamRead(IAsyncResult ar)
	{
		Assert.IsNotNull(ar);

		var receiver   = (AsyncOutputReceiverBase)ar.AsyncState;
		var bytesCount = receiver.EndRead(ar);
		if(bytesCount != 0)
		{
			if(!receiver.IsCanceled)
			{
				receiver.Process(new(receiver._buffer.Array, receiver._buffer.Offset, bytesCount));
			}
			receiver.BeginReadAsync();
		}
		else
		{
			try
			{
				if(!receiver.IsCanceled)
				{
					receiver.Complete();
				}
			}
			finally
			{
				receiver.NotifyCompleted();
			}
		}
	}

	private void BeginReadAsync()
	{
		bool isReading;
		try
		{
			_stream!.BeginRead(_buffer.Array, 0, _buffer.Count, _onStreamRead, this);
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

#endif
}
