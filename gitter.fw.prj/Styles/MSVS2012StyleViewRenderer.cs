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

namespace gitter.Framework.Controls;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Resources = gitter.Framework.Properties.Resources;

public class MSVS2012StyleViewRenderer : ViewRenderer
{
	#region Color Tables

	public interface IColorTable
	{
		Color BackgroundColor { get; }
		Color ViewHostTabsBackground { get; }

		Color DocTabsFooterActive { get; }
		Color DocTabsFooterNormal { get; }
		Color DocTabBackground { get; }
		Color DocTabBackgroundHover { get; }
		Color DocTabBackgroundSelectedActive { get; }
		Color DocTabBackgroundSelected { get; }

		Color ToolTabBackgroundActive { get; }
		Color ToolTabBackgroundHover { get; }
		Color ToolTabSeparator { get; }
		Color ToolTabForeground { get; }
		Color ToolTabForegroundActive { get; }
		Color ToolTabForegroundHover { get; }

		Color DockSideBackground { get; }
		Color DockSideTabOutline { get; }
		Color DockSideTabOutlineHover { get; }
		Color DockSideTabForeground { get; }
		Color DockSideTabForegroundHover { get; }

		Color ViewButtonPressedBackground { get; }
		Color ViewButtonHoverBackgroundActive { get; }
		Color ViewButtonHoverBackgroundInactive { get; }
		Color ViewButtonForeground { get; }

		Color ViewHostHeaderBackgroundNormal { get; }
		Color ViewHostHeaderBackgroundFocused { get; }
		Color ViewHostHeaderAccentNormal { get; }
		Color ViewHostHeaderAccentFocused { get; }

		Color ViewHostHeaderTextNormal { get; }
		Color ViewHostHeaderTextFocused { get; }

		Color DockMarkerBackground { get; }
		Color DockMarkerBorder { get; }
		Color DockMarkerButtonBackground { get; }
		Color DockMarkerButtonContentBorder { get; }
		Color DockMarkerButtonContentArrow { get; }
	}

	private sealed class DarkColorTable : IColorTable
	{
		private static readonly Color _BackgroundColor						= MSVS2012DarkColors.WORK_AREA;
		private static readonly Color _ViewHostTabsBackground				= MSVS2012DarkColors.WORK_AREA;

		private static readonly Color _DocTabsFooterActive					= Color.FromArgb(0, 122, 204);
		private static readonly Color _DocTabsFooterNormal					= Color.FromArgb(63, 63, 70);
		private static readonly Color _DocTabBackground						= Color.FromArgb(41, 57, 85);
		private static readonly Color _DocTabBackgroundHover				= Color.FromArgb(20, 151, 234);
		private static readonly Color _DocTabBackgroundSelectedActive		= Color.FromArgb(0, 122, 204);
		private static readonly Color _DocTabBackgroundSelected				= Color.FromArgb(63, 63, 70);

		private static readonly Color _ToolTabBackgroundActive				= Color.FromArgb(37, 37, 38);
		private static readonly Color _ToolTabBackgroundHover				= Color.FromArgb(62, 62, 64);
		private static readonly Color _ToolTabSeparator						= Color.FromArgb(63, 63, 70);
		private static readonly Color _ToolTabForeground					= Color.FromArgb(208, 208, 208);
		private static readonly Color _ToolTabForegroundActive				= Color.FromArgb(0, 151, 251);
		private static readonly Color _ToolTabForegroundHover				= Color.FromArgb(85, 170, 255);

		private static readonly Color _DockSideBackground					= MSVS2012DarkColors.WORK_AREA;
		private static readonly Color _DockSideTabOutline					= Color.FromArgb(63, 63, 70);
		private static readonly Color _DockSideTabOutlineHover				= Color.FromArgb(0, 122, 204);
		private static readonly Color _DockSideTabForeground				= Color.FromArgb(208, 208, 208);
		private static readonly Color _DockSideTabForegroundHover			= Color.FromArgb(0, 151, 251);

		private static readonly Color _ViewButtonPressedBackground			= Color.FromArgb(14, 97, 152);
		private static readonly Color _ViewButtonHoverBackgroundActive		= Color.FromArgb(82, 176, 239);
		private static readonly Color _ViewButtonHoverBackgroundInactive	= Color.FromArgb(57, 57, 57);
		private static readonly Color _ViewButtonForeground					= Color.FromArgb(255, 255, 255);

		private static readonly Color _ViewHostHeaderBackgroundNormal		= MSVS2012DarkColors.WORK_AREA;
		private static readonly Color _ViewHostHeaderBackgroundFocused		= Color.FromArgb(0, 122, 204);
		private static readonly Color _ViewHostHeaderAccentNormal			= Color.FromArgb(70, 70, 74);
		private static readonly Color _ViewHostHeaderAccentFocused			= Color.FromArgb(89, 168, 222);

		private static readonly Color _ViewHostHeaderTextNormal				= Color.FromArgb(208, 208, 208);
		private static readonly Color _ViewHostHeaderTextFocused			= Color.FromArgb(255, 255, 255);

		private static readonly Color _DockMarkerBackground					= Color.FromArgb(30, 30, 30);
		private static readonly Color _DockMarkerBorder						= Color.FromArgb(51, 51, 55);
		private static readonly Color _DockMarkerButtonBackground			= Color.FromArgb(37, 37, 38);
		private static readonly Color _DockMarkerButtonContentBorder		= Color.FromArgb(0, 122, 204);
		private static readonly Color _DockMarkerButtonContentArrow			= Color.FromArgb(241, 241, 241);

		#region IColorTable

		public Color BackgroundColor => _BackgroundColor;
		public Color ViewHostTabsBackground => _ViewHostTabsBackground;

		public Color DocTabsFooterActive => _DocTabsFooterActive;
		public Color DocTabsFooterNormal => _DocTabsFooterNormal;
		public Color DocTabBackground => _DocTabBackground;
		public Color DocTabBackgroundHover => _DocTabBackgroundHover;
		public Color DocTabBackgroundSelectedActive => _DocTabBackgroundSelectedActive;
		public Color DocTabBackgroundSelected => _DocTabBackgroundSelected;

		public Color ToolTabBackgroundActive => _ToolTabBackgroundActive;
		public Color ToolTabBackgroundHover => _ToolTabBackgroundHover;
		public Color ToolTabSeparator => _ToolTabSeparator;
		public Color ToolTabForeground => _ToolTabForeground;
		public Color ToolTabForegroundActive => _ToolTabForegroundActive;
		public Color ToolTabForegroundHover => _ToolTabForegroundHover;

		public Color DockSideBackground => _DockSideBackground;
		public Color DockSideTabOutline => _DockSideTabOutline;
		public Color DockSideTabOutlineHover => _DockSideTabOutlineHover;
		public Color DockSideTabForeground => _DockSideTabForeground;
		public Color DockSideTabForegroundHover => _DockSideTabForegroundHover;

