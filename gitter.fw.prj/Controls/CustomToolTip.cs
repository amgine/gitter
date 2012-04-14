namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	public abstract class CustomToolTip : ToolTip
	{
		protected const int VerticalMargin = 2;
		protected const int VerticalSpacing = 3;
		protected const int HorizontalMargin = 5;

		protected CustomToolTip()
		{
			OwnerDraw = true;
			Popup += PopupHandler;
			Draw += DrawHandler;
		}

		public abstract Size Size { get; }

		private static void PopupHandler(object sender, PopupEventArgs e)
		{
			var toolTip = (CustomToolTip)sender;
			e.ToolTipSize = toolTip.Size;
			toolTip.OnPopup(e);
		}

		private static void DrawHandler(object sender, DrawToolTipEventArgs e)
		{
			var toolTip = (CustomToolTip)sender;
			var gx = e.Graphics;
			using(var b = new LinearGradientBrush(e.Bounds,
				Color.FromArgb(255, 255, 255),
				Color.FromArgb(228, 229, 240),
				LinearGradientMode.Vertical))
			{
				gx.FillRectangle(b, e.Bounds);
			}
			using(var p = new Pen(Color.FromArgb(118, 118, 118)))
			{
				gx.DrawRoundedRectangle(p, e.Bounds, 1);
			}
			gx.TextRenderingHint = Utility.TextRenderingHint;
			gx.TextContrast = Utility.TextContrast;
			toolTip.OnPaint(e);
		}

		protected virtual void OnPaint(DrawToolTipEventArgs e)
		{
		}

		protected virtual void OnPopup(PopupEventArgs e)
		{
		}

		public void Show(IWin32Window window, int x, int y)
		{
			Show("-", window, x, y);
		}

		public void Show(IWin32Window window, Point p)
		{
			Show("-", window, p.X, p.Y);
		}
	}
}
