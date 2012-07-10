namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	public partial class NotificationForm : Form
	{
		public NotificationForm()
		{
			InitializeComponent();
		}

		protected override bool ShowWithoutActivation
		{
			get { return true; }
		}

		protected override CreateParams CreateParams
		{
			get
			{
				const int WS_SIZEBOX = 0x00040000;
				const int WS_EX_NOACTIVATE = 0x08000000;
				var cp = base.CreateParams;
				cp.Style |= WS_SIZEBOX;
				cp.ExStyle |= WS_EX_NOACTIVATE;
				return cp;
			}
		}

		public new void Show()
		{
			var scr = Screen.PrimaryScreen.WorkingArea;
			var border = SystemInformation.FixedFrameBorderSize;
			var size = ClientSize;
			const int margin = 17;
			Location = new Point(
				scr.Width - size.Width + border.Width - margin,
				scr.Height - size.Height + border.Height - margin);
			NativeMethods.ShowWindow(this.Handle, 8);
			NativeMethods.SetWindowPos(
				this.Handle, (IntPtr)(-1),
				0, 0, 0, 0,
				0x0010 | 0x0002 | 0x001);
		}

		/// <summary>
		/// </summary>
		/// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message"/> to process.</param>
		protected override void WndProc(ref Message m)
		{
			bool processed = false;
			switch((WindowsMessage)m.Msg)
			{
				case WindowsMessage.WM_NCHITTEST:
					m.Result = (IntPtr)1;
					processed = true;
					break;
				case WindowsMessage.WM_MOUSEACTIVATE:
					m.Result = (IntPtr)4;
					processed = true;
					return;
			}
			if(!processed)
			{
				base.WndProc(ref m);
			}
		}
	}
}
