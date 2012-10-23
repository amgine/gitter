namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Threading;

	/// <summary>Reads text from stdio/stderr.</summary>
	internal sealed class AsyncTextReader : IOutputReceiver
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

		public AsyncTextReader(int bufferSize = 0x400)
		{
			Verify.Argument.IsPositive(bufferSize, "bufferSize");

			_bufferSize = bufferSize;
		}

		public void Initialize(Process process, StreamReader reader)
		{
			Verify.Argument.IsNotNull(process, "process");
			Verify.Argument.IsNotNull(reader, "reader");

			var encoding	= reader.CurrentEncoding;
			_stream			= reader.BaseStream;
			_decoder		= encoding.GetDecoder();
			_byteBuffer		= new byte[_bufferSize];
			_charBuffer		= new char[encoding.GetMaxCharCount(_bufferSize)];
			_stringBuilder	= new StringBuilder(_bufferSize);
			_eof			= new ManualResetEvent(false);

			BeginReadAsync();
		}

		public void Close()
		{
			_eof.WaitOne();
			_eof.Close();

			_stream = null;
			_byteBuffer = null;
			_charBuffer = null;
			_decoder = null;
			_eof = null;
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
				Decode(bytesCount);
				BeginReadAsync();
			}
			else
			{
				_eof.Set();
			}
		}

		public string GetText()
		{
			return _stringBuilder.ToString();
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
	}
}
