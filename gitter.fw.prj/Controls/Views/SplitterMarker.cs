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

namespace gitter.Framework.Controls;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Native;

[DesignerCategory("")]
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

		AutoScaleMode = AutoScaleMode.None;

		Bounds = bounds;
		AllowTransparency = true;
		BackColor = Color.Black;
		Opacity = ViewConstants.SplitterOpacity;
		Cursor = orientation switch
		{
			Orientation.Horizontal => Cursors.SizeWE,
			Orientation.Vertical => Cursors.SizeNS,
			_ => throw new ArgumentException($"Unknown Orientation value: {orientation}", nameof(orientation)),
		};
	}

	public new void Show()
	{
		User32.SetWindowPos(Handle, (IntPtr)(-1),
			_initialBounds.X, _initialBounds.Y, _initialBounds.Width, _initialBounds.Height,
			0x0010 | 0x0040 | 0x0200 | 0x0004 | 0x0002);
	}

	protected override void DefWndProc(ref Message m)
	{
		const int MA_NOACTIVATE = 0x0003;

		switch((WM)m.Msg)
		{
			case WM.ACTIVATE:
				m.Result = IntPtr.Zero;
				return;
			case WM.MOUSEACTIVATE:
				m.Result = (IntPtr)MA_NOACTIVATE;
				return;
			case WM.NCHITTEST:
				m.Result = IntPtr.Zero;
				return;
		}
		base.DefWndProc(ref m);
	}

	protected override void WndProc(ref Message m)
	{
		const int MA_NOACTIVATE = 0x0003;

		switch((WM)m.Msg)
		{
			case WM.ACTIVATE:
				m.Result = IntPtr.Zero;
				return;
			case WM.MOUSEACTIVATE:
				m.Result = (IntPtr)MA_NOACTIVATE;
				return;
			case WM.NCHITTEST:
				m.Result = IntPtr.Zero;
				return;
		}
		base.WndProc(ref m);
	}

	protected override void ScaleCore(float x, float y) { }

	protected override void SetClientSizeCore(int x, int y) { }

	protected override void Select(bool directed, bool forward) { }

	protected override void ScaleControl(SizeF factor, BoundsSpecified specified) { }

	protected override Size SizeFromClientSize(Size clientSize) => clientSize;

	protected override bool ScaleChildren => false;

	protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified) => bounds;

	protected override bool CanEnableIme => false;

	protected override bool CanRaiseEvents => false;

	protected override Size DefaultSize => _initialBounds.Size;

	public override Size GetPreferredSize(Size proposedSize) => _initialBounds.Size;

	protected override bool OnGetDpiScaledSize(int deviceDpiOld, int deviceDpiNew, ref Size desiredSize) => true;

	protected override void OnDpiChanged(DpiChangedEventArgs e) { }

	protected override void OnDpiChangedAfterParent(EventArgs e) { }

	protected override void OnDpiChangedBeforeParent(EventArgs e) { }

	protected override void SetVisibleCore(bool value)
	{
		if(value)
		{
			User32.SetWindowPos(Handle, (IntPtr)(-1),
				_initialBounds.X, _initialBounds.Y, _initialBounds.Width, _initialBounds.Height,
				0x0010 | 0x0040 | 0x0200 | 0x0004 | 0x0002);
		}
		base.SetVisibleCore(value);
	}

	protected override bool ShowWithoutActivation => true;

	protected override CreateParams CreateParams
	{
		get
		{
			const int WS_EX_NOACTIVATE = 0x08000000;
			var baseParams = base.CreateParams;
			baseParams.ExStyle |= WS_EX_NOACTIVATE | 0x00000004 | 0x00000008;
			baseParams.X = _initialBounds.X;
			baseParams.Y = _initialBounds.Y;
			baseParams.Width  = _initialBounds.Width;
			baseParams.Height = _initialBounds.Height;
			return baseParams;
		}
	}
}
