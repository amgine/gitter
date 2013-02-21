namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	public class MSVS2012StyleViewRenderer : ViewRenderer
	{
		protected static class ColorTable
		{
			private static readonly Color DefaultBackground					= Color.FromArgb(45, 45, 48);

			public static readonly Color BackgroundColor					= DefaultBackground;
			public static readonly Color ViewHostTabsBackground				= DefaultBackground;

			public static readonly Color DocTabsFooterActive				= Color.FromArgb(0, 122, 204);
			public static readonly Color DocTabsFooterNormal				= Color.FromArgb(63, 63, 70);
			public static readonly Color DocTabBackground					= Color.FromArgb(41, 57, 85);
			public static readonly Color DocTabBackgroundHover				= Color.FromArgb(20, 151, 234);
			public static readonly Color DocTabBackgroundSelectedActive		= Color.FromArgb(0, 122, 204);
			public static readonly Color DocTabBackgroundSelected			= Color.FromArgb(63, 63, 70);

			public static readonly Color ToolTabBackgroundActive			= Color.FromArgb(37, 37, 38);
			public static readonly Color ToolTabBackgroundHover				= Color.FromArgb(62, 62, 64);
			public static readonly Color ToolTabSeparator					= Color.FromArgb(63, 63, 70);
			public static readonly Color ToolTabForeground					= Color.FromArgb(208, 208, 208);
			public static readonly Color ToolTabForegroundActive			= Color.FromArgb(0, 151, 251);
			public static readonly Color ToolTabForegroundHover				= Color.FromArgb(85, 170, 255);

			public static readonly Color DockSideBackground					= DefaultBackground;
			public static readonly Color DockSideTabOutline					= Color.FromArgb(63, 63, 70);
			public static readonly Color DockSideTabOutlineHover			= Color.FromArgb(0, 122, 204);
			public static readonly Color DockSideTabForeground				= Color.FromArgb(208, 208, 208);
			public static readonly Color DockSideTabForegroundHover			= Color.FromArgb(0, 151, 251);

			public static readonly Color ViewButtonPressedBackground		= Color.FromArgb(14, 97, 152);
			public static readonly Color ViewButtonHoverBackgroundActive	= Color.FromArgb(82, 176, 239);
			public static readonly Color ViewButtonHoverBackgroundInactive	= Color.FromArgb(57, 57, 57);
			public static readonly Color ViewButtonForeground				= Color.FromArgb(255, 255, 255);

			public static readonly Color ViewHostHeaderBackgroundNormal		= DefaultBackground;
			public static readonly Color ViewHostHeaderBackgroundFocused	= Color.FromArgb(0, 122, 204);
			public static readonly Color ViewHostHeaderAccentNormal			= Color.FromArgb(70, 70, 74);
			public static readonly Color ViewHostHeaderAccentFocused		= Color.FromArgb(89, 168, 222);

			public static readonly Color ViewHostHeaderTextNormal			= Color.FromArgb(208, 208, 208);
			public static readonly Color ViewHostHeaderTextFocused			= Color.FromArgb(255, 255, 255);

			public static readonly Color DockMarkerBackground				= Color.FromArgb(30, 30, 30);
			public static readonly Color DockMarkerBorder					= Color.FromArgb(51, 51, 55);
			public static readonly Color DockMarkerButtonBackground			= Color.FromArgb(37, 37, 38);
			public static readonly Color DockMarkerButtonContentBorder		= Color.FromArgb(0, 122, 204);
			public static readonly Color DockMarkerButtonContentArrow		= Color.FromArgb(241, 241, 241);
		}

		private static class Constants
		{
			public const int TabHeight			= 19;
			public const int TabFooterHeight	= 2;
			public const int HeaderHeight		= 21;
			public const int FooterHeight		= 0;
			public const int ViewButtonSize		= 15;
			public const int SideTabSpacing		= 12;
			public const int SideTabHeight		= 25;
			public const int SideTabOutline		= 6;
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

		private static void RenderTabContent(ViewTabBase tab, Rectangle bounds, Graphics graphics, Color foregroundColor)
		{
			int x = bounds.X;
			int y = bounds.Y;
			int w = bounds.Width;
			int h = bounds.Height;
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
			var image = tab.Image;
			if(image != null)
			{
				switch(tab.Orientation)
				{
					case Orientation.Horizontal:
						graphics.DrawImage(image, imageRect);
						bounds.Width -= imageRect.Width + 3;
						bounds.X += imageRect.Width + 3;
						break;
					case Orientation.Vertical:
						using(var rotatedImage = (Image)image.Clone())
						{
							rotatedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
							graphics.DrawImage(rotatedImage, imageRect);
						}
						bounds.Height -= imageRect.Height + 3;
						bounds.Y += imageRect.Height + 3;
						break;
					default:
						throw new ApplicationException(
							string.Format("Unexpected ViewTabBase.Orientation value: {0}", tab.Orientation));
				}
			}
			using(var textBrush = new SolidBrush(foregroundColor))
			{
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
						throw new ApplicationException(
							string.Format("Unexpected ViewTabBase.Orientation value: {0}", tab.Orientation));
				}
			}
		}

		public override void RenderViewDockSideTabBackground(ViewDockSideTab tab, Graphics graphics, Rectangle bounds)
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
					throw new ApplicationException(
						string.Format("Unexpected DockSide.Side value: {0}", tab.Side.Anchor));
			}
			var color = tab.IsMouseOver ?
				ColorTable.DockSideTabOutlineHover :
				ColorTable.DockSideTabOutline;
			using(var brush = new SolidBrush(color))
			{
				graphics.FillRectangle(brush, rcOutline);
			}
		}

		public override void RenderViewDockSideTabContent(ViewDockSideTab tab, Graphics graphics, Rectangle bounds)
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
					using(var brush = new SolidBrush(host.IsActive ?
						ColorTable.DocTabBackgroundSelectedActive :
						ColorTable.DocTabBackgroundSelected))
					{
						graphics.FillRectangle(brush, bounds);
					}
				}
				else if(tab.IsMouseOver)
				{
					using(var brush = new SolidBrush(ColorTable.DocTabBackgroundHover))
					{
						graphics.FillRectangle(brush, bounds);
					}
				}
			}
			else
			{
				if(tab.IsActive)
				{
					using(var brush = new SolidBrush(ColorTable.ToolTabBackgroundActive))
					{
						graphics.FillRectangle(brush, bounds);
					}
				}
				else
				{
					if(tab.IsMouseOver)
					{
						using(var brush = new SolidBrush(ColorTable.ToolTabBackgroundHover))
						{
							graphics.FillRectangle(brush, bounds);
						}
					}
				}
				if(tab.Tabs[0] != tab)
				{
					using(var pen = new Pen(ColorTable.ToolTabSeparator))
					{
						graphics.DrawLine(pen, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
					}
				}
			}
		}

		public override void RenderViewHostTabContent(ViewHostTab tab, Graphics graphics, Rectangle bounds)
		{
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
			if(tab.Buttons != null && tab.Buttons.Count != 0)
			{
				var buttonsBounds = new Rectangle(bounds.Right - tab.Buttons.Width - 2, 0, tab.Buttons.Width, bounds.Height);
				tab.Buttons.OnPaint(graphics, buttonsBounds, !tab.IsActive || tab.View.Host.IsActive);
			}
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
				var tabsFooterColor = tabs.ViewHost.IsActive ?
					ColorTable.DocTabsFooterActive :
					ColorTable.DocTabsFooterNormal;
				using(var brush = new SolidBrush(tabsFooterColor))
				{
					graphics.FillRectangle(brush, 0, Constants.TabHeight, tabs.Width, Constants.TabFooterHeight);
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
				var backgroundColor = pressed ?
					ColorTable.ViewButtonPressedBackground :
					focus ? ColorTable.ViewButtonHoverBackgroundActive : ColorTable.ViewButtonHoverBackgroundInactive;
				using(var brush = new SolidBrush(backgroundColor))
				{
					graphics.FillRectangle(brush, rc);
				}
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
						int x = bounds.X + (bounds.Width - 10) / 2;
						int y = bounds.Y + (bounds.Height - 10) / 2;
						using(var pen = new Pen(Color.FromArgb(120, ColorTable.ViewButtonForeground)))
						{
							graphics.DrawLine(pen, x + 0, y + 1, x + 7, y + 8);
							graphics.DrawLine(pen, x + 2, y + 1, x + 9, y + 8);
							graphics.DrawLine(pen, x + 7, y + 1, x + 0, y + 8);
							graphics.DrawLine(pen, x + 9, y + 1, x + 2, y + 8);
						}
						using(var pen = new Pen(ColorTable.ViewButtonForeground))
						{
							graphics.DrawLine(pen, x + 1, y + 1, x + 8, y + 8);
							graphics.DrawLine(pen, x + 8, y + 1, x + 1, y + 8);
						}
					}
					break;
				default:
					if(viewButton.Image != null)
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

			using(var brush = new SolidBrush(backgroundColor))
			{
				graphics.FillRectangle(brush, e.ClipRectangle);
			}

			var client = header.ClientRectangle;
			client.X += BeforeContent;
			client.Width -= BeforeContent;
			if(header.Buttons.Count != 0)
			{
				client.Width -= header.Buttons.Width + BetweenTextAndButtons;
			}
			if(client.Width > 1)
			{
				int textWidth;
				using(var brush = new SolidBrush(textColor))
				{
					graphics.TextRenderingHint = Utility.TextRenderingHint;
					graphics.TextContrast = Utility.TextContrast;
					textWidth = GitterApplication.TextRenderer.MeasureText(
						graphics,
						header.Text,
						GitterApplication.FontManager.UIFont,
						client.Width).Width;
					GitterApplication.TextRenderer.DrawText(
						graphics,
						header.Text,
						GitterApplication.FontManager.UIFont,
						brush,
						client,
						ViewHostHeaderTextFormat);
				}
				client.X += textWidth + 6;
				client.Width -= textWidth + 6;
				if(client.Width > 1)
				{
					const int AccentHeight = 5;
					client.Y = (client.Height - AccentHeight) / 2;
					client.Height = AccentHeight;

					using(var brush = new HatchBrush(HatchStyle.Percent20, accentColor, backgroundColor))
					{
						var ro = graphics.RenderingOrigin;
						graphics.RenderingOrigin = new Point(client.X % 4, client.Y % 4);
						graphics.FillRectangle(brush, client);
						graphics.RenderingOrigin = ro;
					}
				}
			}
		}

		#endregion

		#region View Dock Side Rendering

		public override void RenderViewDockSide(ViewDockSide side, PaintEventArgs e)
		{
			using(var brush = new SolidBrush(ColorTable.DockSideBackground))
			{
				e.Graphics.FillRectangle(brush, e.ClipRectangle);
			}
		}

		#endregion

		#region Dock Marker Button

		private static void PaintDockMarkerButtonBackground(Graphics graphics, Rectangle bounds, bool hover)
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
				graphics.FillRoundedRectangle(brush, bounds, 2);
			}
			graphics.SmoothingMode = oldMode;
		}

		private static void InitDockMarkerButtonContentColors(bool hover, out Color arrow, out Color content, out Color border)
		{
			if(hover)
			{
				arrow	= ColorTable.DockMarkerButtonContentArrow;
				border	= ColorTable.DockMarkerButtonContentBorder;
			}
			else
			{
				var alpha = (byte)(ViewConstants.OpacityNormal * 255);
				arrow	= Color.FromArgb(alpha, ColorTable.DockMarkerButtonContentArrow);
				border	= Color.FromArgb(alpha, ColorTable.DockMarkerButtonContentBorder);
			}
			content	= ColorTable.DockMarkerButtonBackground;
		}

		private static void PaintDockMarkerTopButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color arrowColor, contentColor, borderColor;
			InitDockMarkerButtonContentColors(hover, out arrowColor, out contentColor, out borderColor);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 12);
			var arrow = new PointF[]
				{
					new PointF(rect.X + 11.5f, rect.Y + 25.5f),
					new PointF(rect.X + 15.5f, rect.Y + 21.5f),
					new PointF(rect.X + 16.5f, rect.Y + 21.5f),
					new PointF(rect.X + 19.5f, rect.Y + 25.5f),
				};
			using(var brush = new SolidBrush(borderColor))
			{
				graphics.FillRectangle(brush, rc);
			}
			using(var brush = new SolidBrush(arrowColor))
			{
				graphics.FillPolygon(brush, arrow);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new SolidBrush(contentColor))
			{
				graphics.FillRectangle(brush, rc);
			}
		}

		private static void PaintDockMarkerDocumentTopButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color arrowColor, contentColor, borderColor;
			InitDockMarkerButtonContentColors(hover, out arrowColor, out contentColor, out borderColor);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(borderColor))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new SolidBrush(contentColor))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.Height = 11;
			using(var pen = new Pen(borderColor)
			{
				DashStyle = DashStyle.Dot,
			})
			{
				graphics.DrawLine(pen, rc.X, rc.Bottom - 1, rc.Right, rc.Bottom - 1);
			}
		}

		private static void PaintDockMarkerLeftButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color arrowColor, contentColor, borderColor;
			InitDockMarkerButtonContentColors(hover, out arrowColor, out contentColor, out borderColor);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 12, 24);
			var arrow = new PointF[]
				{
					new PointF(rect.X + 25.5f, rect.Y + 11.5f),
					new PointF(rect.X + 25.5f, rect.Y + 19.5f),
					new PointF(rect.X + 21.5f, rect.Y + 15.5f),
					new PointF(rect.X + 21.5f, rect.Y + 14.5f),
				};
			using(var brush = new SolidBrush(arrowColor))
			{
				graphics.FillPolygon(brush, arrow);
			}
			using(var brush = new SolidBrush(borderColor))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new SolidBrush(contentColor))
			{
				graphics.FillRectangle(brush, rc);
			}
		}

		private static void PaintDockMarkerDocumentLeftButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color arrowColor, contentColor, borderColor;
			InitDockMarkerButtonContentColors(hover, out arrowColor, out contentColor, out borderColor);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(borderColor))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new SolidBrush(contentColor))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.Width = 11;
			using(var pen = new Pen(borderColor)
			{
				DashStyle = DashStyle.Dot,
			})
			{
				graphics.DrawLine(pen, rc.Right - 1, rc.Y, rc.Right - 1, rc.Bottom);
			}
		}

		private static void PaintDockMarkerFillButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color arrowColor, contentColor, borderColor;
			InitDockMarkerButtonContentColors(hover, out arrowColor, out contentColor, out borderColor);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(borderColor))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new SolidBrush(contentColor))
			{
				graphics.FillRectangle(brush, rc);
			}
		}

		private static void PaintDockMarkerRightButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color arrowColor, contentColor, borderColor;
			InitDockMarkerButtonContentColors(hover, out arrowColor, out contentColor, out borderColor);
			var rc = new Rectangle(rect.X + 16, rect.Y + 4, 12, 24);
			var arrow = new PointF[]
				{
					new PointF(rect.X + 5.5f, rect.Y + 20.5f),
					new PointF(rect.X + 5.5f, rect.Y + 11.5f),
					new PointF(rect.X + 9.5f, rect.Y + 14.5f),
					new PointF(rect.X + 9.5f, rect.Y + 16.5f),
				};
			using(var brush = new SolidBrush(borderColor))
			{
				graphics.FillRectangle(brush, rc);
			}
			using(var brush = new SolidBrush(arrowColor))
			{
				graphics.FillPolygon(brush, arrow);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new SolidBrush(contentColor))
			{
				graphics.FillRectangle(brush, rc);
			}
		}

		private static void PaintDockMarkerDocumentRightButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color arrowColor, contentColor, borderColor;
			InitDockMarkerButtonContentColors(hover, out arrowColor, out contentColor, out borderColor);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(borderColor))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new SolidBrush(contentColor))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 11;
			rc.Width = 11;
			using(var pen = new Pen(borderColor)
			{
				DashStyle = DashStyle.Dot,
			})
			{
				graphics.DrawLine(pen, rc.X, rc.Y, rc.X, rc.Bottom);
			}
		}

		private static void PaintDockMarkerBottomButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color arrowColor, contentColor, borderColor;
			InitDockMarkerButtonContentColors(hover, out arrowColor, out contentColor, out borderColor);
			var rc = new Rectangle(rect.X + 4, rect.Y + 16, 24, 12);
			var arrow = new PointF[]
				{
					new PointF(rect.X + 10.5f, rect.Y + 5.5f),
					new PointF(rect.X + 15.5f, rect.Y + 9.5f),
					new PointF(rect.X + 16.5f, rect.Y + 9.5f),
					new PointF(rect.X + 20.5f, rect.Y + 5.5f),
				};
			using(var brush = new SolidBrush(borderColor))
			{
				graphics.FillRectangle(brush, rc);
			}
			using(var brush = new SolidBrush(arrowColor))
			{
				graphics.FillPolygon(brush, arrow);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new SolidBrush(contentColor))
			{
				graphics.FillRectangle(brush, rc);
			}
		}

		private static void PaintDockMarkerDocumentBottomButton(Graphics graphics, Rectangle rect, bool hover)
		{
			Color arrowColor, contentColor, borderColor;
			InitDockMarkerButtonContentColors(hover, out arrowColor, out contentColor, out borderColor);
			var rc = new Rectangle(rect.X + 4, rect.Y + 4, 24, 24);
			using(var brush = new SolidBrush(borderColor))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.X += 1;
			rc.Y += 3;
			rc.Width -= 2;
			rc.Height -= 4;
			using(var brush = new SolidBrush(contentColor))
			{
				graphics.FillRectangle(brush, rc);
			}
			rc.Y += 9;
			rc.Height = 11;
			using(var pen = new Pen(borderColor)
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

			var textColor		= ColorTable.ViewHostHeaderTextFocused;
			var backgroundColor	= ColorTable.ViewHostHeaderBackgroundFocused;
			var accentColor		= ColorTable.ViewHostHeaderAccentFocused;

			using(var brush = new SolidBrush(backgroundColor))
			{
				graphics.FillRectangle(brush, e.ClipRectangle);
			}

			var client = header.ClientRectangle;
			client.X += BeforeContent;
			client.Width -= BeforeContent;
			if(header.Buttons.Count != 0)
			{
				client.Width -= header.Buttons.Width + BetweenTextAndButtons;
			}
			if(client.Width > 1)
			{
				int textWidth;
				using(var brush = new SolidBrush(textColor))
				{
					graphics.TextRenderingHint = Utility.TextRenderingHint;
					graphics.TextContrast = Utility.TextContrast;
					textWidth = GitterApplication.TextRenderer.MeasureText(
						graphics,
						header.Text,
						GitterApplication.FontManager.UIFont,
						client.Width).Width;
					GitterApplication.TextRenderer.DrawText(
						graphics,
						header.Text,
						GitterApplication.FontManager.UIFont,
						brush,
						client,
						ViewHostHeaderTextFormat);
				}
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
}
