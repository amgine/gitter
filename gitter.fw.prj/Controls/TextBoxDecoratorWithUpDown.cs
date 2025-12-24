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
using System.Globalization;
using System.Windows.Forms;

[DesignerCategory("")]
public class TextBoxDecoratorWithUpDown : TextBoxDecorator
{
	private static readonly object ValueChangedEvent = new();

	public event EventHandler? ValueChanged
	{
		add    => Events.AddHandler    (ValueChangedEvent, value);
		remove => Events.RemoveHandler (ValueChangedEvent, value);
	}

	protected virtual void OnValueChanged(EventArgs e)
		=> ((EventHandler?)Events[ValueChangedEvent])?.Invoke(this, e);

	private bool _showButtons = true;
	private bool _isMouseOverUpButton;
	private bool _isMouseOverDownButton;

	private int _value;
	private int _minimum;
	private int _maximum = 100;
	private bool _updatingText;

	private Timer? _scrollTimer;
	private int _scrollDirection;

	public TextBoxDecoratorWithUpDown(TextBox textBox) : base(textBox)
	{
		textBox.Text = "0";
		textBox.MouseWheel += (_, e) => HandleMouseScroll(e);
		textBox.KeyPress += OnTextBoxKeyPress;
		textBox.KeyDown += OnTextBoxKeyDown;
		textBox.TextChanged += OnTextBoxTextChanged;
		
		SetStyle(ControlStyles.StandardDoubleClick, false);
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_scrollTimer is not null)
			{
				_scrollTimer.Dispose();
				_scrollTimer = null;
			}
		}
		base.Dispose(disposing);
	}

	private void StartScroll(int direction)
	{
		_scrollDirection = direction;
		var next = Clamp(Value + _scrollDirection);
		if(Value == next) return;
		Value = next;
		next = Clamp(Value + _scrollDirection);
		if(Value == next) return;
		if(_scrollTimer is null)
		{
			_scrollTimer = new()
			{
				Interval = 600,
			};
			_scrollTimer.Tick += OnScrollTimerTick;
		}
		else
		{
			_scrollTimer.Interval = 600;
		}
		_scrollTimer.Enabled = true;
	}

	private void StopScroll()
	{
		_scrollDirection = 0;
		if(_scrollTimer is not null)
		{
			_scrollTimer.Enabled = false;
		}
	}

	private void OnScrollTimerTick(object? sender, EventArgs e)
	{
		if(sender is not Timer timer) return;
		if(_scrollDirection == 0)
		{
			timer.Enabled = false;
			return;
		}
		timer.Interval = 50;
		var next = Clamp(Value + _scrollDirection);
		if(Value == next)
		{
			timer.Enabled = false;
			return;
		}
		Value = next;
	}

	private void OnTextBoxKeyDown(object? sender, KeyEventArgs e)
	{
		switch(e.KeyCode)
		{
			case Keys.Up:
				Value = Clamp(Value + 1);
				Decorated.SelectAll();
				e.Handled = true;
				break;
			case Keys.Down:
				Value = Clamp(Value - 1);
				Decorated.SelectAll();
				e.Handled = true;
				break;
		}
	}

	private void SetText(string value)
	{
		_updatingText = true;
		try
		{
			Decorated.Text = value;
			Decorated.Select(value.Length, 0);
		}
		finally
		{
			_updatingText = false;
		}
	}

	protected override void OnDecoratedLostFocus(object? sender, EventArgs e)
	{
		base.OnDecoratedLostFocus(sender, e);
		if(!int.TryParse(Decorated.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
		{
			value = Minimum;
			SetText(value.ToString(CultureInfo.InvariantCulture));
			OnValueChanged(EventArgs.Empty);
			return;
		}
		value = Clamp(value);
		var changed = _value != value;
		_value = value;
		SetText(value.ToString(CultureInfo.InvariantCulture));
		if(changed) OnValueChanged(EventArgs.Empty);
	}

	private void OnTextBoxTextChanged(object? sender, EventArgs e)
	{
		if(_updatingText) return;
		if(!int.TryParse(Decorated.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
		{
			value = Minimum;
			SetText(value.ToString(CultureInfo.InvariantCulture));
		}
		value = Clamp(value);
		if(_value == value) return;
		_value = value;
		OnValueChanged(EventArgs.Empty);
	}

	private void OnTextBoxKeyPress(object? sender, KeyPressEventArgs e)
	{
		var symbol = e.KeyChar;
		e.Handled = !char.IsControl(symbol) && !char.IsDigit(symbol);
	}

	private int Clamp(int value)
	{
		if(value < Minimum) return Minimum;
		if(value > Maximum) return Maximum;
		return value;
	}

	public int Value
	{
		get => _value;
		set
		{
			value = Clamp(value);
			if(_value == value) return;

			_value = value;
			SetText(value.ToString(CultureInfo.InvariantCulture));
			OnValueChanged(EventArgs.Empty);
		}
	}

	public int Minimum
	{
		get =>_minimum;
		set
		{
			if(_minimum == value) return;

			_minimum = value;
			if(Value < Minimum) Value = Minimum;
		}
	}

	public int Maximum
	{
		get => _maximum;
		set
		{
			if(_maximum == value) return;

			_maximum = value;
			if(Value > Maximum) Value = Maximum;
		}
	}

	public bool ShowButtons
	{
		get => _showButtons;
		set
		{
			if(_showButtons == value) return;

			_showButtons = value;
			IsMouseOverUpButton   = false;
			IsMouseOverDownButton = false;
			UpdateDecoratedBounds();
			Invalidate();
		}
	}

	protected bool IsMouseOverUpButton
	{
		get => _isMouseOverUpButton;
		private set
		{
			if(_isMouseOverUpButton == value) return;

			_isMouseOverUpButton = value;
			Cursor = (IsMouseOverUpButton | IsMouseOverDownButton) ? Cursors.Hand : Cursors.IBeam;
			Invalidate();
		}
	}

	protected bool IsMouseOverDownButton
	{
		get => _isMouseOverDownButton;
		private set
		{
			if(_isMouseOverDownButton == value) return;

			_isMouseOverDownButton = value;
			Cursor = (IsMouseOverUpButton | IsMouseOverDownButton) ? Cursors.Hand : Cursors.IBeam;
			Invalidate();
		}
	}

	private void HandleMouseScroll(MouseEventArgs e)
	{
		var sign = Math.Sign(e.Delta);
		if(sign == 0) return;
		Value = Clamp(Value + sign);
	}

	/// <inheritdoc/>
	protected override void OnMouseWheel(MouseEventArgs e)
	{
		if(Decorated.Enabled)
		{
			if(!Decorated.Bounds.Contains(e.X, e.Y))
			{
				HandleMouseScroll(e);
			}
		}
		base.OnMouseWheel(e);
	}

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Assert.IsNotNull(e);

		base.OnPaint(e);
		if(ShowButtons)
		{
			PaintButtons(e.Graphics);
		}
	}

	private void PaintButtons(Graphics graphics)
	{
		Assert.IsNotNull(graphics);

		var (up, down) = GetButtonsBounds();
		PaintUpButton   (graphics, up);
		PaintDownButton (graphics, down);
	}

	protected virtual void PaintUpButton(Graphics graphics, Rectangle bounds)
	{
		var colors     = GetColorTable();
		var conv       = DpiConverter.FromDefaultTo(this);
		var glyphColor = Decorated.Enabled ? IsMouseOverUpButton ? colors.Hover.ForeColor : colors.Normal.ForeColor : colors.Disabled.ForeColor;
		var glyphSize  = conv.ConvertX(8);

		var glyphBounds = new Rectangle(
			bounds.X + (bounds.Width  - glyphSize) / 2,
			bounds.Y + (bounds.Height - glyphSize) / 2,
			glyphSize, glyphSize);

		using(var pen = new Pen(glyphColor, conv.ConvertX(1.5f)))
		{
			var h  = glyphBounds.Height / 4;
			var y0 = glyphBounds.Y + h;
			var y1 = glyphBounds.Y + h * 3;
			var x0 = glyphBounds.X;
			var x1 = glyphBounds.X + glyphBounds.Width / 2;
			var x2 = glyphBounds.Right;
#if NET9_0_OR_GREATER
			Span<Point> points = stackalloc Point[3];
#else
			var points = new Point[3];
#endif
			points[0] = new(x0, y1);
			points[1] = new(x1, y0);
			points[2] = new(x2, y1);
			using(graphics.SwitchSmoothingMode(System.Drawing.Drawing2D.SmoothingMode.HighQuality))
			{
				graphics.DrawLines(pen, points);
			}
		}
	}

	protected virtual void PaintDownButton(Graphics graphics, Rectangle bounds)
	{
		var colors     = GetColorTable();
		var conv       = DpiConverter.FromDefaultTo(this);
		var glyphColor = Decorated.Enabled ? IsMouseOverDownButton ? colors.Hover.ForeColor : colors.Normal.ForeColor : colors.Disabled.ForeColor;
		var glyphSize  = conv.ConvertX(8);

		var glyphBounds = new Rectangle(
			bounds.X + (bounds.Width  - glyphSize) / 2,
			bounds.Y + (bounds.Height - glyphSize) / 2,
			glyphSize, glyphSize);

		using(var pen = new Pen(glyphColor, conv.ConvertX(1.5f)))
		{
			var h  = glyphBounds.Height / 4;
			var y0 = glyphBounds.Y + h;
			var y1 = glyphBounds.Y + h * 3;
			var x0 = glyphBounds.X;
			var x1 = glyphBounds.X + glyphBounds.Width / 2;
			var x2 = glyphBounds.Right;
#if NET9_0_OR_GREATER
			Span<Point> points = stackalloc Point[3];
#else
			var points = new Point[3];
#endif
			points[0] = new(x0, y0);
			points[1] = new(x1, y1);
			points[2] = new(x2, y0);
			using(graphics.SwitchSmoothingMode(System.Drawing.Drawing2D.SmoothingMode.HighQuality))
			{
				graphics.DrawLines(pen, points);
			}
		}
	}

	const int iconPadding = 4;
	const int iconSize = 16;

	private (Rectangle up, Rectangle down) GetButtonsBounds()
	{
		var conv    = DpiConverter.FromDefaultTo(this);
		var padding = conv.ConvertX(iconPadding);
		var size    = conv.Convert(new Size(iconSize, iconSize));
		var top     = (Height - size.Height) / 2;
		return (
			new Rectangle(Width - padding - size.Width * 2, top, size.Width, size.Height),
			new Rectangle(Width - padding - size.Width * 1, top, size.Width, size.Height));
	}

	/// <inheritdoc/>
	protected override Rectangle ModifyDecoratedBounds(Rectangle bounds)
	{
		if(ShowButtons)
		{
			var conv = DpiConverter.FromDefaultTo(this);
			bounds.Width -= conv.ConvertX(iconPadding) + conv.ConvertX(iconSize) * 2;
		}
		return bounds;
	}

	/// <inheritdoc/>
	protected override void OnMouseMove(MouseEventArgs e)
	{
		Assert.IsNotNull(e);

		var (up, down) = GetButtonsBounds();
		if(up.Contains(e.X, e.Y))
		{
			IsMouseOverUpButton   = true;
			IsMouseOverDownButton = false;
		}
		else if(down.Contains(e.X, e.Y))
		{
			IsMouseOverDownButton = true;
			IsMouseOverUpButton   = false;
		}
		else
		{
			IsMouseOverUpButton   = false;
			IsMouseOverDownButton = false;
		}
		base.OnMouseMove(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		IsMouseOverUpButton   = false;
		IsMouseOverDownButton = false;
		base.OnMouseLeave(e);
	}

	/// <inheritdoc/>
	protected override bool TryHandleMouseDown(MouseEventArgs e)
	{
		if(!ShowButtons || e.Button != MouseButtons.Left) return false;

		var (up, down) = GetButtonsBounds();
		if(up.Contains(e.X, e.Y))
		{
			StartScroll(1);
			return true;
		}
		else if(down.Contains(e.X, e.Y))
		{
			StartScroll(-1);
			return true;
		}
		return false;
	}

	/// <inheritdoc/>
	protected override void OnMouseUp(MouseEventArgs e)
	{
		StopScroll();
		base.OnMouseUp(e);
	}
}