		public Color ViewButtonPressedBackground => _ViewButtonPressedBackground;
		public Color ViewButtonHoverBackgroundActive => _ViewButtonHoverBackgroundActive;
		public Color ViewButtonHoverBackgroundInactive => _ViewButtonHoverBackgroundInactive;
		public Color ViewButtonForeground => _ViewButtonForeground;

		public Color ViewHostHeaderBackgroundNormal => _ViewHostHeaderBackgroundNormal;
		public Color ViewHostHeaderBackgroundFocused => _ViewHostHeaderBackgroundFocused;
		public Color ViewHostHeaderAccentNormal => _ViewHostHeaderAccentNormal;
		public Color ViewHostHeaderAccentFocused => _ViewHostHeaderAccentFocused;

		public Color ViewHostHeaderTextNormal => _ViewHostHeaderTextNormal;
		public Color ViewHostHeaderTextFocused => _ViewHostHeaderTextFocused;

		public Color DockMarkerBackground => _DockMarkerBackground;
		public Color DockMarkerBorder => _DockMarkerBorder;
		public Color DockMarkerButtonBackground => _DockMarkerBackground;
		public Color DockMarkerButtonContentBorder => _DockMarkerButtonContentBorder;
		public Color DockMarkerButtonContentArrow => _DockMarkerButtonContentArrow;

		#endregion
	}

	private sealed class LightColorTable : IColorTable
	{
		private static readonly Color _BackgroundColor						= MSVS2012LightColors.WORK_AREA;
		private static readonly Color _ViewHostTabsBackground				= MSVS2012LightColors.WORK_AREA;

		private static readonly Color _DocTabsFooterActive					= Color.FromArgb(0, 122, 204);
		private static readonly Color _DocTabsFooterNormal					= Color.FromArgb(63, 63, 70);
		private static readonly Color _DocTabBackground						= Color.FromArgb(41, 57, 85);
		private static readonly Color _DocTabBackgroundHover				= Color.FromArgb(20, 151, 234);
		private static readonly Color _DocTabBackgroundSelectedActive		= Color.FromArgb(0, 122, 204);
		private static readonly Color _DocTabBackgroundSelected				= Color.FromArgb(63, 63, 70);

		private static readonly Color _ToolTabBackgroundActive				= Color.FromArgb(37, 37, 38);
		private static readonly Color _ToolTabBackgroundHover				= Color.FromArgb(62, 62, 64);
		private static readonly Color _ToolTabSeparator						= Color.FromArgb(63, 63, 70);
		private static readonly Color _ToolTabForeground					= Color.FromArgb(208, 208, 208);
		private static readonly Color _ToolTabForegroundActive				= Color.FromArgb(0, 151, 251);
		private static readonly Color _ToolTabForegroundHover				= Color.FromArgb(85, 170, 255);

		private static readonly Color _DockSideBackground					= MSVS2012LightColors.WORK_AREA;
		private static readonly Color _DockSideTabOutline					= Color.FromArgb(63, 63, 70);
		private static readonly Color _DockSideTabOutlineHover				= Color.FromArgb(0, 122, 204);
		private static readonly Color _DockSideTabForeground				= Color.FromArgb(208, 208, 208);
		private static readonly Color _DockSideTabForegroundHover			= Color.FromArgb(0, 151, 251);

		private static readonly Color _ViewButtonPressedBackground			= Color.FromArgb(14, 97, 152);
		private static readonly Color _ViewButtonHoverBackgroundActive		= Color.FromArgb(82, 176, 239);
		private static readonly Color _ViewButtonHoverBackgroundInactive	= Color.FromArgb(57, 57, 57);
		private static readonly Color _ViewButtonForeground					= Color.FromArgb(255, 255, 255);

		private static readonly Color _ViewHostHeaderBackgroundNormal		= MSVS2012LightColors.WORK_AREA;
		private static readonly Color _ViewHostHeaderBackgroundFocused		= Color.FromArgb(0, 122, 204);
		private static readonly Color _ViewHostHeaderAccentNormal			= Color.FromArgb(70, 70, 74);
		private static readonly Color _ViewHostHeaderAccentFocused			= Color.FromArgb(89, 168, 222);

		private static readonly Color _ViewHostHeaderTextNormal				= Color.FromArgb(208, 208, 208);
		private static readonly Color _ViewHostHeaderTextFocused			= Color.FromArgb(255, 255, 255);

		private static readonly Color _DockMarkerBackground					= Color.FromArgb(30, 30, 30);
		private static readonly Color _DockMarkerBorder						= Color.FromArgb(51, 51, 55);
		private static readonly Color _DockMarkerButtonBackground			= Color.FromArgb(37, 37, 38);
		private static readonly Color _DockMarkerButtonContentBorder		= Color.FromArgb(0, 122, 204);
		private static readonly Color _DockMarkerButtonContentArrow			= Color.FromArgb(241, 241, 241);

		#region IColorTable

		public Color BackgroundColor => _BackgroundColor;
		public Color ViewHostTabsBackground => _ViewHostTabsBackground;

		public Color DocTabsFooterActive => _DocTabsFooterActive;
		public Color DocTabsFooterNormal => _DocTabsFooterNormal;
		public Color DocTabBackground => _DocTabBackground;
		public Color DocTabBackgroundHover => _DocTabBackgroundHover;
		public Color DocTabBackgroundSelectedActive => _DocTabBackgroundSelectedActive;
		public Color DocTabBackgroundSelected => _DocTabBackgroundSelected;

		public Color ToolTabBackgroundActive => _ToolTabBackgroundActive;
		public Color ToolTabBackgroundHover => _ToolTabBackgroundHover;
		public Color ToolTabSeparator => _ToolTabSeparator;
		public Color ToolTabForeground => _ToolTabForeground;
		public Color ToolTabForegroundActive => _ToolTabForegroundActive;
		public Color ToolTabForegroundHover => _ToolTabForegroundHover;

		public Color DockSideBackground => _DockSideBackground;
		public Color DockSideTabOutline => _DockSideTabOutline;
		public Color DockSideTabOutlineHover => _DockSideTabOutlineHover;
		public Color DockSideTabForeground => _DockSideTabForeground;
		public Color DockSideTabForegroundHover => _DockSideTabForegroundHover;

		public Color ViewButtonPressedBackground => _ViewButtonPressedBackground;
		public Color ViewButtonHoverBackgroundActive => _ViewButtonHoverBackgroundActive;
		public Color ViewButtonHoverBackgroundInactive => _ViewButtonHoverBackgroundInactive;
		public Color ViewButtonForeground => _ViewButtonForeground;

