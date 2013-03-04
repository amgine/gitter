namespace gitter.Native
{
	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	internal struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;

		public RECT(int l, int t, int r, int b)
		{
			left = l;
			top = t;
			right = r;
			bottom = b;
		}

		public RECT(Rectangle rect)
		{
			left = rect.Left;
			top = rect.Top;
			right = rect.Right;
			bottom = rect.Bottom;
		}
	};
}
