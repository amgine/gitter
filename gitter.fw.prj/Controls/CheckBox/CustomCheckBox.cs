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

[DesignerCategory("")]
public class CustomCheckBox : Control
{
	#region Data

	private CustomCheckBoxRenderer _renderer;
	private CheckState _checkState;
	private Image? _image;
	private bool _threeState;
	private bool _isMouseOver;
	private bool _isPressed;
	private Rectangle? _bounds;

	#endregion

	#region Events

	private static readonly object IsCheckedChangedEvent  = new();
	private static readonly object CheckStateChangedEvent = new();

	public event EventHandler IsCheckedChanged
	{
		add    => Events.AddHandler    (IsCheckedChangedEvent, value);
		remove => Events.RemoveHandler (IsCheckedChangedEvent, value);
	}

	public event EventHandler CheckStateChanged
	{
		add    => Events.AddHandler    (CheckStateChangedEvent, value);
		remove => Events.RemoveHandler (CheckStateChangedEvent, value);
	}

	protected virtual void OnIsCheckedChanged()
		=> ((EventHandler?)Events[IsCheckedChangedEvent])?.Invoke(this, EventArgs.Empty);

	protected virtual void OnCheckStateChanged()
		=> ((EventHandler?)Events[CheckStateChangedEvent])?.Invoke(this, EventArgs.Empty);

	#endregion

	#region .ctor

	public CustomCheckBox()
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
			ControlStyles.SupportsTransparentBackColor |
			ControlStyles.StandardDoubleClick, false);
		_renderer = CustomCheckBoxRenderer.Default;
		Size = new Size(86, 18);
		TabStop = true;
	}

	#endregion

	#region Properties

	public CustomCheckBoxRenderer Renderer
	{
		get => _renderer;
		set
		{
			Verify.Argument.IsNotNull(value);

			if(_renderer != value)
			{
				_renderer = value;
				Invalidate();
			}
		}
	}

	[DefaultValue(CheckState.Unchecked)]
	public CheckState CheckState
	{
		get => _checkState;
		set
		{
			if(_checkState != value)
			{
				bool isCheckedChanged = (_checkState == CheckState.Checked) != (value == CheckState.Checked);
				_checkState = value;
				OnCheckStateChanged();
				if(isCheckedChanged)
				{
					OnIsCheckedChanged();
				}
				Invalidate();
			}
		}
	}

	[DefaultValue(null)]
	public Image? Image
	{
		get => _image;
		set
		{
			if(_image == value) return;

			_image = value;
			Invalidate();
			InvalidateBounds();
		}
	}

	[DefaultValue(false)]
	public bool IsChecked
	{
		get => CheckState == CheckState.Checked;
		set => CheckState = value ? CheckState.Checked : CheckState.Unchecked;
	}

	[DefaultValue(false)]
	public bool ThreeState
	{
		get => _threeState;
		set
		{
			if(_threeState != value)
			{
				_threeState = value;
				if(!value && CheckState == CheckState.Indeterminate)
				{
					CheckState = CheckState.Checked;
				}
			}
		}
	}

	public bool IsMouseOver
	{
		get => _isMouseOver;
		private set
		{
			if(_isMouseOver == value) return;
			_isMouseOver = value;
			Invalidate();
		}
	}

	public bool IsPressed
	{
		get => _isPressed;
		private set
		{
			if(_isPressed == value) return;
			_isPressed = value;
			Invalidate();
		}
	}

	#endregion

	#region Methods

	private void CycleState()
	{
		if(ThreeState)
		{
			CheckState = CheckState switch
			{
				CheckState.Checked       => CheckState.Indeterminate,
				CheckState.Indeterminate => CheckState.Unchecked,
				CheckState.Unchecked     => CheckState.Checked,
				_ => CheckState.Checked,
			};
		}
		else
		{
			IsChecked = !IsChecked;
		}
	}

	private Rectangle GetVisualBounds()
	{
		if(_bounds.HasValue) return _bounds.Value;
		_bounds = Renderer.Measure(this);
		return _bounds.Value;
	}

	private void InvalidateBounds()
	{
		_bounds = default;
	}

	#endregion

	#region Overrides

	/// <inheritdoc/>
	protected override void WndProc(ref Message m)
	{
		switch((Native.WM)m.Msg)
		{
			case Native.WM.NCHITTEST:
				var x = Native.Macro.LOWORD(m.LParam);
				var y = Native.Macro.HIWORD(m.LParam);
				var p = PointToClient(new Point(x, y));
				if(!GetVisualBounds().Contains(p))
				{
					m.Result = (IntPtr)(-1);
					return;
				}
				break;

		}
		base.WndProc(ref m);
	}

	/// <inheritdoc/>
	protected override void OnMouseClick(MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Left)
		{
			CycleState();
		}
		base.OnMouseClick(e);
	}

	/// <inheritdoc/>
	protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
	{
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
		base.OnKeyUp(e);
		switch(e.KeyCode)
		{
			case Keys.Space:
				IsPressed = false;
				CycleState();
				break;
		}
	}

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
		=> Renderer.Render(e.Graphics, e.ClipRectangle, this);

	/// <inheritdoc/>
	protected override void OnPaintBackground(PaintEventArgs pevent) { }

	/// <inheritdoc/>
	protected override void OnSizeChanged(EventArgs e)
	{
		InvalidateBounds();
		base.OnSizeChanged(e);
	}

	/// <inheritdoc/>
	protected override void OnDpiChangedAfterParent(EventArgs e)
	{
		InvalidateBounds();
		base.OnDpiChangedAfterParent(e);
	}

	/// <inheritdoc/>
	protected override void OnFontChanged(EventArgs e)
	{
		InvalidateBounds();
		base.OnFontChanged(e);
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
		IsPressed = false;
		Invalidate();
	}

	/// <inheritdoc/>
	protected override void OnMouseEnter(EventArgs e)
	{
		IsMouseOver = true;
		base.OnMouseEnter(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		IsMouseOver = false;
		base.OnMouseLeave(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		IsPressed = true;
		base.OnMouseDown(e);
		Focus();
	}

	/// <inheritdoc/>
	protected override void OnMouseUp(MouseEventArgs e)
	{
		IsPressed = false;
		base.OnMouseUp(e);
	}

	/// <inheritdoc/>
	protected override void OnTextChanged(EventArgs e)
	{
		InvalidateBounds();
		base.OnTextChanged(e);
	}

	#endregion
}