		public Color ViewHostHeaderBackgroundNormal => _ViewHostHeaderBackgroundNormal;
		public Color ViewHostHeaderBackgroundFocused => _ViewHostHeaderBackgroundFocused;
		public Color ViewHostHeaderAccentNormal => _ViewHostHeaderAccentNormal;
		public Color ViewHostHeaderAccentFocused => _ViewHostHeaderAccentFocused;

		public Color ViewHostHeaderTextNormal => _ViewHostHeaderTextNormal;
		public Color ViewHostHeaderTextFocused => _ViewHostHeaderTextFocused;

		public Color DockMarkerBackground => _DockMarkerBackground;
		public Color DockMarkerBorder => _DockMarkerBorder;
		public Color DockMarkerButtonBackground => _DockMarkerBackground;
		public Color DockMarkerButtonContentBorder => _DockMarkerButtonContentBorder;
		public Color DockMarkerButtonContentArrow => _DockMarkerButtonContentArrow;

		#endregion
	}

	private static IColorTable _darkColors;
	private static IColorTable _lightColors;

	public static IColorTable DarkColors => _darkColors ??= new DarkColorTable();

	public static IColorTable LightColors => _lightColors ??= new LightColorTable();

	#endregion

	#region Constants

	private static class Constants
	{
		public static readonly int TabHeight        = 16 + 3;
		public static readonly int HeaderHeight     = 16 + 5;
		public static readonly int ViewButtonSize   = 16 - 1;
		public static readonly int FloatTitleHeight = SystemInformation.SmallIconSize.Height;
		public const int TabFooterHeight	= 2;
		public const int FooterHeight		= 0;
		public const int SideTabSpacing		= 12;
		public const int SideTabHeight		= 25;
		public const int SideTabOutline		= 6;
		public const int FloatBorderSize	= 1;
		public const int FloatCornerRadius	= 0;
	}

	#endregion

	#region Data

	private readonly IColorTable _colorTable;

	#endregion

	#region .ctor

	public MSVS2012StyleViewRenderer(IColorTable colorTable)
	{
		Verify.Argument.IsNotNull(colorTable);

		_colorTable = colorTable;
	}

	#endregion

	private IColorTable ColorTable => _colorTable;

	public override Color AccentColor => ColorTable.DocTabBackgroundSelectedActive;

	public override Color BackgroundColor => ColorTable.BackgroundColor;

	public override Color DockMarkerBackgroundColor => ColorTable.DockMarkerBackground;

	public override Color DockMarkerBorderColor => ColorTable.DockMarkerBorder;

	public override IDpiBoundValue<int> TabHeight { get; } = DpiBoundValue.ScaleY(Constants.TabHeight);

	public override IDpiBoundValue<int> TabFooterHeight { get; } = DpiBoundValue.ScaleY(Constants.TabFooterHeight);

	public override IDpiBoundValue<int> HeaderHeight { get; } = DpiBoundValue.ScaleY(Constants.HeaderHeight);

	public override IDpiBoundValue<int> FooterHeight { get; } = DpiBoundValue.ScaleY(Constants.FooterHeight);

	public override IDpiBoundValue<int> ViewButtonSize { get; } = DpiBoundValue.ScaleY(Constants.ViewButtonSize);

	public override int SideTabSpacing => Constants.SideTabSpacing;

	public override IDpiBoundValue<Size> SideTabSize { get; } = DpiBoundValue.Size(new(Constants.SideTabHeight, Constants.SideTabHeight));

	public override IDpiBoundValue<Size> FloatBorderSize { get; } = DpiBoundValue.Size(new(Constants.FloatBorderSize, Constants.FloatBorderSize));

	public override IDpiBoundValue<SizeF> FloatCornerRadius { get; } = DpiBoundValue.Size(new SizeF(Constants.FloatCornerRadius, Constants.FloatCornerRadius));

	public override IDpiBoundValue<int> FloatTitleHeight { get; } = DpiBoundValue.ScaleY(Constants.FloatTitleHeight);

	#region Tabs Rendering

	protected static readonly StringFormat NormalStringFormat = new(StringFormat.GenericTypographic)
	{
		LineAlignment = StringAlignment.Center,
		Trimming = StringTrimming.EllipsisCharacter,
	};

	protected static readonly StringFormat VerticalStringFormat = new(NormalStringFormat)
	{
		FormatFlags = StringFormatFlags.DirectionVertical,
	};

	private static int MeasureTabLength(ViewTabBase tab, Graphics graphics)
	{
		var dpi    = Dpi.FromControl(tab.ViewHost);
		var conv   = DpiConverter.FromDefaultTo(dpi);
		var font   = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
		var length = GitterApplication.TextRenderer.MeasureText(
			graphics, tab.Text, font, int.MaxValue, NormalStringFormat).Width;
		if(tab.ImageProvider?.GetImage(conv.ConvertX(16)) is not null)
		{
			length += conv.ConvertX(16) + conv.ConvertX(ViewConstants.ImageSpacing);
		}
		length += conv.ConvertX(ViewConstants.BeforeTabContent) + conv.ConvertX(ViewConstants.AfterTabContent);
		return length;
	}

	public override int MeasureViewDockSideTabLength(DockPanelSideTab tab, Graphics graphics)
	{
		return MeasureTabLength(tab, graphics);
	}

	public override int MeasureViewHostTabLength(ViewHostTab tab, Graphics graphics)
	{
		var length = MeasureTabLength(tab, graphics);
		length += tab.Buttons.Width;
		return length;
	}

