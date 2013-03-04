namespace gitter.Native
{
	using System;

	[Flags]
	internal enum WVR
	{
		ALIGHTOP = 0x10,
		ALIGHTLEFT = 0x20,
		ALIGHTBOTTOM = 0x40,
		ALIGHTRIGHT = 0x80,
		HREDRAW = 0x100,
		VREDRAW = 0x200,
		REDRAW = 0x300, //(HDRAW | VDRAW)
		VALIDRECTS = 0x400
	}
}
