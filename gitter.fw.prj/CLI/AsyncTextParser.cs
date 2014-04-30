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

namespace gitter.Framework.CLI
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Threading;

	/// <summary>Reads text from stdio/stderr and parses it.</summary>
	public sealed class AsyncTextParser : IOutputReceiver
	{
		#region Data

		private readonly IParser _parser;
		private readonly int _bufferSize;
		private readonly CharArrayTextSegment _textSegment;
		private Stream _stream;
		private byte[] _byteBuffer;
		private char[] _charBuffer;
		private Decoder _decoder;
		private ManualResetEvent _eof;
		private volatile bool _isCanceled;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="AsyncTextParser"/> class.</summary>
		/// <param name="parser">Output parser.</param>
		/// <param name="bufferSize">Size of the internal buffer.</param>
		public AsyncTextParser(IParser parser, int bufferSize)
		{
			Verify.Argument.IsNotNull(parser, "parser");
			Verify.Argument.IsPositive(bufferSize, "bufferSize");

			_parser      = parser;
			_bufferSize  = bufferSize;
			_textSegment = new CharArrayTextSegment();
		}

		/// <summary>Initializes a new instance of the <see cref="AsyncTextParser"/> class.</summary>
		/// <param name="parser">Output parser.</param>
		public AsyncTextParser(IParser parser)
			: this(parser, 0x400)
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
		/// <param name="reader">StreamReader to read from.</param>
		public void Initialize(Process process, StreamReader reader)
		{
			Verify.Argument.IsNotNull(process, "process");
			Verify.Argument.IsNotNull(reader, "reader");
			Verify.State.IsFalse(IsInitialized);

			var encoding = reader.CurrentEncoding;
			_stream      = reader.BaseStream;
			_decoder     = encoding.GetDecoder();
			_byteBuffer  = new byte[_bufferSize];
			_charBuffer  = new char[encoding.GetMaxCharCount(_bufferSize) + 1];
			_eof         = new ManualResetEvent(false);

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

			var eof = _eof;
			if(eof != null)
			{
				try
				{
					eof.WaitOne();
					eof.Dispose();
				}
				catch(ObjectDisposedException)
				{
				}
			}

			_stream = null;
			_byteBuffer = null;
			_charBuffer = null;
			_decoder = null;
			_eof = null;
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
					DecodeAndParse(bytesCount);
				}
				BeginReadAsync();
			}
			else
			{
				if(_isCanceled)
				{
					return;
				}
				OnStringCompleted();
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
				try
				{
					_stream.BeginRead(_byteBuffer, 0, _byteBuffer.Length, OnStreamRead, null);
				}
				catch(ObjectDisposedException)
				{
				}
			}
			else
			{
				bool isReading;
				var eof = _eof;
				try
				{
					_stream.BeginRead(_byteBuffer, 0, _byteBuffer.Length, OnStreamRead, eof);
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

		private void DecodeAndParse(int bytesCount)
		{
			int charsCount = _decoder.GetChars(_byteBuffer, 0, bytesCount, _charBuffer, 0);
			if(charsCount != 0)
			{
				_textSegment.SetBuffer(_charBuffer, 0, charsCount);
				_parser.Parse(_textSegment);
			}
		}

		private void OnStringCompleted()
		{
			_parser.Complete();
		}

		#endregion
	}
}
