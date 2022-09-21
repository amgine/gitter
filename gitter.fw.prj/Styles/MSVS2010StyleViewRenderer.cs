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
using System.Windows.Forms;

public class MSVS2010StyleViewRenderer : ViewRenderer
{
	private static Bitmap LoadBitmap(string name)
	{
		using var stream = typeof(MSVS2010StyleViewRenderer)
			.Assembly
			.GetManifestResourceStream(@"gitter.Framework.Resources.images." + name + ".png");
		if(stream is null) return default;
		return new Bitmap(stream);
	}

	private static readonly Lazy<Bitmap> ImgMenu        = new(() => LoadBitmap(@"arrow-small"));
	private static readonly Lazy<Bitmap> ImgNormalize   = new(() => LoadBitmap(@"normalize"));
	private static readonly Lazy<Bitmap> ImgMaximize    = new(() => LoadBitmap(@"maximize"));
	private static readonly Lazy<Bitmap> ImgPin         = new(() => LoadBitmap(@"pin-small"));
	private static readonly Lazy<Bitmap> ImgClose       = new(() => LoadBitmap(@"cross-small"));
	private static readonly Lazy<Bitmap> ImgScrollLeft  = new(() => LoadBitmap(@"tab-scroll-left"));
	private static readonly Lazy<Bitmap> ImgScrollRight = new(() => LoadBitmap(@"tab-scroll-right"));
	private static readonly Lazy<Bitmap> ImgTabMenu     = new(() => LoadBitmap(@"tab-menu"));
	private static readonly Lazy<Bitmap> ImgTabMenuExt  = new(() => LoadBitmap(@"tab-menu-extends"));

	protected static class ColorTable
	{
		public static readonly Color BackgroundColor = Color.FromArgb(41, 57, 85);
		public static readonly Color ViewHostTabsBackground = Color.FromArgb(41, 57, 85);

		public static readonly Color ViewHostTabsSelectedBackgroundActiveEnd = Color.FromArgb(255, 232, 166);
		public static readonly Color ViewHostTabsSelectedBackgroundNormalEnd = Color.FromArgb(174, 185, 205);

		public static readonly Color TabBackground = Color.FromArgb(41, 57, 85);

		public static readonly Color TabNormalBorder = Color.FromArgb(54, 78, 111);
		public static readonly Color TabNormalBackgroundStart = Color.FromArgb(77, 96, 130);
		public static readonly Color TabNormalBackgroundEnd = Color.FromArgb(62, 83, 120);

		public static readonly Color TabHoverBorder = Color.FromArgb(155, 167, 183);
		public static readonly Color TabHoverBackgroundStart = Color.FromArgb(111, 119, 118);
		public static readonly Color TabHoverBackgroundEnd = Color.FromArgb(77, 93, 116);

		public static readonly Color TabSelectedBackground = Color.White;

		public static readonly Color TabSelectedBackgroundActiveStart = Color.FromArgb(255, 252, 242);
		public static readonly Color TabSelectedBackgroundActiveEnd = Color.FromArgb(255, 232, 166);
		public static readonly Color TabSelectedBackgroundNormalStart = Color.FromArgb(202, 211, 226);
		public static readonly Color TabSelectedBackgroundNormalEnd = Color.FromArgb(174, 185, 205);

		public static readonly Color ViewButtonPressedBorder = Color.FromArgb(229, 195, 101);
		public static readonly Color ViewButtonPressedBackground = Color.FromArgb(255, 232, 166);

		public static readonly Color ViewButtonHoverBorder = Color.FromArgb(229, 195, 101);
		public static readonly Color ViewButtonHoverBackground = Color.FromArgb(255, 252, 244);

		public static readonly Color ViewHostFooterBackground = Color.FromArgb(41, 57, 85);
		public static readonly Color ViewHostForegroundNormal = Color.FromArgb(206, 212, 223);
		public static readonly Color ViewHostForegroundActive = Color.FromArgb(255, 232, 166);

		public static readonly Color ViewHostHeaderBackground = Color.FromArgb(47, 57, 85);

		public static readonly Color ViewHostHeaderBackgroundNormalStart = Color.FromArgb(77, 96, 130);
		public static readonly Color ViewHostHeaderBackgroundNormalEnd = Color.FromArgb(61, 82, 119);
		public static readonly Color ViewHostHeaderBackgroundFocusedStart = Color.FromArgb(255, 252, 242);
		public static readonly Color ViewHostHeaderBackgroundFocusedEnd = Color.FromArgb(255, 232, 166);

		public static readonly Color ViewHostHeaderTextNormal = Color.White;
		public static readonly Color ViewHostHeaderTextFocused = Color.Black;

		public static readonly Color ViewDockSideBackground = Color.FromArgb(41, 57, 85);

