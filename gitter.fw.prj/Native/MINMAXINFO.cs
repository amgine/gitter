namespace gitter.Native
{
	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	internal struct MINMAXINFO
	{
		public Point reserved;
		public Size maxSize;
		public Point maxPosition;
		public Size minTrackSize;
		public Size maxTrackSize;
	}
}
