namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public sealed class DragImage : Form
	{
		#region Data

		private readonly int _dx;
		private readonly int _dy;
		private Action<PaintEventArgs> _paintProc;
		private Timer _timer;

		#endregion

		public DragImage(Size size, int dx, int dy, Action<PaintEventArgs> paintProc)
		{
			_dx = dx;
			_dy = dy;
			_paintProc = paintProc;

			SetStyle(
				ControlStyles.ContainerControl |
				ControlStyles.Selectable |
				ControlStyles.ResizeRedraw,
				false);
			SetStyle(
				ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint,
				true);

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

			MinimumSize = size;
			MaximumSize = size;

			AllowTransparency = true;
			BackColor = Color.Magenta;

			TransparencyKey = Color.Magenta;

			_timer = new Timer()
			{
				Interval = 1,
			};
			_timer.Tick += OnTimerTick;
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			UpdatePosition();
		}

		public void UpdatePosition()
		{
			UpdatePosition(Cursor.Position);
		}

		public void UpdatePosition(Point point)
		{
			var scrPos = point;
			scrPos.X -= _dx;
			scrPos.Y -= _dy;
			Location = scrPos;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.Clear(TransparencyKey);
			e.Graphics.TextContrast = Utility.TextContrast;
			e.Graphics.TextRenderingHint = Utility.TextRenderingHint;
			if(_paintProc != null) _paintProc(e);
		}

		public new void Show()
		{
			UpdatePosition();
			NativeMethods.ShowWindow(this.Handle, 8);
			NativeMethods.SetWindowPos(
				this.Handle, (IntPtr)(-1),
				0, 0, 0, 0,
				0x0010 | 0x0002 | 0x001);
			_timer.Enabled = true;
		}

		protected override void DefWndProc(ref Message m)
		{
			const int WM_MOUSEACTIVATE = 0x21;
			const int WM_NCHITTEST = 0x0084;
			const int MA_NOACTIVATE = 0x0003;
			const int HTTRANSPARENT = -1;

			switch(m.Msg)
			{
				case WM_MOUSEACTIVATE:
					m.Result = (IntPtr)MA_NOACTIVATE;
					return;
				case WM_NCHITTEST:
					m.Result = (IntPtr)HTTRANSPARENT;
					return;
			}
			base.DefWndProc(ref m);
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

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				_timer.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
