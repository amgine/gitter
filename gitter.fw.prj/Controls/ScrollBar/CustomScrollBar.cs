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

[DefaultEvent(nameof(Scroll))]
[DefaultProperty(nameof(Value))]
[DesignerCategory("")]
public abstract class CustomScrollBar : Control
{
	#region Constants

	protected const int MinThumbSize = 17;

	#endregion

	#region Data

	private int _value;
	private int _maximum;
	private int _minimum;
	private int _smallChange;
	private int _largeChange;
	private CustomScrollBarPart _hoveredPart;
	private CustomScrollBarPart _pressedPart;
	private ICustomScrollBarRenderer _renderer;
	private Timer _timer;
	private Point _mouseDownPoint;
	private int _trackValue;

	#endregion

	#region Events

	private static readonly object ScrollEvent       = new();
	private static readonly object ValueChangedEvent = new();

	public event EventHandler<ScrollEventArgs> Scroll
	{
		add    => Events.AddHandler    (ScrollEvent, value);
		remove => Events.RemoveHandler (ScrollEvent, value);
	}

	public event EventHandler ValueChanged
	{
		add    => Events.AddHandler    (ValueChangedEvent, value);
		remove => Events.RemoveHandler (ValueChangedEvent, value);
	}

	protected virtual void OnValueChanged()
		=> ((EventHandler)Events[ValueChangedEvent])?.Invoke(this, EventArgs.Empty);

	protected virtual void OnScroll(ScrollEventType eventType, int oldValue, int newValue)
		=> ((EventHandler<ScrollEventArgs>)Events[ScrollEvent])?.Invoke(this, new ScrollEventArgs(eventType, oldValue, newValue, ScrollOrientation));

	#endregion

	#region .ctor

	protected CustomScrollBar()
	{
		SetStyle(
			ControlStyles.AllPaintingInWmPaint |
			ControlStyles.OptimizedDoubleBuffer |
			ControlStyles.Opaque |
			ControlStyles.ResizeRedraw |
			ControlStyles.Selectable |
			ControlStyles.UserPaint, true);
		SetStyle(
			ControlStyles.ContainerControl |
			ControlStyles.UseTextForAccessibility |
			ControlStyles.SupportsTransparentBackColor, false);
		TabStop = false;
		_renderer = CustomScrollBarRenderer.Default;
		_maximum = 100;
		_smallChange = 1;
		_largeChange = 10;
		_timer = new Timer();
		_timer.Tick += OnTimerTick;
	}

	#endregion

	public abstract Orientation Orientation { get; }

	protected abstract Rectangle DecreaseButtonBounds { get; }

	protected abstract Rectangle DecreaseTrackBarBounds { get; }

	protected abstract Rectangle ThumbBounds { get; }

	protected abstract Rectangle IncreaseTrackBarBounds { get; }

	protected abstract Rectangle IncreaseButtonBounds { get; }

	protected abstract void ArrangeInvalidate();

	protected abstract void BeginScroll(Point from);

	protected abstract bool PerformScroll(Point from, Point to);

	protected abstract bool EndScroll(Point from, Point to);

	protected abstract int ThumbPositionToValue();

	public ScrollOrientation ScrollOrientation
		=> Orientation switch
		{
			Orientation.Vertical   => ScrollOrientation.VerticalScroll,
			Orientation.Horizontal => ScrollOrientation.HorizontalScroll,
			_ => throw new ApplicationException(),
		};

	/// <inheritdoc/>
	protected override void ScaleControl(SizeF factor, BoundsSpecified specified) { }

	protected int ClampValue(int value)
	{
		if(value > Maximum - LargeChange + 1) return Maximum - LargeChange + 1;
		if(value < Minimum) return Minimum;
		return value;
	}

	public ICustomScrollBarRenderer Renderer
	{
		get => _renderer;
		set
		{
			Verify.Argument.IsNotNull(value);

			_renderer = value;
			Invalidate();
		}
	}

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Assert.IsNotNull(e);

