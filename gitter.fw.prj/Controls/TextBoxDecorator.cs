#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Windows.Forms;

[DesignerCategory("")]
public class TextBoxDecorator : BorderDecorator<TextBoxBase>
{
	public TextBoxDecorator(TextBoxBase textBox) : base(textBox)
	{
		textBox.ReadOnlyChanged += OnTextBoxReadOnlyChanged;
		textBox.BorderStyle = BorderStyle.None;
		UpdateColors();
		Cursor = Cursors.IBeam;
	}

	#if NETCOREAPP
	[System.Diagnostics.CodeAnalysis.NotNull]
	#endif
	public override string? Text
	{
		get => Decorated.Text;
		set => Decorated.Text = value;
	}

	private void OnTextBoxReadOnlyChanged(object? sender, EventArgs e)
	{
		UpdateColors();
		Invalidate();
	}

	/// <inheritdoc/>
	protected override void OnMouseDoubleClick(MouseEventArgs e)
	{
		Decorated.SelectAll();
		base.OnMouseDoubleClick(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		Decorated.Focus();
		if(!TryHandleMouseDown(e))
		{
			SendMouseDownToDecorated(e);
		}
		base.OnMouseDown(e);
	}

	private void SendMouseDownToDecorated(MouseEventArgs e)
	{
		var (msg, wparam) = e.Button switch
		{
			MouseButtons.Left     => (Native.WM.LBUTTONDOWN, 0),
			MouseButtons.Right    => (Native.WM.RBUTTONDOWN, 0),
			MouseButtons.Middle   => (Native.WM.MBUTTONDOWN, 0),
			MouseButtons.XButton1 => (Native.WM.XBUTTONDOWN, 0x0001 << 16),
			MouseButtons.XButton2 => (Native.WM.XBUTTONDOWN, 0x0002 << 16),
			_ => default,
		};
		if(msg != default)
		{
			var x = e.X - Decorated.Left;
			var y = Decorated.Height / 2;
			if(x >= Decorated.Width) x = Decorated.Width - 1;
			if(x < 0) x = 0;
			_ = Native.User32.SendMessage(Decorated.Handle, msg, (nint)wparam, (nint)(x | (y << 16)));
		}
	}

	protected virtual bool TryHandleMouseDown(MouseEventArgs e) => false;

	protected override void UpdateDecoratedBounds()
	{
		var conv = DpiConverter.FromDefaultTo(this);
		var paddingX = conv.ConvertX(3);
		var paddingY = conv.ConvertY(3);

		var bounds = ClientRectangle;
		var th = Decorated.Multiline
			? Height - paddingY * 2
			: Decorated.Height;
		bounds.Y += (bounds.Height - th) / 2;
		bounds.Height = th;
		bounds.X += paddingX;
		bounds.Width -= 2 * paddingX;

		Decorated.Bounds = ModifyDecoratedBounds(bounds);
	}
}
