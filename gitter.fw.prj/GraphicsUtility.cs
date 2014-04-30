#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Drawing.Text;

	public static class GraphicsUtility
	{
		private static readonly Image _dummyImage = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		private static readonly Graphics _measurementGraphics = Graphics.FromImage(_dummyImage);

		public static Graphics MeasurementGraphics
		{
			get { return _measurementGraphics; }
		}

		public static Bitmap QueryIcon(string fileName)
		{
			return IconCache.GetIcon(fileName);
		}

		public const TextRenderingHint TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

		public const int TextContrast = 0;

		public static GraphicsPath GetRoundedRectangle(RectangleF rect, float arcRadius)
		{
			var x = rect.X;
			var y = rect.Y;
			var w = rect.Width;
			var h = rect.Height;
			var d = 2 * arcRadius;

			var gp = new GraphicsPath();
			if(arcRadius == 0)
			{
				gp.AddRectangle(rect);
			}
			else
			{
				gp.AddArc(x, y, d, d, 180, 90);
				gp.AddLine(x + arcRadius, y, x + w - arcRadius - 1, y);
				gp.AddArc(x + w - d - 1, y, d, d, 270, 90);
				gp.AddLine(x + w - 1, y + arcRadius, x + w - 1, y + h - arcRadius - 1);
				gp.AddArc(x + w - d - 1, y + h - d - 1, d, d, 0, 90);
				gp.AddLine(x + w - arcRadius - 1, y + h - 1, x + arcRadius, y + h - 1);
				gp.AddArc(x, y + h - d - 1, d, d, 90, 90);
				gp.AddLine(x, y + h - arcRadius - 1, x, y + arcRadius);
			}
			gp.CloseFigure();
			return gp;
		}

		public static Region GetRoundedRegion(RectangleF rect, float arcRadius)
		{
			var x = rect.X;
			var y = rect.Y;
			var w = rect.Width;
			var h = rect.Height;
			var d = 2 * arcRadius;

			using(var gp = new GraphicsPath())
			{
				if(arcRadius == 0)
				{
					gp.AddRectangle(rect);
				}
				else
				{
					gp.AddArc(x, y, d, d, 180, 90);
					gp.AddLine(x + arcRadius, y, x + w - arcRadius + 1, y);
					gp.AddArc(x + w - d, y, d - 1, d, 270, 90);
					gp.AddLine(x + w, y + arcRadius, x + w, y + h - arcRadius);
					gp.AddArc(x + w - d, y + h - d - 1, d - 1, d, 0, 90);
					gp.AddLine(x + w - arcRadius - 1, y + h, x + arcRadius, y + h);
					gp.AddArc(x, y + h - d - 1, d, d, 90, 90);
					gp.AddLine(x, y + h - arcRadius, x, y + arcRadius);
				}
				gp.CloseFigure();
				return new Region(gp);
			}
		}

		public static GraphicsPath GetRoundedRectangle(RectangleF rect, float topLeftCorner, float topRightCorner, float bottomLeftCorner, float bottomRightCorner)
		{
			var x = rect.X;
			var y = rect.Y;
			var w = rect.Width;
			var h = rect.Height;

			var gp = new GraphicsPath();
			if(topLeftCorner != 0)
			{
				gp.AddArc(x, y,
					2 * topLeftCorner, 2 * topLeftCorner, 180, 90);
			}
			gp.AddLine(x, y, x + w - topRightCorner - 1, y);
			if(topRightCorner != 0)
			{
				gp.AddArc(
					x + w - 2 * topRightCorner - 1,
					y,
					2 * topRightCorner, 2 * topRightCorner, 270, 90);
			}
			gp.AddLine(x + w - 1, y, x + w - 1, y + h - bottomRightCorner - 1);
			if(bottomRightCorner != 0)
			{
				gp.AddArc(
					x + w - 2 * bottomRightCorner - 1,
					y + h - 2 * bottomRightCorner - 1,
					2 * bottomRightCorner, 2 * bottomRightCorner, 0, 90);
			}
			gp.AddLine(x + w, y + h - 1, x + bottomLeftCorner, y + h - 1);
			if(bottomLeftCorner != 0)
			{
				gp.AddArc(
					x,
					y + h - 2 * bottomLeftCorner - 1,
					2 * bottomLeftCorner, 2 * bottomLeftCorner, 90, 90);
			}
			gp.AddLine(x, y + h - bottomLeftCorner - 1, x, y + topLeftCorner);
			gp.CloseFigure();
			return gp;
		}
	}
}
