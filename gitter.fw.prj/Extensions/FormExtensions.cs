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
using System.Windows.Forms;

using gitter.Native;

/// <summary>Extension methods for <see cref="System.Windows.Forms.Form"/>.</summary>
public static class FormExtensions
{
	private const int SC_MOVE = 0xf010;
	private const int SC_MINIMIZE = 0xf020;
	private const int SC_MAXIMIZE = 0xf030;
	private const int SC_CLOSE = 0xf060;
	private const int SC_RESTORE = 0xf120;

	private const int MF_BYCOMMAND = 0x00000000;
	private const int MF_BYPOSITION = 0x00000400;
	private const int MF_DISABLED = 0x00000002;
	private const int MF_ENABLED = 0x00000000;
	private const int MF_GRAYED = 0x00000001;

	public static void DisableCloseButton(this Form form)
	{
		Verify.Argument.IsNotNull(form);

		var hMenu = User32.GetSystemMenu(form.Handle, false);
		_ = User32.EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
	}

	public static void EnableCloseButton(this Form form)
	{
		Verify.Argument.IsNotNull(form);

		var hMenu = User32.GetSystemMenu(form.Handle, false);
		_ = User32.EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
	}
}
