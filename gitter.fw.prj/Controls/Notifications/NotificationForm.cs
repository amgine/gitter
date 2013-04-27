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
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Native;

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
			User32.ShowWindow(this.Handle, 8);
			User32.SetWindowPos(
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
			switch((WM)m.Msg)
			{
				case WM.NCHITTEST:
					m.Result = (IntPtr)1;
					processed = true;
					break;
				case WM.MOUSEACTIVATE:
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
