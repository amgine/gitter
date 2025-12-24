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

namespace gitter.Native;

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

	[DllImport(DllName, SetLastError = true, CharSet = CharSet.Unicode)]
	public static unsafe extern int GetObject(IntPtr h, int c, void* pv);

	[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern uint GetTextAlign(IntPtr hdc);

	[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern uint SetTextAlign(IntPtr hdc, uint align);

	[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern uint SetTextColor(IntPtr hdc, uint crColor);

	[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern uint GetTextColor(IntPtr hDC);

	[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern uint SetBkColor(IntPtr hDC, uint clr);

	[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern int SetBkMode(IntPtr hDC, int nBkMode);

	[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern int GetBkMode(IntPtr hDC);

	[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern int GetBkColor(IntPtr hDC);

	[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool Rectangle([In] IntPtr hdc, [In] int nLeftRect, [In] int nTopRect, [In] int nRightRect, [In] int nBottomRect);

	[DllImport(DllName)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern unsafe bool Polyline(IntPtr hdc, POINT* apt, int cpt);

	[DllImport(DllName)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern unsafe bool Polygon(IntPtr hdc, POINT* apt, int cpt);

	[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern IntPtr GetStockObject([In] StockObject fnObject);

	[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern IntPtr CreateSolidBrush([In] uint crColor);

	[DllImport(DllName)]
	public static extern IntPtr CreatePen([In] PenStyle fnPenStyle, [In] int nWidth, [In] uint crColor);

	[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern int GetDeviceCaps(IntPtr hdc, DeviceCaps nIndex);

	[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool DeleteDC([In] IntPtr hDC);

	[DllImport(DllName, EntryPoint = "GetTextExtentPoint32W", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static unsafe extern bool GetTextExtentPoint32(IntPtr hdc, char* str, int len, out SIZE size);

	[DllImport(DllName, EntryPoint = "TextOutW", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static unsafe extern bool TextOut(IntPtr hdc, int x, int y, char* str, int len);

	[DllImport(DllName, EntryPoint = "IntersectClipRect", ExactSpelling = true, PreserveSig = true)]
	public static extern int IntersectClipRect(IntPtr hdc, int left, int top, int right, int bottom);

	[DllImport(DllName, EntryPoint = "SelectClipRgn", ExactSpelling = true, PreserveSig = true)]
	public static extern int SelectClipRgn(IntPtr hdc, IntPtr hrgn);

	[DllImport(DllName, EntryPoint = "SaveDC", ExactSpelling = true, PreserveSig = true)]
	public static extern int SaveDC(IntPtr hdc);

	[DllImport(DllName, EntryPoint = "RestoreDC", ExactSpelling = true, PreserveSig = true)]
	[return: MarshalAs (UnmanagedType.Bool)]
	public static extern bool RestoreDC(IntPtr hdc, int nSavedDC);
}
