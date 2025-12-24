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
using System.Windows.Forms;

using gitter.Framework.Configuration;

public sealed class MSVS2012ScrollBarRenderer : CustomScrollBarRenderer
{
	#region Color Tables

	private static IColorTable? _darkColor;
	private static IColorTable? _lightColor;

	/// <summary>Returns dark color table.</summary>
	public static IColorTable DarkColor => _darkColor ??= new DarkColorTable();

	/// <summary>Returns light color table.</summary>
	public static IColorTable LightColor => _lightColor ??= new LightColorTable();

	/// <summary>Defines colors for renderer.</summary>
	public interface IColorTable
	{
		Color Background { get; }

		Color ArrowNormal { get; }
		Color ArrowHover { get; }
		Color ArrowPressed { get; }
		Color ArrowDisabled { get; }

		Color ThumbNormal { get; }
		Color ThumbHover { get; }
		Color ThumbPressed { get; }
		Color ThumbDisabled { get; }
	}

	/// <summary>Colors for dark color theme.</summary>
	private sealed class DarkColorTable : IColorTable
	{
		#region Static

		private static readonly Color BACKGROUND     = MSVS2012DarkColors.SCROLLBAR_SPACING;
		private static readonly Color ARROW_NORMAL   = Color.FromArgb(153, 153, 153);
		private static readonly Color ARROW_HOVER    = Color.FromArgb( 28, 151, 234);
		private static readonly Color ARROW_PRESSED  = Color.FromArgb(  0, 122, 204);
		private static readonly Color ARROW_DISABLED = Color.FromArgb( 85,  85,  88);
		private static readonly Color THUMB_NORMAL   = Color.FromArgb(104, 104, 104);
		private static readonly Color THUMB_HOVER    = Color.FromArgb(158, 158, 158);
		private static readonly Color THUMB_PRESSED  = Color.FromArgb(239, 235, 239);
		private static readonly Color THUMB_DISABLED = Color.FromArgb( 85,  85,  88);

		#endregion

		#region IColorTable

		public Color Background => BACKGROUND;
		public Color ArrowNormal => ARROW_NORMAL;
		public Color ArrowHover => ARROW_HOVER;
		public Color ArrowPressed => ARROW_PRESSED;
		public Color ArrowDisabled => ARROW_DISABLED;
		public Color ThumbNormal => THUMB_NORMAL;
		public Color ThumbHover => THUMB_HOVER;
		public Color ThumbPressed => THUMB_PRESSED;
		public Color ThumbDisabled => THUMB_DISABLED;

		#endregion
	}

	/// <summary>Colors for light color theme.</summary>
	private sealed class LightColorTable : IColorTable
	{
		#region Static

		private static readonly Color BACKGROUND     = MSVS2012LightColors.SCROLLBAR_SPACING;
		private static readonly Color ARROW_NORMAL   = Color.FromArgb(134, 137, 153);
		private static readonly Color ARROW_HOVER    = Color.FromArgb( 28, 151, 234);
		private static readonly Color ARROW_PRESSED  = Color.FromArgb(  0, 122, 204);
		private static readonly Color ARROW_DISABLED = Color.FromArgb(202, 203, 211);
		private static readonly Color THUMB_NORMAL   = Color.FromArgb(208, 209, 215);
		private static readonly Color THUMB_HOVER    = Color.FromArgb(136, 136, 136);
		private static readonly Color THUMB_PRESSED  = Color.FromArgb(106, 106, 106);
		private static readonly Color THUMB_DISABLED = Color.FromArgb(202, 203, 211);

		#endregion

		#region IColorTable

		public Color Background => BACKGROUND;
		public Color ArrowNormal => ARROW_NORMAL;
		public Color ArrowHover => ARROW_HOVER;
		public Color ArrowPressed => ARROW_PRESSED;
		public Color ArrowDisabled => ARROW_DISABLED;
		public Color ThumbNormal => THUMB_NORMAL;
		public Color ThumbHover => THUMB_HOVER;
		public Color ThumbPressed => THUMB_PRESSED;
		public Color ThumbDisabled => THUMB_DISABLED;

		#endregion
	}

	/// <summary>Allows full color customization.</summary>
	public sealed class CustomColorTable : IColorTable, ICloneable
	{
		#region .ctor

