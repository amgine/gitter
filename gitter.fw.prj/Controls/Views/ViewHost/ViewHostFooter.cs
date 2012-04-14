namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;
	using System.Drawing;
	using System.Text;

	sealed class ViewHostFooter : Control
	{
		private static readonly Color Background = Color.FromArgb(41, 57, 85);
		private static readonly Color ForegroundNormal = Color.FromArgb(206, 212, 223);
		private static readonly Color ForegroundActive = Color.FromArgb(255, 232, 166);

		private readonly ViewHost _host;

		public ViewHostFooter(ViewHost host)
		{
			if(host == null) throw new ArgumentNullException("host");
			_host = host;

			SetStyle(
				ControlStyles.ContainerControl |
				ControlStyles.Selectable |
				ControlStyles.SupportsTransparentBackColor,
				false);
			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.OptimizedDoubleBuffer,
				true);
		}

		public ViewHost ViewHost
		{
			get { return _host; }
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			using(var brush = new SolidBrush(Background))
			{
				e.Graphics.FillRectangle(brush, e.ClipRectangle);
			}
			var color = _host.IsActive ? ForegroundActive : ForegroundNormal;
			using(var brush = new SolidBrush(color))
			{
				var rc = (RectangleF)ClientRectangle;
				rc.X -= 0.5f;
				rc.Y -= 0.5f;
				rc.Width += 1;
				rc.Height += 1;
				e.Graphics.FillRoundedRectangle(brush, rc, 0, 0, 2, 2);
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			_host.Activate();
			base.OnMouseDown(e);
		}
	}
}
