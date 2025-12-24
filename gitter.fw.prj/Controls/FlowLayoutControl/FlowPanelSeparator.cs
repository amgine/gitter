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

/// <summary>Panel for separating other panels.</summary>
public class FlowPanelSeparator : FlowPanel
{
	private int _height;
	private FlowPanelSeparatorStyle _style;

	/// <summary>Create <see cref="FlowPanelSeparator"/>.</summary>
	public FlowPanelSeparator()
	{
		_height = 16;
	}

	/// <summary>Separator height.</summary>
	public int Height
	{
		get => _height;
		set
		{
			if(_height == value) return;

			_height = value;
			InvalidateSize();
		}
	}

	public FlowPanelSeparatorStyle SeparatorStyle
	{
		get => _style;
		set
		{
			if(_style == value) return;

			_style = value;
			Invalidate();
		}
	}

	/// <inheritdoc/>
	protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		return new Size(0, DpiConverter.FromDefaultTo(measureEventArgs.Dpi).ConvertY(Height));
	}

	/// <inheritdoc/>
	protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		if(FlowControl is null) return;

		var graphics = paintEventArgs.Graphics;
		var rect = paintEventArgs.Bounds;
		switch(_style)
		{
			case FlowPanelSeparatorStyle.Line:
				{
					var y = rect.Height / 2;
					var x = 8;
					var w = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width) - 2 * 8;
					x += rect.X;
					y += rect.Y;
					var rc = new Rectangle(x, y, w, DpiConverter.FromDefaultTo(paintEventArgs.Dpi).ConvertY(1));
					rc.Intersect(paintEventArgs.ClipRectangle);
					if(rc is { Width: > 0, Height: > 0 })
					{
						graphics.GdiFill(Color.Gray, rc);
					}
				}
				break;
		}
	}
}
