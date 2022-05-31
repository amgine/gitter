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

namespace gitter;

using System;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

static class RepositoryDragImage
{
	public static DragImage Create(string path, Dpi dpi)
	{
		var conv = DpiConverter.FromDefaultTo(dpi);
		var size = GitterApplication.TextRenderer.MeasureText(
			GraphicsUtility.MeasurementGraphics,
			path,
			GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi),
			int.MaxValue / 2);
		size = new Size(conv.ConvertX(16) + 2 * conv.ConvertX(4) + size.Width, conv.ConvertY(16) + 2 * conv.ConvertY(3));
		return new DragImage(size, 9, 9, e => PaintImage(e, dpi, size, path));
	}

	private static void PaintImage(PaintEventArgs e, Dpi dpi, Size size, string path)
	{
		var rc   = new Rectangle(Point.Empty, size);
		var conv = DpiConverter.FromDefaultTo(dpi);
		BackgroundStyle.Selected.Draw(e.Graphics, conv.To, rc);
		var icon = Icons.Repository.GetImage(conv.ConvertX(16));
		if(icon is not null)
		{
			e.Graphics.DrawImage(icon, conv.ConvertX(2), conv.ConvertY(3));
		}
		GitterApplication.TextRenderer.DrawText(
			e.Graphics,
			path,
			GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi),
			SystemBrushes.WindowText, conv.ConvertX(16) + conv.ConvertX(4), conv.ConvertY(4));
	}
}
