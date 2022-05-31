#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Windows.Forms;
using System.Drawing;

/// <summary>Dock position indicator.</summary>
[System.ComponentModel.DesignerCategory("")]
[System.ComponentModel.ToolboxItem(defaultType: false)]
sealed class DockPositionMarker : Form
{
	/// <summary>Initializes a new instance of the <see cref="DockPositionMarker"/> class.</summary>
	/// <param name="bounds">Dock position marker bounds.</param>
	public DockPositionMarker(Rectangle bounds)
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

		MinimumSize = new Size(1, 1);

		Bounds = bounds;
		AllowTransparency = true;
		BackColor = Color.FromArgb(120, 170, 240);
		Opacity = ViewConstants.DockPositionMarkerOpacity;
	}

	/// <summary>Displays the control to the user.</summary>
	public new void Show()
	{
		const int SW_SHOWNA = 8;
		_ = Native.User32.ShowWindow(Handle, SW_SHOWNA);
	}

	/// <inheritdoc/>
	protected override void DefWndProc(ref Message m)
	{
		const int MA_NOACTIVATE = 0x0003;

		switch((Native.WM)m.Msg)
		{
			case Native.WM.MOUSEACTIVATE:
				m.Result = (IntPtr)MA_NOACTIVATE;
				return;
		}
		base.DefWndProc(ref m);
	}

	/// <inheritdoc/>
	protected override void OnShown(EventArgs e)
	{
		TopMost = true;
	}

	/// <inheritdoc/>
	protected override bool ShowWithoutActivation => true;

	/// <inheritdoc/>
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
