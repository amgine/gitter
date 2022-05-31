#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Windows.Forms;

#nullable enable

public class ControlLayout
{
	private IContent? _content;
	private int _dpiChanging;

	public ControlLayout(Control control)
	{
		Verify.Argument.IsNotNull(control);

		Control = control;

		if(control is Form form)
		{
			form.DpiChanged += OnFormDpiChanged;
		}
		else
		{
			Control.DpiChangedBeforeParent += OnControlDpiChangedBeforeParent;
			Control.DpiChangedAfterParent  += OnControlDpiChangedAfterParent;
			Control.ParentChanged          += OnControlParentChanged;
		}
		Control.ClientSizeChanged += OnControlClientSizeChanged;
	}

	public Control Control { get; }

	public IContent? Content
	{
		get => _content;
		set
		{
			if(_content != value)
			{
				_content = value;
				UpdateLayout();
			}
		}
	}

	private void OnFormDpiChanged(object? sender, DpiChangedEventArgs e)
	{
		UpdateLayout();
	}

	private void OnControlClientSizeChanged(object? sender, EventArgs e)
	{
		if(_dpiChanging > 0) return;
		UpdateLayout();
	}

	private void OnControlDpiChangedBeforeParent(object? sender, EventArgs e)
	{
		++_dpiChanging;
	}

	private void OnControlDpiChangedAfterParent(object? sender, EventArgs e)
	{
		UpdateLayout();
		--_dpiChanging;
	}

	private void OnControlParentChanged(object? sender, EventArgs e)
	{
		UpdateLayout();
	}

	private void UpdateLayout()
	{
		var cr  = Control.ClientRectangle;
		var dpi = Control.DeviceDpi;
		Content?.UpdateBounds(cr, new(dpi));
	}
}
