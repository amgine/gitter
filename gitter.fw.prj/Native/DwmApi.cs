namespace gitter.Native
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;

	[SuppressUnmanagedCodeSecurity]
	internal static class DwmApi
	{
		private const string DllName = "dwmapi.dll";

		[DllImport(DllName, PreserveSig = false)]
		public static extern bool DwmIsCompositionEnabled();

		[DllImport(DllName)]
		public static extern uint DwmSetWindowAttribute(
			[In] IntPtr hwnd,
			[In] DWMWA dwAttribute,
			[In] IntPtr pvAttribute,
			[In] uint cbAttribute);

		[DllImport(DllName)]
		public static extern uint DwmSetWindowAttribute(
			[In] IntPtr hwnd,
			[In] DWMWA dwAttribute,
			[In] ref uint pvAttribute,
			[In] uint cbAttribute);
	}
}
