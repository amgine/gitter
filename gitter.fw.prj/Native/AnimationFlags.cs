namespace gitter.Native
{
	using System;

	[Flags]
	internal enum AnimationFlags : int
	{
		Roll = 0x0000, // Uses a roll animation.
		HorizontalPositive = 0x00001, // Animates the window from left to right. This flag can be used with roll or slide animation.
		HorizontalNegative = 0x00002, // Animates the window from right to left. This flag can be used with roll or slide animation.
		VerticalPositive = 0x00004, // Animates the window from top to bottom. This flag can be used with roll or slide animation.
		VerticalNegative = 0x00008, // Animates the window from bottom to top. This flag can be used with roll or slide animation.
		Center = 0x00010, // Makes the window appear to collapse inward if <c>Hide</c> is used or expand outward if the <c>Hide</c> is not used.
		Hide = 0x10000, // Hides the window. By default, the window is shown.
		Activate = 0x20000, // Activates the window.
		Slide = 0x40000, // Uses a slide animation. By default, roll animation is used.
		Blend = 0x80000, // Uses a fade effect. This flag can be used only with a top-level window.
		Mask = 0xfffff,
	}
}
