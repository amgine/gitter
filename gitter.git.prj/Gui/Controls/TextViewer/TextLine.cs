namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	protected abstract class TextLineBase
	{
		public void Paint(Graphics graphics, Rectangle rect, int headerWidth)
		{
			var rcHeader = rect;
			rcHeader.Width = headerWidth;
			var rcText = rect;
			rcText.X += headerWidth;
			rcText.Width -= headerWidth;
			OnPaintHeaderBackgorund(graphics, rcHeader);
			OnPaintTextBackground(graphics, rcText);
			OnPaintHeader(graphics, rcHeader);
			OnPaintText(graphics, rcText);
		}

		protected abstract void OnPaintHeaderBackgorund(Graphics graphics, Rectangle rect);

		protected abstract void OnPaintHeader(Graphics graphics, Rectangle rect);

		protected abstract void OnPaintTextBackground(Graphics graphics, Rectangle rect);

		protected abstract void OnPaintText(Graphics graphics, Rectangle rect);

		protected static void PaintMonospaceText(Graphics graphics, Rectangle rect, string text)
		{
		}
	}
}
