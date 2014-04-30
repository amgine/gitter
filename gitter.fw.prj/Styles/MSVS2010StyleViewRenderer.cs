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
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	public class MSVS2010StyleViewRenderer : ViewRenderer
	{
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
			public static readonly int TabHeight        = SystemInformation.SmallIconSize.Height + 4;
			public static readonly int HeaderHeight     = SystemInformation.SmallIconSize.Height + 3;
			public static readonly int ViewButtonSize   = SystemInformation.SmallIconSize.Height - 1;
			public static readonly int FloatTitleHeight = SystemInformation.SmallIconSize.Height;
			public const int TabFooterHeight	= 4;
			public const int FooterHeight		= 4;
			public const int SideTabSpacing		= 1;
			public const int SideTabHeight		= 21;
			public const int FloatBorderSize	= 4;
			public const int FloatCornerRadius	= 3;
		}

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

		public override int TabHeight
		{
			get { return Constants.TabHeight; }
		}

		public override int TabFooterHeight
		{
			get { return Constants.TabFooterHeight; }
		}

		public override int HeaderHeight
		{
			get { return Constants.HeaderHeight; }
		}

		public override int FooterHeight
		{
			get { return Constants.FooterHeight; }
		}

		public override int ViewButtonSize
		{
			get { return Constants.ViewButtonSize; }
		}

		public override int SideTabSpacing
		{
			get { return Constants.SideTabSpacing; }
		}

		public override int SideTabHeight
		{
			get { return Constants.SideTabHeight; }
		}

		public override int FloatBorderSize
		{
			get { return Constants.FloatBorderSize; }
		}

		public override int FloatCornerRadius
		{
			get { return Constants.FloatCornerRadius; }
		}

		public override int FloatTitleHeight
		{
			get { return Constants.FloatTitleHeight; }
		}

		#region Tabs Rendering

		protected static readonly StringFormat NormalStringFormat = new StringFormat(StringFormat.GenericTypographic)
		{
			LineAlignment = StringAlignment.Center,
			Trimming = StringTrimming.EllipsisCharacter,
		};

		protected static readonly StringFormat VerticalStringFormat = new StringFormat(NormalStringFormat)
		{
			FormatFlags = StringFormatFlags.DirectionVertical,
		};

		private static int MeasureTabLength(ViewTabBase tab, Graphics graphics)
		{
			var length = GitterApplication.TextRenderer.MeasureText(
				graphics, tab.Text, GitterApplication.FontManager.UIFont, int.MaxValue, NormalStringFormat).Width;
			if(tab.Image != null)
			{
				length += 16 + ViewConstants.ImageSpacing;
			}
			length += ViewConstants.BeforeTabContent + ViewConstants.AfterTabContent;
			return length;
		}

		public override int MeasureViewDockSideTabLength(ViewDockSideTab tab, Graphics graphics)
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
			int x = bounds.X;
			int y = bounds.Y;
			int w = bounds.Width;
			int h = bounds.Height;
			Brush textBrush;
			Rectangle imageRect;
			StringFormat stringFormat;
			switch(tab.Anchor)
			{
				case AnchorStyles.Right:
					imageRect = new Rectangle(x + (w - 16) / 2, y + 3, 16, 16);
					stringFormat = VerticalStringFormat;
					break;
				case AnchorStyles.Left:
					imageRect = new Rectangle(x + (w - 16) / 2, y + 3, 16, 16);
					stringFormat = VerticalStringFormat;
					break;
				case AnchorStyles.Top:
					imageRect = new Rectangle(x + 3, y + (h - 16) / 2, 16, 16);
					stringFormat = NormalStringFormat;
					break;
				case AnchorStyles.Bottom:
					imageRect = new Rectangle(x + 3, y + (h - 16) / 2, 16, 16);
					stringFormat = NormalStringFormat;
					break;
				default:
					throw new ApplicationException();
			}
			var host = tab.View.Host;
			if(tab.IsActive)
			{
				textBrush = Brushes.Black;
			}
			else
			{
				textBrush = Brushes.White;
			}
			var image = tab.Image;
			if(image != null)
			{
				switch(tab.Orientation)
				{
					case Orientation.Horizontal:
						{
							graphics.DrawImage(image, imageRect);
							bounds.Width -= imageRect.Width + 3;
							bounds.X += imageRect.Width + 3;
						}
						break;
					case Orientation.Vertical:
						using(var rotatedImage = (Image)image.Clone())
						{
							rotatedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
							graphics.DrawImage(rotatedImage, imageRect);
							bounds.Height -= imageRect.Height + 3;
							bounds.Y += imageRect.Height + 3;
						}
						break;
					default:
						throw new ApplicationException();
				}
			}
			switch(tab.Orientation)
			{
				case Orientation.Horizontal:
					bounds.X += ViewConstants.BeforeTabContent;
					bounds.Width -= ViewConstants.BeforeTabContent + ViewConstants.AfterTabContent - 1;
					GitterApplication.TextRenderer.DrawText(
						graphics, tab.Text, GitterApplication.FontManager.UIFont, textBrush, bounds, stringFormat);
					break;
				case Orientation.Vertical:
					bounds.Y += ViewConstants.BeforeTabContent;
					bounds.Height -= ViewConstants.BeforeTabContent + ViewConstants.AfterTabContent - 1;
					bounds.Height += 10;
					GitterApplication.GdiPlusTextRenderer.DrawText(
						graphics, tab.Text, GitterApplication.FontManager.UIFont, textBrush, bounds, stringFormat);
					break;
				default:
					throw new ApplicationException();
			}
		}

		public override void RenderViewDockSideTabBackground(ViewDockSideTab tab, Graphics graphics, Rectangle bounds)
		{
			RenderTabBackground(tab, bounds, graphics);
		}

		public override void RenderViewDockSideTabContent(ViewDockSideTab tab, Graphics graphics, Rectangle bounds)
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
					var rc = new RectangleF(
						-0.5f, -0.5f + Constants.TabHeight,
						tabs.Width + 1, Constants.TabFooterHeight + 1);
					var ltCorner = ((tabs.LeftButtons == null || tabs.LeftButtons.Width == 0) && tabs[tabs.FirstTabIndex].IsActive) ? 0 : 2;
					graphics.FillRoundedRectangle(brush, rc, ltCorner, 2, 0, 0);
				}
			}
		}

		#endregion

		#region View Button rendering

		public override void RenderViewButton(ViewButton viewButton, Graphics graphics, Rectangle bounds, bool focus, bool hover, bool pressed)
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
			if(viewButton.Image != null)
			{
				graphics.DrawImage(viewButton.Image, bounds);
			}
		}

		#endregion

		#region View Host Footer Rendering

		public override void RenderViewHostFooter(ViewHostFooter footer, PaintEventArgs e)
		{
			var graphics = e.Graphics;

			using(var brush = new SolidBrush(ColorTable.ViewHostFooterBackground))
			{
				graphics.FillRectangle(brush, e.ClipRectangle);
			}
			var color = footer.ViewHost.IsActive ?
				ColorTable.ViewHostForegroundActive :
				ColorTable.ViewHostForegroundNormal;
			using(var brush = new SolidBrush(color))
			{
				var rc = (RectangleF)footer.ClientRectangle;
				rc.X -= 0.5f;
				rc.Y -= 0.5f;
				rc.Width += 1;
				rc.Height += 1;
				graphics.FillRoundedRectangle(brush, rc, 0, 0, 2, 2);
			}
		}

		#endregion

		#region  Viwe Host Header Rendering

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

			if(header.ViewHost.Status == ViewHostStatus.Floating &&
				((Form)header.ViewHost.TopLevelControl).WindowState == FormWindowState.Maximized)
			{
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				using(var brush = new LinearGradientBrush(Point.Empty, new Point(0, Constants.HeaderHeight), backgroundStart, backgroundEnd))
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
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				using(var brush = new LinearGradientBrush(Point.Empty, new Point(0, Constants.HeaderHeight), backgroundStart, backgroundEnd))
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

		public override void RenderViewDockSide(ViewDockSide side, PaintEventArgs e)
		{
			using(var brush = new SolidBrush(ColorTable.ViewDockSideBackground))
			{
				e.Graphics.FillRectangle(brush, e.ClipRectangle);
			}
		}

		#endregion

		#region Dock Marker Button

		private static void PaintDockMarkerButtonBackground(Graphics graphics, Rectangle bounds, bool hover)
		{
			Color start, end, border;
			if(hover)
			{
				start	= ColorTable.DockMarkerButtonBackgroundStart;
				end		= ColorTable.DockMarkerButtonBackgroundEnd;
				border	= ColorTable.DockMarkerButtonBorder;
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
				graphics.FillRoundedRectangle(brush, pen, bounds, 2);
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

		private static void PaintDockMarkerTopButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitDockMarkerButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 12);
			var arrow = new PointF[]
				{
					new PointF(rect.X + 11.5f, rect.Y + 25.5f),
					new PointF(rect.X + 15.5f, rect.Y + 21.5f),
					new PointF(rect.X + 16.5f, rect.Y + 21.5f),
					new PointF(rect.X + 19.5f, rect.Y + 25.5f),
				};
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
				graphics.FillPolygon(brush, arrow);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
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

		private static void PaintDockMarkerDocumentTopButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitDockMarkerButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
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

		private static void PaintDockMarkerLeftButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitDockMarkerButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 12, 24);
			var arrow = new PointF[]
				{
					new PointF(rect.X + 25.5f, rect.Y + 11.5f),
					new PointF(rect.X + 25.5f, rect.Y + 19.5f),
					new PointF(rect.X + 21.5f, rect.Y + 15.5f),
					new PointF(rect.X + 21.5f, rect.Y + 14.5f),
				};
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
				graphics.FillPolygon(brush, arrow);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
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

		private static void PaintDockMarkerDocumentLeftButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitDockMarkerButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new LinearGradientBrush(rc,
				start, end, LinearGradientMode.Vertical))
			{
				graphics.FillRectangle(Brushes.White, rc);
				rc.Width = 11;
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

		private static void PaintDockMarkerFillButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitDockMarkerButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
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

		private static void PaintDockMarkerRightButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitDockMarkerButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 16, rect.Y + 4, 12, 24);
			var arrow = new PointF[]
				{
					new PointF(rect.X + 5.5f, rect.Y + 20.5f),
					new PointF(rect.X + 5.5f, rect.Y + 11.5f),
					new PointF(rect.X + 9.5f, rect.Y + 14.5f),
					new PointF(rect.X + 9.5f, rect.Y + 16.5f),
				};
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
				graphics.FillPolygon(brush, arrow);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
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

		private static void PaintDockMarkerDocumentRightButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitDockMarkerButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new LinearGradientBrush(rc,
				start, end, LinearGradientMode.Vertical))
			{
				graphics.FillRectangle(Brushes.White, rc);
				rc.X += 11;
				rc.Width = 11;
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

		private static void PaintDockMarkerBottomButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitDockMarkerButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 16, 24, 12);
			var arrow = new PointF[]
				{
					new PointF(rect.X + 10.5f, rect.Y + 5.5f),
					new PointF(rect.X + 15.5f, rect.Y + 9.5f),
					new PointF(rect.X + 16.5f, rect.Y + 9.5f),
					new PointF(rect.X + 20.5f, rect.Y + 5.5f),
				};
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
				graphics.FillPolygon(brush, arrow);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
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

		private static void PaintDockMarkerDocumentBottomButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color start, end, border;
			InitDockMarkerButtonContentColors(hover, out start, out end, out border);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(border))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new LinearGradientBrush(rc,
				start, end, LinearGradientMode.Vertical))
			{
				graphics.FillRectangle(Brushes.White, rc);
				rc.Y += 9;
				rc.Height = 11;
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

		private static void PaintDockMarkerButtonContent(DockMarkerButton button, Graphics graphics, bool hover)
		{
			switch(button.Type)
			{
				case DockResult.Top:
					PaintDockMarkerTopButton(graphics, button.Bounds, hover);
					break;
				case DockResult.DocumentTop:
					PaintDockMarkerDocumentTopButton(graphics, button.Bounds, hover);
					break;
				case DockResult.Left:
					PaintDockMarkerLeftButton(graphics, button.Bounds, hover);
					break;
				case DockResult.DocumentLeft:
					PaintDockMarkerDocumentLeftButton(graphics, button.Bounds, hover);
					break;
				case DockResult.Fill:
					PaintDockMarkerFillButton(graphics, button.Bounds, hover);
					break;
				case DockResult.Right:
					PaintDockMarkerRightButton(graphics, button.Bounds, hover);
					break;
				case DockResult.DocumentRight:
					PaintDockMarkerDocumentRightButton(graphics, button.Bounds, hover);
					break;
				case DockResult.Bottom:
					PaintDockMarkerBottomButton(graphics, button.Bounds, hover);
					break;
				case DockResult.DocumentBottom:
					PaintDockMarkerDocumentBottomButton(graphics, button.Bounds, hover);
					break;
			}
		}

		public override void RenderDockMarkerButton(DockMarkerButton button, Graphics graphics, bool hover)
		{
			PaintDockMarkerButtonBackground(graphics, button.Bounds, hover);
			PaintDockMarkerButtonContent(button, graphics, hover);
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
			using(var brush = new LinearGradientBrush(Point.Empty, new Point(0, Constants.HeaderHeight), backgroundStart, backgroundEnd))
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
}
