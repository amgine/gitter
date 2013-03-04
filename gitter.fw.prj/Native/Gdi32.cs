namespace gitter.Native
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;

	[SuppressUnmanagedCodeSecurity]
	internal static class Gdi32
	{
		private const string DllName = "gdi32.dll";

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
	}
}
