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

		public AsyncBytesReader(int bufferSize = 0x400)
		{
			Verify.Argument.IsPositive(bufferSize, "bufferSize");

			_bufferSize		= bufferSize;
		}

		public void Initialize(Process process, StreamReader reader)
		{
			_stream			= reader.BaseStream;
			_bufferChain	= new LinkedList<byte[]>();
			_eof			= new ManualResetEvent(false);
			_bufferChain.AddLast(new byte[_bufferSize]);

			BeginReadAsync();
		}

		public void Close()
		{
			_eof.WaitOne();
			_eof.Close();

			_stream = null;
			_offset = 0;
			_eof = null;
		}

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
	}
}
