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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

sealed class ViewHostResizingProcess : IMouseDragProcess, IDisposable
{
	private SplitterMarker? _splitterMarker;
	private int _resizeOffset;
	private int _minimumPosition;
	private int _maximumPosition;

	public ViewHostResizingProcess(ViewHost viewHost)
	{
		Verify.Argument.IsNotNull(viewHost);

		ViewHost = viewHost;
	}

	public ViewHost ViewHost { get; }

	public bool IsActive { get; private set; }

	public bool Start(Point location)
	{
		Verify.State.IsFalse(IsActive);
		Verify.State.IsTrue(ViewHost.Status == ViewHostStatus.AutoHide);
		Verify.State.IsTrue(ViewHost.Visible);

		var dpi = Dpi.FromControl(ViewHost);

		IsActive = true;
		var size = ViewHost.Size;
		Rectangle bounds;
		var side = ViewHost.DockSide ?? throw new InvalidOperationException();
		var grid = side.DockPanel;
		Orientation orientation;
		switch(side.Side)
		{
			case AnchorStyles.Left:
				_minimumPosition = ViewConstants.MinimumHostWidth.GetValue(dpi);
				_maximumPosition = grid.HorizontalClientSpace - ViewConstants.SideDockPanelBorderSize;
				bounds = new Rectangle(
					size.Width - ViewConstants.SideDockPanelBorderSize, 0,
					ViewConstants.SideDockPanelBorderSize, size.Height);
				_resizeOffset = location.X - bounds.X;
				orientation = Orientation.Horizontal;
				break;
			case AnchorStyles.Top:
				_minimumPosition = ViewConstants.MinimumHostHeight.GetValue(dpi);
				_maximumPosition = grid.VerticalClientSpace - ViewConstants.SideDockPanelBorderSize;
				bounds = new Rectangle(
					0, size.Height - ViewConstants.SideDockPanelBorderSize,
					size.Width, ViewConstants.SideDockPanelBorderSize);
				_resizeOffset = location.Y - bounds.Y;
				orientation = Orientation.Vertical;
				break;
			case AnchorStyles.Right:
				_minimumPosition = size.Width - grid.HorizontalClientSpace;
				_maximumPosition = size.Width - ViewConstants.MinimumHostWidth.GetValue(dpi);
				bounds = new Rectangle(
					0, 0,
					ViewConstants.SideDockPanelBorderSize, size.Height);
				_resizeOffset = location.X - bounds.X;
				orientation = Orientation.Horizontal;
				break;
			case AnchorStyles.Bottom:
				_minimumPosition = size.Height - grid.VerticalClientSpace;
				_maximumPosition = size.Height - ViewConstants.MinimumHostHeight.GetValue(dpi);
				bounds = new Rectangle(
					0, 0,
					size.Width, ViewConstants.SideDockPanelBorderSize);
				_resizeOffset = location.Y - bounds.Y;
				orientation = Orientation.Vertical;
				break;
			default:
				throw new ApplicationException();
		}
		bounds.Location = ViewHost.PointToClient(bounds.Location);
		SpawnMarker(bounds, orientation);
		return true;
	}

	private int ClampPosition(int position)
	{
		if(position <= _minimumPosition) return _minimumPosition;
		if(position >= _maximumPosition) return _maximumPosition;
		return position;
	}

	public void Update(Point location)
	{
		Verify.State.IsTrue(IsActive);

		var side = ViewHost.DockSide ?? throw new InvalidOperationException();
		switch(side.Orientation)
		{
			case Orientation.Vertical:
				{
					var x = ClampPosition(location.X - _resizeOffset);
					location = new Point(x, 0);
				}
				break;
			case Orientation.Horizontal:
				{
					var y = ClampPosition(location.Y - _resizeOffset);
					location = new Point(0, y);
				}
				break;
			default:
				throw new ApplicationException($"Unexpected ViewDockSide.Orientation: {side.Orientation}");
		}
		location = ViewHost.PointToScreen(location);
		if(_splitterMarker is not null)
		{
			_splitterMarker.Location = location;
		}
	}

	public void Commit(Point e)
	{
		Verify.State.IsTrue(IsActive);

		KillMarker();
		IsActive = false;
		var side = ViewHost.DockSide ?? throw new InvalidOperationException();
		switch(side.Side)
		{
			case AnchorStyles.Left:
				{
					var x = ClampPosition(e.X - _resizeOffset);
					ViewHost.Width = x + ViewConstants.SideDockPanelBorderSize;
				}
				break;
			case AnchorStyles.Top:
				{
					var y = ClampPosition(e.Y - _resizeOffset);
					ViewHost.Height = y + ViewConstants.SideDockPanelBorderSize;
				}
				break;
			case AnchorStyles.Right:
				{
					var x  = ClampPosition(e.X - _resizeOffset);
					var w  = ViewHost.Width - x;
					var dw = ViewHost.Width - w;
					ViewHost.SetBounds(ViewHost.Left + dw, 0, w, 0, BoundsSpecified.X | BoundsSpecified.Width);
				}
				break;
			case AnchorStyles.Bottom:
				{
					var y  = ClampPosition(e.Y - _resizeOffset);
					var h  = ViewHost.Height - y;
					var dh = ViewHost.Height - h;
					ViewHost.SetBounds(0, ViewHost.Top + dh, 0, h, BoundsSpecified.Y | BoundsSpecified.Height);
				}
				break;
			default:
				throw new ApplicationException($"Unexpected ViewDockSide.Side: {side.Side}");
		}
	}

	public void Cancel()
	{
		Verify.State.IsTrue(IsActive);

		KillMarker();
		IsActive = false;
	}

	#if NETCOREAPP
	[MemberNotNull(nameof(_splitterMarker))]
	#endif
	private void SpawnMarker(Rectangle bounds, Orientation orientation)
	{
		Verify.State.IsTrue(_splitterMarker is null);

		_splitterMarker = new SplitterMarker(bounds, orientation);
		_splitterMarker.Show();
	}

	private void KillMarker()
	{
		if(_splitterMarker is null) return;

		_splitterMarker.Close();
		_splitterMarker.Dispose();
		_splitterMarker = null;
	}

	public void Dispose()
	{
		if(_splitterMarker is not null)
		{
			_splitterMarker.Dispose();
			_splitterMarker = null;
		}
		IsActive = false;
	}
}
