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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework.Services;

public sealed class ViewButtons : IEnumerable<ViewButton>
{
	#region Data

	private ViewButton[] _buttons;
	private readonly TrackingService<ViewButton> _buttonHover;
	private readonly TrackingService<ViewButton> _buttonPress;

	#endregion

	public event EventHandler<ViewButtonClickEventArgs> ButtonClick;

	internal ViewButtons(Control hostControl)
	{
		Verify.Argument.IsNotNull(hostControl);

		Host = hostControl;
		_buttonHover = new TrackingService<ViewButton>(_ => Host.Invalidate());
		_buttonPress = new TrackingService<ViewButton>(_ => Host.Invalidate());
	}

	public void SetAvailableButtons(params ViewButtonType[] buttons)
	{
		var dpi = Dpi.FromControl(Host);
		var viewButtonSize = ViewManager.Renderer.ViewButtonSize.GetValue(dpi);
		_buttonHover.Reset(-1, null);
		_buttonPress.Reset(-1, null);
		if(buttons is not { Length: not 0 })
		{
			_buttons = null;
		}
		else
		{
			if(_buttons is null || _buttons.Length != buttons.Length)
			{
				_buttons = new ViewButton[buttons.Length];
			}
			int x = buttons.Length * viewButtonSize + 2;
			for(int i = 0; i < buttons.Length; ++i)
			{
				_buttons[i] = new ViewButton(x, buttons[i]);
				x -= viewButtonSize;
			}
		}
		Host.Invalidate();
	}

	public void Clear()
	{
		_buttonHover.Reset(-1, null);
		_buttonPress.Reset(-1, null);
		_buttons = null;
		Host.Invalidate();
	}

	public int ButtonSpacing { get; set; }

	public Control Host { get; }

	public ViewButton PressedButton => _buttonPress.Item;

	public ViewButton HoveredButton => _buttonHover.Item;

	public int Count => _buttons != null ? _buttons.Length : 0;

	public int Width
	{
		get
		{
			if(_buttons is not { Length: not 0 })
			{
				return 0;
			}
			var dpi = Dpi.FromControl(Host);
			var viewButtonSize = ViewManager.Renderer.ViewButtonSize.GetValue(dpi);
			return viewButtonSize + (_buttons.Length - 1) * (viewButtonSize + ButtonSpacing);
		}
	}

	public int Height { get; set; }

	public ViewButton this[int index] => _buttons[index];

	private int HitTest(int x, int y)
	{
		if(_buttons is not { Length: not 0 }) return -1;
		if(x < 0) return -1;
		var dpi = Dpi.FromControl(Host);
		var viewButtonSize = ViewManager.Renderer.ViewButtonSize.GetValue(dpi);
		int y1 = (Height - viewButtonSize) / 2;
		if(y < y1) return -1;
		if(y >= y1 + viewButtonSize) return -1;
		int id = x / (viewButtonSize + ButtonSpacing);
		if(id >= _buttons.Length) return -1;
		return id;
	}

	public void OnPaint(Graphics graphics, Rectangle bounds, bool focus)
	{
		if(_buttons is not { Length: not 0 }) return;
		var dpi = Dpi.FromControl(Host);
		var viewButtonSize = ViewManager.Renderer.ViewButtonSize.GetValue(dpi);
		var y = bounds.Y + (Height - viewButtonSize) / 2;
		var rc = new Rectangle(bounds.X, y, viewButtonSize, viewButtonSize);
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
			_buttons[i].OnPaint(graphics, dpi, rc, focus, hovered, pressed);
			rc.X += viewButtonSize + ButtonSpacing;
		}
	}

	public void OnMouseDown(int x, int y, MouseButtons button)
	{
		int id = HitTest(x, y);
		if(id == -1)
		{
			_buttonPress.Drop();
		}
		else
		{
			_buttonPress.Track(id, _buttons[id]);
		}
	}

	public void OnMouseMove(int x, int y, MouseButtons button)
	{
		int id = HitTest(x, y);
		if(id == -1)
		{
			_buttonHover.Drop();
		}
		else
		{
			_buttonHover.Track(id, _buttons[id]);
		}
	}

	public void OnMouseUp(int x, int y, MouseButtons button)
	{
		if(_buttonPress.IsTracked)
		{
			int id = HitTest(x, y);
			if(id == _buttonPress.Index)
			{
				ButtonClick?.Invoke(this, new ViewButtonClickEventArgs(_buttonPress.Item.Type));
			}
			_buttonPress.Drop();
		}
	}

	public void OnMouseLeave() => _buttonHover.Drop();

	public IEnumerator<ViewButton> GetEnumerator()
		=> ((IEnumerable<ViewButton>)_buttons).GetEnumerator();

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		=> _buttons.GetEnumerator();
}
