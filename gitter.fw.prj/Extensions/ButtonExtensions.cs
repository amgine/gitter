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

namespace gitter;

using System;
using System.Windows.Forms;

using gitter.Native;
using gitter.Framework;

/// <summary>Extension methods for <see cref="Button"/> class.</summary>
public static class ButtonExtenstions
{
	extension(Button button)
	{
		public void ShowUACShield()
		{
			Verify.Argument.IsNotNull(button);

			if(Utility.IsOSVistaOrNewer)
			{
				User32.SendMessage(button.Handle, 0x1600 + 0x000C, IntPtr.Zero, (IntPtr)(-1));
			}
		}

		public void HideUACShield()
		{
			Verify.Argument.IsNotNull(button);

			if(Utility.IsOSVistaOrNewer)
			{
				User32.SendMessage(button.Handle, 0x1600 + 0x000C, IntPtr.Zero, IntPtr.Zero);
			}
		}
	}
}
