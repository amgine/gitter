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

namespace gitter.Framework;

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

public static class ClipboardEx
{
	public static void SetTextSafe(string text, int attemptsCount = 3, int retryInterval = 50)
	{
		const int CLIPBRD_E_CANT_OPEN = unchecked((int)0x800401D0);
		while(attemptsCount > 0)
		{
			try
			{
				Clipboard.SetText(text);
				return;
			}
			catch(COMException exc) when(exc.ErrorCode == CLIPBRD_E_CANT_OPEN)
			{
			}
			--attemptsCount;
			Thread.Sleep(retryInterval);
		}
	}
}
