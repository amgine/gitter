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

	/// <summary>Helper class for drawing item backgrounds.</summary>
	public static class BackgroundStyle
	{
		private sealed class SolidBackgroundStyle : IBackgroundStyle
		{
			#region Data

			private readonly Pen _outerBorder;
			private readonly Pen _innerBorder;
			private readonly Color _topColor;
			private readonly Color _bottomColor;

			#endregion

			#region .ctor

			public SolidBackgroundStyle(Color outerColor, Color innerColor, Color innerTop, Color innerBottom)
			{
				_outerBorder = new Pen(outerColor);
				_innerBorder = new Pen(innerColor);

				_topColor = innerTop;
				_bottomColor = innerBottom;
			}

			#endregion

			#region Properties

			public Pen InnerBorderPen
			{
				get { return _innerBorder; }
			}

			public Pen OuterBorderPen
			{
				get { return _outerBorder; }
			}

			#endregion

			#region Methods

			public Brush GetBackgroundBrush(int y1, int y2)
			{
				return new LinearGradientBrush(new Point(0, y1), new Point(0, y2), _topColor, _bottomColor);
			}

			public void Draw(Graphics g, Rectangle rect)
			{
				using(var b = GetBackgroundBrush(rect.Y + 1, rect.Bottom - 2))
				{
					g.FillRectangle(b, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
				}
				g.DrawRectangle(InnerBorderPen, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3);
				g.DrawRoundedRectangle(OuterBorderPen, rect, 2);
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				_outerBorder.Dispose();
				_innerBorder.Dispose();
			}

			#endregion
		}

		private sealed class SimpleBackgroundStyle : IBackgroundStyle
		{
			#region Data

			private readonly Pen _outerBorder;

			#endregion

			#region .ctor

			public SimpleBackgroundStyle(Color outerColor)
			{
				_outerBorder = new Pen(outerColor);
			}

			#endregion

			#region Properties

			public Pen OuterBorderPen
			{
				get { return _outerBorder; }
			}

			#endregion

			#region Methods

			public void Draw(Graphics g, Rectangle rect)
			{
				g.DrawRoundedRectangle(OuterBorderPen, rect, 2);
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				_outerBorder.Dispose();
			}

			#endregion
		}

		#region Static

		/// <summary>Focused item style.</summary>
		public static readonly IBackgroundStyle Focused = new SimpleBackgroundStyle(
				Color.FromArgb(125, 162, 206));

		/// <summary>Focused+Selected item style.</summary>
		public static readonly IBackgroundStyle SelectedFocused = new SolidBackgroundStyle(
				Color.FromArgb(125, 162, 206),
				Color.FromArgb(235, 244, 253),
				Color.FromArgb(220, 235, 252),
				Color.FromArgb(193, 219, 252));

		/// <summary>Selected item style.</summary>
		public static readonly IBackgroundStyle Selected = new SolidBackgroundStyle(
				Color.FromArgb(132, 172, 221),
				Color.FromArgb(235, 244, 253),
				Color.FromArgb(235, 244, 254),
				Color.FromArgb(207, 228, 254));

		/// <summary>Selected without control focus.</summary>
		public static readonly IBackgroundStyle SelectedNoFocus = new SolidBackgroundStyle(
				Color.FromArgb(217, 217, 217),
				Color.FromArgb(250, 250, 251),
				Color.FromArgb(248, 248, 248),
				Color.FromArgb(229, 229, 229));

		/// <summary>Hovered item status.</summary>
		public static readonly IBackgroundStyle Hovered = new SolidBackgroundStyle(
				Color.FromArgb(184, 214, 251),
				Color.FromArgb(252, 253, 254),
				Color.FromArgb(250, 251, 253),
				Color.FromArgb(242, 247, 254));

		/// <summary>Hovered+Focused item status.</summary>
		public static readonly IBackgroundStyle HoveredFocused = new SolidBackgroundStyle(
				Color.FromArgb(125, 162, 206),
				Color.FromArgb(252, 253, 254),
				Color.FromArgb(250, 251, 253),
				Color.FromArgb(242, 247, 254));

		#endregion
	}
}
