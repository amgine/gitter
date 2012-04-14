namespace gitter.Framework.Controls
{
	using System;

	/// <summary>Item state flags.</summary>
	[Flags]
	public enum ItemState
	{
		/// <summary>Default state.</summary>
		None		= 0,
		/// <summary>Item is selected.</summary>
		Selected	= (1 << 0),
		/// <summary>Item is hovered.</summary>
		Hovered		= (1 << 1),
		/// <summary>Item is focused.</summary>
		Focused		= (1 << 2),
		/// <summary>Item is pressed.</summary>
		Pressed		= (1 << 3),
	}
}
