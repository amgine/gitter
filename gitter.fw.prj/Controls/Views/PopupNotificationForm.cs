namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public sealed class PopupNotificationForm : Form
	{
		private readonly NotificationContent _content;
		private Timer _timer;

		public PopupNotificationForm(NotificationContent content)
		{
			if(content == null) throw new ArgumentNullException("content");

			_content		= content;
			Text			= content.Text;
			Font			= GitterApplication.FontManager.UIFont;
			BackColor		= GitterApplication.Style.ViewRenderer.BackgroundColor;
			FormBorderStyle	= FormBorderStyle.None;
			StartPosition	= FormStartPosition.Manual;
			Padding			= new Padding(ViewConstants.FloatBorderSize);
			ShowInTaskbar	= false;
			ShowIcon		= false;
			ControlBox		= false;
			MinimizeBox		= false;
			MaximizeBox		= true;
			TopMost			= true;

			var header = new PopupNotificationHeader()
			{
				Text = content.Text,
				Bounds = new Rectangle(
					ViewConstants.FloatBorderSize,
					ViewConstants.FloatBorderSize,
					ViewConstants.PopupWidth - ViewConstants.FloatBorderSize * 2,
					ViewManager.Renderer.HeaderHeight),
				Parent	= this,
			};

			var cr = ClientRectangle;
			content.Width	= ViewConstants.PopupWidth - ViewConstants.FloatBorderSize * 2;
			content.Top		= cr.Top + ViewManager.Renderer.HeaderHeight + ViewConstants.FloatBorderSize;
			ClientSize		= new Size(ViewConstants.PopupWidth, content.Height + ViewManager.Renderer.HeaderHeight + ViewConstants.FloatBorderSize * 2);
			content.Left	= ViewConstants.FloatBorderSize;
			content.Parent	= this;

			AssignEventHandlers(this);
		}

		private void AssignEventHandlers(Control control)
		{
			foreach(Control ctl in control.Controls)
			{
				ctl.MouseEnter += (s, e) => PreventAutoClose();
				ctl.MouseLeave += (s, e) => AllowAutoClose();

				if(ctl.HasChildren)
				{
					AssignEventHandlers(ctl);
				}
			}
		}

		private void PreventAutoClose()
		{
			if(_timer != null)
			{
				_timer.Enabled = false;
			}
		}

		private void AllowAutoClose()
		{
			if(_timer != null)
			{
				_timer.Enabled = true;
			}
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			PreventAutoClose();
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			AllowAutoClose();
			base.OnMouseLeave(e);
		}

		public new void Show()
		{
			NativeMethods.ShowWindow(this.Handle, 8);
			if(_content.Timeout != TimeSpan.MaxValue)
			{
				_timer = new Timer();
				_timer.Interval = (int)_content.Timeout.TotalMilliseconds;
				_timer.Tick += OnTimerTick;
				_timer.Enabled = true;
			}
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

		private void OnTimerTick(object sender, EventArgs e)
		{
			_timer.Enabled = false;
			Close();
		}

		protected override bool ShowWithoutActivation
		{
			get { return true; }
		}

		protected override CreateParams CreateParams
		{
			get
			{
				const int WS_EX_TOOLWINDOW = 0x80;
				var cp = base.CreateParams;
				cp.ExStyle |= WS_EX_TOOLWINDOW;
				return cp;
			}
		}

		protected override void OnResize(EventArgs e)
		{
			var region = Region;
			Region = Utility.GetRoundedRegion(ClientRectangle, ViewConstants.FloatCornderRadius);
			if(region != null) region.Dispose();
			base.OnResize(e);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_timer != null)
				{
					_timer.Dispose();
					_timer = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
