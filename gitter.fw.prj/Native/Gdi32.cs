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

namespace gitter.Native
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;

	[SuppressUnmanagedCodeSecurity]
	internal static class Gdi32
	{
		private const string DllName = @"gdi32.dll";

		[DllImport(DllName)]
		public static extern int SetDIBitsToDevice(
			IntPtr hdc,
			int XDest,
			int YDest,
			int dwWidth,
			int dwHeight,
			int XSrc,
			int YSrc,
			int uStartScan,
			int cScanLines,
			IntPtr lpvBits,
			ref BITMAPINFOHEADER lpbmi,
			int fuColorUse);

		[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool Ellipse(IntPtr hDc, int x1, int y1, int x2, int y2);

		[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);

		[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetCurrentObject(IntPtr hDC, int uObjectType);

		[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int SetTextColor(IntPtr hdc, int crColor);

		[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetTextAlign(IntPtr hdc);

		[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetTextColor(IntPtr hDC);

		[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int SetBkColor(IntPtr hDC, int clr);

		[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int SetBkMode(IntPtr hDC, int nBkMode);

		[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetBkMode(IntPtr hDC);

		[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetBkColor(IntPtr hDC);
	}
}
