namespace gitter.Native
{
	using System;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	internal struct COMBOBOXINFO
	{
		public uint cbSize;
		public RECT rcItem;
		public RECT rcButton;
		public uint stateButton;
		public IntPtr hwndCombo;
		public IntPtr hwndItem;
		public IntPtr hwndList;
	};
}
