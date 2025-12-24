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

namespace gitter.Framework.Layout;

using System;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework.Controls;

public sealed class WidgetContent : IContent
{
	private Rectangle _bounds;
	private Dpi _dpi;
	private int _updating;

	private IDpiBoundValue<Size>?      _sizeOverride;
	private IDpiBoundValue<Padding>?   _marginOverride;
	private HorizontalContentAlignment _horizontalContentAlignment;
	private VerticalContentAlignment   _verticalContentAlignment;

	public WidgetContent(Func<IWidget> widget,
		IDpiBoundValue<Size>?      sizeOverride   = default,
		IDpiBoundValue<Padding>?   marginOverride = default,
		HorizontalContentAlignment horizontalContentAlignment = HorizontalContentAlignment.Stretch,
		VerticalContentAlignment   verticalContentAlignment   = VerticalContentAlignment.Stretch)
	{
		Verify.Argument.IsNotNull(widget);

		Widget                      = widget();
		_sizeOverride               = sizeOverride;
		_marginOverride             = marginOverride;
		_verticalContentAlignment   = verticalContentAlignment;
		_horizontalContentAlignment = horizontalContentAlignment;

		Init(Widget);
	}

	public WidgetContent(IWidget  widget,
		IDpiBoundValue<Size>?      sizeOverride   = default,
		IDpiBoundValue<Padding>?   marginOverride = default,
		HorizontalContentAlignment horizontalContentAlignment = HorizontalContentAlignment.Stretch,
		VerticalContentAlignment   verticalContentAlignment   = VerticalContentAlignment.Stretch)
	{
		Verify.Argument.IsNotNull(widget);

		Widget                      = widget;
		_sizeOverride               = sizeOverride;
		_marginOverride             = marginOverride;
		_verticalContentAlignment   = verticalContentAlignment;
		_horizontalContentAlignment = horizontalContentAlignment;

		Init(Widget);
	}

	private void Init(IWidget widget)
	{
		if(widget.Control is ContainerControl container)
		{
			container.AutoScaleMode = AutoScaleMode.None;
		}
		widget.Control.SizeChanged     += OnControlSizeChanged;
		widget.Control.LocationChanged += OnControlLocationChanged;
	}

	private void OnControlLocationChanged(object? sender, EventArgs e)
		=> UpdateControlBounds();

	private void OnControlSizeChanged(object? sender, EventArgs e)
		=> UpdateControlBounds();

	public IWidget Widget { get; }

	public IDpiBoundValue<Size>? SizeOverride
	{
		get => _sizeOverride;
		set
		{
			if(_sizeOverride == value) return;

			_sizeOverride = value;
			UpdateControlBounds();
		}
	}

	public IDpiBoundValue<Padding>? MarginOverride
	{
		get => _marginOverride;
		set
		{
			if(_marginOverride == value) return;

			_marginOverride = value;
			UpdateControlBounds();
		}
	}

	public HorizontalContentAlignment HorizontalContentAlignment
	{
		get => _horizontalContentAlignment;
		set
		{
			if(_horizontalContentAlignment == value) return;

			_horizontalContentAlignment = value;
			UpdateControlBounds();
		}
	}

	public VerticalContentAlignment VerticalContentAlignment
	{
		get => _verticalContentAlignment;
		set
		{
			if(_verticalContentAlignment == value) return;

			_verticalContentAlignment = value;
			UpdateControlBounds();
		}
	}

	private Size GetSize(Dpi dpi)
		=> SizeOverride is not null
			? SizeOverride.GetValue(dpi)
			: Widget.Bounds.Size;

	private Padding GetMargin(Dpi dpi)
		=> MarginOverride is not null
			? MarginOverride.GetValue(dpi)
			: Widget.Margin;

	private void ApplyVerticalAlignment(ref Rectangle bounds, Size controlSize)
	{
		if(controlSize.Height >= bounds.Height) return;

		switch(VerticalContentAlignment)
		{
			case VerticalContentAlignment.Top:
				bounds.Height = controlSize.Height;
				break;
			case VerticalContentAlignment.Bottom:
				bounds.Y     += bounds.Height - controlSize.Height;
				bounds.Height = controlSize.Height;
				break;
			case VerticalContentAlignment.Center:
				bounds.Y     += (bounds.Height - controlSize.Height) / 2;
				bounds.Height = controlSize.Height;
				break;
		}
	}

	private void ApplyHorizontalAlignment(ref Rectangle bounds, Size controlSize)
	{
		if(controlSize.Width >= bounds.Width) return;

		switch(HorizontalContentAlignment)
		{
			case HorizontalContentAlignment.Left:
				bounds.Width = controlSize.Width;
				break;
			case HorizontalContentAlignment.Right:
				bounds.X    += bounds.Width - controlSize.Width;
				bounds.Width = controlSize.Width;
				break;
			case HorizontalContentAlignment.Center:
				bounds.X    += (bounds.Width - controlSize.Width) / 2;
				bounds.Width = controlSize.Width;
				break;
		}
	}

	private void UpdateControlBounds()
	{
		if(_updating > 0 || _dpi == default) return;

		var bounds      = _bounds.WithPadding(GetMargin(_dpi));
		var controlSize = GetSize(_dpi);

		ApplyVerticalAlignment  (ref bounds, controlSize);
		ApplyHorizontalAlignment(ref bounds, controlSize);

		++_updating;
		try
		{
			if(bounds is { Width: > 0, Height: > 0 })
			{
				Widget.Bounds = new(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			}
			else
			{
				Widget.Bounds = new(bounds.X, bounds.Y, 0, 0);
			}
		}
		finally
		{
			--_updating;
		}
	}

	public void UpdateBounds(Rectangle bounds, Dpi dpi)
	{
		_bounds = bounds;
		_dpi    = dpi;
		UpdateControlBounds();
	}
}
