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
using System.Drawing;
using System.Windows.Forms;

[System.ComponentModel.DesignerCategory("")]
public sealed class CustomButton : Control, IButtonControl
{
	#region Data

	private CustomButtonRenderer? _renderer;
	private bool _isPressed;
	private bool _isMouseOver;

	#endregion

	#region .ctor

	public CustomButton()
	{
		SetStyle(
			ControlStyles.AllPaintingInWmPaint |
			ControlStyles.OptimizedDoubleBuffer |
			ControlStyles.Opaque |
			ControlStyles.Selectable |
			ControlStyles.ResizeRedraw |
			ControlStyles.UserPaint, true);
		SetStyle(
			ControlStyles.ContainerControl |
			ControlStyles.SupportsTransparentBackColor, false);
		Size = new Size(86, 18);
		TabStop = true;
	}

	#endregion

	#region Properties

	public CustomButtonRenderer Renderer
	{
		get => _renderer ?? CustomButtonRenderer.Default;
		set
		{
			if(_renderer == value) return;

			bool needsInvalidate =
				!(_renderer is null && value == CustomButtonRenderer.Default) &&
				!(_renderer == CustomButtonRenderer.Default && value is null);
			_renderer = value;
			if(needsInvalidate)
			{
				Invalidate();
			}
		}
	}

	public bool IsPressed
	{
		get => _isPressed;
		private set
		{
			if(_isPressed != value)
			{
				_isPressed = value;
				Invalidate();
			}
		}
	}

	public bool IsMouseOver
	{
		get => _isMouseOver;
		private set
		{
			if(_isMouseOver != value)
			{
				_isMouseOver = value;
				Invalidate();
			}
		}
	}

	public bool IsDefault { get; private set; }

	DialogResult IButtonControl.DialogResult { get; set; }

	#endregion

	#region Overrides

	/// <inheritdoc/>
	protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
	{
		Assert.IsNotNull(e);

		switch(e.KeyCode)
		{
			case Keys.Space:
				e.IsInputKey = true;
				break;
			default:
				base.OnPreviewKeyDown(e);
				break;
		}
	}

	/// <inheritdoc/>
	protected override void OnKeyDown(KeyEventArgs e)
	{
		Assert.IsNotNull(e);

		base.OnKeyDown(e);
		switch(e.KeyCode)
		{
			case Keys.Space:
				IsPressed = true;
				break;
		}
	}

	/// <inheritdoc/>
	protected override void OnKeyUp(KeyEventArgs e)
	{
		Assert.IsNotNull(e);

		base.OnKeyUp(e);
		switch(e.KeyCode)
		{
			case Keys.Space:
				IsPressed = false;
				base.OnClick(EventArgs.Empty);
				break;
		}
	}

	/// <inheritdoc/>
	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		Invalidate();
	}

	/// <inheritdoc/>
	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
		Invalidate();
	}

	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Left)
		{
			IsPressed = true;
			Focus();
		}
		base.OnMouseDown(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseUp(MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Left)
		{
			IsPressed = false;
		}
		base.OnMouseUp(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		IsMouseOver = false;
		base.OnMouseLeave(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseEnter(EventArgs e)
	{
		IsMouseOver = true;
		base.OnMouseEnter(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseMove(MouseEventArgs e)
	{
		IsMouseOver = ClientRectangle.Contains(e.Location);
		base.OnMouseMove(e);
	}

	/// <inheritdoc/>
	protected override void OnClick(EventArgs e)
	{
		IsPressed = false;
		base.OnClick(e);
	}

	/// <inheritdoc/>
	protected override void OnPaintBackground(PaintEventArgs pevent)
	{
	}

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
		=> Renderer.Render(e.Graphics, e.ClipRectangle, this);

	void IButtonControl.NotifyDefault(bool value)
		=> IsDefault = value;

	void IButtonControl.PerformClick()
		=> OnClick(EventArgs.Empty);

	#endregion
}
