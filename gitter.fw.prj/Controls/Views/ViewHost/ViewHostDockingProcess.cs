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
		public ViewHostDockingProcess(ViewHost viewHost)
		{
			Verify.Argument.IsNotNull(viewHost, nameof(viewHost));

			ViewHost = viewHost;
		}

		public ViewHost ViewHost { get; }

		public bool IsActive { get; private set; }

		public ViewHost HoveredViewHost { get; private set; }

		public ViewDockGrid HoveredDockGrid { get; private set; }

		private static int ZOrderComparison(Control control1, Control control2)
			=> ZOrderComparison(control1.TopLevelControl.Handle, control2.TopLevelControl.Handle);

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
			if(candidates.Count == 0) return null;
			if(candidates.Count == 1) return candidates[0];
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
						if(host != ViewHost && host.Visible && host.Status != ViewHostStatus.AutoHide)
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
			if(candidates.Count == 0) return null;
			if(candidates.Count == 1) return candidates[0];
			candidates.Sort(ZOrderComparison);
			return candidates[0];
		}

		public bool Start(Point location)
		{
			Verify.State.IsFalse(IsActive);

			IsActive = true;
			location = ViewHost.PointToScreen(location);
			var grid = HitTestGrid(location);
			if(grid is not null)
			{
				HoveredDockGrid = grid;
				grid.DockMarkers.Show(ViewHost);
				grid.DockMarkers.UpdateHover(location);
			}
			var host = HitTestViewHost(location);
			if(host is not null)
			{
				HoveredViewHost = host;
				host.DockMarkers.Show(ViewHost);
				host.DockMarkers.UpdateHover(location);
			}
			return true;
		}

		public void Update(Point location)
		{
			Verify.State.IsTrue(IsActive);

			bool gridHit = false;
			location = ViewHost.PointToScreen(location);
			var grid = HitTestGrid(location);
			if(grid is not null)
			{
				if(HoveredDockGrid is not null)
				{
					if(HoveredDockGrid != grid)
					{
						HoveredDockGrid.DockMarkers.Hide();
						HoveredDockGrid = grid;
						grid.DockMarkers.Show(ViewHost);
					}
				}
				else
				{
					HoveredDockGrid = grid;
					grid.DockMarkers.Show(ViewHost);
				}
				gridHit = grid.DockMarkers.UpdateHover(location);
			}
			else
			{
				if(HoveredDockGrid is not null)
				{
					HoveredDockGrid.DockMarkers.Hide();
					HoveredDockGrid = null;
				}
			}
			var host = HitTestViewHost(location);
			if(host is not null)
			{
				if(HoveredViewHost is not null)
				{
					if(HoveredViewHost != host)
					{
						HoveredViewHost.DockMarkers.Hide();
						HoveredViewHost = host;
						host.DockMarkers.Show(ViewHost);
					}
				}
				else
				{
					HoveredViewHost = host;
					host.DockMarkers.Show(ViewHost);
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
				if(HoveredViewHost is not null)
				{
					HoveredViewHost.DockMarkers.Hide();
					HoveredViewHost = null;
				}
			}
		}

		public void Commit(Point location)
		{
			Verify.State.IsTrue(IsActive);

			IsActive = false;
			bool docking = false;
			location = ViewHost.PointToScreen(location);
			if(HoveredDockGrid is not null)
			{
				var dockResult = HoveredDockGrid.DockMarkers.HitTest(location);
				if(HoveredDockGrid.CanDock(ViewHost, dockResult))
				{
					docking = true;
					HoveredDockGrid.PerformDock(ViewHost, dockResult);
				}
				HoveredDockGrid.DockMarkers.Hide();
				HoveredDockGrid = null;
			}
			if(HoveredViewHost is not null)
			{
				var host = HoveredViewHost;
				if(!docking)
				{
					var dockResult = host.DockMarkers.HitTest(location);
					if(host.CanDock(ViewHost, dockResult))
					{
						host.PerformDock(ViewHost, dockResult);
					}
				}
				host.DockMarkers.Hide();
			}
		}

		public void Cancel()
		{
			Verify.State.IsTrue(IsActive);

			IsActive = false;
			if(HoveredDockGrid is not null)
			{
				HoveredDockGrid.DockMarkers.Hide();
				HoveredDockGrid = null;
			}
			if(HoveredViewHost is not null)
			{
				HoveredViewHost.DockMarkers.Hide();
				HoveredViewHost = null;
			}
		}

		public void Dispose()
		{
			if(HoveredDockGrid is not null)
			{
				HoveredDockGrid.DockMarkers.Hide();
				HoveredDockGrid = null;
			}
			if(HoveredViewHost is not null)
			{
				HoveredViewHost.DockMarkers.Hide();
				HoveredViewHost = null;
			}
			IsActive = false;
		}
	}
}
