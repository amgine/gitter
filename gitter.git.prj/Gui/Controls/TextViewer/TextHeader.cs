namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	class TextHeader
	{
		public void Paint(Graphics graphics, Rectangle rect)
		{
			OnPaintBackground(graphics, rect);
			OnPaintContent(graphics, rect);
		}

		protected void OnPaintBackground(Graphics graphics, Rectangle rect)
		{
		}

		protected void OnPaintContent(Graphics graphics, Rectangle rect)
		{
		}
	}
}
