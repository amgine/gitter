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

namespace gitter
{
	using System.Drawing;

	using gitter.Framework;

	/// <summary>Extension methods for <see cref="System.Drawing.Graphics"/>.</summary>
	public static class GraphicsExtensions
	{
		public static void DrawRoundedRectangle(this Graphics g, Pen pen, RectangleF rect, float cornerRadius)
		{
			using(var gp = GraphicsUtility.GetRoundedRectangle(rect, cornerRadius))
			{
				g.DrawPath(pen, gp);
			}
		}

		public static void FillRoundedRectangle(this Graphics g, Brush brush, RectangleF rect, float cornerRadius)
		{
			using(var gp = GraphicsUtility.GetRoundedRectangle(rect, cornerRadius))
			{
				g.FillPath(brush, gp);
			}
		}

		public static void FillRoundedRectangle(this Graphics g, Brush brush, RectangleF rect, float topLeftCorner, float topRightCorner, float bottomLeftCorner, float bottomRightCorner)
		{
			using(var gp = GraphicsUtility.GetRoundedRectangle(rect, topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner))
			{
				g.FillPath(brush, gp);
			}
		}

		public static void FillRoundedRectangle(this Graphics g, Brush fillBrush, Pen borderPen, RectangleF rect, float cornerRadius)
		{
			using(var gp = GraphicsUtility.GetRoundedRectangle(rect, cornerRadius))
			{
				g.FillPath(fillBrush, gp);
				g.DrawPath(borderPen, gp);
			}
		}
	}
}
