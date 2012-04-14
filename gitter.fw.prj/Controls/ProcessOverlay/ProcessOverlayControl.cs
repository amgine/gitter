namespace gitter.Framework.Controls
{
	using System;
	using System.Windows.Forms;

	public partial class ProcessOverlayControl : UserControl
	{
		private readonly ProcessOverlay _overlay;

		public ProcessOverlayControl()
		{
			InitializeComponent();

			_overlay = new ProcessOverlay(this);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.Clear(BackColor);
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			_overlay.DrawMessage(e.Graphics, Font, ClientRectangle, "Test status");
		}
	}
}
