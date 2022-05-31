namespace gitter.Git.Gui.Controls;

using System;
using System.Drawing;

using gitter.Framework;
using gitter.Framework.Controls;

static class FakeRevisionPainterHelper
{
	public static bool TryGetTextBrush(SubItemPaintEventArgs paintEventArgs, out Brush textBrush, out bool disposeBrush)
	{
		Assert.IsNotNull(paintEventArgs);

		var style = paintEventArgs.ListBox.Style;
		if((paintEventArgs.State & ItemState.Selected) == ItemState.Selected && style.Type == GitterStyleType.DarkBackground)
		{
			textBrush = default;
			disposeBrush = false;
			return false;
		}
		else
		{
			textBrush = new SolidBrush(style.Colors.GrayText);
			disposeBrush = true;
			return true;
		}
	}
}