		/// <summary>Creates new empty color table.</summary>
		public CustomColorTable()
		{
		}

		/// <summary>Creates new color table based on existing color table.</summary>
		/// <param name="colorTable">Color table to copy colors from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="colorTable"/> == <c>null</c>.</exception>
		public CustomColorTable(IColorTable colorTable)
		{
			Verify.Argument.IsNotNull(colorTable);

			Background		= colorTable.Background;
			ArrowNormal		= colorTable.ArrowNormal;
			ArrowHover		= colorTable.ArrowHover;
			ArrowPressed	= colorTable.ArrowPressed;
			ArrowDisabled	= colorTable.ArrowDisabled;
			ThumbNormal		= colorTable.ThumbNormal;
			ThumbHover		= colorTable.ThumbHover;
			ThumbPressed	= colorTable.ThumbPressed;
			ThumbDisabled	= colorTable.ThumbDisabled;
		}

		/// <summary>Loads color table from configuration.</summary>
		/// <param name="section">Configuration section to load colors from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="section"/> == <c>null</c>.</exception>
		public CustomColorTable(Section section)
		{
			Verify.Argument.IsNotNull(section);

			Background		= section.GetValue<Color>("Background");
			ArrowNormal		= section.GetValue<Color>("ArrowNormal");
			ArrowHover		= section.GetValue<Color>("ArrowHover");
			ArrowPressed	= section.GetValue<Color>("ArrowPressed");
			ArrowDisabled	= section.GetValue<Color>("ArrowDisabled");
			ThumbNormal		= section.GetValue<Color>("ThumbNormal");
			ThumbHover		= section.GetValue<Color>("ThumbHover");
			ThumbPressed	= section.GetValue<Color>("ThumbPressed");
			ThumbDisabled	= section.GetValue<Color>("ThumbDisabled");
		}

		#endregion

		#region IColorTable

		public Color Background { get; set; }

		public Color ArrowNormal { get; set; }

		public Color ArrowHover { get; set; }

		public Color ArrowPressed { get; set; }

		public Color ArrowDisabled { get; set; }

		public Color ThumbNormal { get; set; }

		public Color ThumbHover { get; set; }

		public Color ThumbPressed { get; set; }

		public Color ThumbDisabled { get; set; }

		#endregion

		#region ICloneable

		/// <summary>Creates a copy of this color table.</summary>
		/// <returns>Clone of this color table.</returns>
		public CustomColorTable Clone() => new(this);

		/// <summary>Creates a copy of this color table.</summary>
		/// <returns>Clone of this color table.</returns>
		object ICloneable.Clone() => Clone();

		#endregion

		#region Methods

		/// <summary>Saves this color table to configuration.</summary>
		/// <param name="section">Configuration section to save colors in.</param>
		/// <exception cref="ArgumentNullException"><paramref name="section"/> == <c>null</c>.</exception>
		public void Save(Section section)
		{
			Verify.Argument.IsNotNull(section);

			section.SetValue<Color>("Background",    Background);
			section.SetValue<Color>("ArrowNormal",   ArrowNormal);
			section.SetValue<Color>("ArrowHover",    ArrowHover);
			section.SetValue<Color>("ArrowPressed",  ArrowPressed);
			section.SetValue<Color>("ArrowDisabled", ArrowDisabled);
			section.SetValue<Color>("ThumbNormal",   ThumbNormal);
			section.SetValue<Color>("ThumbHover",    ThumbHover);
			section.SetValue<Color>("ThumbPressed",  ThumbPressed);
			section.SetValue<Color>("ThumbDisabled", ThumbDisabled);
		}

		#endregion
	}

	#endregion

#if !NET9_0_OR_GREATER

	[ThreadStatic]
	private static Point[]? _triangle;

#endif

	#region Constants

	private const int ArrowSize = 5;

	#endregion

	#region .ctor

	public MSVS2012ScrollBarRenderer(IColorTable colorTable)
	{
		Verify.Argument.IsNotNull(colorTable);

		ColorTable = colorTable;
	}

	#endregion

