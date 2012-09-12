namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Services;

	public sealed class ViewButtons : IEnumerable<ViewButton>
	{
		#region Data

		private readonly Control _host;
		private ViewButton[] _buttons;
		private readonly TrackingService<ViewButton> _buttonHover;
		private readonly TrackingService<ViewButton> _buttonPress;
		private int _buttonSpacing;
		private int _height;

		#endregion

		public event EventHandler<ViewButtonClickEventArgs> ButtonClick;

		internal ViewButtons(Control hostControl)
		{
			Verify.Argument.IsNotNull(hostControl, "hostControl");

			_host = hostControl;
			_buttonHover = new TrackingService<ViewButton>((e) => _host.Invalidate());
			_buttonPress = new TrackingService<ViewButton>((e) => _host.Invalidate());
		}

		public void SetAvailableButtons(params ViewButtonType[] buttons)
		{
			_buttonHover.Reset(-1, null);
			_buttonPress.Reset(-1, null);
			if(buttons == null || buttons.Length == 0)
			{
				_buttons = null;
			}
			else
			{
				_buttons = new ViewButton[buttons.Length];
				int x = buttons.Length * ViewConstants.ViewButtonSize + 2;
				for(int i = 0; i < buttons.Length; ++i)
				{
					_buttons[i] = new ViewButton(x, buttons[i]);
					x -= ViewConstants.ViewButtonSize;
				}
			}
			_host.Invalidate();
		}

		public void Clear()
		{
			_buttonHover.Reset(-1, null);
			_buttonPress.Reset(-1, null);
			_buttons = null;
			_host.Invalidate();
		}

		public int ButtonSpacing
		{
			get { return _buttonSpacing; }
			set { _buttonSpacing = value; }
		}

		public Control Host
		{
			get { return _host; }
		}

		public ViewButton PressedButton
		{
			get { return _buttonPress.Item; }
		}

		public ViewButton HoveredButton
		{
			get { return _buttonHover.Item; }
		}

		public int Count
		{
			get { return _buttons != null ? _buttons.Length : 0; }
		}

		public int Width
		{
			get
			{
				if(_buttons == null || _buttons.Length == 0)
					return 0;
				return ViewConstants.ViewButtonSize +
					(_buttons.Length - 1) * (ViewConstants.ViewButtonSize + _buttonSpacing);
			}
		}

		public int Height
		{
			get { return _height; }
			set { _height = value; }
		}

		public ViewButton this[int index]
		{
			get { return _buttons[index]; }
		}

		private int HitTest(int x, int y)
		{
			if(_buttons == null || _buttons.Length == 0) return -1;
			if(x < 0) return -1;
			int y1 = (_height - ViewConstants.ViewButtonSize) / 2;
			if(y < y1) return -1;
			if(y >= y1 + ViewConstants.ViewButtonSize) return -1;
			int id = x / (ViewConstants.ViewButtonSize + _buttonSpacing);
			if(id >= _buttons.Length)
				return -1;
			return id;
		}

		public void OnPaint(Graphics graphics, Rectangle bounds, bool focus)
		{
			if(_buttons == null || _buttons.Length == 0) return;
			var y = bounds.Y + (_height - ViewConstants.ViewButtonSize) / 2;
			var rc = new Rectangle(bounds.X, y, ViewConstants.ViewButtonSize, ViewConstants.ViewButtonSize);
			for(int i = 0; i < _buttons.Length; ++i)
			{
				bool hovered = _buttonHover.Index == i;
				bool pressed = _buttonPress.Index == i;
				if(hovered && _buttonPress.IsTracked)
				{
					hovered = _buttonPress.Index == i;
				}
				if(pressed && !hovered)
				{
					pressed = false;
					hovered = true;
				}
				_buttons[i].OnPaint(graphics, rc, focus, hovered, pressed);
				rc.X += ViewConstants.ViewButtonSize + _buttonSpacing;
			}
		}

		public void OnMouseDown(int x, int y, MouseButtons button)
		{
			int id = HitTest(x, y);
			if(id == -1)
				_buttonPress.Drop();
			else
				_buttonPress.Track(id, _buttons[id]);
		}

		public void OnMouseMove(int x, int y, MouseButtons button)
		{
			int id = HitTest(x, y);
			if(id == -1)
				_buttonHover.Drop();
			else
				_buttonHover.Track(id, _buttons[id]);
		}

		public void OnMouseUp(int x, int y, MouseButtons button)
		{
			if(_buttonPress.IsTracked)
			{
				int id = HitTest(x, y);
				if(id == _buttonPress.Index)
				{
					ButtonClick.Raise(this, new ViewButtonClickEventArgs(_buttonPress.Item.Type));
				}
				_buttonPress.Drop();
			}
		}

		public void OnMouseLeave()
		{
			_buttonHover.Drop();
		}

		public IEnumerator<ViewButton> GetEnumerator()
		{
			return ((IEnumerable<ViewButton>)_buttons).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _buttons.GetEnumerator();
		}
	}
}
