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

namespace gitter.Native
{
	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;
	using System.Security;

	[SuppressUnmanagedCodeSecurity]
	internal static class User32
	{
		private const string DllName = "user32.dll";

		[DllImport(DllName, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern unsafe int DrawTextExW(IntPtr hDC, char* lpszString, int nCount, ref RECT lpRect, DT nFormat, ref DRAWTEXTPARAMS lpDTParams);

		[DllImport(DllName)]
		public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool revert);

		[DllImport(DllName)]
		public static extern int EnableMenuItem(IntPtr hMenu, int IDEnableItem, int wEnable);

		[DllImport(DllName, CharSet = CharSet.Auto)]
		public static extern int RegisterWindowMessage(string lpString);

		[DllImport(DllName)]
		public static extern IntPtr WindowFromPoint(POINT point);

		[DllImport(DllName)]
		public static extern IntPtr WindowFromPoint(Point point);

		[DllImport(DllName)]
		public static extern IntPtr GetAncestor(IntPtr hwnd, int gaFlags);

		[DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		public static extern IntPtr GetWindow(IntPtr hwnd, int wFlag);

		[DllImport(DllName, CharSet = CharSet.Auto)]
		public static extern int AnimateWindow(HandleRef windowHandle, int time, AnimationFlags flags);

		[DllImport(DllName, CharSet = CharSet.Auto)]
		public static extern IntPtr SetCapture(IntPtr hWnd);

		[DllImport(DllName, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ReleaseCapture();

		[DllImport(DllName)]
		public static extern IntPtr SendMessage(IntPtr hWnd, WM msg, IntPtr wParam, IntPtr lParam);

		[DllImport(DllName)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		[DllImport(DllName)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PostMessage(IntPtr hWnd, WM msg, IntPtr wParam, IntPtr lParam);

		[DllImport(DllName)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		[DllImport(DllName)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindow(IntPtr handle, int flags);

		[DllImport(DllName)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(
			IntPtr hWnd,
			IntPtr hWndInsertAfter,
			int X,
			int Y,
			int cx,
			int cy,
			int uFlags);

		[DllImport(DllName)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool LockWindowUpdate(IntPtr hWndLock);

		[DllImport(DllName)]
		public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, WNDPROC dwNewLong);

		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, WNDPROC dwNewLong);

		[DllImport(DllName)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool InflateRect(
		  [In, Out] ref RECT lprc,
		  [In]      int dx,
		  [In]      int dy);

		[DllImport(DllName)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool InflateRect(
		  [In] IntPtr lprc,
		  [In] int dx,
		  [In] int dy);

		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern bool RedrawWindow(IntPtr hWnd, ref RECT lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern IntPtr GetDC(
			[In] IntPtr hWnd);

		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern IntPtr GetDCEx(
			[In] IntPtr hWnd,
			[In] IntPtr hrgnClip,
			[In] DCX flags);

		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern int ReleaseDC(
			[In] IntPtr hWnd,
			[In] IntPtr hDC);

		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern bool ScrollDC(
			[In] IntPtr hDC,
			[In] int dx,
			[In] int dy,
			[In] ref RECT lprcScroll,
			[In] ref RECT lprcClip,
			[In] IntPtr hrgnUpdate,
			[Out]out RECT lprcUpdate);

		[DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		public static extern int ScrollWindowEx(
			[In]  IntPtr hWnd,
			[In]  int nXAmount,
			[In]  int nYAmount,
			[In]  ref RECT rectScrollRegion,
			[In]  ref RECT rectClip,
			[In]  IntPtr hrgnUpdate,
			[Out] out RECT prcUpdate,
			[In]  int flags);

		[DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		public static extern int ScrollWindowEx(
			[In]  IntPtr hWnd,
			[In]  int nXAmount,
			[In]  int nYAmount,
			[In]  ref RECT rectScrollRegion,
			[In]  ref RECT rectClip,
			[In]  IntPtr hrgnUpdate,
			[In]  IntPtr prcUpdate,
			[In]  int flags);

		[DllImport(DllName)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetComboBoxInfo(IntPtr hwndCombo, ref COMBOBOXINFO pcbi);

		[DllImport(DllName)]
		public static extern IntPtr SetWindowsHookEx(
			[In]  WH idHook,
			[MarshalAs(UnmanagedType.FunctionPtr)]
			[In]  HookProc lpfn,
			[In]  IntPtr hMod,
			[In]  int dwThreadId);

		[DllImport(DllName)]
		public static extern IntPtr CallNextHookEx(
			[In]  IntPtr hhk,
			[In]  int nCode,
			[In]  IntPtr wParam,
			[In]  IntPtr lParam);

		[DllImport(DllName)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(
			[In]  IntPtr hhk);
	}
}