	/// <inheritdoc/>
	protected override void RenderPart(
		CustomScrollBarPart part, Orientation scrollBarOrientation,
		Graphics graphics, Dpi dpi, Rectangle bounds,
		bool isEnabled, bool isHovered, bool isPressed)
	{
		if(bounds is not { Width: > 0, Height: > 0 }) return;

		switch(part)
		{
			case CustomScrollBarPart.DecreaseButton when scrollBarOrientation == Orientation.Vertical:
				RenderUpArrow(graphics, dpi, bounds, isEnabled, isHovered, isPressed);
				break;
			case CustomScrollBarPart.DecreaseButton when scrollBarOrientation == Orientation.Horizontal:
				RenderLeftArrow(graphics, dpi, bounds, isEnabled, isHovered, isPressed);
				break;
			case CustomScrollBarPart.DecreaseTrackBar:
				RenderTrackBar(graphics, bounds);
				break;
			case CustomScrollBarPart.Thumb when scrollBarOrientation == Orientation.Vertical:
				RenderVerticalThumb(graphics, bounds, isEnabled, isHovered, isPressed);
				break;
			case CustomScrollBarPart.Thumb when scrollBarOrientation == Orientation.Horizontal:
				RenderHorizontalThumb(graphics, bounds, isEnabled, isHovered, isPressed);
				break;
			case CustomScrollBarPart.IncreaseTrackBar:
				RenderTrackBar(graphics, bounds);
				break;
			case CustomScrollBarPart.IncreaseButton when scrollBarOrientation == Orientation.Vertical:
				RenderDownArrow(graphics, dpi, bounds, isEnabled, isHovered, isPressed);
				break;
			case CustomScrollBarPart.IncreaseButton when scrollBarOrientation == Orientation.Horizontal:
				RenderRightArrow(graphics, dpi, bounds, isEnabled, isHovered, isPressed);
				break;
		}
	}

	/// <summary>Returns current color table.</summary>
	public IColorTable ColorTable { get; }

	private Color GetArrowColor(bool isEnabled, bool isHovered, bool isPressed)
	{
		if(!isEnabled) return ColorTable.ArrowDisabled;
		if(isPressed)  return ColorTable.ArrowPressed;
		if(isHovered)  return ColorTable.ArrowHover;
		return ColorTable.ArrowNormal;
	}

	private Color GetThumbColor(bool isEnabled, bool isHovered, bool isPressed)
	{
		if(!isEnabled) return ColorTable.ThumbDisabled;
		if(isPressed)  return ColorTable.ThumbPressed;
		if(isHovered)  return ColorTable.ThumbHover;
		return ColorTable.ThumbNormal;
	}

	private void RenderTrackBar(Graphics graphics, Rectangle bounds)
	{
		Assert.IsNotNull(graphics);

		graphics.GdiFill(ColorTable.Background, bounds);
	}

	private static void DrawTriangle(Graphics graphics, Brush brush, Point p1, Point p2, Point p3)
	{
#if NET9_0_OR_GREATER
		Span<Point> triangle = [p1, p2, p3];
#else
		var triangle = _triangle ??= new Point[3];
		triangle[0] = p1;
		triangle[1] = p2;
		triangle[2] = p3;
#endif
		graphics.FillPolygon(brush, triangle);
	}

	private void RenderArrow(Graphics graphics, Rectangle bounds, Color color, Point p1, Point p2, Point p3)
	{
		graphics.GdiFill(ColorTable.Background, bounds);
		using var brush = SolidBrushCache.Get(color);
		DrawTriangle(graphics, brush, p1, p2, p3);
	}

	private static void GetUpArrow(Dpi dpi, Rectangle bounds, out Point p1, out Point p2, out Point p3)
	{
		var conv = DpiConverter.FromDefaultTo(dpi);
		var arrowSize = conv.Convert(new Size(ArrowSize, ArrowSize));
		p1 = new Point(bounds.X + bounds.Width / 2, bounds.Y + (bounds.Height - arrowSize.Height - 1) / 2);
		p2 = new Point(p1.X + arrowSize.Width, p1.Y + arrowSize.Height + 1);
		p3 = new Point(p1.X - arrowSize.Width, p1.Y + arrowSize.Height + 1);
	}