		public static readonly Color DockMarkerBackground = Color.FromArgb(255, 255, 255);
		public static readonly Color DockMarkerBorder = Color.FromArgb(99, 104, 113);
		public static readonly Color DockMarkerButtonBorder = Color.FromArgb(138, 145, 156);
		public static readonly Color DockMarkerButtonBackgroundStart = Color.FromArgb(245, 248, 251);
		public static readonly Color DockMarkerButtonBackgroundEnd = Color.FromArgb(222, 226, 233);

		public static readonly Color DockMarkerButtonContentBorder = Color.FromArgb(68, 88, 121);
		public static readonly Color DockMarkerButtonContentStart = Color.FromArgb(253, 231, 165);
		public static readonly Color DockMarkerButtonContentEnd = Color.FromArgb(247, 198, 113);
	}

	private static class Constants
	{
		public static readonly int TabHeight        = 16 + 4;
		public static readonly int HeaderHeight     = 16 + 3;
		public static readonly int ViewButtonSize   = 16 - 1;
		public static readonly int FloatTitleHeight = 16;
		public const int TabFooterHeight	= 4;
		public const int FooterHeight		= 4;
		public const int SideTabSpacing		= 1;
		public const int SideTabHeight		= 21;
		public const int FloatBorderSize	= 4;
		public const int FloatCornerRadius	= 3;
	}

	public override Color AccentColor => ColorTable.TabSelectedBackgroundActiveStart;

	public override Color BackgroundColor
	{
		get { return ColorTable.BackgroundColor; }
	}

	public override Color DockMarkerBackgroundColor
	{
		get { return ColorTable.DockMarkerBackground; }
	}

	public override Color DockMarkerBorderColor
	{
		get { return ColorTable.DockMarkerBorder; }
	}

	public override IDpiBoundValue<int> TabHeight { get; } = DpiBoundValue.ScaleY(Constants.TabHeight);

	public override IDpiBoundValue<int> TabFooterHeight { get; } = DpiBoundValue.ScaleY(Constants.TabFooterHeight);

	public override IDpiBoundValue<int> HeaderHeight { get; } = DpiBoundValue.ScaleY(Constants.HeaderHeight);

	public override IDpiBoundValue<int> FooterHeight { get; } = DpiBoundValue.ScaleY(Constants.FooterHeight);

	public override IDpiBoundValue<int> ViewButtonSize { get; } = DpiBoundValue.ScaleY(Constants.ViewButtonSize);

	public override int SideTabSpacing
	{
		get { return Constants.SideTabSpacing; }
	}

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
		var conv   = new DpiConverter(tab.ViewHost);
		var length = GitterApplication.TextRenderer.MeasureText(
			graphics, tab.Text, GitterApplication.FontManager.UIFont, int.MaxValue, NormalStringFormat).Width;
		if(tab.ImageProvider is not null)
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

