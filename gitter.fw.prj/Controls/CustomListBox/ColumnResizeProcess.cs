#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

/// <summary><see cref="CustomListBoxColumn"/> resize process.</summary>
class ColumnResizeProcess
{
	#region Data

	private readonly Point _mouseDownLocation;

	#endregion

	#region Static

	/// <summary>Create resize process from column list, active (hovered) column and side (left or right) of resize.</summary>
	public static ColumnResizeProcess FromActiveColumn(IList<CustomListBoxColumn> columns, int activeIndex, ColumnResizeSide side, Point mouseDownLocation)
	{
		Assert.IsNotNull(columns);

		var activeColumn = columns[activeIndex];

		int autoSizeId = int.MaxValue;
		for(int i = 0; i < columns.Count; ++i)
		{
			if(columns[i].IsVisible && columns[i].SizeMode == ColumnSizeMode.Fill)
			{
				autoSizeId = i;
				break;
			}
		}
		switch(side)
		{
			case ColumnResizeSide.Left:
				var prevColumn = columns.FindPrevious(activeIndex, column => column.IsVisible);
				if(prevColumn != null && activeIndex <= autoSizeId)
				{
					if(prevColumn.SizeMode == ColumnSizeMode.Sizeable)
					{
						return new ColumnResizeProcess(prevColumn, 1, mouseDownLocation);
					}
				}
				if(activeColumn.SizeMode != ColumnSizeMode.Sizeable)
				{
					return null;
				}
				return new ColumnResizeProcess(activeColumn, -1, mouseDownLocation);
			case ColumnResizeSide.Right:
				if(activeColumn.SizeMode != ColumnSizeMode.Sizeable || activeIndex > autoSizeId)
				{
					var nextColumn = columns.FindNext(activeIndex, column => column.IsVisible);
					if(nextColumn is not null)
					{
						return new ColumnResizeProcess(nextColumn, -1, mouseDownLocation);
					}
					else
					{
						return null;
					}
				}
				else
				{
					return new ColumnResizeProcess(activeColumn, 1, mouseDownLocation);
				}

			default:
				throw new ArgumentException("Invalid column resize side.", nameof(side));
		}
	}

	#endregion

	#region .ctor

	private ColumnResizeProcess(CustomListBoxColumn resizingColumn, int deltaSign, Point mouseDownLocation)
	{
		Assert.IsNotNull(resizingColumn);

		_mouseDownLocation = mouseDownLocation;
		ResizingColumn    = resizingColumn;
		InitialWidth      = resizingColumn.Width;
		DeltaSign         = deltaSign;
	}

	#endregion

	#region Properties

	/// <summary> Column actually resizing. May differ from active column. </summary>
	public CustomListBoxColumn ResizingColumn { get; }

	/// <summary>Sign to apply to x-coordinate delta when resizing.</summary>
	public int DeltaSign { get; }

	/// <summary>Initial column width.</summary>
	public ValueWithDpi<int> InitialWidth { get; }

	#endregion

	#region Methods

	/// <summary>Cancels column resize.</summary>
	public void Cancel()
	{
		ResizingColumn.Width = InitialWidth;
	}

	/// <summary>Updates resized column width.</summary>
	/// <param name="mouseLocation">Current mouse pointer location.</param>
	public void Update(Point mouseLocation)
	{
		var dpi = Dpi.FromControl(ResizingColumn.ListBox);
		int dx = (mouseLocation.X - _mouseDownLocation.X) * DeltaSign;
		int w;
		if(InitialWidth.Dpi.X == 0)
		{
			w = dx;
		}
		else
		{
			w = InitialWidth.Value * dpi.X / InitialWidth.Dpi.X + dx;
		}
		if(w < ResizingColumn.MinWidth)
		{
			w = ResizingColumn.MinWidth;
		}
		ResizingColumn.Width = new(w, dpi);
	}

	#endregion
}