	private static void GetDownArrow(Dpi dpi, Rectangle bounds, out Point p1, out Point p2, out Point p3)
	{
		var conv = DpiConverter.FromDefaultTo(dpi);
		var arrowSize = conv.Convert(new Size(ArrowSize, ArrowSize));
		p1 = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height - (bounds.Height - arrowSize.Height) / 2);
		p2 = new Point(p1.X + arrowSize.Width, p1.Y - arrowSize.Height);
		p3 = new Point(p1.X - arrowSize.Width + 1, p1.Y - arrowSize.Height);
	}

	private static void GetLeftArrow(Dpi dpi, Rectangle bounds, out Point p1, out Point p2, out Point p3)
	{
		var conv = DpiConverter.FromDefaultTo(dpi);
		var arrowSize = conv.Convert(new Size(ArrowSize, ArrowSize));
		p1 = new Point(bounds.X + (bounds.Width - arrowSize.Width) / 2, bounds.Y + bounds.Height / 2);
		p2 = new Point(p1.X + arrowSize.Width, p1.Y - arrowSize.Height);
		p3 = new Point(p1.X + arrowSize.Width, p1.Y + arrowSize.Height);
	}

	private static void GetRightArrow(Dpi dpi, Rectangle bounds, out Point p1, out Point p2, out Point p3)
	{
		var conv = DpiConverter.FromDefaultTo(dpi);
		var arrowSize = conv.Convert(new Size(ArrowSize, ArrowSize));
		p1 = new Point(bounds.X + bounds.Width - (bounds.Width - arrowSize.Width) / 2, bounds.Y + bounds.Height / 2);
		p2 = new Point(p1.X - arrowSize.Width, p1.Y - arrowSize.Height);
		p3 = new Point(p1.X - arrowSize.Width, p1.Y + arrowSize.Height);
	}

	private void RenderUpArrow(Graphics graphics, Dpi dpi, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed)
	{
		Assert.IsNotNull(graphics);

		GetUpArrow(dpi, bounds, out var p1, out var p2, out var p3);
		RenderArrow(graphics, bounds, GetArrowColor(isEnabled, isHovered, isPressed), p1, p2, p3);
	}

	private void RenderDownArrow(Graphics graphics, Dpi dpi, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed)
	{
		Assert.IsNotNull(graphics);

		GetDownArrow(dpi, bounds, out var p1, out var p2, out var p3);
		RenderArrow(graphics, bounds, GetArrowColor(isEnabled, isHovered, isPressed), p1, p2, p3);
	}

	private void RenderLeftArrow(Graphics graphics, Dpi dpi, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed)
	{
		Assert.IsNotNull(graphics);

		GetLeftArrow(dpi, bounds, out var p1, out var p2, out var p3);
		RenderArrow(graphics, bounds, GetArrowColor(isEnabled, isHovered, isPressed), p1, p2, p3);
	}

	private void RenderRightArrow(Graphics graphics, Dpi dpi, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed)
	{
		Assert.IsNotNull(graphics);

		GetRightArrow(dpi, bounds, out var p1, out var p2, out var p3);
		RenderArrow(graphics, bounds, GetArrowColor(isEnabled, isHovered, isPressed), p1, p2, p3);
	}

	private void RenderHorizontalThumb(Graphics graphics, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed)
	{
		Assert.IsNotNull(graphics);

		var backgroundSize = bounds.Height / 4;
		var thumbSize      = bounds.Height - backgroundSize * 2;

		bounds.Height = backgroundSize;

		using var gdi = graphics.AsGdi();

		gdi.Fill(ColorTable.Background, bounds);

		bounds.Y     += backgroundSize;
		bounds.Height = thumbSize;

		gdi.Fill(GetThumbColor(isEnabled, isHovered, isPressed), bounds);

		bounds.Y     += thumbSize;
		bounds.Height = backgroundSize;

		gdi.Fill(ColorTable.Background, bounds);
	}

	private void RenderVerticalThumb(Graphics graphics, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed)
	{
		Assert.IsNotNull(graphics);

		var backgroundSize = bounds.Width / 4;
		var thumbSize      = bounds.Width - backgroundSize * 2;

		bounds.Width = backgroundSize;

		using var gdi = graphics.AsGdi();

		gdi.Fill(ColorTable.Background, bounds);

		bounds.X    += backgroundSize;
		bounds.Width = thumbSize;

		gdi.Fill(GetThumbColor(isEnabled, isHovered, isPressed), bounds);

		bounds.X    += thumbSize;
		bounds.Width = backgroundSize;

		gdi.Fill(ColorTable.Background, bounds);
	}
}