	private static void RenderTabBackground(ViewTabBase tab, Rectangle bounds, Graphics graphics)
	{
		const int corner = 1;
		int x = bounds.X;
		int y = bounds.Y;
		int w = bounds.Width;
		int h = bounds.Height;
		var linePoints = new Point[6];
		var polyPoints = new Point[6];
		LinearGradientMode gradient;
		switch(tab.Anchor)
		{
			case AnchorStyles.Right:
				linePoints[0] = new Point(x, y);
				linePoints[1] = new Point(x + w - corner - 1, y);
				linePoints[2] = new Point(x + w - 1, y + corner);
				linePoints[3] = new Point(x + w - 1, y + h - corner - 1);
				linePoints[4] = new Point(x + w - corner - 1, y + h - 1);
				linePoints[5] = new Point(x, y + h - 1);
				polyPoints[0] = new Point(x, y);
				polyPoints[1] = new Point(x + w - corner - 1, y);
				polyPoints[2] = new Point(x + w - 1, y + corner);
				polyPoints[3] = new Point(x + w - 1, y + h - corner - 1);
				polyPoints[4] = new Point(x + w - corner - 1, y + h - 1);
				polyPoints[5] = new Point(x, y + h - 1);
				break;
			case AnchorStyles.Left:
				linePoints[0] = new Point(x + w - 1, y);
				linePoints[1] = new Point(x + corner, y);
				linePoints[2] = new Point(x, y + corner);
				linePoints[3] = new Point(x, y + h - corner - 1);
				linePoints[4] = new Point(x + corner, y + h - 1);
				linePoints[5] = new Point(x + w - 1, y + h - 1);
				polyPoints[0] = new Point(x + w - 1, y);
				polyPoints[1] = new Point(x + corner, y);
				polyPoints[2] = new Point(x, y + corner);
				polyPoints[3] = new Point(x, y + h - corner - 1);
				polyPoints[4] = new Point(x + corner, y + h - 1);
				polyPoints[5] = new Point(x + w - 1, y + h - 1);
				break;
			case AnchorStyles.Top:
				linePoints[0] = new Point(x, y + h - 1);
				linePoints[1] = new Point(x, y + corner);
				linePoints[2] = new Point(x + corner, y);
				linePoints[3] = new Point(x + w - corner - 1, y);
				linePoints[4] = new Point(x + w - 1, y + corner);
				linePoints[5] = new Point(x + w - 1, y + h - 1);
				polyPoints[0] = new Point(x, y + h);
				polyPoints[1] = new Point(x, y + corner);
				polyPoints[2] = new Point(x + corner, y);
				polyPoints[3] = new Point(x + w - corner, y);
				polyPoints[4] = new Point(x + w, y + corner);
				polyPoints[5] = new Point(x + w, y + h);
				break;
			case AnchorStyles.Bottom:
				linePoints[0] = new Point(x, y);
				linePoints[1] = new Point(x, y + h - corner - 1);
				linePoints[2] = new Point(x + corner, y + h - 1);
				linePoints[3] = new Point(x + w - corner - 1, y + h - 1);
				linePoints[4] = new Point(x + w - 1, y + h - corner - 1);
				linePoints[5] = new Point(x + w - 1, y);
				polyPoints[0] = new Point(x, y);
				polyPoints[1] = new Point(x, y + h - corner - 1);
				polyPoints[2] = new Point(x + corner + 1, y + h);
				polyPoints[3] = new Point(x + w - corner - 1, y + h);
				polyPoints[4] = new Point(x + w, y + h - corner - 1);
				polyPoints[5] = new Point(x + w, y);
				break;
			default:
				throw new ApplicationException();
		}
		switch(tab.Orientation)
		{
			case Orientation.Horizontal:
				gradient = LinearGradientMode.Vertical;
				break;
			case Orientation.Vertical:
				gradient = LinearGradientMode.Horizontal;
				break;
			default:
				throw new ApplicationException();
		}
		var host = tab.View.Host;
		if(tab.IsActive)
		{
			if(host.IsDocumentWell)
			{
				Color start, end;
				if(host.IsActive)
				{
					start = ColorTable.TabSelectedBackgroundActiveStart;
					end = ColorTable.TabSelectedBackgroundActiveEnd;
				}
				else
				{
					start = ColorTable.TabSelectedBackgroundNormalStart;
					end = ColorTable.TabSelectedBackgroundNormalEnd;
				}
				using(var brush = new LinearGradientBrush(
					bounds, start, end, gradient))
				{
					graphics.FillPolygon(brush, polyPoints);
				}
			}
			else
			{
				using(var brush = new SolidBrush(ColorTable.TabSelectedBackground))
				{
					graphics.FillPolygon(brush, polyPoints);
				}
			}
		}
		else if(tab.IsMouseOver)
		{
			using(var brush = new LinearGradientBrush(bounds,
				ColorTable.TabHoverBackgroundStart,
				ColorTable.TabHoverBackgroundEnd,
				gradient))
			{
				graphics.FillPolygon(brush, polyPoints);
			}
			using(var pen = new Pen(ColorTable.TabHoverBorder))
			{
				graphics.DrawLines(pen, linePoints);
			}
		}
		else
		{
			if(!host.IsDocumentWell)
			{
				using(var brush = new LinearGradientBrush(bounds,
					ColorTable.TabNormalBackgroundStart,
					ColorTable.TabNormalBackgroundEnd,
					gradient))
				{
					graphics.FillPolygon(brush, polyPoints);
				}
				using(var pen = new Pen(ColorTable.TabNormalBorder))
				{
					graphics.DrawLines(pen, linePoints);
				}
			}
		}
	}

	private static void RenderTabContent(ViewTabBase tab, Rectangle bounds, Graphics graphics)
	{
		var conv = new DpiConverter(tab.ViewHost);
		int x = bounds.X;
		int y = bounds.Y;
		int w = bounds.Width;
		int h = bounds.Height;
		Rectangle imageRect;
		StringFormat stringFormat;
		var iconSize = conv.Convert(new Size(16, 16));
		switch(tab.Anchor)
		{
			case AnchorStyles.Right or AnchorStyles.Left:
				imageRect = new Rectangle(x + (w - iconSize.Width) / 2, y + conv.ConvertY(3), iconSize.Width, iconSize.Height);
				stringFormat = VerticalStringFormat;
				break;
			case AnchorStyles.Top or AnchorStyles.Bottom:
				imageRect = new Rectangle(x + conv.ConvertX(3), y + (h - iconSize.Height) / 2, iconSize.Width, iconSize.Height);
				stringFormat = NormalStringFormat;
				break;
			default:
				throw new ApplicationException();
		}
		var textBrush = tab.IsActive ? Brushes.Black : Brushes.White;
		var image = tab.ImageProvider?.GetImage(iconSize.Width);
		if(image is not null)
		{
			switch(tab.Orientation)
			{
				case Orientation.Horizontal:
					{
						graphics.DrawImage(image, imageRect);
						var dx = imageRect.Width + conv.ConvertX(3);
						bounds.Width -= dx;
						bounds.X     += dx;
					}
					break;
				case Orientation.Vertical:
					using(var rotatedImage = (Image)image.Clone())
					{
						rotatedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
						graphics.DrawImage(rotatedImage, imageRect);
						var dy = imageRect.Height + conv.ConvertY(3);
						bounds.Height -= dy;
						bounds.Y      += dy;
					}
					break;
				default:
					throw new ApplicationException();
			}
		}
		switch(tab.Orientation)
		{
			case Orientation.Horizontal:
				bounds.X     += conv.ConvertX(ViewConstants.BeforeTabContent);
				bounds.Width -= conv.ConvertX(ViewConstants.BeforeTabContent) + conv.ConvertX(ViewConstants.AfterTabContent) - 1;
				GitterApplication.TextRenderer.DrawText(
					graphics, tab.Text, GitterApplication.FontManager.UIFont, textBrush, bounds, stringFormat);
				break;
			case Orientation.Vertical:
				bounds.Y      += conv.ConvertY(ViewConstants.BeforeTabContent);
				bounds.Height -= conv.ConvertY(ViewConstants.BeforeTabContent) + conv.ConvertY(ViewConstants.AfterTabContent) - 1;
				bounds.Height += 10;
				GitterApplication.GdiPlusTextRenderer.DrawText(
					graphics, tab.Text, GitterApplication.FontManager.UIFont, textBrush, bounds, stringFormat);
				break;
			default:
				throw new ApplicationException();
		}
	}

