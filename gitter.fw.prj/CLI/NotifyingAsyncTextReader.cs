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

	public class NotifyingAsyncTextReader : AsyncTextReader
	{
		#region Data

		private int _lastEolIndex;

		#endregion

		#region Events

		public event EventHandler<TextLineReceivedEventArgs> TextLineReceived;

		private void OnTextLineReceived(string text)
		{
			var handler = TextLineReceived;
			if(handler != null) handler(this, new TextLineReceivedEventArgs(text));
		}

		private void OnTextLineReceived(char[] buffer, int startIndex, int count)
		{
			var handler = TextLineReceived;
			if(handler != null)
			{
				var str = new string(buffer, startIndex, count);
				handler(this, new TextLineReceivedEventArgs(str));
			}
		}

		#endregion

		#region .ctor

		public NotifyingAsyncTextReader()
		{
		}

		public NotifyingAsyncTextReader(int bufferSize)
			: base(bufferSize)
		{
		}

		#endregion

		#region Overrides

		protected override void OnStringDecoded(char[] buffer, int startIndex, int length)
		{
			int bufferedLineChars = Length - _lastEolIndex - 1;
			int bufferEolIndex = -1;
			bool isEol = false;
			bool hadR = false;
			for(int i = startIndex; i < startIndex + length; ++i)
			{
				char current = buffer[i];
				if(current == '\r')
				{
					isEol = true;
					hadR = true;
				}
				else if(current == '\n')
				{
					if(!hadR)
					{
						isEol = true;
					}
					else
					{
						bufferEolIndex = i;
						hadR = false;
						continue;
					}
				}
				else
				{
					hadR = false;
				}
				if(isEol)
				{
					if(bufferedLineChars > 0)
					{
						var temp = new char[bufferedLineChars + i - startIndex];
						bufferedLineChars = 0;
						CopyTo(_lastEolIndex + 1, temp, 0, bufferedLineChars);
						Array.Copy(buffer, startIndex, temp, bufferedLineChars, i - startIndex);
						int offset = temp[0] != '\n' ? 0 : 1;
						OnTextLineReceived(temp, offset, temp.Length - offset);
					}
					else
					{
						OnTextLineReceived(buffer, bufferEolIndex + 1, i - bufferEolIndex - 1);
					}
					bufferEolIndex = i;
					isEol = false;
				}
			}
			if(bufferEolIndex != -1)
			{
				_lastEolIndex = Length + bufferEolIndex - startIndex;
			}
			base.OnStringDecoded(buffer, startIndex, length);
		}

		protected override void OnStringCompleted()
		{
			if(TextLineReceived != null)
			{
				if(_lastEolIndex + 1 < Length - 1)
				{
					OnTextLineReceived(GetText(_lastEolIndex + 1, Length - (_lastEolIndex + 1)));
				}
			}
			base.OnStringCompleted();
		}

		#endregion
	}
}
