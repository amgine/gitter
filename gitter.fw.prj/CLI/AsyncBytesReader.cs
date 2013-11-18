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

namespace gitter.Framework.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Diagnostics;
	using System.IO;

	/// <summary>Reads bytes from stdio/stderr.</summary>
	public class AsyncBytesReader : IOutputReceiver
	{
		#region Data

		private readonly int _bufferSize;
		private Stream _stream;
		private LinkedList<byte[]> _bufferChain;
		private ManualResetEvent _eof;
		private int _offset;
		private int _length;
		private volatile bool _isCanceled;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="AsyncBytesReader"/> class.</summary>
		/// <param name="bufferSize">Size of the internal buffer.</param>
		public AsyncBytesReader(int bufferSize)
		{
			Verify.Argument.IsPositive(bufferSize, "bufferSize");

			_bufferSize		= bufferSize;
			_bufferChain	= new LinkedList<byte[]>();
		}

		/// <summary>Initializes a new instance of the <see cref="AsyncBytesReader"/> class.</summary>
		public AsyncBytesReader()
			: this(0x400)
		{
		}

		#endregion

		#region IOutputReceiver

		/// <summary>Gets a value indicating whether this instance is initialized.</summary>
		/// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
		public bool IsInitialized
		{
			get { return _stream != null; }
		}

		/// <summary>Initializes output reader.</summary>
		/// <param name="process">Process to read from.</param>
		/// <param name="sr">StreamReader to read from.</param>
		public void Initialize(Process process, StreamReader reader)
		{
			Verify.Argument.IsNotNull(process, "process");
			Verify.Argument.IsNotNull(reader, "reader");
			Verify.State.IsFalse(IsInitialized);

			_stream = reader.BaseStream;
			_eof    = new ManualResetEvent(false);
			_offset = 0;
			_length = 0;

			_bufferChain.Clear();
			_bufferChain.AddLast(new byte[_bufferSize]);

			BeginReadAsync();
		}

		/// <summary>Notifies receiver that output is no longer required.</summary>
		/// <remarks>Reader should still receive bytes, but disable any stream processing.</remarks>
		public void NotifyCanceled()
		{
			Verify.State.IsTrue(IsInitialized);

			_isCanceled = true;
			var eof = _eof;
			if(eof != null)
			{
				_eof = null;
				eof.Dispose();
			}
		}

		/// <summary>Closes the reader.</summary>
		public void WaitForEndOfStream()
		{
			Verify.State.IsTrue(IsInitialized);

			_eof.WaitOne();
			_eof.Dispose();

			_stream = null;
			_eof = null;
		}

		#endregion

		#region Public

		/// <summary>Returns the number of collected bytes.</summary>
		public int Length
		{
			get { return _length; }
		}

		/// <summary>Returns collected bytes.</summary>
		/// <returns>Collected bytes.</returns>
		public byte[] GetBytes()
		{
			var res = new byte[_length];
			int offset = 0;
			foreach(var buffer in _bufferChain)
			{
				var partLength = _length;
				if(partLength > _bufferSize)
				{
					partLength = _bufferSize;
				}
				Array.Copy(buffer, 0, res, offset, partLength);
				offset += partLength;
				_length -= partLength;
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

		#endregion

		#region Private

		private void OnStreamRead(IAsyncResult ar)
		{
			int bytesCount;
			try
			{
				bytesCount = _stream.EndRead(ar);
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
				if(!_isCanceled)
				{
					_offset += bytesCount;
					_length += bytesCount;
					if(_offset >= _bufferSize)
					{
						_offset = 0;
						_bufferChain.AddLast(new byte[_bufferSize]);
					}
				}
				BeginReadAsync();
			}
			else
			{
				if(_isCanceled)
				{
					return;
				}
				var eof = (EventWaitHandle)ar.AsyncState;
				if(eof != null)
				{
					try
					{
						eof.Set();
					}
					catch(ObjectDisposedException)
					{
					}
				}
			}
		}

		private void BeginReadAsync()
		{
			if(_isCanceled)
			{
				while(_bufferChain.Count > 1)
				{
					_bufferChain.RemoveLast();
				}
				if(_bufferChain.Count == 0)
				{
					_bufferChain.AddLast(new byte[_bufferSize]);
				}
				var buffer = _bufferChain.Last.Value;
				try
				{
					_stream.BeginRead(buffer, 0, buffer.Length, OnStreamRead, null);
				}
				catch(ObjectDisposedException)
				{
				}
			}
			else
			{
				bool isReading;
				var eof = _eof;
				var buffer = _bufferChain.Last.Value;
				try
				{
					_stream.BeginRead(buffer, _offset, buffer.Length - _offset, OnStreamRead, eof);
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
					if(eof != null)
					{
						try
						{
							eof.Set();
						}
						catch(ObjectDisposedException)
						{
						}
					}
				}

			}
		}

		#endregion
	}
}