	public override void RenderViewDockSideTabBackground(DockPanelSideTab tab, Graphics graphics, Rectangle bounds)
	{
		RenderTabBackground(tab, bounds, graphics);
	}

	public override void RenderViewDockSideTabContent(DockPanelSideTab tab, Graphics graphics, Rectangle bounds)
	{
		RenderTabContent(tab, bounds, graphics);
	}

	public override void RenderViewHostTabBackground(ViewHostTab tab, Graphics graphics, Rectangle bounds)
	{
		RenderTabBackground(tab, bounds, graphics);
	}

	public override void RenderViewHostTabContent(ViewHostTab tab, Graphics graphics, Rectangle bounds)
	{
		RenderTabContent(tab, bounds, graphics);
		var buttonsBounds = new Rectangle(bounds.Right - tab.Buttons.Width - 2, 0, tab.Buttons.Width, bounds.Height);
		tab.Buttons.OnPaint(graphics, buttonsBounds, tab.IsActive);
	}

	public override void RenderViewHostTabsBackground(ViewHostTabs tabs, PaintEventArgs e)
	{
		var graphics = e.Graphics;

		using(var brush = new SolidBrush(ColorTable.ViewHostTabsBackground))
		{
			graphics.FillRectangle(brush, e.ClipRectangle);
		}
		if(tabs.ViewHost.IsDocumentWell)
		{
			using(var brush = new SolidBrush(
				tabs.ViewHost.IsActive ?
					ColorTable.ViewHostTabsSelectedBackgroundActiveEnd :
					ColorTable.ViewHostTabsSelectedBackgroundNormalEnd))
			{
				var dpi  = Dpi.FromControl(tabs);
				var rc = new RectangleF(
					-0.5f, -0.5f + TabHeight.GetValue(dpi),
					tabs.Width + 1, TabFooterHeight.GetValue(dpi) + 1);
				var ltCorner = ((tabs.LeftButtons == null || tabs.LeftButtons.Width == 0) && tabs[tabs.FirstTabIndex].IsActive) ? 0 : 2;
				graphics.FillRoundedRectangle(brush, rc, ltCorner, 2, 0, 0);
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
			Color border;
			Color background;
			if(pressed)
			{
				border = ColorTable.ViewButtonPressedBorder;
				background = ColorTable.ViewButtonPressedBackground;
			}
			else
			{
				border = ColorTable.ViewButtonHoverBorder;
				background = ColorTable.ViewButtonHoverBackground;
			}
			using(var brush = new SolidBrush(background))
			{
				graphics.FillRectangle(brush, rc);
			}
			using(var pen = new Pen(border))
			{
				graphics.DrawRectangle(pen, rc);
			}
		}
		var image = viewButton.Type switch
		{
			ViewButtonType.Menu            => ImgMenu.Value,
			ViewButtonType.Pin             => ImgPin.Value,
			ViewButtonType.Unpin           => ImgPin.Value,
			ViewButtonType.Normalize       => ImgNormalize.Value,
			ViewButtonType.Maximize        => ImgMaximize.Value,
			ViewButtonType.Close           => ImgClose.Value,
			ViewButtonType.ScrollTabsLeft  => ImgScrollLeft.Value,
			ViewButtonType.ScrollTabsRight => ImgScrollRight.Value,
			ViewButtonType.TabsMenu        => ImgTabMenu.Value,
			ViewButtonType.TabsScrollMenu  => ImgTabMenuExt.Value,
			_ => viewButton.Image,
		};
		if(image is not null)
		{
			graphics.DrawImage(image, bounds);
		}
	}

	#endregion

	#region View Host Footer Rendering

	public override void RenderViewHostFooter(ViewHostFooter footer, PaintEventArgs e)
	{
		Verify.Argument.IsNotNull(footer);
		Verify.Argument.IsNotNull(e);

		var graphics = e.Graphics;

		graphics.GdiFill(ColorTable.ViewHostFooterBackground, e.ClipRectangle);
		var color = footer.ViewHost.IsActive ?
			ColorTable.ViewHostForegroundActive :
			ColorTable.ViewHostForegroundNormal;
		using(var brush = new SolidBrush(color))
		{
			var dpi = Dpi.FromControl(footer);
			var conv = DpiConverter.FromDefaultTo(dpi);
			var rc = (RectangleF)footer.ClientRectangle;
			rc.X -= 0.5f;
			rc.Y -= 0.5f;
			rc.Width += 1;
			rc.Height += 1;
			graphics.FillRoundedRectangle(brush, rc, 0, 0, conv.ConvertX(2), conv.ConvertX(2));
		}
	}

	#endregion

	#region  View Host Header Rendering

	private static readonly StringFormat ViewHostHeaderTextFormat =
		new StringFormat(StringFormat.GenericDefault)
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
		const int BetweenTextAndButtons = 2;
		const int BeforeContent = 2;

		var graphics = e.Graphics;
		var client = header.ClientRectangle;

		var rect = (RectangleF)client;
		rect.X -= .5f;
		rect.Width += 1;
		rect.Y -= .5f;
		rect.Height += 1;

		graphics.TextRenderingHint = GraphicsUtility.TextRenderingHint;
		graphics.TextContrast       = GraphicsUtility.TextContrast;

		Color textColor, backgroundStart, backgroundEnd;
		if(header.ViewHost.IsActive)
		{
			textColor		= ColorTable.ViewHostHeaderTextFocused;
			backgroundStart	= ColorTable.ViewHostHeaderBackgroundFocusedStart;
			backgroundEnd	= ColorTable.ViewHostHeaderBackgroundFocusedEnd;
		}
		else
		{
			textColor		= ColorTable.ViewHostHeaderTextNormal;
			backgroundStart	= ColorTable.ViewHostHeaderBackgroundNormalStart;
			backgroundEnd	= ColorTable.ViewHostHeaderBackgroundNormalEnd;
		}

		var dpi = Dpi.FromControl(header);
		if(header.ViewHost.Status == ViewHostStatus.Floating &&
			((Form)header.ViewHost.TopLevelControl).WindowState == FormWindowState.Maximized)
		{
			using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
			using(var brush = new LinearGradientBrush(Point.Empty, new Point(0, HeaderHeight.GetValue(dpi)), backgroundStart, backgroundEnd))
			{
				graphics.FillRectangle(brush, rect);
			}
		}
		else
		{
			using(var brush = new SolidBrush(ColorTable.ViewHostHeaderBackground))
			{
				graphics.FillRectangle(brush, e.ClipRectangle);
			}
			using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
			using(var brush = new LinearGradientBrush(Point.Empty, new Point(0, HeaderHeight.GetValue(dpi)), backgroundStart, backgroundEnd))
			{
				graphics.FillRoundedRectangle(brush, rect, 2, 2, 0, 0);
			}
		}
		rect.X += BeforeContent;
		rect.Width -= BeforeContent;
		if(header.Buttons.Count != 0)
		{
			rect.Width -= header.Buttons.Width + BetweenTextAndButtons;
		}
		using(var brush = new SolidBrush(textColor))
		{
			GitterApplication.TextRenderer.DrawText(
				graphics,
				header.Text,
				GitterApplication.FontManager.UIFont,
				brush,
				Rectangle.Truncate(rect),
				ViewHostHeaderTextFormat);
		}
	}

