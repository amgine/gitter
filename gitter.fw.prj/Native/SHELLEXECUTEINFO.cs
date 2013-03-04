namespace gitter.Native
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	internal struct SHELLEXECUTEINFO
	{
		public int cbSize;
		public uint fMask;
		public IntPtr hwnd;
		public string lpVerb;
		public string lpFile;
		public string lpParameters;
		public string lpDirectory;
		public uint nShow;
		public IntPtr hInstApp;
		public IntPtr lpIDList;
		public string lpClass;
		public IntPtr hkeyClass;
		public uint dwHotKey;
		public IntPtr hIcon;
		public IntPtr hProcess;
	}
}
