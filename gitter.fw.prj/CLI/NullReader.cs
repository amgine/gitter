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
using System.Diagnostics;
using System.IO;
using System.Threading;

/// <summary>Used to discard any read data.</summary>
public sealed class NullReader : IOutputReceiver
{
#if NET5_0_OR_GREATER
	private static readonly byte[] _buffer = GC.AllocateUninitializedArray<byte>(0x400, pinned: true);
#else
	private static readonly byte[] _buffer = new byte[0x400];
#endif
	private object _syncRoot = new();
	private bool _canceled;
	private bool _completed;

	private Stream Stream { get; set; }

	/// <inheritdoc/>
	public bool IsInitialized => Stream is not null;

	/// <inheritdoc/>
	public void Initialize(Process process, StreamReader reader)
	{
		Verify.Argument.IsNotNull(process);
		Verify.Argument.IsNotNull(reader);
		Verify.State.IsFalse(IsInitialized);

		Stream = reader.BaseStream;

#if NETCOREAPP
		ReadLoop();
#else
		BeginReadAsync();
#endif
	}

	/// <inheritdoc/>
	public void NotifyCanceled()
	{
		Verify.State.IsTrue(IsInitialized);

		if(!_canceled && !_completed)
		{
			Monitor.Enter(_syncRoot);
			try
			{
				if(!_canceled && !_completed)
				{
					_canceled = true;
					Monitor.PulseAll(_syncRoot);
				}
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
		}
	}

	/// <inheritdoc/>
	public void WaitForEndOfStream()
	{
		Verify.State.IsTrue(IsInitialized);

		if(!_completed)
		{
			Monitor.Enter(_syncRoot);
			try
			{
				while(!_completed)
				{
					Monitor.Wait(_syncRoot);
				}
			}
			finally
			{
				Monitor.Exit(_syncRoot);
			}
		}
		Stream = null;
	}

	private void NotifyCompleted()
	{
		Monitor.Enter(_syncRoot);
		try
		{
			if(!_completed)
			{
				_completed = true;
				Monitor.PulseAll(_syncRoot);
			}
		}
		finally
		{
			Monitor.Exit(_syncRoot);
		}
	}


#if NETCOREAPP

	private async void ReadLoop()
	{
		while(true)
		{
			int bytesCount;
			try
			{
				bytesCount = await Stream
					.ReadAsync(new Memory<byte>(_buffer))
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			catch(IOException)
			{
				bytesCount = 0;
			}
			catch(ObjectDisposedException)
			{
				bytesCount = 0;
			}
			catch(OperationCanceledException)
			{
				bytesCount = 0;
			}
			if(bytesCount == 0)
			{
				NotifyCompleted();
				break;
			}
		}
	}

#else

	private static void OnStreamRead(IAsyncResult ar)
	{
		var reader = (NullReader)ar.AsyncState;

		int bytesCount;
		try
		{
			bytesCount = reader.Stream.EndRead(ar);
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
			reader.BeginReadAsync();
		}
		else
		{
			reader.NotifyCompleted();
		}
	}

	private void BeginReadAsync()
	{
		bool isReading;
		try
		{
			Stream.BeginRead(_buffer, 0, _buffer.Length, OnStreamRead, this);
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
