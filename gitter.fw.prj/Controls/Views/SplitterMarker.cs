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
					throw new ArgumentException(
						"Unknown Orientation value: {0}".UseAsFormat(orientation),
						"orientation");
			}
		}

		public new void Show()
		{
			User32.ShowWindow(this.Handle, 8);
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
