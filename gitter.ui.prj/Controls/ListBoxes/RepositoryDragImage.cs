#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
				GraphicsUtility.MeasurementGraphics,
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
