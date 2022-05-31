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

namespace gitter.Git.Gui.Controls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Services;

/// <summary><see cref="FlowPanel"/> which displays basic commit information: author, hash, date, subject, etc.</summary>
public sealed class RevisionHeaderPanel : FlowPanel
{
	#region Constants

	private const int SelectionMargin = 5;

	#endregion

	#region Data

	private readonly RevisionHeaderContent _content;
	private Revision _revision;
	private bool _isSelected;

	#endregion

	#region .ctor

	/// <summary>Create <see cref="RevisionHeaderPanel"/>.</summary>
	public RevisionHeaderPanel()
	{
		_content = new RevisionHeaderContent(AdditionalHyperlinkExtractors);
		_content.Invalidated          += OnContentInvalidated;
		_content.SizeChanged          += OnContentSizeChanged;
		_content.ContextMenuRequested += OnContentContextMenuRequested;
		_content.CursorChanged        += OnContentCursorChanged;
	}

	#endregion

	private void OnContentContextMenuRequested(object sender, ContentContextMenuEventArgs e)
	{
		int x = e.Position.X;
		int y = e.Position.Y;
		if(IsSelectable)
		{
			var dpi  = Dpi.FromControl(FlowControl);
			var conv = DpiConverter.FromDefaultTo(dpi);
			x += conv.ConvertX(SelectionMargin);
		}
		ShowContextMenu(e.ContextMenu, x, y);
	}

	private void OnContentInvalidated(object sender, ContentInvalidatedEventArgs e)
	{
		var bounds = e.Bounds;
		if(IsSelectable)
		{
			var dpi  = Dpi.FromControl(FlowControl);
			var conv = DpiConverter.FromDefaultTo(dpi);
			bounds.X += conv.ConvertX(SelectionMargin);
		}
		InvalidateSafe(bounds);
	}

	private void OnContentSizeChanged(object sender, EventArgs e)
	{
		InvalidateSize();
	}

	private void OnContentCursorChanged(object sender, EventArgs e)
	{
		FlowControl.Cursor = _content.Cursor;
	}

	/// <inheritdoc/>
	protected override void OnMouseMove(int x, int y)
	{
		if(IsSelectable)
		{
			var dpi  = Dpi.FromControl(FlowControl);
			var conv = DpiConverter.FromDefaultTo(dpi);
			x -= conv.ConvertX(SelectionMargin);
		}
		if(x < 0)
		{
			_content.OnMouseLeave();
		}
		else
		{
			_content.OnMouseMove(x, y);
		}
		base.OnMouseMove(x, y);
	}

	/// <inheritdoc/>
	protected override void OnMouseLeave()
	{
		base.OnMouseLeave();
		_content.OnMouseLeave();
	}

	public bool IsSelectable { get; set; }

	public bool IsSelected
	{
		get => _isSelected;
		set
		{
			if(_isSelected != value)
			{
				_isSelected = value;
				Invalidate();
				if(value && FlowControl is not null)
				{
					foreach(var p in FlowControl.Panels)
					{
						if(p != this && p is RevisionHeaderPanel rhp)
						{
							rhp.IsSelected = false;
						}
					}
				}
			}
		}
	}

	/// <summary>Displayed <see cref="T:gitter.Git.Revision"/>.</summary>
	public Revision Revision
	{
		get => _revision;
		set
		{
			if(_revision != value)
			{
				_revision = value;
				if(FlowControl is not null)
				{
					_content.Revision = _revision;
				}
			}
		}
	}

	public List<IHyperlinkExtractor> AdditionalHyperlinkExtractors { get; } = new();

	/// <inheritdoc/>
	protected override void OnFlowControlAttached()
	{
		_content.Revision = _revision;
		base.OnFlowControlAttached();
	}

	/// <inheritdoc/>
	protected override void OnFlowControlDetached()
	{
		_content.Revision = null;
		base.OnFlowControlDetached();
	}

	/// <inheritdoc/>
	protected override void OnMouseDown(int x, int y, MouseButtons button)
	{
		base.OnMouseDown(x, y, button);
		if(IsSelectable)
		{
			if(button == MouseButtons.Left)
			{
				IsSelected = true;
			}
			var dpi  = Dpi.FromControl(FlowControl);
			var conv = DpiConverter.FromDefaultTo(dpi);
			x -= conv.ConvertX(SelectionMargin);
		}
		_content.OnMouseDown(x, y, button);
	}

	/// <inheritdoc/>
	protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		var conv    = DpiConverter.FromDefaultTo(measureEventArgs.Dpi);
		var marginX = conv.ConvertX(SelectionMargin);
		var width   = measureEventArgs.Width;
		if(IsSelectable)
		{
			width -= marginX;
		}
		_content.Style = Style;
		var size = _content.OnMeasure(measureEventArgs.Graphics, new Dpi(FlowControl.DeviceDpi), width);
		if(IsSelectable)
		{
			size.Width += marginX;
		}
		return size;
	}

	/// <inheritdoc/>
	protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		var conv     = DpiConverter.FromDefaultTo(paintEventArgs.Dpi);
		var marginX  = conv.ConvertX(SelectionMargin);
		var bounds   = paintEventArgs.Bounds;
		var graphics = paintEventArgs.Graphics;
		graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
		if(IsSelectable)
		{
			if(IsSelected)
			{
				var rcFill = Rectangle.Intersect(paintEventArgs.ClipRectangle,
					new Rectangle(bounds.X, bounds.Y, marginX, bounds.Height));
				graphics.GdiFill(SystemColors.Highlight, rcFill);
			}
			bounds.Width -= marginX;
			bounds.X     += marginX;
		}
		_content.Style = Style;
		var clip = Rectangle.Intersect(paintEventArgs.ClipRectangle, bounds);
		if(clip is { Width: > 0, Height: > 0 })
		{
			graphics.SetClip(clip);
			_content.OnPaint(graphics, new Dpi(FlowControl.DeviceDpi), bounds, clip);
		}
	}
}