	private static void RenderTabContent(ViewTabBase tab, Rectangle bounds, Graphics graphics, Color foregroundColor)
	{
		int x = bounds.X;
		int y = bounds.Y;
		int w = bounds.Width;
		int h = bounds.Height;
		Rectangle imageRect;
		StringFormat stringFormat;
		var dpi      = Dpi.FromControl(tab.ViewHost);
		var conv     = DpiConverter.FromDefaultTo(dpi);
		var iconSize = conv.Convert(new Size(16, 16));
		switch(tab.Anchor)
		{
			case AnchorStyles.Right or AnchorStyles.Left:
				imageRect = new Rectangle(x + (w - iconSize.Width) / 2, y + conv.ConvertY(3), iconSize.Height, iconSize.Width);
				stringFormat = VerticalStringFormat;
				break;
			case AnchorStyles.Top or AnchorStyles.Bottom:
				imageRect = new Rectangle(x + conv.ConvertX(3), y + (h - iconSize.Height) / 2, iconSize.Width, iconSize.Height);
				stringFormat = NormalStringFormat;
				break;
			default:
				throw new ApplicationException();
		}
		var image = tab.ImageProvider?.GetImage(iconSize.Width);
		if(image is not null)
		{
			switch(tab.Orientation)
			{
				case Orientation.Horizontal:
					graphics.DrawImage(image, imageRect);
					var dx = imageRect.Width + conv.ConvertX(3);
					bounds.Width -= dx;
					bounds.X     += dx;
					break;
				case Orientation.Vertical:
					using(var rotatedImage = (Image)image.Clone())
					{
						rotatedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
						graphics.DrawImage(rotatedImage, imageRect);
					}
					var dy = imageRect.Height + conv.ConvertY(3);
					bounds.Height -= dy;
					bounds.Y      += dy;
					break;
				default:
					throw new ApplicationException($"Unexpected ViewTabBase.Orientation value: {tab.Orientation}");
			}
		}
		var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
		switch(tab.Orientation)
		{
			case Orientation.Horizontal:
				bounds.X     += conv.ConvertX(ViewConstants.BeforeTabContent);
				bounds.Width -= conv.ConvertX(ViewConstants.BeforeTabContent) + conv.ConvertX(ViewConstants.AfterTabContent) - 1;
				GitterApplication.TextRenderer.DrawText(
					graphics, tab.Text, font, foregroundColor, bounds, stringFormat);
				break;
			case Orientation.Vertical:
				bounds.Y      += conv.ConvertY(ViewConstants.BeforeTabContent);
				bounds.Height -= conv.ConvertY(ViewConstants.BeforeTabContent) + conv.ConvertY(ViewConstants.AfterTabContent) - 1;
				bounds.Height += 10;
				GitterApplication.GdiPlusTextRenderer.DrawText(
					graphics, tab.Text, font, foregroundColor, bounds, stringFormat);
				break;
			default:
				throw new ApplicationException($"Unexpected ViewTabBase.Orientation value: {tab.Orientation}");
		}
	}

	public override void RenderViewDockSideTabBackground(DockPanelSideTab tab, Graphics graphics, Rectangle bounds)
	{
		var rcOutline = bounds;
		switch(tab.Side.Side)
		{
			case AnchorStyles.Left:
				rcOutline.Width = Constants.SideTabOutline;
				break;
			case AnchorStyles.Top:
				rcOutline.Height = Constants.SideTabOutline;
				break;
			case AnchorStyles.Right:
				rcOutline.X = rcOutline.Right - Constants.SideTabOutline;
				rcOutline.Width = Constants.SideTabOutline;
				break;
			case AnchorStyles.Bottom:
				rcOutline.Y = rcOutline.Bottom - Constants.SideTabOutline;
				rcOutline.Height  = Constants.SideTabOutline;
				break;
			default:
				throw new ApplicationException($"Unexpected DockSide.Side value: {tab.Side.Anchor}");
		}
		var color = tab.IsMouseOver ?
			ColorTable.DockSideTabOutlineHover :
			ColorTable.DockSideTabOutline;
		graphics.GdiFill(color, rcOutline);
	}

	public override void RenderViewDockSideTabContent(DockPanelSideTab tab, Graphics graphics, Rectangle bounds)
	{
		switch(tab.Side.Side)
		{
			case AnchorStyles.Left:
				bounds.X += Constants.SideTabOutline;
				bounds.Width -= Constants.SideTabOutline;
				break;
			case AnchorStyles.Top:
				bounds.Y += Constants.SideTabOutline;
				bounds.Height -= Constants.SideTabOutline;
				break;
			case AnchorStyles.Right:
				bounds.Width -= Constants.SideTabOutline;
				break;
			case AnchorStyles.Bottom:
				bounds.Height -= Constants.SideTabOutline;
				break;
			default:
				throw new ApplicationException(
					string.Format("Unexpected DockSide.Side value: {0}", tab.Side.Anchor));
		}
		var color = tab.IsMouseOver ?
			ColorTable.DockSideTabForegroundHover :
			ColorTable.DockSideTabForeground;
		RenderTabContent(tab, bounds, graphics, color);
	}

