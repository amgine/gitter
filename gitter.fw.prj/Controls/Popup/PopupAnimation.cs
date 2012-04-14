namespace gitter.Framework.Controls
{
	using System;

	/// <summary>Popup animation.</summary>
	[Flags]
	public enum PopupAnimations : int
	{
		None = 0,

		LeftToRight = 0x00001,
		RightToLeft = 0x00002,
		TopToBottom = 0x00004,
		BottomToTop = 0x00008,

		Center = 0x00010,

		Slide = 0x40000,

		Blend = 0x80000,

		Roll = 0x100000,

		SystemDefault = 0x200000,
	}
}
