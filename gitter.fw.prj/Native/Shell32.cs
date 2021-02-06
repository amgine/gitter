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
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Security;

	[SuppressUnmanagedCodeSecurity]
	internal static class Shell32
	{
		private const string DllName = "shell32.dll";

		[DllImport(DllName, CharSet = CharSet.Unicode)]
		public static extern int SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)]string appID);

		[DllImport(DllName, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

		[DllImport(DllName)]
		public static extern IntPtr ExtractAssociatedIcon(
			IntPtr hInst,
			string lpIconPath,
			ref short lpiIcon
		);

		[DllImport(DllName)]
		public static extern int ExtractIconEx(
			string lpszFile,
			int nIconIndex,
			IntPtr[] phiconLarge,
			IntPtr[] phiconSmall,
			int nIcons
		);

		[DllImport(DllName)]
		public static extern IntPtr SHGetFileInfo(
			string pszPath,
			int dwFileAttributes,
			ref SHFILEINFO psfi,
			int cbFileInfo,
			int uFlags
		);
	}
}
