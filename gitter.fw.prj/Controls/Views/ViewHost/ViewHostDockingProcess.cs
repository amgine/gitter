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

namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Native;

	sealed class ViewHostDockingProcess : IMouseDragProcess, IDisposable
	{
		#region Data

		private readonly ViewHost _viewHost;
		private bool _isActive;
		private ViewHost _hoveredHost;
		private ViewDockGrid _hoveredGrid;

		#endregion

		public ViewHostDockingProcess(ViewHost viewHost)
		{
			Verify.Argument.IsNotNull(viewHost, "viewHost");

			_viewHost = viewHost;
		}

		#region Properties

		public ViewHost ViewHost
		{
			get { return _viewHost; }
		}

		public bool IsActive
		{
			get { return _isActive; }
		}

		public ViewHost HoveredViewHost
		{
			get { return _hoveredHost; }
		}

		public ViewDockGrid HoveredDockGrid
		{
			get { return _hoveredGrid; }
		}

		#endregion

		private static int ZOrderComparison(Control control1, Control control2)
		{
			return ZOrderComparison(control1.TopLevelControl.Handle, control2.TopLevelControl.Handle);
		}

		private static int ZOrderComparison(IntPtr hWnd1, IntPtr hWnd2)
		{
			const int GW_HWNDNEXT = 2;
			const int GW_HWNDPREV = 3;
			IntPtr hWnd;
			hWnd = hWnd1;
			while(hWnd != IntPtr.Zero)
			{
				hWnd = User32.GetWindow(hWnd, GW_HWNDNEXT);
				if(hWnd == hWnd2) return -1;
			}
			hWnd = hWnd1;
			while(hWnd != IntPtr.Zero)
			{
				hWnd = User32.GetWindow(hWnd, GW_HWNDPREV);
				if(hWnd == hWnd2) return 1;
			}
			return 0;
		}

		private static ViewDockGrid HitTestGrid(Point point)
		{
			var candidates = new List<ViewDockGrid>();
			lock(ViewDockGrid.Grids)
			{
				foreach(var grid in ViewDockGrid.Grids)
				{
					if(grid.Created && grid.Visible)
					{
						var p = grid.PointToClient(point);
						var bounds = grid.ClientRectangle;
						if(bounds.Contains(p))
						{
							candidates.Add(grid);
						}
					}
				}
			}
			if(candidates.Count == 0)
			{
				return null;
			}
			if(candidates.Count == 1)
			{
				return candidates[0];
			}
			candidates.Sort(ZOrderComparison);
			return candidates[0];
		}

		private ViewHost HitTestViewHost(Point point)
		{
			var candidates = new List<ViewHost>();
			lock(ViewHost.ViewHosts)
			{
				foreach(var host in ViewHost.ViewHosts)
				{
					if(host.Created && host.Visible)
					{
						if(host != _viewHost && host.Visible && host.Status != ViewHostStatus.AutoHide)
						{
							var p = host.PointToClient(point);
							var bounds = host.ClientRectangle;
							if(bounds.Contains(p))
							{
								candidates.Add(host);
							}
						}
					}
				}
			}
			if(candidates.Count == 0)
			{
				return null;
			}
			if(candidates.Count == 1)
			{
				return candidates[0];
			}
			candidates.Sort(ZOrderComparison);
			return candidates[0];
		}

		public bool Start(Point location)
		{
			Verify.State.IsFalse(IsActive);

			_isActive = true;
			location = _viewHost.PointToScreen(location);
			var grid = HitTestGrid(location);
			if(grid != null)
			{
				_hoveredGrid = grid;
				grid.DockMarkers.Show(_viewHost);
				grid.DockMarkers.UpdateHover(location);
			}
			var host = HitTestViewHost(location);
			if(host != null)
			{
				_hoveredHost = host;
				host.DockMarkers.Show(_viewHost);
				host.DockMarkers.UpdateHover(location);
			}
			return true;
		}

		public void Update(Point location)
		{
			Verify.State.IsTrue(IsActive);

			bool gridHit = false;
			location = _viewHost.PointToScreen(location);
			var grid = HitTestGrid(location);
			if(grid != null)
			{
				if(_hoveredGrid != null)
				{
					if(_hoveredGrid != grid)
					{
						_hoveredGrid.DockMarkers.Hide();
						_hoveredGrid = grid;
						grid.DockMarkers.Show(_viewHost);
					}
				}
				else
				{
					_hoveredGrid = grid;
					grid.DockMarkers.Show(_viewHost);
				}
				gridHit = grid.DockMarkers.UpdateHover(location);
			}
			else
			{
				if(_hoveredGrid != null)
				{
					_hoveredGrid.DockMarkers.Hide();
					_hoveredGrid = null;
				}
			}
			var host = HitTestViewHost(location);
			if(host != null)
			{
				if(_hoveredHost != null)
				{
					if(_hoveredHost != host)
					{
						_hoveredHost.DockMarkers.Hide();
						_hoveredHost = host;
						host.DockMarkers.Show(_viewHost);
					}
				}
				else
				{
					_hoveredHost = host;
					host.DockMarkers.Show(_viewHost);
				}
				if(!gridHit)
				{
					host.DockMarkers.UpdateHover(location);
				}
				else
				{
					host.DockMarkers.Unhover();
				}
			}
			else
			{
				if(_hoveredHost != null)
				{
					_hoveredHost.DockMarkers.Hide();
					_hoveredHost = null;
				}
			}
		}

		public void Commit(Point location)
		{
			Verify.State.IsTrue(IsActive);

			_isActive = false;
			bool docking = false;
			location = _viewHost.PointToScreen(location);
			if(_hoveredGrid != null)
			{
				var dockResult = _hoveredGrid.DockMarkers.HitTest(location);
				if(_hoveredGrid.CanDock(_viewHost, dockResult))
				{
					docking = true;
					_hoveredGrid.PerformDock(_viewHost, dockResult);
				}
				_hoveredGrid.DockMarkers.Hide();
				_hoveredGrid = null;
			}
			if(_hoveredHost != null)
			{
				var host = _hoveredHost;
				if(!docking)
				{
					var dockResult = host.DockMarkers.HitTest(location);
					if(host.CanDock(_viewHost, dockResult))
					{
						host.PerformDock(_viewHost, dockResult);
					}
				}
				host.DockMarkers.Hide();
				host = null;
			}
		}

		public void Cancel()
		{
			Verify.State.IsTrue(IsActive);

			_isActive = false;
			if(_hoveredGrid != null)
			{
				_hoveredGrid.DockMarkers.Hide();
				_hoveredGrid = null;
			}
			if(_hoveredHost != null)
			{
				_hoveredHost.DockMarkers.Hide();
				_hoveredHost = null;
			}
		}

		public void Dispose()
		{
			if(_hoveredGrid != null)
			{
				_hoveredGrid.DockMarkers.Hide();
				_hoveredGrid = null;
			}
			if(_hoveredHost != null)
			{
				_hoveredHost.DockMarkers.Hide();
				_hoveredHost = null;
			}
			_isActive = false;
		}
	}
}