	#endregion

	#region View Dock Side Rendering

	public override void RenderViewDockSide(DockPanelSide side, PaintEventArgs e)
	{
		using(var brush = new SolidBrush(ColorTable.ViewDockSideBackground))
		{
			e.Graphics.FillRectangle(brush, e.ClipRectangle);
		}
	}

	#endregion

	#region Dock Marker Button

	private static void PaintDockMarkerButtonBackground(Graphics graphics, Dpi dpi, Rectangle bounds, bool hover)
	{
		Color start, end, border;
		if(hover)
		{
			start  = ColorTable.DockMarkerButtonBackgroundStart;
			end    = ColorTable.DockMarkerButtonBackgroundEnd;
			border = ColorTable.DockMarkerButtonBorder;
		}
		else
		{
			start = Color.FromArgb((byte)(ViewConstants.OpacityNormal * 255),
				ColorTable.DockMarkerButtonBackgroundStart.R,
				ColorTable.DockMarkerButtonBackgroundStart.G,
				ColorTable.DockMarkerButtonBackgroundStart.B);
			end = Color.FromArgb((byte)(ViewConstants.OpacityNormal * 255),
				ColorTable.DockMarkerButtonBackgroundEnd.R,
				ColorTable.DockMarkerButtonBackgroundEnd.G,
				ColorTable.DockMarkerButtonBackgroundEnd.B);
			border = Color.FromArgb((byte)(ViewConstants.OpacityNormal * 255),
				ColorTable.DockMarkerButtonBorder.R,
				ColorTable.DockMarkerButtonBorder.G,
				ColorTable.DockMarkerButtonBorder.B);
		}
		using(var pen = new Pen(border))
		using(var brush = new LinearGradientBrush(bounds,
			start, end, LinearGradientMode.Vertical))
		{
			graphics.FillRoundedRectangle(brush, pen, bounds, DpiConverter.FromDefaultTo(dpi).ConvertX(2.0f));
		}
	}

