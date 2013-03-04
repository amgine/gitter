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
