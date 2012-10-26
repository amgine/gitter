using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

namespace gitter
{
	static class RepositoryDragImage
	{
		private static readonly Bitmap ImgRepository = CachedResources.Bitmaps["ImgRepository"];

		public static DragImage Create(string path)
		{
			var size = GitterApplication.TextRenderer.MeasureText(
				Utility.MeasurementGraphics,
				path,
				GitterApplication.FontManager.UIFont,
				int.MaxValue / 2);
			size = new Size(16 + 4 + size.Width + 4, 16 + 6);
			return new DragImage(size, 9, 9, e => PaintImage(e, size, path));
		}

		private static void PaintImage(PaintEventArgs e, Size size, string path)
		{
			var rc = new Rectangle(Point.Empty, size);
			BackgroundStyle.Selected.Draw(e.Graphics, rc);
			e.Graphics.DrawImage(ImgRepository, 2, 3);
			GitterApplication.TextRenderer.DrawText(
				e.Graphics,
				path,
				GitterApplication.FontManager.UIFont,
				SystemBrushes.WindowText, 16 + 4, 4);
		}
	}
}