		Renderer.Render(
			Orientation,
			Enabled,
			e.Graphics, new Dpi(DeviceDpi),
			e.ClipRectangle,
			DecreaseButtonBounds,
			DecreaseTrackBarBounds,
			ThumbBounds,
			IncreaseTrackBarBounds,
			IncreaseButtonBounds,
			HoveredPart,
			PressedPart);
	}

	/// <inheritdoc/>
	protected override void OnResize(EventArgs e)
	{
		ArrangeInvalidate();
		base.OnResize(e);
	}

	/// <inheritdoc/>
	protected override void OnPaintBackground(PaintEventArgs pevent)
	{
	}

	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		HoveredPart = CustomScrollBarPart.None;
		base.OnMouseLeave(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseMove(MouseEventArgs e)
	{
		if(PressedPart != CustomScrollBarPart.None)
		{
			if(e.Button == MouseButtons.Left && PressedPart == CustomScrollBarPart.Thumb)
			{
				if(PerformScroll(_mouseDownPoint, e.Location))
				{
					int newValue = ClampValue(ThumbPositionToValue());
					if(newValue != _trackValue)
					{
						OnScroll(ScrollEventType.ThumbTrack, _trackValue, newValue);
						_trackValue = newValue;
					}
					if(newValue != Value)
					{
						_value = newValue;
						OnValueChanged();
					}
					Invalidate();
					Update();
				}
			}
		}
		else
		{
			HoveredPart = HitTest(e.X, e.Y);
		}
		base.OnMouseMove(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		switch(e.Button)
		{
			case MouseButtons.Left:
				PressedPart = HitTest(e.X, e.Y);
				_mouseDownPoint = e.Location;
				switch(PressedPart)
				{
					case CustomScrollBarPart.DecreaseButton:
					case CustomScrollBarPart.DecreaseTrackBar:
					case CustomScrollBarPart.IncreaseTrackBar:
					case CustomScrollBarPart.IncreaseButton:
						OnTimerTick(_timer, EventArgs.Empty);
						_timer.Interval = 400;
						_timer.Enabled = true;
						break;
					case CustomScrollBarPart.Thumb:
						_trackValue = Value;
						OnScroll(ScrollEventType.ThumbTrack, _trackValue, _trackValue);
						BeginScroll(_mouseDownPoint);
						break;
				}
				break;
		}
		base.OnMouseDown(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseUp(MouseEventArgs e)
	{
		if(PressedPart != CustomScrollBarPart.None)
		{
			if(PressedPart == CustomScrollBarPart.Thumb)
			{
				if(EndScroll(_mouseDownPoint, e.Location))
				{
					Invalidate();
				}
				var value = ClampValue(ThumbPositionToValue());
				if(Value != value)
				{
					_value = value;
					OnValueChanged();
				}
				OnScroll(ScrollEventType.ThumbPosition, _trackValue, value);
				OnScroll(ScrollEventType.EndScroll, value, value);
			}
			else
			{
				if(_timer.Enabled)
				{
					_timer.Enabled = false;
					OnScroll(ScrollEventType.EndScroll, Value, Value);
				}
			}
			PressedPart = CustomScrollBarPart.None;
		}
		base.OnMouseUp(e);
	}

	/// <inheritdoc/>
	protected override void OnDpiChangedAfterParent(EventArgs e)
	{
		ArrangeInvalidate();
		base.OnDpiChangedAfterParent(e);
	}

	private void OnTimerTick(object sender, EventArgs e)
	{
		_timer.Interval = 25;
		int newValue;
		switch(PressedPart)
		{
			case CustomScrollBarPart.DecreaseButton:
				newValue = ClampValue(Value - SmallChange);
				OnScroll(ScrollEventType.SmallDecrement, Value, newValue);
				Value = newValue;
				break;
			case CustomScrollBarPart.DecreaseTrackBar:
				newValue = ClampValue(Value - LargeChange);
				OnScroll(ScrollEventType.LargeDecrement, Value, newValue);
				Value = newValue;
				break;
			case CustomScrollBarPart.IncreaseTrackBar:
				newValue = ClampValue(Value + LargeChange);
				OnScroll(ScrollEventType.LargeIncrement, Value, newValue);
				Value = newValue;
				break;
			case CustomScrollBarPart.IncreaseButton:
				newValue = ClampValue(Value + SmallChange);
				OnScroll(ScrollEventType.SmallIncrement, Value, newValue);
				Value = newValue;
				break;
			default:
				_timer.Enabled = false;
				break;
		}
	}

	private CustomScrollBarPart HitTest(int x, int y)
	{
		if(DecreaseButtonBounds.Contains(x, y))
		{
			return CustomScrollBarPart.DecreaseButton;
		}
		if(IncreaseButtonBounds.Contains(x, y))
		{
			return CustomScrollBarPart.IncreaseButton;
		}
		if(DecreaseTrackBarBounds.Contains(x, y))
		{
			return CustomScrollBarPart.DecreaseTrackBar;
		}
		if(IncreaseTrackBarBounds.Contains(x, y))
		{
			return CustomScrollBarPart.IncreaseTrackBar;
		}
		if(ThumbBounds.Contains(x, y))
		{
			return CustomScrollBarPart.Thumb;
		}
		return CustomScrollBarPart.None;
	}

	private CustomScrollBarPart HoveredPart
	{
		get => _hoveredPart;
		set
		{
			if(_hoveredPart != value)
			{
				InvalidatePart(_hoveredPart);
				_hoveredPart = value;
				InvalidatePart(_hoveredPart);
			}
		}
	}

	private CustomScrollBarPart PressedPart
	{
		get => _pressedPart;
		set
		{
			if(_pressedPart != value)
			{
				InvalidatePart(_pressedPart);
				_pressedPart = value;
				if(_pressedPart == CustomScrollBarPart.None)
				{
					InvalidatePart(_hoveredPart);
				}
				else
				{
					InvalidatePart(_pressedPart);
				}
			}
		}
	}

	private void InvalidatePart(CustomScrollBarPart area)
	{
		switch(area)
		{
			case CustomScrollBarPart.DecreaseButton:
				Invalidate(DecreaseButtonBounds);
				break;
			case CustomScrollBarPart.IncreaseButton:
				Invalidate(IncreaseButtonBounds);
				break;
			case CustomScrollBarPart.DecreaseTrackBar:
				Invalidate(DecreaseTrackBarBounds);
				break;
			case CustomScrollBarPart.IncreaseTrackBar:
				Invalidate(IncreaseTrackBarBounds);
				break;
			case CustomScrollBarPart.Thumb:
				Invalidate(ThumbBounds);
				break;
		}
	}

	[DefaultValue(0)]
	public int Value
	{
		get => _value;
		set
		{
			Verify.Argument.IsInRange(Minimum, value, Maximum, nameof(value));

			if(_value != value)
			{
				_value = value;
				ArrangeInvalidate();
				Invalidate();
				OnValueChanged();
				Update();
			}
		}
	}

	[DefaultValue(100)]
	public int Maximum
	{
		get => _maximum;
		set
		{
			if(_maximum != value)
			{
				if(_minimum > value)
				{
					_minimum = value;
				}
				if(_value > value)
				{
					_value = value;
					OnValueChanged();
				}
				_maximum = value;
				ArrangeInvalidate();
				Invalidate();
			}
		}
	}

	[DefaultValue(0)]
	public int Minimum
	{
		get => _minimum;
		set
		{
			if(_minimum != value)
			{
				if(_maximum < value)
				{
					_maximum = value;
				}
				if(_value < value)
				{
					_value = value;
					OnValueChanged();
				}
				_minimum = value;
				ArrangeInvalidate();
				Invalidate();
			}
		}
	}

	[DefaultValue(1)]
	public int SmallChange
	{
		get => _smallChange;
		set
		{
			Verify.Argument.IsNotNegative(value);

			_smallChange = value;
		}
	}

	[DefaultValue(10)]
	public int LargeChange
	{
		get => _largeChange;
		set
		{
			Verify.Argument.IsNotNegative(value);

			if(_largeChange != value)
			{
				if(_smallChange > value)
				{
					_smallChange = value;
				}
				_largeChange = value;
				ArrangeInvalidate();
				Invalidate();
			}
		}
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_timer is not null)
			{
				_timer.Dispose();
				_timer = null;
			}
		}
		base.Dispose(disposing);
	}
}
