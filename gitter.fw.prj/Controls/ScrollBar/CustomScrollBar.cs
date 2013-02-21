namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	[DefaultEvent("Scroll")]
	[DefaultProperty("Value")]
	public abstract class CustomScrollBar : Control
	{
		#region Data

		private int _value;
		private int _maximum;
		private int _minimum;
		private int _smallChange;
		private int _largeChange;
		private CustomScrollBarPart _hoveredPart;
		private CustomScrollBarPart _pressedPart;
		private CustomScrollBarRenderer _renderer;
		private Timer _timer;
		private Point _mouseDownPoint;
		private int _trackValue;

		#endregion

		#region Events

		private static readonly object ScrollEvent = new object();
		private static readonly object ValueChangedEvent = new object();

		public event EventHandler<ScrollEventArgs> Scroll
		{
			add { Events.AddHandler(ScrollEvent, value); }
			remove { Events.RemoveHandler(ScrollEvent, value); }
		}

		public event EventHandler ValueChanged
		{
			add { Events.AddHandler(ValueChangedEvent, value); }
			remove { Events.RemoveHandler(ValueChangedEvent, value); }
		}

		protected virtual void OnValueChanged()
		{
			var handler = (EventHandler)Events[ValueChangedEvent];
			if(handler != null) handler(this, EventArgs.Empty);
		}

		protected virtual void OnScroll(ScrollEventType eventType, int oldValue, int newValue)
		{
			var handler = (EventHandler<ScrollEventArgs>)Events[ScrollEvent];
			if(handler != null) handler(this, new ScrollEventArgs(eventType, oldValue, newValue, ScrollOrientation));
		}

		#endregion

		#region .ctor

		public CustomScrollBar()
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
		{
			get
			{
				switch(Orientation)
				{
					case Orientation.Vertical:
						return ScrollOrientation.VerticalScroll;
					case Orientation.Horizontal:
						return ScrollOrientation.HorizontalScroll;
					default:
						throw new ApplicationException();
				}
			}
		}

		protected int ClampValue(int value)
		{
			if(value >= Maximum - LargeChange + 1) return Maximum - LargeChange + 1;
			if(value <= Minimum) return Minimum;
			return value;
		}

		public CustomScrollBarRenderer Renderer
		{
			get { return _renderer; }
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				_renderer = value;
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Renderer.Render(
				Orientation,
				Enabled,
				e.Graphics,
				e.ClipRectangle,
				DecreaseButtonBounds,
				DecreaseTrackBarBounds,
				ThumbBounds,
				IncreaseTrackBarBounds,
				IncreaseButtonBounds,
				HoveredPart,
				PressedPart);
		}

		protected override void OnResize(EventArgs e)
		{
			ArrangeInvalidate();
			base.OnResize(e);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			HoveredPart = CustomScrollBarPart.None;
			base.OnMouseLeave(e);
		}

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
			get { return _hoveredPart; }
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
			get { return _pressedPart; }
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
			get { return _value; }
			set
			{
				Verify.Argument.IsInRange(Minimum, value, Maximum, "value");

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
			get { return _maximum; }
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
			get { return _minimum; }
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
			get { return _smallChange; }
			set
			{
				Verify.Argument.IsNotNegative(value, "value");

				_smallChange = value;
			}
		}

		[DefaultValue(10)]
		public int LargeChange
		{
			get { return _largeChange; }
			set
			{
				Verify.Argument.IsNotNegative(value, "value");

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

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_timer != null)
				{
					_timer.Dispose();
					_timer = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
