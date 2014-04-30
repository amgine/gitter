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

namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	public abstract class CustomToolTip : ToolTip
	{
		protected const int VerticalMargin = 2;
		protected const int VerticalSpacing = 3;
		protected const int HorizontalMargin = 5;

		protected CustomToolTip()
		{
			OwnerDraw = true;
			Popup += PopupHandler;
			Draw += DrawHandler;
		}

		public abstract Size Size { get; }

		private static void PopupHandler(object sender, PopupEventArgs e)
		{
			var toolTip = (CustomToolTip)sender;
			e.ToolTipSize = toolTip.Size;
			toolTip.OnPopup(e);
		}

		private static void DrawHandler(object sender, DrawToolTipEventArgs e)
		{
			var toolTip = (CustomToolTip)sender;
			var gx = e.Graphics;
			using(var b = new LinearGradientBrush(e.Bounds,
				Color.FromArgb(255, 255, 255),
				Color.FromArgb(228, 229, 240),
				LinearGradientMode.Vertical))
			{
				gx.FillRectangle(b, e.Bounds);
			}
			using(var p = new Pen(Color.FromArgb(118, 118, 118)))
			{
				gx.DrawRoundedRectangle(p, e.Bounds, 1);
			}
			gx.TextRenderingHint = GraphicsUtility.TextRenderingHint;
			gx.TextContrast = GraphicsUtility.TextContrast;
			toolTip.OnPaint(e);
		}

		protected virtual void OnPaint(DrawToolTipEventArgs e)
		{
		}

		protected virtual void OnPopup(PopupEventArgs e)
		{
		}

		public void Show(IWin32Window window, int x, int y)
		{
			Show("-", window, x, y);
		}

		public void Show(IWin32Window window, Point p)
		{
			Show("-", window, p.X, p.Y);
		}
	}
}
