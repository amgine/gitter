namespace gitter.Framework.CLI
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Threading;

	/// <summary>Reads text from stdio/stderr.</summary>
	public sealed class AsyncTextReader : IOutputReceiver
	{
		#region Data

		private readonly int _bufferSize;
		private Stream _stream;
		private byte[] _byteBuffer;
		private char[] _charBuffer;
		private Decoder _decoder;
		private StringBuilder _stringBuilder;
		private ManualResetEvent _eof;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="AsyncTextReader"/> class.</summary>
		/// <param name="bufferSize">Size of the internal buffer.</param>
		public AsyncTextReader(int bufferSize)
		{
			Verify.Argument.IsPositive(bufferSize, "bufferSize");

			_bufferSize		= bufferSize;
			_stringBuilder	= new StringBuilder(bufferSize);
		}

		/// <summary>Initializes a new instance of the <see cref="AsyncTextReader"/> class.</summary>
		public AsyncTextReader()
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
		/// <param name="reader">StreamReader to read from.</param>
		public void Initialize(Process process, StreamReader reader)
		{
			Verify.Argument.IsNotNull(process, "process");
			Verify.Argument.IsNotNull(reader, "reader");
			Verify.State.IsFalse(IsInitialized);

			var encoding	= reader.CurrentEncoding;
			_stream			= reader.BaseStream;
			_decoder		= encoding.GetDecoder();
			_byteBuffer		= new byte[_bufferSize];
			_charBuffer		= new char[encoding.GetMaxCharCount(_bufferSize) + 1];
			_eof			= new ManualResetEvent(false);

			_stringBuilder.Clear();
			BeginReadAsync();
		}

		/// <summary>Closes the reader.</summary>
		public void Close()
		{
			Verify.State.IsTrue(IsInitialized);

			_eof.WaitOne();
			_eof.Close();

			_stream = null;
			_byteBuffer = null;
			_charBuffer = null;
			_decoder = null;
			_eof = null;
		}

		#endregion

		#region Public

		/// <summary>Returns the length of composed text.</summary>
		public int Length
		{
			get { return _stringBuilder.Length; }
		}

		/// <summary>Returns composed text.</summary>
		/// <returns>Composed text.</returns>
		public string GetText()
		{
			return _stringBuilder.ToString();
		}

		/// <summary>Returns array of collected characters.</summary>
		/// <returns>Array of collected characters.</returns>
		public char[] GetCharArray()
		{
			var array = new char[_stringBuilder.Length];
			_stringBuilder.CopyTo(0, array, 0, _stringBuilder.Length);
			return array;
		}

		/// <summary>Clears internal buffer.</summary>
		public void Clear()
		{
			Verify.State.IsFalse(IsInitialized);

			_stringBuilder.Clear();
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
				Decode(bytesCount);
				BeginReadAsync();
			}
			else
			{
				_eof.Set();
			}
		}

		private void BeginReadAsync()
		{
			_stream.BeginRead(_byteBuffer, 0, _byteBuffer.Length, OnStreamRead, null);
		}

		private void Decode(int bytesCount)
		{
			int charsCount = _decoder.GetChars(_byteBuffer, 0, bytesCount, _charBuffer, 0);
			_stringBuilder.Append(_charBuffer, 0, charsCount);
		}

		#endregion
	}
}