	public override void RenderViewHostTabBackground(ViewHostTab tab, Graphics graphics, Rectangle bounds)
	{
		var host = tab.View.Host;
		if(host.IsDocumentWell)
		{
			if(tab.IsActive)
			{
				var color = host.IsActive
					? ColorTable.DocTabBackgroundSelectedActive
					: ColorTable.DocTabBackgroundSelected;
				graphics.GdiFill(color, bounds);
			}
			else if(tab.IsMouseOver)
			{
				graphics.GdiFill(ColorTable.DocTabBackgroundHover, bounds);
			}
		}
		else
		{
			if(tab.IsActive)
			{
				graphics.GdiFill(ColorTable.ToolTabBackgroundActive, bounds);
			}
			else
			{
				if(tab.IsMouseOver)
				{
					graphics.GdiFill(ColorTable.ToolTabBackgroundHover, bounds);
				}
			}
			if(tab.Tabs[0] != tab)
			{
				using var pen = new Pen(ColorTable.ToolTabSeparator);
				graphics.DrawLine(pen, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
			}
		}
	}

	public override void RenderViewHostTabContent(ViewHostTab tab, Graphics graphics, Rectangle bounds)
	{
		Assert.IsNotNull(tab);
		Assert.IsNotNull(graphics);

		if(tab.View.Host.IsDocumentWell)
		{
			RenderTabContent(tab, bounds, graphics, Color.White);
		}
		else
		{
			Color foregroundColor;
			if(tab.IsActive)
			{
				foregroundColor = ColorTable.ToolTabForegroundActive;
			}
			else if(tab.IsMouseOver)
			{
				foregroundColor = ColorTable.ToolTabForegroundHover;
			}
			else
			{
				foregroundColor = ColorTable.ToolTabForeground;
			}
			RenderTabContent(tab, bounds, graphics, foregroundColor);
		}
		if(tab.Buttons is { Count: > 0 })
		{
			var buttonsBounds = new Rectangle(bounds.Right - tab.Buttons.Width - 2, 0, tab.Buttons.Width, bounds.Height);
			tab.Buttons.OnPaint(graphics, buttonsBounds, !tab.IsActive || tab.View.Host.IsActive);
		}
	}

	public override void RenderViewHostTabsBackground(ViewHostTabs tabs, PaintEventArgs e)
	{
		Assert.IsNotNull(tabs);
		Assert.IsNotNull(e);

		var graphics = e.Graphics;

		using var gdi = graphics.AsGdi();
		gdi.Fill(ColorTable.ViewHostTabsBackground, e.ClipRectangle);
		if(tabs.ViewHost.IsDocumentWell)
		{
			var dpi = Dpi.FromControl(tabs);
			var h1 = TabHeight.GetValue(dpi);
			var h2 = TabFooterHeight.GetValue(dpi);
			var rc = new Rectangle(0, h1, tabs.Width, h2);
			if(Rectangle.Intersect(e.ClipRectangle, rc) is { Width: > 0, Height: > 0 } clip)
			{
				var tabsFooterColor = tabs.ViewHost.IsActive
					? ColorTable.DocTabsFooterActive
					: ColorTable.DocTabsFooterNormal;
				gdi.Fill(tabsFooterColor, clip);
			}
		}
	}

	#endregion

	#region View Button rendering

	public override void RenderViewButton(ViewButton viewButton, Graphics graphics, Dpi dpi, Rectangle bounds, bool focus, bool hover, bool pressed)
	{
		if(hover || pressed)
		{
			var rc = bounds;
			rc.Width -= 1;
			rc.Height -= 1;
			var backgroundColor = pressed ?
				ColorTable.ViewButtonPressedBackground :
				focus ? ColorTable.ViewButtonHoverBackgroundActive : ColorTable.ViewButtonHoverBackgroundInactive;
			graphics.GdiFill(backgroundColor, rc);
		}
		switch(viewButton.Type)
		{
			case ViewButtonType.TabsMenu:
				{
					int x = bounds.X + (bounds.Width - 8) / 2;
					int y = bounds.Y + (bounds.Height - 8) / 2;
					using(var brush = new SolidBrush(ColorTable.ViewButtonForeground))
					{
						graphics.FillPolygon(brush, new[]
							{
								new Point(x + 1, y + 4),
								new Point(x + 7, y + 4),
								new Point(x + 4, y + 7),
								new Point(x + 3, y + 7),
							});
					}
					using(var pen = new Pen(Color.FromArgb(120, ColorTable.ViewButtonForeground)))
					{
						graphics.DrawLine(pen, x + 0, y + 4, x + 3, y + 7);
						graphics.DrawLine(pen, x + 4, y + 7, x + 7, y + 4);
					}
				}
				break;
			case ViewButtonType.TabsScrollMenu:
				{
					int x = bounds.X + (bounds.Width - 8) / 2;
					int y = bounds.Y + (bounds.Height - 8) / 2;
					using(var brush = new SolidBrush(ColorTable.ViewButtonForeground))
					{
						graphics.FillRectangle(brush, x, y, 8, 2);
						graphics.FillPolygon(brush, new[]
							{
								new Point(x + 1, y + 4),
								new Point(x + 7, y + 4),
								new Point(x + 4, y + 7),
								new Point(x + 3, y + 7),
							});
					}
					using(var pen = new Pen(Color.FromArgb(120, ColorTable.ViewButtonForeground)))
					{
						graphics.DrawLine(pen, x + 0, y + 4, x + 3, y + 7);
						graphics.DrawLine(pen, x + 4, y + 7, x + 7, y + 4);
					}
				}
				break;
			case ViewButtonType.ScrollTabsLeft:
				{
					using(var brush = new SolidBrush(ColorTable.ViewButtonForeground))
					{
						var p1 = new Point(bounds.X + (bounds.Width - 5) / 2, bounds.Y + bounds.Height / 2);
						var p2 = new Point(p1.X + 5, p1.Y - 5);
						var p3 = new Point(p1.X + 5, p1.Y + 5);
						var triangle = new[] { p1, p2, p3 };
						graphics.FillPolygon(brush, triangle);
					}
				}
				break;
			case ViewButtonType.ScrollTabsRight:
				{
					using(var brush = new SolidBrush(ColorTable.ViewButtonForeground))
					{
						var p1 = new Point(bounds.X + bounds.Width - (bounds.Width - 5) / 2, bounds.Y + bounds.Height / 2);
						var p2 = new Point(p1.X - 5, p1.Y - 5);
						var p3 = new Point(p1.X - 5, p1.Y + 5);
						var triangle = new[] { p1, p2, p3 };
						graphics.FillPolygon(brush, triangle);
					}
				}
				break;
			case ViewButtonType.Normalize:
				{
					int x = bounds.X + (bounds.Width - 10) / 2;
					int y = bounds.Y + (bounds.Height - 10) / 2;
					using(var pen = new Pen(ColorTable.ViewButtonForeground))
					{
						graphics.DrawRectangle(pen, x + 0, y + 3, 6, 6);
						graphics.DrawLine(pen, x + 1, y + 4, x + 5, y + 4);
						graphics.DrawLine(pen, x + 3, y + 0, x + 3, y + 2);
						graphics.DrawLine(pen, x + 9, y + 0, x + 9, y + 6);
						graphics.DrawLine(pen, x + 9, y + 0, x + 9, y + 6);
						graphics.DrawLine(pen, x + 4, y + 0, x + 8, y + 0);
						graphics.DrawLine(pen, x + 4, y + 1, x + 8, y + 1);
						graphics.DrawLine(pen, x + 7, y + 6, x + 8, y + 6);
					}
				}
				break;
			case ViewButtonType.Maximize:
				{
					int x = bounds.X + (bounds.Width - 9) / 2;
					int y = bounds.Y + (bounds.Height - 9) / 2;
					using(var pen = new Pen(ColorTable.ViewButtonForeground))
					{
						graphics.DrawRectangle(pen, x + 0, y + 0, 8, 8);
						graphics.DrawLine(pen, x + 1, y + 1, x + 7, y + 1);
						graphics.DrawLine(pen, x + 1, y + 2, x + 7, y + 2);
					}
				}
				break;
			case ViewButtonType.Unpin:
				{
					int x = bounds.X + (bounds.Width - 11) / 2;
					int y = bounds.Y + (bounds.Height - 11) / 2;
					using(var pen = new Pen(ColorTable.ViewButtonForeground))
					{
						graphics.DrawLine(pen, x + 3, y + 0, x + 7, y + 0);
						graphics.DrawLine(pen, x + 3, y + 1, x + 3, y + 5);
						graphics.DrawLine(pen, x + 6, y + 1, x + 6, y + 5);
						graphics.DrawLine(pen, x + 7, y + 1, x + 7, y + 5);
						graphics.DrawLine(pen, x + 2, y + 6, x + 8, y + 6);
						graphics.DrawLine(pen, x + 5, y + 7, x + 5, y + 10);
					}
				}
				break;
			case ViewButtonType.Pin:
				{
					int x = bounds.X + (bounds.Width - 11) / 2;
					int y = bounds.Y + (bounds.Height - 11) / 2;
					using(var pen = new Pen(ColorTable.ViewButtonForeground))
					{
						graphics.DrawLine(pen, x + 10, y + 3, x + 10, y + 7);
						graphics.DrawLine(pen, x + 9, y + 3, x + 5, y + 3);
						graphics.DrawLine(pen, x + 9, y + 6, x + 5, y + 6);
						graphics.DrawLine(pen, x + 9, y + 7, x + 5, y + 7);
						graphics.DrawLine(pen, x + 4, y + 2, x + 4, y + 8);
						graphics.DrawLine(pen, x + 3, y + 5, x + 0, y + 5);
					}
				}
				break;
			case ViewButtonType.Close:
				{
					var conv = DpiConverter.FromDefaultTo(dpi);
					var size = conv.Convert(new Size(10, 10));
					int x = bounds.X + (bounds.Width  - size.Width)  / 2;
					int y = bounds.Y + (bounds.Height - size.Height) / 2;
					using var pen = new Pen(ColorTable.ViewButtonForeground, conv.ConvertX(1.51f));
					using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
					{
						var x0 = x + conv.ConvertX(1);
						var y0 = y + conv.ConvertY(1);
						var x1 = x + conv.ConvertX(8);
						var y1 = y + conv.ConvertY(8);
						graphics.DrawLine(pen, x0, y0, x1, y1);
						graphics.DrawLine(pen, x1, y0, x0, y1);
					}
				}
				break;
			default:
				if(viewButton.Image is not null)
				{
					graphics.DrawImage(viewButton.Image, bounds);
				}
				break;
		}
	}

	#endregion

	#region View Host Footer Rendering

	public override void RenderViewHostFooter(ViewHostFooter footer, PaintEventArgs e)
	{
	}

	#endregion

	#region  View Host Header Rendering

	private static readonly StringFormat ViewHostHeaderTextFormat =
		new(StringFormat.GenericDefault)
		{
			Alignment = StringAlignment.Near,
			LineAlignment = StringAlignment.Center,
			FormatFlags =
				StringFormatFlags.FitBlackBox |
				StringFormatFlags.LineLimit,
			Trimming = StringTrimming.EllipsisCharacter,
		};

	public override void RenderViewHostHeader(ViewHostHeader header, PaintEventArgs e)
	{
		Assert.IsNotNull(header);
		Assert.IsNotNull(e);

		const int BetweenTextAndButtons = 2;
		const int BeforeContent = 2;

		var graphics = e.Graphics;
		var dpi      = Dpi.FromControl(header);
		var conv     = DpiConverter.FromDefaultTo(dpi);

		Color textColor, backgroundColor, accentColor;
		if(header.ViewHost.IsActive)
		{
			textColor		= ColorTable.ViewHostHeaderTextFocused;
			backgroundColor	= ColorTable.ViewHostHeaderBackgroundFocused;
			accentColor		= ColorTable.ViewHostHeaderAccentFocused;
		}
		else
		{
			textColor		= ColorTable.ViewHostHeaderTextNormal;
			backgroundColor	= ColorTable.ViewHostHeaderBackgroundNormal;
			accentColor		= ColorTable.ViewHostHeaderAccentNormal;
		}

		graphics.GdiFill(backgroundColor, e.ClipRectangle);

		int dx = conv.ConvertX(BeforeContent);

		var client = header.ClientRectangle;
		client.X     += dx;
		client.Width -= dx;
		if(header.Buttons.Count != 0)
		{
			client.Width -= header.Buttons.Width + conv.ConvertX(BetweenTextAndButtons);
		}
		if(client.Width > 1)
		{
			var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			graphics.TextRenderingHint = GraphicsUtility.TextRenderingHint;
			graphics.TextContrast      = GraphicsUtility.TextContrast;
			var textWidth = GitterApplication.TextRenderer.MeasureText(
				graphics,
				header.Text,
				font,
				client.Width).Width;
			GitterApplication.TextRenderer.DrawText(
				graphics,
				header.Text,
				font,
				textColor,
				client,
				ViewHostHeaderTextFormat);
			dx = textWidth + conv.ConvertX(6);
			client.X     += dx;
			client.Width -= dx;
			if(client.Width > 1)
			{
				int accentHeight = conv.ConvertY(5);
				client.Y      = (client.Height - accentHeight) / 2;
				client.Height = accentHeight;

				using(var brush = new HatchBrush(HatchStyle.Percent20, accentColor, backgroundColor))
				{
					var ro = default(Point);
					try
					{
						ro = graphics.RenderingOrigin;
						graphics.RenderingOrigin = new Point(client.X % 4, client.Y % 4);
					}
					catch(NotImplementedException)
					{
					}
					graphics.FillRectangle(brush, client);
					try
					{
						graphics.RenderingOrigin = ro;
					}
					catch(NotImplementedException)
					{
					}
				}
			}
		}
	}

	#endregion

	#region View Dock Side Rendering

	public override void RenderViewDockSide(DockPanelSide side, PaintEventArgs e)
	{
		using var brush = SolidBrushCache.Get(ColorTable.DockSideBackground);
		e.Graphics.FillRectangle(brush, e.ClipRectangle);
	}

	#endregion

	#region Dock Marker Button

	private void PaintDockMarkerButtonBackground(Graphics graphics, Dpi dpi, Rectangle bounds, bool hover)
	{
		Color backgroundColor;
		if(hover)
		{
			backgroundColor = ColorTable.DockMarkerButtonBackground;
		}
		else
		{
			backgroundColor = Color.FromArgb((byte)(ViewConstants.OpacityNormal * 255),
				ColorTable.DockMarkerButtonBackground);
		}
		var oldMode = graphics.SmoothingMode;
		graphics.SmoothingMode = SmoothingMode.HighQuality;
		using(var brush = new SolidBrush(backgroundColor))
		{
			graphics.FillRoundedRectangle(brush, bounds, DpiConverter.FromDefaultTo(dpi).ConvertX(2.0f));
		}
		graphics.SmoothingMode = oldMode;
	}

	private void InitDockMarkerButtonContentColors(bool hover, out Color arrow, out Color content, out Color border)
	{
		if(hover)
		{
			arrow  = ColorTable.DockMarkerButtonContentArrow;
			border = ColorTable.DockMarkerButtonContentBorder;
		}
		else
		{
			var alpha = (byte)(ViewConstants.OpacityNormal * 255);
			arrow  = Color.FromArgb(alpha, ColorTable.DockMarkerButtonContentArrow);
			border = Color.FromArgb(alpha, ColorTable.DockMarkerButtonContentBorder);
		}
		content	= ColorTable.DockMarkerButtonBackground;
	}

	[ThreadStatic]
	private static readonly PointF[] _arrowPoints = new PointF[4];

	private static PointF[] SetupArrow(int x, int y, Size padding, Dpi dpi, Point p0, Point p1, Point p2, Point p3)
	{
		var points = _arrowPoints;
		x += padding.Width;
		y += padding.Height;
		var conv = DpiConverter.FromDefaultTo(dpi);
		points[0] = new PointF(x + conv.ConvertX(p0.X) + 0.5f, y + conv.ConvertY(p0.Y) + 0.5f);
		points[1] = new PointF(x + conv.ConvertX(p1.X) + 0.5f, y + conv.ConvertY(p1.Y) + 0.5f);
		points[2] = new PointF(x + conv.ConvertX(p2.X) + 0.5f, y + conv.ConvertY(p2.Y) + 0.5f);
		points[3] = new PointF(x + conv.ConvertX(p3.X) + 0.5f, y + conv.ConvertY(p3.Y) + 0.5f);
		return points;
	}

	private void PaintDockMarkerTopButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out var arrowColor, out var contentColor, out var borderColor);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var padding = conv.Convert(new Size(4, 4));
		var rc = new Rectangle(rect.X + padding.Width, rect.Y + padding.Height, rect.Width - padding.Width * 2, (rect.Height - padding.Height * 2) / 2);
		var arrow = SetupArrow(rect.X, rect.Y, padding, dpi, new Point(7, 21), new Point(11, 17), new Point(12, 17), new Point(15, 21));
		using(var brush = SolidBrushCache.Get(borderColor))
		{
			graphics.FillRectangle(brush, rc);
		}
		using(var brush = SolidBrushCache.Get(arrowColor))
		{
			graphics.FillPolygon(brush, arrow);
		}
		var h = conv.ConvertY(3);
		var b = conv.ConvertX(1);
		rc.X += b;
		rc.Y += h;
		rc.Width  -= 2 * b;
		rc.Height -= h + b;
		using(var brush = SolidBrushCache.Get(contentColor))
		{
			graphics.FillRectangle(brush, rc);
		}
	}

	private void PaintDockMarkerDocumentTopButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out _, out var contentColor, out var borderColor);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var padding = conv.Convert(new Size(4, 4));
		var rc = new Rectangle(rect.X + padding.Width, rect.Y + padding.Height, rect.Width - padding.Width * 2, rect.Height - padding.Height * 2);
		using(var brush = SolidBrushCache.Get(borderColor))
		{
			graphics.FillRectangle(brush, rc);
		}
		var h = conv.ConvertY(3);
		var b = conv.ConvertX(1);
		rc.X += b;
		rc.Y += h;
		rc.Width  -= 2 * b;
		rc.Height -= h + b;
		using(var brush = SolidBrushCache.Get(contentColor))
		{
			graphics.FillRectangle(brush, rc);
		}
		rc.Height = conv.ConvertY(11);
		using var pen = new Pen(borderColor) { DashStyle = DashStyle.Dot };
		graphics.DrawLine(pen, rc.X, rc.Bottom - 1, rc.Right, rc.Bottom - 1);
	}

	private void PaintDockMarkerLeftButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out var arrowColor, out var contentColor, out var borderColor);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var padding = conv.Convert(new Size(4, 4));
		var rc = new Rectangle(rect.X + padding.Width, rect.Y + padding.Height, (rect.Width - padding.Width * 2) / 2, rect.Height - padding.Height * 2);
		var arrow = SetupArrow(rect.X, rect.Y, padding, dpi, new Point(21, 7), new Point(21, 15), new Point(17, 11), new Point(17, 11));
		using(var brush = SolidBrushCache.Get(arrowColor))
		{
			graphics.FillPolygon(brush, arrow);
		}
		using(var brush = SolidBrushCache.Get(borderColor))
		{
			graphics.FillRectangle(brush, rc);
		}
		var h = conv.ConvertY(3);
		var b = conv.ConvertX(1);
		rc.X += b;
		rc.Y += h;
		rc.Width  -= 2 * b;
		rc.Height -= h + b;
		using(var brush = SolidBrushCache.Get(contentColor))
		{
			graphics.FillRectangle(brush, rc);
		}
	}

	private void PaintDockMarkerDocumentLeftButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out _, out var contentColor, out var borderColor);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var padding = conv.Convert(new Size(4, 4));
		var rc = new Rectangle(rect.X + padding.Width, rect.Y + padding.Height, rect.Width - padding.Width * 2, rect.Height - padding.Height * 2);
		using(var brush = new SolidBrush(borderColor))
		{
			graphics.FillRectangle(brush, rc);
		}
		var h = conv.ConvertY(3);
		var b = conv.ConvertX(1);
		rc.X += b;
		rc.Y += h;
		rc.Width  -= 2 * b;
		rc.Height -= h + b;
		using(var brush = SolidBrushCache.Get(contentColor))
		{
			graphics.FillRectangle(brush, rc);
		}
		rc.Width = conv.ConvertX(11);
		using(var pen = new Pen(borderColor)
		{
			DashStyle = DashStyle.Dot,
		})
		{
			graphics.DrawLine(pen, rc.Right - 1, rc.Y, rc.Right - 1, rc.Bottom);
		}
	}

	private void PaintDockMarkerFillButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out _, out var contentColor, out var borderColor);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var padding = conv.Convert(new Size(4, 4));
		var rc = new Rectangle(rect.X + padding.Width, rect.Y + padding.Height, rect.Width - padding.Width * 2, rect.Height - padding.Height * 2);
		using(var brush = SolidBrushCache.Get(borderColor))
		{
			graphics.FillRectangle(brush, rc);
		}
		var h = conv.ConvertY(3);
		var b = conv.ConvertX(1);
		rc.X += b;
		rc.Y += h;
		rc.Width  -= 2 * b;
		rc.Height -= h + b;
		using(var brush = SolidBrushCache.Get(contentColor))
		{
			graphics.FillRectangle(brush, rc);
		}
	}

	private void PaintDockMarkerRightButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out var arrowColor, out var contentColor, out var borderColor);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var padding = conv.Convert(new Size(4, 4));
		var rw = (rect.Width - padding.Width * 2) / 2;
		var rc = new Rectangle(rect.X + padding.Width + rw, rect.Y + padding.Height, rw, rect.Height - padding.Height * 2);
		var arrow = SetupArrow(rect.X, rect.Y, padding, dpi, new Point(1, 16), new Point(1, 7), new Point(5, 11), new Point(5, 12));
		using(var brush = new SolidBrush(borderColor))
		{
			graphics.FillRectangle(brush, rc);
		}
		using(var brush = SolidBrushCache.Get(arrowColor))
		{
			graphics.FillPolygon(brush, arrow);
		}
		var h = conv.ConvertY(3);
		var b = conv.ConvertX(1);
		rc.X += b;
		rc.Y += h;
		rc.Width  -= 2 * b;
		rc.Height -= h + b;
		using(var brush = SolidBrushCache.Get(contentColor))
		{
			graphics.FillRectangle(brush, rc);
		}
	}

	private void PaintDockMarkerDocumentRightButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out _, out var contentColor, out var borderColor);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var padding = conv.Convert(new Size(4, 4));
		var rc = new Rectangle(rect.X + padding.Width, rect.Y + padding.Height, rect.Width - padding.Width * 2, rect.Height - padding.Height * 2);
		using(var brush = new SolidBrush(borderColor))
		{
			graphics.FillRectangle(brush, rc);
		}
		var h = conv.ConvertY(3);
		var b = conv.ConvertX(1);
		rc.X += b;
		rc.Y += h;
		rc.Width  -= 2 * b;
		rc.Height -= h + b;
		using(var brush = SolidBrushCache.Get(contentColor))
		{
			graphics.FillRectangle(brush, rc);
		}
		var dx = conv.ConvertX(11);
		rc.X    += dx;
		rc.Width = dx;
		using(var pen = new Pen(borderColor, conv.ConvertX(1.0f))
		{
			DashStyle = DashStyle.Dot,
		})
		{
			graphics.DrawLine(pen, rc.X, rc.Y, rc.X, rc.Bottom);
		}
	}

	private void PaintDockMarkerBottomButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out var arrowColor, out var contentColor, out var borderColor);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var padding = conv.Convert(new Size(4, 4));
		var rh = (rect.Height - padding.Height * 2) / 2;
		var rc = new Rectangle(rect.X + padding.Width, rect.Y + rh, rect.Width - padding.Width * 2, rh);
		var arrow = SetupArrow(rect.X, rect.Y, padding, dpi, new Point(7, 1), new Point(11, 5), new Point(12, 5), new Point(16, 1));
		using(var brush = new SolidBrush(borderColor))
		{
			graphics.FillRectangle(brush, rc);
		}
		using(var brush = SolidBrushCache.Get(arrowColor))
		{
			graphics.FillPolygon(brush, arrow);
		}
		var h = conv.ConvertY(3);
		var b = conv.ConvertX(1);
		rc.X += b;
		rc.Y += h;
		rc.Width  -= 2 * b;
		rc.Height -= h + b;
		using(var brush = SolidBrushCache.Get(contentColor))
		{
			graphics.FillRectangle(brush, rc);
		}
	}

	private void PaintDockMarkerDocumentBottomButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out _, out var contentColor, out var borderColor);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var padding = conv.Convert(new Size(4, 4));
		var rc = new Rectangle(
			rect.X + padding.Width,
			rect.Y + padding.Height,
			rect.Width  - padding.Width  * 2,
			rect.Height - padding.Height * 2);
		using(var brush = SolidBrushCache.Get(borderColor))
		{
			graphics.FillRectangle(brush, rc);
		}
		var h = conv.ConvertY(3);
		var b = conv.ConvertX(1);
		rc.X += b;
		rc.Y += h;
		rc.Width  -= 2 * b;
		rc.Height -= h + b;
		using(var brush = SolidBrushCache.Get(contentColor))
		{
			graphics.FillRectangle(brush, rc);
		}
		rc.Y += conv.ConvertY(9);
		rc.Height = conv.ConvertY(11);
		using var pen = new Pen(borderColor, conv.ConvertX(1.0f)) { DashStyle = DashStyle.Dot };
		graphics.DrawLine(pen, rc.X, rc.Y, rc.Right, rc.Y);
	}


	private void PaintDockMarkerButtonContent(DockMarkerButton button, Graphics graphics, Dpi dpi, bool hover)
	{
		Assert.IsNotNull(button);

		switch(button.Type)
		{
			case DockResult.Top:
				PaintDockMarkerTopButton(graphics, dpi, button.Bounds, hover);
				break;
			case DockResult.DocumentTop:
				PaintDockMarkerDocumentTopButton(graphics, dpi, button.Bounds, hover);
				break;
			case DockResult.Left:
				PaintDockMarkerLeftButton(graphics, dpi, button.Bounds, hover);
				break;
			case DockResult.DocumentLeft:
				PaintDockMarkerDocumentLeftButton(graphics, dpi, button.Bounds, hover);
				break;
			case DockResult.Fill:
				PaintDockMarkerFillButton(graphics, dpi, button.Bounds, hover);
				break;
			case DockResult.Right:
				PaintDockMarkerRightButton(graphics, dpi, button.Bounds, hover);
				break;
			case DockResult.DocumentRight:
				PaintDockMarkerDocumentRightButton(graphics, dpi, button.Bounds, hover);
				break;
			case DockResult.Bottom:
				PaintDockMarkerBottomButton(graphics, dpi, button.Bounds, hover);
				break;
			case DockResult.DocumentBottom:
				PaintDockMarkerDocumentBottomButton(graphics, dpi, button.Bounds, hover);
				break;
		}
	}

	public override void RenderDockMarkerButton(DockMarkerButton button, Control host, Graphics graphics, bool hover)
	{
		var dpi = Dpi.FromControl(host);
		PaintDockMarkerButtonBackground(graphics, dpi, button.Bounds, hover);
		PaintDockMarkerButtonContent(button, graphics, dpi, hover);
	}

	#endregion

	#region Popup Notifications

	public override void RenderPopupNotificationHeader(PopupNotificationHeader header, PaintEventArgs e)
	{
		const int BetweenTextAndButtons = 2;
		const int BeforeContent = 2;

		var graphics = e.Graphics;
		var dpi      = Dpi.FromControl(header);
		var conv     = DpiConverter.FromDefaultTo(dpi);

		var textColor		= ColorTable.ViewHostHeaderTextFocused;
		var backgroundColor	= ColorTable.ViewHostHeaderBackgroundFocused;
		var accentColor		= ColorTable.ViewHostHeaderAccentFocused;

		using(var brush = new SolidBrush(backgroundColor))
		{
			graphics.FillRectangle(brush, e.ClipRectangle);
		}

		var client = header.ClientRectangle;
		client.X     += conv.ConvertX(BeforeContent);
		client.Width -= conv.ConvertX(BeforeContent);
		if(header.Buttons.Count != 0)
		{
			client.Width -= header.Buttons.Width + conv.ConvertX(BetweenTextAndButtons);
		}
		if(client.Width > 1)
		{
			var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			graphics.TextRenderingHint = GraphicsUtility.TextRenderingHint;
			graphics.TextContrast      = GraphicsUtility.TextContrast;
			var textWidth = GitterApplication.TextRenderer.MeasureText(
				graphics,
				header.Text,
				font,
				client.Width).Width;
			GitterApplication.TextRenderer.DrawText(
				graphics,
				header.Text,
				font,
				textColor,
				client,
				ViewHostHeaderTextFormat);
			client.X += textWidth + 6;
			client.Width -= textWidth + 6;
			if(client.Width > 1)
			{
				const int AccentHeight = 5;
				client.Y = (client.Height - AccentHeight) / 2;
				client.Height = AccentHeight;

				using(var brush = new HatchBrush(HatchStyle.Percent20, accentColor, backgroundColor))
				{
					graphics.RenderingOrigin = new Point(client.X % 4, client.Y % 4);
					graphics.FillRectangle(brush, client);
					graphics.RenderingOrigin = Point.Empty;
				}
			}
		}
	}

	#endregion
}
