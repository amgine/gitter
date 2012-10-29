namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Diagnostics;
	using System.IO;

	/// <summary>Reads bytes from stdio/stderr.</summary>
	internal sealed class AsyncBytesReader : IOutputReceiver
	{
		#region Data

		private readonly int _bufferSize;
		private Stream _stream;
		private LinkedList<byte[]> _bufferChain;
		private ManualResetEvent _eof;
		private int _offset;
		private int _length;

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

			_stream			= reader.BaseStream;
			_eof			= new ManualResetEvent(false);
			_offset			= 0;
			_length			= 0;

			_bufferChain.Clear();
			_bufferChain.AddLast(new byte[_bufferSize]);

			BeginReadAsync();
		}

		/// <summary>Closes the reader.</summary>
		public void Close()
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
			int bytesCount = 0;
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
				_offset += bytesCount;
				_length += bytesCount;
				if(_offset >= _bufferSize)
				{
					_offset = 0;
					_bufferChain.AddLast(new byte[_bufferSize]);
				}
				BeginReadAsync();
			}
			else
			{
				_eof.Set();
			}
		}

		private void BeginReadAsync()
		{
			var buffer = _bufferChain.Last.Value;
			_stream.BeginRead(buffer, _offset, buffer.Length - _offset, OnStreamRead, null);
		}

		#endregion
	}
}
