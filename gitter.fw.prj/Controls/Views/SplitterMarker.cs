namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	sealed class SplitterMarker : Form
	{
		public SplitterMarker(Rectangle bounds, Orientation orientation)
		{
			SetStyle(
				ControlStyles.ContainerControl |
				ControlStyles.Selectable |
				ControlStyles.ResizeRedraw |
				ControlStyles.SupportsTransparentBackColor,
				false);

			StartPosition = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			ControlBox = false;
			MaximizeBox = false;
			MinimizeBox = false;
			Text = string.Empty;
			ShowIcon = false;
			ShowInTaskbar = false;
			Enabled = false;
			ImeMode = ImeMode.Disable;

			MinimumSize = bounds.Size;
			MaximumSize = bounds.Size;

			Bounds = bounds;
			AllowTransparency = true;
			BackColor = Color.Black;
			Opacity = ViewConstants.SplitterOpacity;
			switch(orientation)
			{
				case Orientation.Horizontal:
					Cursor = Cursors.SizeWE;
					break;
				case Orientation.Vertical:
					Cursor = Cursors.SizeNS;
					break;
				default:
					throw new ArgumentException("orientation");
			}
		}

		public new void Show()
		{
			NativeMethods.ShowWindow(this.Handle, 8);
		}

		protected override void DefWndProc(ref Message m)
		{
			const int WM_MOUSEACTIVATE = 0x21;
			const int MA_NOACTIVATE = 0x0003;

			switch(m.Msg)
			{
				case WM_MOUSEACTIVATE:
					m.Result = (IntPtr)MA_NOACTIVATE;
					return;
			}
			base.DefWndProc(ref m);
		}

		protected override void OnShown(EventArgs e)
		{
			TopMost = true;
		}

		protected override bool ShowWithoutActivation
		{
			get { return true; }
		}

		protected override CreateParams CreateParams
		{
			get
			{
				const int WS_EX_NOACTIVATE = 0x08000000;
				var baseParams = base.CreateParams;
				baseParams.ExStyle |= WS_EX_NOACTIVATE;
				return baseParams;
			}
		}
	}
}
