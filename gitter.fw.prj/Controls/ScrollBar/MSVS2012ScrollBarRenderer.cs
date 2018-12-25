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
	using System.Windows.Forms;

	using gitter.Framework.Configuration;

	public sealed class MSVS2012ScrollBarRenderer : CustomScrollBarRenderer
	{
		#region Color Tables

		private static IColorTable _darkColor;
		private static IColorTable _lightColor;

		/// <summary>Returns dark color table.</summary>
		public static IColorTable DarkColor
		{
			get
			{
				if(_darkColor == null)
				{
					_darkColor = new DarkColorTable();
				}
				return _darkColor;
			}
		}

		/// <summary>Returns light color table.</summary>
		public static IColorTable LightColor
		{
			get
			{
				if(_lightColor == null)
				{
					_lightColor = new LightColorTable();
				}
				return _lightColor;
			}
		}

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

			private static readonly Color BACKGROUND		= MSVS2012DarkColors.SCROLLBAR_SPACING;
			private static readonly Color ARROW_NORMAL		= Color.FromArgb(153, 153, 153);
			private static readonly Color ARROW_HOVER		= Color.FromArgb( 28, 151, 234);
			private static readonly Color ARROW_PRESSED		= Color.FromArgb(  0, 122, 204);
			private static readonly Color ARROW_DISABLED	= Color.FromArgb( 85,  85,  88);
			private static readonly Color THUMB_NORMAL		= Color.FromArgb(104, 104, 104);
			private static readonly Color THUMB_HOVER		= Color.FromArgb(158, 158, 158);
			private static readonly Color THUMB_PRESSED		= Color.FromArgb(239, 235, 239);
			private static readonly Color THUMB_DISABLED	= Color.FromArgb( 85,  85,  88);

			#endregion

			#region IColorTable

			public Color Background
			{
				get { return BACKGROUND; }
			}

			public Color ArrowNormal
			{
				get { return ARROW_NORMAL; }
			}

			public Color ArrowHover
			{
				get { return ARROW_HOVER; }
			}

			public Color ArrowPressed
			{
				get { return ARROW_PRESSED; }
			}

			public Color ArrowDisabled
			{
				get { return ARROW_DISABLED; }
			}

			public Color ThumbNormal
			{
				get { return THUMB_NORMAL; }
			}

			public Color ThumbHover
			{
				get { return THUMB_HOVER; }
			}

			public Color ThumbPressed
			{
				get { return THUMB_PRESSED; }
			}

			public Color ThumbDisabled
			{
				get { return THUMB_DISABLED; }
			}

			#endregion
		}

		/// <summary>Colors for light color theme.</summary>
		private sealed class LightColorTable : IColorTable
		{
			#region Static

			private static readonly Color BACKGROUND		= MSVS2012LightColors.SCROLLBAR_SPACING;
			private static readonly Color ARROW_NORMAL		= Color.FromArgb(134, 137, 153);
			private static readonly Color ARROW_HOVER		= Color.FromArgb( 28, 151, 234);
			private static readonly Color ARROW_PRESSED		= Color.FromArgb(  0, 122, 204);
			private static readonly Color ARROW_DISABLED	= Color.FromArgb(202, 203, 211);
			private static readonly Color THUMB_NORMAL		= Color.FromArgb(208, 209, 215);
			private static readonly Color THUMB_HOVER		= Color.FromArgb(136, 136, 136);
			private static readonly Color THUMB_PRESSED		= Color.FromArgb(106, 106, 106);
			private static readonly Color THUMB_DISABLED	= Color.FromArgb(202, 203, 211);

			#endregion

			#region IColorTable

			public Color Background
			{
				get { return BACKGROUND; }
			}

			public Color ArrowNormal
			{
				get { return ARROW_NORMAL; }
			}

			public Color ArrowHover
			{
				get { return ARROW_HOVER; }
			}

			public Color ArrowPressed
			{
				get { return ARROW_PRESSED; }
			}

			public Color ArrowDisabled
			{
				get { return ARROW_DISABLED; }
			}

			public Color ThumbNormal
			{
				get { return THUMB_NORMAL; }
			}

			public Color ThumbHover
			{
				get { return THUMB_HOVER; }
			}

			public Color ThumbPressed
			{
				get { return THUMB_PRESSED; }
			}

			public Color ThumbDisabled
			{
				get { return THUMB_DISABLED; }
			}

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
				Verify.Argument.IsNotNull(colorTable, nameof(colorTable));

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
				Verify.Argument.IsNotNull(section, nameof(section));

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

			public Color Background
			{
				get;
				set;
			}

			public Color ArrowNormal
			{
				get;
				set;
			}

			public Color ArrowHover
			{
				get;
				set;
			}

			public Color ArrowPressed
			{
				get;
				set;
			}

			public Color ArrowDisabled
			{
				get;
				set;
			}

			public Color ThumbNormal
			{
				get;
				set;
			}

			public Color ThumbHover
			{
				get;
				set;
			}

			public Color ThumbPressed
			{
				get;
				set;
			}

			public Color ThumbDisabled
			{
				get;
				set;
			}

			#endregion

			#region ICloneable

			/// <summary>Creates a cole of this color table.</summary>
			/// <returns>Clone of this color table.</returns>
			public CustomColorTable Clone()
			{
				return new CustomColorTable(this);
			}

			/// <summary>Creates a cole of this color table.</summary>
			/// <returns>Clone of this color table.</returns>
			object ICloneable.Clone()
			{
				return Clone();
			}

			#endregion

			#region Methods

			/// <summary>Saves this color table to configuration.</summary>
			/// <param name="section">Configuration section to save colors in.</param>
			/// <exception cref="ArgumentNullException"><paramref name="section"/> == <c>null</c>.</exception>
			public void Save(Section section)
			{
				Verify.Argument.IsNotNull(section, nameof(section));

				section.SetValue<Color>("Background",		Background);
				section.SetValue<Color>("ArrowNormal",		ArrowNormal);
				section.SetValue<Color>("ArrowHover",		ArrowHover);
				section.SetValue<Color>("ArrowPressed",		ArrowPressed);
				section.SetValue<Color>("ArrowDisabled",	ArrowDisabled);
				section.SetValue<Color>("ThumbNormal",		ThumbNormal);
				section.SetValue<Color>("ThumbHover",		ThumbHover);
				section.SetValue<Color>("ThumbPressed",		ThumbPressed);
				section.SetValue<Color>("ThumbDisabled",	ThumbDisabled);
			}

			#endregion
		}

		#endregion

		#region Constants

		private const int ArrowSize = 5;

		#endregion

		#region Data

		private readonly IColorTable _colorTable;

		#endregion

		#region .ctor

		public MSVS2012ScrollBarRenderer(IColorTable colorTable)
		{
			Verify.Argument.IsNotNull(colorTable, nameof(colorTable));

			_colorTable = colorTable;
		}

		#endregion

		protected override void RenderPart(CustomScrollBarPart part, Orientation scrollBarOrientation, Graphics graphics, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed)
		{
			if(bounds.Width <= 0 || bounds.Height <= 0) return;

			switch(part)
			{
				case CustomScrollBarPart.DecreaseButton:
					switch(scrollBarOrientation)
					{
						case Orientation.Horizontal:
							RenderLeftArrow(graphics, bounds, isEnabled, isHovered, isPressed);
							break;
						case Orientation.Vertical:
							RenderUpArrow(graphics, bounds, isEnabled, isHovered, isPressed);
							break;
						default:
							throw new ArgumentException();
					}
					break;
				case CustomScrollBarPart.DecreaseTrackBar:
					RenderTrackBar(graphics, bounds);
					break;
				case CustomScrollBarPart.Thumb:
					switch(scrollBarOrientation)
					{
						case Orientation.Horizontal:
							RenderHorizontalThumb(graphics, bounds, isEnabled, isHovered, isPressed);
							break;
						case Orientation.Vertical:
							RenderVerticalThumb(graphics, bounds, isEnabled, isHovered, isPressed);
							break;
						default:
							throw new ArgumentException();
					}
					break;
				case CustomScrollBarPart.IncreaseTrackBar:
					RenderTrackBar(graphics, bounds);
					break;
				case CustomScrollBarPart.IncreaseButton:
					switch(scrollBarOrientation)
					{
						case Orientation.Horizontal:
							RenderRightArrow(graphics, bounds, isEnabled, isHovered, isPressed);
							break;
						case Orientation.Vertical:
							RenderDownArrow(graphics, bounds, isEnabled, isHovered, isPressed);
							break;
						default:
							throw new ArgumentException();
					}
					break;
			}
		}

		/// <summary>Returns current color table.</summary>
		public IColorTable ColorTable
		{
			get { return _colorTable; }
		}

		private Color GetArrowColor(bool isEnabled, bool isHovered, bool isPressed)
		{
			if(isEnabled)
			{
				if(isPressed)
				{
					return ColorTable.ArrowPressed;
				}
				else
				{
					if(isHovered)
					{
						return ColorTable.ArrowHover;
					}
					else
					{
						return ColorTable.ArrowNormal;
					}
				}
			}
			else
			{
				return ColorTable.ArrowDisabled;
			}
		}

		private Color GetThumbColor(bool isEnabled, bool isHovered, bool isPressed)
		{
			if(isEnabled)
			{
				if(isPressed)
				{
					return ColorTable.ThumbPressed;
				}
				else
				{
					if(isHovered)
					{
						return ColorTable.ThumbHover;
					}
					else
					{
						return ColorTable.ThumbNormal;
					}
				}
			}
			else
			{
				return ColorTable.ThumbDisabled;
			}
		}

		private void RenderTrackBar(Graphics graphics, Rectangle bounds)
		{
			using(var brush = new SolidBrush(ColorTable.Background))
			{
				graphics.FillRectangle(brush, bounds);
			}
		}

		private void RenderUpArrow(Graphics graphics, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed)
		{
			using(var brush = new SolidBrush(ColorTable.Background))
			{
				graphics.FillRectangle(brush, bounds);
			}
			using(var brush = new SolidBrush(GetArrowColor(isEnabled, isHovered, isPressed)))
			{
				var p1 = new Point(bounds.X + bounds.Width / 2, bounds.Y + (bounds.Height - ArrowSize - 1) / 2);
				var p2 = new Point(p1.X + ArrowSize, p1.Y + ArrowSize + 1);
				var p3 = new Point(p1.X - ArrowSize, p1.Y + ArrowSize + 1);
				var triangle = new[] { p1, p2, p3 };
				graphics.FillPolygon(brush, triangle);
			}
		}

		private void RenderDownArrow(Graphics graphics, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed)
		{
			using(var brush = new SolidBrush(ColorTable.Background))
			{
				graphics.FillRectangle(brush, bounds);
			}
			using(var brush = new SolidBrush(GetArrowColor(isEnabled, isHovered, isPressed)))
			{
				var p1 = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height - (bounds.Height - ArrowSize) / 2);
				var p2 = new Point(p1.X + ArrowSize, p1.Y - ArrowSize);
				var p3 = new Point(p1.X - ArrowSize + 1, p1.Y - ArrowSize);
				var triangle = new[] { p1, p2, p3 };
				graphics.FillPolygon(brush, triangle);
			}
		}

		private void RenderLeftArrow(Graphics graphics, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed)
		{
			using(var brush = new SolidBrush(ColorTable.Background))
			{
				graphics.FillRectangle(brush, bounds);
			}
			using(var brush = new SolidBrush(GetArrowColor(isEnabled, isHovered, isPressed)))
			{
				var p1 = new Point(bounds.X + (bounds.Width - ArrowSize) / 2, bounds.Y + bounds.Height / 2);
				var p2 = new Point(p1.X + ArrowSize, p1.Y - ArrowSize);
				var p3 = new Point(p1.X + ArrowSize, p1.Y + ArrowSize);
				var triangle = new[] { p1, p2, p3 };
				graphics.FillPolygon(brush, triangle);
			}
		}

		private void RenderRightArrow(Graphics graphics, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed)
		{
			using(var brush = new SolidBrush(ColorTable.Background))
			{
				graphics.FillRectangle(brush, bounds);
			}
			using(var brush = new SolidBrush(GetArrowColor(isEnabled, isHovered, isPressed)))
			{
				var p1 = new Point(bounds.X + bounds.Width - (bounds.Width - ArrowSize) / 2, bounds.Y + bounds.Height / 2);
				var p2 = new Point(p1.X - ArrowSize, p1.Y - ArrowSize);
				var p3 = new Point(p1.X - ArrowSize, p1.Y + ArrowSize);
				var triangle = new[] { p1, p2, p3 };
				graphics.FillPolygon(brush, triangle);
			}
		}

		private void RenderHorizontalThumb(Graphics graphics, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed)
		{
			var backgroundSize = bounds.Height / 2;
			var thumbSize = bounds.Height - backgroundSize;
			var backgroundSize1 = backgroundSize / 2;
			var backgroundSize2 = backgroundSize - backgroundSize1;

			using(var brush = new SolidBrush(ColorTable.Background))
			{
				graphics.FillRectangle(brush, bounds.X, bounds.Y, bounds.Width, backgroundSize1);
				graphics.FillRectangle(brush, bounds.X, bounds.Bottom - backgroundSize2, bounds.Width, backgroundSize2);
			}
			using(var brush = new SolidBrush(GetThumbColor(isEnabled, isHovered, isPressed)))
			{
				graphics.FillRectangle(brush, bounds.X, bounds.Y + backgroundSize1, bounds.Width, thumbSize);
			}
		}

		private void RenderVerticalThumb(Graphics graphics, Rectangle bounds, bool isEnabled, bool isHovered, bool isPressed)
		{
			var backgroundSize = bounds.Width / 2;
			var thumbSize = bounds.Width - backgroundSize;
			var backgroundSize1 = backgroundSize / 2;
			var backgroundSize2 = backgroundSize - backgroundSize1;

			using(var brush = new SolidBrush(ColorTable.Background))
			{
				graphics.FillRectangle(brush, bounds.X, bounds.Y, backgroundSize1, bounds.Height);
				graphics.FillRectangle(brush, bounds.Right - backgroundSize2, bounds.Y, backgroundSize2, bounds.Height);
			}
			using(var brush = new SolidBrush(GetThumbColor(isEnabled, isHovered, isPressed)))
			{
				graphics.FillRectangle(brush, bounds.X + backgroundSize1, bounds.Y, thumbSize, bounds.Height);
			}
		}
	}
}
