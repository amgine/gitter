#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Native;
	using gitter.Framework.Hooks;

	public sealed class DragImage : Form
	{
		#region Data

		private readonly int _dx;
		private readonly int _dy;
		private Action<PaintEventArgs> _paintProc;
		private Timer _timer;
		private LowLevelMouseHook _hook;

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
			point.X -= _dx;
			point.Y -= _dy;
			Location = point;
		}

		public void UpdatePosition(Rectangle bounds)
		{
			bounds.X -= _dx;
			bounds.Y -= _dy;
			Bounds = bounds;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.Clear(TransparencyKey);
			e.Graphics.TextContrast      = GraphicsUtility.TextContrast;
			e.Graphics.TextRenderingHint = GraphicsUtility.TextRenderingHint;
			if(_paintProc != null) _paintProc(e);
		}

		public void ShowDragVisual(Control control)
		{
			UpdatePosition();
			User32.ShowWindow(this.Handle, 8);
			User32.SetWindowPos(
				this.Handle, (IntPtr)(-1),
				0, 0, 0, 0,
				0x0010 | 0x0002 | 0x001);

			try
			{
				_hook = new LowLevelMouseHook();
				_hook.Activate(0);
			}
			catch(Win32Exception)
			{
				if(_hook != null)
				{
					_hook.Dispose();
					_hook = null;
				}
			}

			if(_hook == null)
			{
				_timer.Enabled = true;
				if(control != null)
				{
					control.GiveFeedback += OnControlGiveFeedback;
				}
			}
			else
			{
				_hook.MouseMove += (s, e) =>
					{
						if(InvokeRequired)
						{
							BeginInvoke(new Action<Point>(UpdatePosition), e.Location);
						}
						else
						{
							UpdatePosition(e.Location);
						}
					};
				_hook.MouseWheel += (s, e) =>
					{
						if(e.Delta == 0)
						{
							return;
						}
						var h = User32.WindowFromPoint(new POINT(e.X, e.Y));
						if(h == IntPtr.Zero)
						{
							return;
						}
						User32.SendMessage(h, WM.MOUSEWHEEL, (IntPtr)(e.Delta << 16), (IntPtr)((e.Y << 16) | e.X));
					};
			}
		}

		private void OnControlGiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			UpdatePosition();
		}

		protected override void DefWndProc(ref Message m)
		{
			const int MA_NOACTIVATE = 0x0003;
			const int HTTRANSPARENT = -1;

			switch((WM)m.Msg)
			{
				case WM.MOUSEACTIVATE:
					m.Result = (IntPtr)MA_NOACTIVATE;
					return;
				case WM.NCHITTEST:
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
				if(_hook != null)
				{
					_hook.Dispose();
					_hook = null;
				}
				_timer.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