	private static void InitDockMarkerButtonContentColors(bool hover, out Color start, out Color end, out Color border)
	{
		if(hover)
		{
			start	= ColorTable.DockMarkerButtonContentStart;
			end		= ColorTable.DockMarkerButtonContentEnd;
			border	= ColorTable.DockMarkerButtonContentBorder;
		}
		else
		{
			start = Color.FromArgb((byte)(ViewConstants.OpacityNormal * 255),
				ColorTable.DockMarkerButtonContentStart.R,
				ColorTable.DockMarkerButtonContentStart.G,
				ColorTable.DockMarkerButtonContentStart.B);
			end = Color.FromArgb((byte)(ViewConstants.OpacityNormal * 255),
				ColorTable.DockMarkerButtonContentEnd.R,
				ColorTable.DockMarkerButtonContentEnd.G,
				ColorTable.DockMarkerButtonContentEnd.B);
			border = Color.FromArgb((byte)(ViewConstants.OpacityNormal * 255),
				ColorTable.DockMarkerButtonContentBorder.R,
				ColorTable.DockMarkerButtonContentBorder.G,
				ColorTable.DockMarkerButtonContentBorder.B);
		}
	}

	private static void PaintDockMarkerTopButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out var start, out var end, out var border);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var rc = new Rectangle(rect.X + conv.ConvertX(4), rect.Y + conv.ConvertY(4), conv.ConvertX(24), conv.ConvertY(12));
		var arrow = new PointF[]
			{
				new PointF(rect.X + conv.ConvertX(11) + .5f, rect.Y + conv.ConvertY(25) + .5f),
				new PointF(rect.X + conv.ConvertX(15) + .5f, rect.Y + conv.ConvertY(21) + .5f),
				new PointF(rect.X + conv.ConvertX(16) + .5f, rect.Y + conv.ConvertY(21) + .5f),
				new PointF(rect.X + conv.ConvertX(19) + .5f, rect.Y + conv.ConvertY(25) + .5f),
			};
		using(var brush = new SolidBrush(border))
		{
			graphics.FillRectangle(brush, rc);
			graphics.FillPolygon(brush, arrow);
		}
		rc.X += conv.ConvertX(1);
		rc.Y += conv.ConvertY(3);
		rc.Width  -= conv.ConvertX(2);
		rc.Height -= conv.ConvertY(4);
		using(var brush = new LinearGradientBrush(rc,
			start, end, LinearGradientMode.Vertical))
		{
			if(!hover)
			{
				graphics.FillRectangle(Brushes.White, rc);
			}
			graphics.FillRectangle(brush, rc);
		}
	}

	private static void PaintDockMarkerDocumentTopButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out var start, out var end, out var border);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var rc = new Rectangle(rect.X + conv.ConvertX(4), rect.Y + conv.ConvertY(4), conv.ConvertX(24), conv.ConvertY(24));
		using(var brush = new SolidBrush(border))
		{
			graphics.FillRectangle(brush, rc);
		}
		rc.X += conv.ConvertX(1);
		rc.Y += conv.ConvertY(3);
		rc.Width  -= conv.ConvertX(2);
		rc.Height -= conv.ConvertY(4);
		using(var brush = new LinearGradientBrush(rc,
			start, end, LinearGradientMode.Vertical))
		{
			graphics.FillRectangle(Brushes.White, rc);
			rc.Height = 11;
			graphics.FillRectangle(brush, rc);
		}
		using(var pen = new Pen(border)
		{
			DashStyle = DashStyle.Dot,
		})
		{
			graphics.DrawLine(pen, rc.X, rc.Bottom - 1, rc.Right, rc.Bottom - 1);
		}
	}

	private static void PaintDockMarkerLeftButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out var start, out var end, out var border);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var rc = new Rectangle(rect.X + conv.ConvertX(4), rect.Y + conv.ConvertY(4), conv.ConvertX(12), conv.ConvertY(24));
		var arrow = new PointF[]
			{
				new PointF(rect.X + conv.ConvertX(25) + .5f, rect.Y + conv.ConvertY(11) + .5f),
				new PointF(rect.X + conv.ConvertX(25) + .5f, rect.Y + conv.ConvertY(19) + .5f),
				new PointF(rect.X + conv.ConvertX(21) + .5f, rect.Y + conv.ConvertY(15) + .5f),
				new PointF(rect.X + conv.ConvertX(21) + .5f, rect.Y + conv.ConvertY(14) + .5f),
			};
		using(var brush = new SolidBrush(border))
		{
			graphics.FillRectangle(brush, rc);
			graphics.FillPolygon(brush, arrow);
		}
		rc.X += conv.ConvertX(1);
		rc.Y += conv.ConvertY(3);
		rc.Width  -= conv.ConvertX(2);
		rc.Height -= conv.ConvertY(4);
		using(var brush = new LinearGradientBrush(rc,
			start, end, LinearGradientMode.Vertical))
		{
			if(!hover)
			{
				graphics.FillRectangle(Brushes.White, rc);
			}
			graphics.FillRectangle(brush, rc);
		}
	}

	private static void PaintDockMarkerDocumentLeftButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out var start, out var end, out var border);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var rc = new Rectangle(rect.X + conv.ConvertX(4), rect.Y + conv.ConvertY(4), conv.ConvertX(24), conv.ConvertY(24));
		using(var brush = new SolidBrush(border))
		{
			graphics.FillRectangle(brush, rc);
		}
		rc.X += conv.ConvertX(1);
		rc.Y += conv.ConvertY(3);
		rc.Width  -= conv.ConvertX(2);
		rc.Height -= conv.ConvertY(4);
		using(var brush = new LinearGradientBrush(rc,
			start, end, LinearGradientMode.Vertical))
		{
			graphics.FillRectangle(Brushes.White, rc);
			rc.Width = conv.ConvertX(11);
			graphics.FillRectangle(brush, rc);
		}
		using(var pen = new Pen(border)
		{
			DashStyle = DashStyle.Dot,
		})
		{
			graphics.DrawLine(pen, rc.Right - 1, rc.Y, rc.Right - 1, rc.Bottom);
		}
	}

	private static void PaintDockMarkerFillButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out var start, out var end, out var border);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var rc = new Rectangle(rect.X + conv.ConvertX(4), rect.Y + conv.ConvertY(4), conv.ConvertX(24), conv.ConvertY(24));
		using(var brush = new SolidBrush(border))
		{
			graphics.FillRectangle(brush, rc);
		}
		rc.X += conv.ConvertX(1);
		rc.Y += conv.ConvertY(3);
		rc.Width  -= conv.ConvertX(2);
		rc.Height -= conv.ConvertY(4);
		using(var brush = new LinearGradientBrush(rc,
			start, end, LinearGradientMode.Vertical))
		{
			if(!hover)
			{
				graphics.FillRectangle(Brushes.White, rc);
			}
			graphics.FillRectangle(brush, rc);
		}
	}

	private static void PaintDockMarkerRightButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out var start, out var end, out var border);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var rc = new Rectangle(rect.X + conv.ConvertX(16), rect.Y + conv.ConvertY(4), conv.ConvertX(12), conv.ConvertY(24));
		var arrow = new PointF[]
			{
				new PointF(rect.X + conv.ConvertX(5) + .5f, rect.Y + conv.ConvertY(20) + .5f),
				new PointF(rect.X + conv.ConvertX(5) + .5f, rect.Y + conv.ConvertY(11) + .5f),
				new PointF(rect.X + conv.ConvertX(9) + .5f, rect.Y + conv.ConvertY(14) + .5f),
				new PointF(rect.X + conv.ConvertX(9) + .5f, rect.Y + conv.ConvertY(16) + .5f),
			};
		using(var brush = new SolidBrush(border))
		{
			graphics.FillRectangle(brush, rc);
			graphics.FillPolygon(brush, arrow);
		}
		rc.X += conv.ConvertX(1);
		rc.Y += conv.ConvertY(3);
		rc.Width  -= conv.ConvertX(2);
		rc.Height -= conv.ConvertY(4);
		using(var brush = new LinearGradientBrush(rc,
			start, end, LinearGradientMode.Vertical))
		{
			if(!hover)
			{
				graphics.FillRectangle(Brushes.White, rc);
			}
			graphics.FillRectangle(brush, rc);
		}
	}

	private static void PaintDockMarkerDocumentRightButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out var start, out var end, out var border);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var rc = new Rectangle(rect.X + conv.ConvertX(4), rect.Y + conv.ConvertY(4), conv.ConvertX(24), conv.ConvertY(24));
		using(var brush = new SolidBrush(border))
		{
			graphics.FillRectangle(brush, rc);
		}
		rc.X += conv.ConvertX(1);
		rc.Y += conv.ConvertY(3);
		rc.Width  -= conv.ConvertX(2);
		rc.Height -= conv.ConvertY(4);
		using(var brush = new LinearGradientBrush(rc,
			start, end, LinearGradientMode.Vertical))
		{
			graphics.FillRectangle(Brushes.White, rc);
			var dx = conv.ConvertX(11);
			rc.X    += dx;
			rc.Width = dx;
			graphics.FillRectangle(brush, rc);
		}
		using(var pen = new Pen(border)
		{
			DashStyle = DashStyle.Dot,
		})
		{
			graphics.DrawLine(pen, rc.X, rc.Y, rc.X, rc.Bottom);
		}
	}

	private static void PaintDockMarkerBottomButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out var start, out var end, out var border);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var rc = new Rectangle(rect.X + conv.ConvertX(4), rect.Y + conv.ConvertY(16), conv.ConvertX(24), conv.ConvertY(12));
		var arrow = new PointF[]
			{
				new PointF(rect.X + conv.ConvertX(10) + .5f, rect.Y + conv.ConvertY(5) + .5f),
				new PointF(rect.X + conv.ConvertX(15) + .5f, rect.Y + conv.ConvertY(9) + .5f),
				new PointF(rect.X + conv.ConvertX(16) + .5f, rect.Y + conv.ConvertY(9) + .5f),
				new PointF(rect.X + conv.ConvertX(20) + .5f, rect.Y + conv.ConvertY(5) + .5f),
			};
		using(var brush = new SolidBrush(border))
		{
			graphics.FillRectangle(brush, rc);
			graphics.FillPolygon(brush, arrow);
		}
		rc.X += conv.ConvertX(1);
		rc.Y += conv.ConvertY(3);
		rc.Width  -= conv.ConvertX(2);
		rc.Height -= conv.ConvertY(4);
		using(var brush = new LinearGradientBrush(rc,
			start, end, LinearGradientMode.Vertical))
		{
			if(!hover)
			{
				graphics.FillRectangle(Brushes.White, rc);
			}
			graphics.FillRectangle(brush, rc);
		}
	}

	private static void PaintDockMarkerDocumentBottomButton(Graphics graphics, Dpi dpi, Rectangle rect, bool hover)
	{
		InitDockMarkerButtonContentColors(hover, out var start, out var end, out var border);
		var conv = DpiConverter.FromDefaultTo(dpi);
		var rc = new Rectangle(rect.X + conv.ConvertX(4), rect.Y + conv.ConvertY(4), conv.ConvertX(24), conv.ConvertY(24));
		using(var brush = new SolidBrush(border))
		{
			graphics.FillRectangle(brush, rc);
		}
		rc.X += conv.ConvertX(1);
		rc.Y += conv.ConvertY(3);
		rc.Width  -= conv.ConvertX(2);
		rc.Height -= conv.ConvertY(4);
		using(var brush = new LinearGradientBrush(rc,
			start, end, LinearGradientMode.Vertical))
		{
			graphics.FillRectangle(Brushes.White, rc);
			rc.Y += conv.ConvertY(9);
			rc.Height = conv.ConvertY(11);
			graphics.FillRectangle(brush, rc);
		}
		using(var pen = new Pen(border)
		{
			DashStyle = DashStyle.Dot,
		})
		{
			graphics.DrawLine(pen, rc.X, rc.Y, rc.Right, rc.Y);
		}
	}

	private static void PaintDockMarkerButtonContent(DockMarkerButton button, Graphics graphics, Dpi dpi, bool hover)
	{
		Assert.IsNotNull(button);
		Assert.IsNotNull(graphics);

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
		var client = header.ClientRectangle;

		var rect = (RectangleF)client;
		rect.X -= .5f;
		rect.Width += 1;
		rect.Y -= .5f;
		rect.Height += 1;

		var dpi = Dpi.FromControl(header);

		graphics.TextRenderingHint = GraphicsUtility.TextRenderingHint;
		graphics.TextContrast      = GraphicsUtility.TextContrast;

		Color textColor, backgroundStart, backgroundEnd;
		textColor = ColorTable.ViewHostHeaderTextFocused;
		backgroundStart = ColorTable.ViewHostHeaderBackgroundFocusedStart;
		backgroundEnd = ColorTable.ViewHostHeaderBackgroundFocusedEnd;
		using(var brush = new SolidBrush(ColorTable.ViewHostHeaderBackground))
		{
			graphics.FillRectangle(brush, e.ClipRectangle);
		}
		graphics.SmoothingMode = SmoothingMode.HighQuality;
		using(var brush = new LinearGradientBrush(Point.Empty, new Point(0, HeaderHeight.GetValue(dpi)), backgroundStart, backgroundEnd))
		{
			graphics.FillRoundedRectangle(brush, rect, 2, 2, 0, 0);
		}
		rect.X += BeforeContent;
		rect.Width -= BeforeContent;
		if(header.Buttons.Count != 0)
		{
			rect.Width -= header.Buttons.Width + BetweenTextAndButtons;
		}
		using(var brush = new SolidBrush(textColor))
		{
			GitterApplication.TextRenderer.DrawText(
				graphics,
				header.Text,
				GitterApplication.FontManager.UIFont,
				brush,
				Rectangle.Truncate(rect),
				ViewHostHeaderTextFormat);
		}
	}

	#endregion
}
