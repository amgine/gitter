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

namespace gitter.Framework.Controls;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

/// <summary>Helper class for drawing item backgrounds.</summary>
public static class BackgroundStyle
{
	const float cornerRadius = 2.0f;

	private sealed class SolidBackgroundStyle(
		Color outerColor,
		Color innerColor,
		Color innerTop,
		Color innerBottom) : IBackgroundStyle
	{
		public Pen InnerBorderPen { get; } = new Pen(innerColor);

		public Pen OuterBorderPen { get; } = new Pen(outerColor);

		public Brush GetBackgroundBrush(Rectangle rcBackground)
			=> new LinearGradientBrush(rcBackground, innerTop, innerBottom, LinearGradientMode.Vertical);

		public void Draw(Graphics graphics, BackgroundBounds background)
		{
			const int PaddingX = 1;
			const int PaddingY = 1;

			var paddingX = PaddingX * background.Dpi.X / 96;
			var paddingY = PaddingY * background.Dpi.Y / 96;
			var rcBackground = new Rectangle(
				background.X +      paddingX,
				background.Y +      paddingY,
				background.Width  - paddingX * 2,
				background.Height - paddingY * 2);
			using(var brush = GetBackgroundBrush(rcBackground))
			{
				graphics.FillRectangle(brush, rcBackground);
			}
			graphics.DrawRectangle(InnerBorderPen, background.X + paddingX, background.Y + paddingY, background.Width - paddingX * 2 - 1, background.Height - paddingY * 2 - 1);
			graphics.DrawRoundedRectangle(OuterBorderPen, background.Bounds, cornerRadius * background.Dpi.X / 96.0f);
		}

		public void Dispose()
		{
			OuterBorderPen.Dispose();
			InnerBorderPen.Dispose();
		}
	}

	private sealed class SimpleBackgroundStyle(Pen pen) : IBackgroundStyle
	{
		public SimpleBackgroundStyle(Color outerColor) : this(new Pen(outerColor))
		{
		}

		public Pen OuterBorderPen { get; } = pen;

		public void Draw(Graphics graphics, BackgroundBounds background)
		{
			graphics.DrawRoundedRectangle(OuterBorderPen, background.Bounds, cornerRadius * background.Dpi.X / 96.0f);
		}

		public void Dispose()
		{
			OuterBorderPen.Dispose();
		}
	}

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
}
