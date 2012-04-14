namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Globalization;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	using Resources = gitter.Git.Properties.Resources;

	public abstract class FilePanel : FlowPanel
	{
		protected static readonly StringFormat HeaderFormat = new StringFormat(StringFormat.GenericDefault)
		{
			Alignment = StringAlignment.Near,
			FormatFlags =
				StringFormatFlags.LineLimit |
				StringFormatFlags.NoClip |
				StringFormatFlags.NoWrap,
			LineAlignment = StringAlignment.Center,
			Trimming = StringTrimming.None,
		};

		protected static readonly StringFormat ContentFormat = new StringFormat(StringFormat.GenericTypographic)
		{
			Alignment = StringAlignment.Near,
			FormatFlags =
				StringFormatFlags.LineLimit |
				StringFormatFlags.NoClip |
				StringFormatFlags.NoWrap |
				StringFormatFlags.FitBlackBox |
				StringFormatFlags.MeasureTrailingSpaces,
			LineAlignment = StringAlignment.Near,
			Trimming = StringTrimming.None,
		};

		private static readonly Color HeaderColor1 = Color.FromArgb(245, 245, 245);
		private static readonly Color HeaderColor2 = Color.FromArgb(232, 232, 232);

		protected static readonly Brush LineHoverBackground =
			new SolidBrush(ColorScheme.LineHoverBackColor);
		protected static readonly Brush LineSelectedBackground =
			new SolidBrush(ColorScheme.LineSelectedBackColor);
		protected static readonly Brush LineSelectedHoverBackground =
			new SolidBrush(ColorScheme.LineSelectedHoverBackColor);

		protected static readonly Brush LineNumberText =
			new SolidBrush(ColorScheme.LineNumberText);
		protected static readonly Brush LineNumberBackground =
			new SolidBrush(ColorScheme.LineNumberBackColor);
		protected static readonly Brush LineNumberHoverBackground =
			new SolidBrush(ColorScheme.LineNumberHoverBackColor);

		protected static readonly Size CellSize;

		protected const int HeaderHeight = 35;
		protected const int Margin = 5;
		protected const int MinDigits = 4;

		static FilePanel()
		{
			using(var bmp = new Bitmap(1, 1))
			{
				using(var graphics = Graphics.FromImage(bmp))
				{
					CellSize = GitterApplication.TextRenderer.MeasureText(
						graphics,
						"0",
						GitterApplication.FontManager.ViewerFont.Font,
						int.MaxValue / 2,
						StringFormat.GenericTypographic);
				}
			}

			float tabSize = CellSize.Width * TabSize;
			ContentFormat.SetTabStops(TabSize, new[]
				{ tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, tabSize, });
		}

		protected virtual bool ShowHeader
		{
			get { return true; }
		}

		protected static int TabSize
		{
			get
			{
				return (GitterApplication.TextRenderer == GitterApplication.GdiPlusTextRenderer) ? 4 : 8;
			}
		}

		protected Rectangle GetLineBounds(int line)
		{
			int contentWidth = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
			int x = Margin;
			int w = contentWidth - Margin * 2;
			int yOffset = ShowHeader ? HeaderHeight : 1;
			return new Rectangle(x, yOffset + line * CellSize.Height, w, CellSize.Height);
		}

		protected Rectangle GetLineBounds(int line, int count)
		{
			int contentWidth = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
			int x = Margin;
			int w = contentWidth - Margin * 2;
			int yOffset = ShowHeader ? HeaderHeight : 1;
			return new Rectangle(x, yOffset + line * CellSize.Height, w, CellSize.Height * count);
		}

		protected static void PaintHeader(Graphics graphics, Rectangle rcHeader, Image icon, Image overlay, string text)
		{
			var font = GitterApplication.FontManager.ViewerFont.Font;
			using(var brush = new LinearGradientBrush(
				rcHeader, HeaderColor1, HeaderColor2, LinearGradientMode.Vertical))
			{
				graphics.FillRectangle(brush, rcHeader);
			}
			var rcBorder = rcHeader;
			--rcBorder.Height;
			graphics.DrawRectangle(Pens.Gray, rcBorder);
			var rcImage = new Rectangle(0, 0, 16, 16);
			const int offset = (HeaderHeight - 16) / 2;
			var rcStatus = new Rectangle(rcHeader.X + offset, rcHeader.Y + offset, 16, 16);
			graphics.DrawImage(icon, rcStatus, rcImage, GraphicsUnit.Pixel);
			if(overlay != null)
			{
				graphics.DrawImage(overlay, rcStatus, rcImage, GraphicsUnit.Pixel);
			}

			rcHeader.X += offset * 2 + rcImage.Width;
			rcHeader.Width -= offset * 2 + rcImage.Width;
			GitterApplication.TextRenderer.DrawText(
				graphics, text, font, SystemBrushes.WindowText, rcHeader, HeaderFormat);
		}

		protected static int GetDecimalDigits(int num)
		{
			if(num < 1000)
			{
				return MinDigits;
			}
			else
			{
				var digits = 1;
				while(num != 0)
				{
					num /= 10;
					++digits;
				}
				return digits;
			}
		}
	}
}
