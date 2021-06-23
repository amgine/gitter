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
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Native;

	public sealed class PopupNotificationForm : Form
	{
		private readonly NotificationContent _content;
		private readonly PopupNotificationHeader _header;
		private Timer _timer;

		public PopupNotificationForm(NotificationContent content)
		{
			Verify.Argument.IsNotNull(content, nameof(content));

			_content        = content;
			Text            = content.Text;
			Font            = GitterApplication.FontManager.UIFont;
			BackColor       = GitterApplication.Style.ViewRenderer.BackgroundColor;
			FormBorderStyle = FormBorderStyle.None;
			StartPosition   = FormStartPosition.Manual;
			Padding         = new Padding(Renderer.FloatBorderSize);
			ShowInTaskbar   = false;
			ShowIcon        = false;
			ControlBox      = false;
			MinimizeBox     = false;
			MaximizeBox     = false;
			TopMost         = false;

			_header = new PopupNotificationHeader()
			{
				Text = content.Text,
				Bounds = new Rectangle(
					Renderer.FloatBorderSize,
					Renderer.FloatBorderSize,
					ViewConstants.PopupWidth - Renderer.FloatBorderSize * 2,
					ViewManager.Renderer.HeaderHeight),
				Parent	= this,
			};

			var cr = ClientRectangle;
			content.Width	= ViewConstants.PopupWidth - Renderer.FloatBorderSize * 2;
			content.Top		= cr.Top + ViewManager.Renderer.HeaderHeight + Renderer.FloatBorderSize;
			ClientSize		= new Size(ViewConstants.PopupWidth, content.Height + Renderer.HeaderHeight + Renderer.FloatBorderSize * 2);
			content.Left	= Renderer.FloatBorderSize;
			content.Parent	= this;

			AssignEventHandlers(this);
		}

		private ViewRenderer Renderer => ViewManager.Renderer;

		private void AssignEventHandlers(Control control)
		{
			foreach(Control ctl in control.Controls)
			{
				ctl.MouseEnter += (_, _) => PreventAutoClose();
				ctl.MouseLeave += (_, _) => AllowAutoClose();

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

		/// <inheritdoc/>
		protected override void OnMouseEnter(EventArgs e)
		{
			PreventAutoClose();
			base.OnMouseEnter(e);
		}

		/// <inheritdoc/>
		protected override void OnMouseLeave(EventArgs e)
		{
			AllowAutoClose();
			base.OnMouseLeave(e);
		}

		public void Prepare()
		{
			if(!IsHandleCreated)
			{
				CreateControl();
			}
			if(DeviceDpi != 96)
			{
				var conv = new DpiConverter(this);
				var cr = ClientRectangle;
				_content.Width = conv.ConvertX(ViewConstants.PopupWidth) - Renderer.FloatBorderSize * 2;
				_content.Top   = cr.Top + Renderer.HeaderHeight + Renderer.FloatBorderSize;
				ClientSize = new Size(conv.ConvertX(ViewConstants.PopupWidth), _content.Height + Renderer.HeaderHeight + Renderer.FloatBorderSize * 2);
				_content.Left = Renderer.FloatBorderSize;

				_header.Size = new Size(_content.Width, Renderer.HeaderHeight);
			}
		}

		public new void Show()
		{
			Owner = GitterApplication.MainForm;

			User32.ShowWindow(this.Handle, 4);
			User32.SetWindowPos(this.Handle, new IntPtr(-1), 0, 0, 0, 0, 0x0010 | 0x0002 | 0x0001);
			if(_content.Timeout != TimeSpan.MaxValue)
			{
				_timer = new Timer();
				_timer.Interval = (int)_content.Timeout.TotalMilliseconds;
				_timer.Tick += OnTimerTick;
				_timer.Enabled = true;
			}
		}

		/// <inheritdoc/>
		protected override void DefWndProc(ref Message m)
		{
			const int MA_NOACTIVATE = 0x0003;

			switch(m.Msg)
			{
				case (int)WM.MOUSEACTIVATE:
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

		/// <inheritdoc/>
		protected override bool ShowWithoutActivation => true;

		/// <inheritdoc/>
		protected override CreateParams CreateParams
		{
			get
			{
				const int WS_EX_TOOLWINDOW = 0x00000080;
				const int WS_EX_NOACTIVATE = 0x08000000;
				var cp = base.CreateParams;
				cp.ExStyle |= WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
				return cp;
			}
		}

		/// <inheritdoc/>
		protected override void OnResize(EventArgs e)
		{
			var cornerRadius = Renderer.FloatCornerRadius;
			if(cornerRadius != 0)
			{
				Region = GraphicsUtility.GetRoundedRegion(ClientRectangle, cornerRadius);
			}
			base.OnResize(e);
		}

		/// <inheritdoc/>
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
