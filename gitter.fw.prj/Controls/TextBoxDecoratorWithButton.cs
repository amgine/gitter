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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

[DesignerCategory("")]
public abstract class TextBoxDecoratorWithButton(TextBox textBox)
	: TextBoxDecorator(textBox)
{
	private bool _showButton = true;
	private bool _isMouseOverButton;

	protected abstract void OnButtonClick();

	protected virtual Bitmap? GetIcon(Dpi dpi) => default;

	public bool ShowButton
	{
		get => _showButton;
		set
		{
			if(_showButton == value) return;

			_showButton = value;
			IsMouseOverButton = false;
			UpdateDecoratedBounds();
			Invalidate();
		}
	}

	protected bool IsMouseOverButton
	{
		get => _isMouseOverButton;
		private set
		{
			if(_isMouseOverButton == value) return;

			_isMouseOverButton = value;
			Cursor = value ? Cursors.Hand : Cursors.IBeam;
			Invalidate();
		}
	}

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Assert.IsNotNull(e);

		base.OnPaint(e);
		if(ShowButton)
		{
			PaintButton(e.Graphics);
		}
	}

	private void PaintButton(Graphics graphics)
	{
		Assert.IsNotNull(graphics);

		var bounds = GetButtonBounds();
		PaintButton(graphics, bounds);
	}

	protected virtual void PaintButton(Graphics graphics, Rectangle bounds)
	{
		var icon = GetIcon(Dpi.FromControl(this));
		if(icon is null) return;

		PaintImageMaskIcon(graphics, icon, bounds);
	}

	const int iconPadding = 4;
	const int iconSize = 16;

	private Rectangle GetButtonBounds()
	{
		var conv    = DpiConverter.FromDefaultTo(this);
		var padding = conv.ConvertX(iconPadding);
		var size    = conv.Convert(new Size(iconSize, iconSize));
		return new Rectangle(Width - padding - size.Width, (Height - size.Height) / 2, size.Width, size.Height);
	}

	protected void PaintImageMaskIcon(Graphics graphics, Bitmap icon, Rectangle bounds)
	{
		var colorTable = GetColorTable();
		Color color;
		if(!Enabled)               color = colorTable.Disabled.ForeColor;
		else if(IsMouseOverButton) color = colorTable.Hover.ForeColor;
		else                       color = colorTable.Normal.ForeColor;

		using var attr = new ImageAttributes();
		attr.SetColorMatrix(new()
		{
			Matrix00 = 0,
			Matrix11 = 0,
			Matrix22 = 0,
			Matrix30 = color.R / 255.0f,
			Matrix31 = color.G / 255.0f,
			Matrix32 = color.B / 255.0f,
			Matrix33 = IsMouseOverButton ? 1.0f : 0.5f,
		});

		graphics.DrawImage(icon, bounds, 0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel, attr);
	}

	/// <inheritdoc/>
	protected override Rectangle ModifyDecoratedBounds(Rectangle bounds)
	{
		if(ShowButton)
		{
			var conv = DpiConverter.FromDefaultTo(this);
			bounds.Width -= conv.ConvertX(iconPadding) + conv.ConvertX(iconSize);
		}
		return bounds;
	}

	/// <inheritdoc/>
	protected override void OnMouseMove(MouseEventArgs e)
	{
		Assert.IsNotNull(e);

		IsMouseOverButton = ShowButton && GetButtonBounds().Contains(e.X, e.Y);
		base.OnMouseMove(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		IsMouseOverButton = false;
		base.OnMouseLeave(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		Assert.IsNotNull(e);

		if(TryHandleMouseDown(e))
		{
			OnButtonClick();
		}
		base.OnMouseDown(e);
	}

	/// <inheritdoc/>
	protected override bool TryHandleMouseDown(MouseEventArgs e)
		=> ShowButton && e.Button == MouseButtons.Left && GetButtonBounds().Contains(e.X, e.Y);
}
