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
		private readonly Rectangle _initialBounds;

		public SplitterMarker(Rectangle bounds, Orientation orientation)
		{
			_initialBounds = bounds;

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
			Cursor = orientation switch
			{
				Orientation.Horizontal => Cursors.SizeWE,
				Orientation.Vertical   => Cursors.SizeNS,
				_ => throw new ArgumentException($"Unknown Orientation value: {orientation}", nameof(orientation)),
			};
		}

		public new void Show()
		{
			User32.SetWindowPos(Handle, IntPtr.Zero,
				_initialBounds.X, _initialBounds.Y, _initialBounds.Width, _initialBounds.Height,
				0x0010 | 0x0040);
		}

		protected override void DefWndProc(ref Message m)
		{
			const int MA_NOACTIVATE = 0x0003;

			switch((WM)m.Msg)
			{
				case WM.MOUSEACTIVATE:
					m.Result = (IntPtr)MA_NOACTIVATE;
					return;
			}
			base.DefWndProc(ref m);
		}

		protected override void OnShown(EventArgs e)
		{
			TopMost = true;
		}

		protected override bool ShowWithoutActivation => true;

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
