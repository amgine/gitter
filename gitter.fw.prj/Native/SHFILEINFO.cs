namespace gitter.Native
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	internal struct SHFILEINFO
	{
		public IntPtr hIcon;
		public int iIcon;
		public int dwAttributes;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string szDisplayName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
		public string szTypeName;
	}
}
