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
	using System.Diagnostics;
	using System.IO;
	using System.Threading;

	public sealed class NullReader : IOutputReceiver
	{
		#region Data

		private readonly byte[] _buffer;
		private Stream _stream;
		private ManualResetEvent _eof;

		#endregion

		#region .ctor

		public NullReader()
		{
			_buffer = new byte[0x400];
		}

		#endregion

		#region IOutputReceiver Members

		public bool IsInitialized
		{
			get { return _stream != null; }
		}

		public void Initialize(Process process, System.IO.StreamReader reader)
		{
			Verify.Argument.IsNotNull(process, "process");
			Verify.Argument.IsNotNull(reader, "reader");
			Verify.State.IsFalse(IsInitialized);

			_stream	= reader.BaseStream;
			_eof	= new ManualResetEvent(false);

			BeginReadAsync();
		}

		public void WaitForEndOfStream()
		{
			Verify.State.IsTrue(IsInitialized);

			_eof.WaitOne();
			_eof.Dispose();

			_stream = null;
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
				BeginReadAsync();
			}
			else
			{
				_eof.Set();
			}
		}

		private void BeginReadAsync()
		{
			_stream.BeginRead(_buffer, 0, _buffer.Length, OnStreamRead, null);
		}

		#endregion
	}
}
