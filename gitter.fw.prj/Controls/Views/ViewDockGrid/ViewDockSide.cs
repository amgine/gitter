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

namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;
	using System.ComponentModel;
	using System.Globalization;

	using gitter.Framework.Services;

	/// <summary>
	/// Collection of auto-hiding <see cref="ViewHost"/>'s on one side of <see cref="ViewDockGrid"/>,
	/// represented as tabs.
	/// </summary>
	[ToolboxItem(false)]
	public sealed class ViewDockSide : Control, IEnumerable<ViewHost>
	{
		#region Static Data

		private static readonly Color BackgroundColor = Color.FromArgb(41, 57, 85);

		#endregion

		#region Data

		private readonly ViewDockGrid _grid;
		private readonly AnchorStyles _side;
		private readonly Orientation _orientation;
		private readonly List<ViewHost> _dockedHosts;
		private readonly List<ViewDockSideTab> _tabs;
		private readonly TrackingService<ViewDockSideTab> _tabHover;
		private readonly TrackingService<ViewDockSideTab> _tabPress;
		private readonly Timer _autoShowTimer;
		private readonly Timer _autoHideTimer;
		private ViewHost _visibleHost;
		private int _size;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="ViewDockSide"/> class.</summary>
		/// <param name="grid">Host grid.</param>
		/// <param name="side">Align side.</param>
		public ViewDockSide(ViewDockGrid grid, AnchorStyles side)
		{
			Verify.Argument.IsNotNull(grid, "grid");

			switch(side)
			{
				case AnchorStyles.Left:
				case AnchorStyles.Right:
					_orientation = Orientation.Vertical;
					break;
				case AnchorStyles.Top:
				case AnchorStyles.Bottom:
					_orientation = Orientation.Horizontal;
					break;
				default:
					throw new ArgumentException(
						"Unknown AnchorStyles value: {0}".UseAsFormat(side),
						"side");
			}
			_grid = grid;
			_side = side;
			_dockedHosts = new List<ViewHost>();
			_tabs = new List<ViewDockSideTab>();

			_tabHover = new TrackingService<ViewDockSideTab>(OnTabHoverChanged);
			_tabPress = new TrackingService<ViewDockSideTab>(OnTabPressChanged);
			_autoShowTimer = new Timer() { Interval = 500 };
			_autoHideTimer = new Timer() { Interval = 500 };

			_autoShowTimer.Tick += OnAutoShowTimerTick;
			_autoHideTimer.Tick += OnAutoHideTimerTick;

			SetStyle(
				ControlStyles.ContainerControl |
				ControlStyles.Selectable |
				ControlStyles.SupportsTransparentBackColor,
				false);
			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.OptimizedDoubleBuffer,
				true);

			MinimumSize = new Size(1, 1);
		}

		#endregion

		#region Properties

		public override Size GetPreferredSize(Size proposedSize)
		{
			switch(Orientation)
			{
				case Orientation.Horizontal:
					return new Size(_size, Renderer.SideTabHeight);
				case Orientation.Vertical:
					return new Size(Renderer.SideTabHeight, _size);
				default:
					throw new ApplicationException(string.Format(
						CultureInfo.InvariantCulture,
						"Unexpected {0}.Orientation: {1}", GetType().Name, Orientation));
			}
		}

		private ViewRenderer Renderer
		{
			get { return ViewManager.Renderer; }
		}

		/// <summary>Host <see cref="ViewDockGrid"/>.</summary>
		public ViewDockGrid Grid
		{
			get { return _grid; }
		}

		/// <summary>Align side.</summary>
		/// <value>Align side.</value>
		public AnchorStyles Side
		{
			get { return _side; }
		}

		/// <summary>Optiomal control length.</summary>
		public int OptimalLength
		{
			get { return _size + Renderer.SideTabSpacing * (_tabs.Count - 1); }
		}

		/// <summary>Gets control orientation.</summary>
		/// <value>Control orientation.</value>
		public Orientation Orientation
		{
			get { return _orientation; }
		}

		public ViewHost this[int index]
		{
			get { return _dockedHosts[index]; }
		}

		/// <summary>Adds <see cref="ViewHost"/> to this <see cref="ViewDockSide"/>.</summary>
		/// <param name="viewHost"><see cref="ViewHost"/> to add.</param>
		public void AddHost(ViewHost viewHost)
		{
			Verify.Argument.IsNotNull(viewHost, "viewHost");

			_dockedHosts.Add(viewHost);
			viewHost.DockSide = this;
			viewHost.Status = ViewHostStatus.AutoHide;
			int size = 0;
			using(var graphics = CreateGraphics())
			{
				if(viewHost.ViewsCount != 0)
				{
					foreach(var view in viewHost.Views)
					{
						var tab = new ViewDockSideTab(this, viewHost, view);
						tab.ResetLength(graphics);
						size += tab.Length;
						_tabs.Add(tab);
						view.TextChanged += OnViewTextChanged;
					}
				}
			}
			switch(Orientation)
			{
				case Orientation.Vertical:
					{
						var space = _grid.VerticalClientSpace;
						var height = _size + size;
						_size = height;
						height += _tabs.Count * Renderer.SideTabSpacing;
						if(height > space) height = space;
						Height = height;
					}
					break;
				case Orientation.Horizontal:
					{
						var space = _grid.HorizontalClientSpace;
						var width = _size + size;
						_size = width;
						width += _tabs.Count * Renderer.SideTabSpacing;
						if(width > space) width = space;
						Width = width;
					}
					break;
				default:
					throw new ApplicationException(string.Format(
						CultureInfo.InvariantCulture,
						"Unexpected {0}.Orientation: {1}", GetType().Name, Orientation));
			}
			viewHost.ViewAdded += OnViewAdded;
			viewHost.ViewRemoved += OnViewRemoved;
		}

		public void RemoveHost(ViewHost viewHost)
		{
			Verify.Argument.IsNotNull(viewHost, "viewHost");

			if(_dockedHosts.Remove(viewHost))
			{
				if(_dockedHosts.Count == 0)
				{
					_grid.KillSide(_side);
				}
				else
				{
					int size = 0;
					for(int i = _tabs.Count - 1; i >= 0; --i)
					{
						var tab = _tabs[i];
						if(tab.ViewHost == viewHost)
						{
							tab.View.TextChanged -= OnViewTextChanged;
							_tabs.RemoveAt(i);
						}
						else
						{
							size += tab.Length;
						}
					}
					_size = size;
					switch(_orientation)
					{
						case Orientation.Vertical:
							{
								var space = _grid.VerticalClientSpace;
								size += _tabs.Count * Renderer.SideTabSpacing;
								if(size > space) size = space;
								Height = size;
							}
							break;
						case Orientation.Horizontal:
							{
								var space = _grid.HorizontalClientSpace;
								size += _tabs.Count * Renderer.SideTabSpacing;
								if(size > space) size = space;
								Width = size;
							}
							break;
						default:
							throw new ApplicationException(string.Format(
								CultureInfo.InvariantCulture,
								"Unexpected {0}.Orientation: {1}", GetType().Name, Orientation));
					}
				}
				viewHost.ViewAdded -= OnViewAdded;
				viewHost.ViewRemoved -= OnViewRemoved;
				if(_visibleHost == viewHost)
				{
					DespawnPanel();
				}
			}
		}

		private void OnViewAdded(object sender, ViewEventArgs e)
		{
			var host = (ViewHost)sender;
			int i = _tabs.Count - 1;
			for(; i >= 0; --i)
			{
				if(_tabs[i].ViewHost == host)
				{
					break;
				}
			}
			var tab = new ViewDockSideTab(this, host, e.View);
			tab.ResetLength();
			var size = tab.Length;
			_tabs.Insert(i, tab);
			switch(_orientation)
			{
				case Orientation.Vertical:
					{
						var space = _grid.VerticalClientSpace;
						var height = _size + size;
						_size = height;
						if(_tabs.Count != 1)
						{
							height += Renderer.SideTabSpacing;
						}
						if(height > space) height = space;
						Height = height;
					}
					break;
				case Orientation.Horizontal:
					{
						var space = _grid.HorizontalClientSpace;
						var width = _size + size;
						_size = width;
						if(_tabs.Count != 1)
						{
							width += Renderer.SideTabSpacing;
						}
						if(width > space) width = space;
						Width = width;
					}
					break;
				default:
					throw new ApplicationException(string.Format(
						CultureInfo.InvariantCulture,
						"Unexpected {0}.Orientation: {1}", GetType().Name, Orientation));
			}
			e.View.TextChanged += OnViewTextChanged;
			Invalidate();
		}

		private void OnViewRemoved(object sender, ViewEventArgs e)
		{
			e.View.TextChanged -= OnViewTextChanged;
			var size = 0;
			int index = -1;
			for(int i = _tabs.Count - 1; i >= 0; --i)
			{
				if(_tabs[i].View == e.View)
				{
					size = _tabs[i].Length;
					_tabs.RemoveAt(i);
					index = i;
					break;
				}
			}
			if(index != -1)
			{
				_size -= size;
				size = _size;
				switch(_orientation)
				{
					case Orientation.Vertical:
						{
							var space = _grid.VerticalClientSpace;
							size += _tabs.Count * Renderer.SideTabSpacing;
							if(size > space) size = space;
							Height = size;
						}
						break;
					case Orientation.Horizontal:
						{
							var space = _grid.HorizontalClientSpace;
							size += _tabs.Count * Renderer.SideTabSpacing;
							if(size > space) size = space;
							Width = size;
						}
						break;
					default:
						throw new ApplicationException(string.Format(
							CultureInfo.InvariantCulture,
							"Unexpected {0}.Orientation: {1}", GetType().Name, Orientation));
				}
				Invalidate();
				if(_tabHover.Index == index)
				{
					_tabHover.Reset(-1, null);
				}
				else if(_tabHover.Index > index)
				{
					_tabHover.ResetIndex(_tabHover.Index - 1);
				}
				if(sender == _visibleHost)
				{
					DespawnPanel();
				}
			}
		}

		private void OnViewTextChanged(object sender, EventArgs e)
		{
			var view   = (ViewBase)sender;
			var tab    = GetTab(view);
			var length = tab.Length;
			tab.ResetLength(GraphicsUtility.MeasurementGraphics);
			var dl = tab.Length - length;
			_size += dl;
			Invalidate();
		}

		/// <summary>Gets the count of docked <see cref="ViewHost"/>s.</summary>
		public int Count
		{
			get { return _dockedHosts.Count; }
		}

		/// <summary>Gets the count of <see cref="ViewDockSideTab"/>s.</summary>
		public int TabCount
		{
			get { return _tabs.Count; }
		}

		#endregion

		private int HitTest(int x, int y)
		{
			if(x < 0 || y < 0 || x >= Width || y >= Height) return -1;
			int coord;
			switch(_orientation)
			{
				case Orientation.Horizontal:
					coord = x;
					break;
				case Orientation.Vertical:
					coord = y;
					break;
				default:
					throw new ApplicationException();
			}
			int offset = 0;
			for(int i = 0; i < _tabs.Count; ++i)
			{
				var tab = _tabs[i];
				var offset2 = offset + tab.Length;
				if(coord >= offset && coord < offset2)
					return i;
				offset = offset2 + Renderer.SideTabSpacing;
			}
			return -1;
		}

		private ViewDockSideTab GetTab(ViewBase view)
		{
			for(int i = 0; i < _tabs.Count; ++i)
			{
				if(_tabs[i].View == view)
				{
					return _tabs[i];
				}
			}
			return null;
		}

		private Rectangle GetTabBounds(int index)
		{
			Verify.Argument.IsValidIndex(index, _tabs.Count, "index");

			var bounds = Rectangle.Empty;
			switch(_orientation)
			{
				case Orientation.Horizontal:
					bounds.Height = Renderer.SideTabHeight;
					break;
				case Orientation.Vertical:
					bounds.Width = Renderer.SideTabHeight;
					break;
				default:
					throw new ApplicationException();
			}
			for(int i = 0; i < _tabs.Count; ++i)
			{
				var tab = _tabs[i];
				var length = tab.Length;
				switch(_orientation)
				{
					case Orientation.Horizontal:
						{
							bounds.Width = length;
							if(i == index) return bounds;
							bounds.X += bounds.Width + Renderer.SideTabSpacing;
						}
						break;
					case Orientation.Vertical:
						{
							bounds.Height = length;
							if(i == index) return bounds;
							bounds.Y += bounds.Height + Renderer.SideTabSpacing;
						}
						break;
					default:
						throw new ApplicationException(string.Format(
							CultureInfo.InvariantCulture,
							"Unexpected {0}.Orientation: {1}", GetType().Name, Orientation));
				}
			}
			throw new ApplicationException();
		}

		private void SpawnPanel(ViewDockSideTab tab)
		{
			SpawnPanel(tab, false);
		}

		private void SpawnPanel(ViewDockSideTab tab, bool activate)
		{
			_grid.SuspendLayout();
			if(_visibleHost == tab.ViewHost)
			{
				if(activate)
					_visibleHost.Activate(tab.View);
				else
					_visibleHost.SetActiveView(tab.View);
			}
			else
			{
				DespawnPanel();
				Rectangle bounds;
				AnchorStyles anchor;
				_visibleHost = tab.ViewHost;
				switch(_side)
				{
					case AnchorStyles.Left:
						{
							var w = _visibleHost.Width;
							bounds = new Rectangle(Renderer.SideTabHeight, Top, w, _grid.VerticalClientSpace);
							anchor = ViewConstants.AnchorDockLeft;
						}
						break;
					case AnchorStyles.Top:
						{
							var h = _visibleHost.Height;
							bounds = new Rectangle(Left, Renderer.SideTabHeight, _grid.HorizontalClientSpace, h);
							anchor = ViewConstants.AnchorDockTop;
						}
						break;
					case AnchorStyles.Right:
						{
							var w = _visibleHost.Width;
							bounds = new Rectangle(Left - w, Top, w, _grid.VerticalClientSpace);
							anchor = ViewConstants.AnchorDockRight;
						}
						break;
					case AnchorStyles.Bottom:
						{
							var h = _visibleHost.Height;
							bounds = new Rectangle(Left, Top - h, _grid.HorizontalClientSpace, h);
							anchor = ViewConstants.AnchorDockBottom;
						}
						break;
					default:
						_visibleHost = null;
						throw new ApplicationException(string.Format(
							CultureInfo.InvariantCulture,
							"Unexpected {0}.Side: {1}", GetType().Name, Side));
				}
				if(_side != AnchorStyles.Left && _grid.LeftSide != null)
				{
					_grid.LeftSide.DespawnPanel();
				}
				if(_side != AnchorStyles.Top && _grid.TopSide != null)
				{
					_grid.TopSide.DespawnPanel();
				}
				if(_side != AnchorStyles.Right && _grid.RightSide != null)
				{
					_grid.RightSide.DespawnPanel();
				}
				if(_side != AnchorStyles.Bottom && _grid.BottomSide != null)
				{
					_grid.BottomSide.DespawnPanel();
				}
				_visibleHost.SetActiveView(tab.View);
				_visibleHost.Bounds = bounds;
				_visibleHost.Anchor = anchor;
				_visibleHost.Parent = _grid;
				_visibleHost.BringToFront();
				_grid.ResumeLayout(true);
				if(activate) _visibleHost.Activate();
			}
			_autoHideTimer.Enabled = true;
		}

		public void ActivateView(ViewBase view)
		{
			Verify.Argument.IsNotNull(view, "view");

			for(int i = 0; i < _tabs.Count; ++i)
			{
				var tab = _tabs[i];
				if(tab.View == view)
				{
					SpawnPanel(tab, true);
					return;
				}
			}
			throw new ArgumentException("View was not found", "view");
		}

		internal void KillPanel()
		{
			if(_visibleHost != null)
			{
				_visibleHost.Parent = null;
				_visibleHost.Dispose();
				_visibleHost = null;
			}
		}

		private void OnTabHoverChanged(object sender, TrackingEventArgs<ViewDockSideTab> e)
		{
			if(e.IsTracked)
			{
				e.Item.OnMouseEnter();
				_autoShowTimer.Enabled = true;
			}
			else
			{
				e.Item.OnMouseLeave();
				_autoShowTimer.Enabled = false;
			}
			var bounds = GetTabBounds(e.Index);
			Invalidate(bounds);
		}

		private void OnTabPressChanged(object sender, TrackingEventArgs<ViewDockSideTab> e)
		{
		}

		private void OnAutoShowTimerTick(object sender, EventArgs e)
		{
			_autoShowTimer.Enabled = false;
			var tab = _tabHover.Item;
			if(tab != null)
			{
				if(_visibleHost == null)
				{
					SpawnPanel(tab);
				}
				else
				{
					if(_visibleHost != tab.ViewHost)
					{
						SpawnPanel(tab);
					}
					else
					{
						_visibleHost.SetActiveView(tab.View);
					}
				}
			}
		}

		private void DespawnPanel()
		{
			if(_visibleHost != null)
			{
				if(_visibleHost.Parent == _grid)
					_visibleHost.Parent = null;
				_visibleHost = null;
				_autoHideTimer.Enabled = false;
			}
		}

		private void OnAutoHideTimerTick(object sender, EventArgs e)
		{
			if(_visibleHost == null)
			{
				_autoHideTimer.Enabled = false;
			}
			else
			{
				if(!_visibleHost.IsActive && !_visibleHost.IsResizing)
				{
					var position = Control.MousePosition;
					var pos1 = PointToClient(position);
					if(!ClientRectangle.Contains(pos1))
					{
						var pos2 = _visibleHost.PointToClient(position);
						if(!_visibleHost.ClientRectangle.Contains(pos2))
						{
							DespawnPanel();
							_autoHideTimer.Enabled = false;
						}
					}
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			int index = HitTest(e.X, e.Y);
			if(index != -1)
				_tabHover.Track(index, _tabs[index]);
			else
				_tabHover.Drop();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			_tabHover.Drop();
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			int index = HitTest(e.X, e.Y);
			if(index != -1)
			{
				var tab = _tabs[index];
				Capture = false;
				if(_visibleHost != tab.ViewHost)
				{
					SpawnPanel(tab, true);
				}
				else
				{
					_visibleHost.Activate(tab.View);
				}
				_autoShowTimer.Enabled = false;
			}
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			ViewManager.Renderer.RenderViewDockSide(this, e);

			var graphics = e.Graphics;
			var clip = e.ClipRectangle;
			var bounds = Rectangle.Empty;
			switch(Orientation)
			{
				case Orientation.Horizontal:
					bounds.Height = Renderer.SideTabHeight;
					break;
				case Orientation.Vertical:
					bounds.Width = Renderer.SideTabHeight;
					break;
				default:
					throw new ApplicationException(string.Format(
						CultureInfo.InvariantCulture,
						"Unexpected {0}.Orientation: {1}", GetType().Name, Orientation));
			}
			for(int i = 0; i < _tabs.Count; ++i)
			{
				var tab = _tabs[i];
				var length = tab.Length;
				switch(Orientation)
				{
					case Orientation.Horizontal:
						{
							bounds.Width = length;
							if(bounds.Right >= clip.X)
							{
								tab.OnPaint(graphics, bounds);
							}
							bounds.X += bounds.Width + Renderer.SideTabSpacing;
							if(bounds.X >= clip.Right) return;
						}
						break;
					case Orientation.Vertical:
						{
							bounds.Height = length;
							if(bounds.Bottom >= clip.Y)
							{
								tab.OnPaint(graphics, bounds);
							}
							bounds.Y += bounds.Height + Renderer.SideTabSpacing;
							if(bounds.Y >= clip.Bottom) return;
						}
						break;
					default:
						throw new ApplicationException(string.Format(
							CultureInfo.InvariantCulture,
							"Unexpected {0}.Orientation: {1}", GetType().Name, Orientation));
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				foreach(var host in _dockedHosts)
				{
					host.ViewAdded -= OnViewAdded;
					host.ViewRemoved -= OnViewRemoved;
				}
				_dockedHosts.Clear();
				foreach(var tab in _tabs)
				{
					tab.View.TextChanged -= OnViewTextChanged;
					tab.Dispose();
				}
				_tabs.Clear();
				_tabPress.Reset(-1, null);
				_tabHover.Reset(-1, null);
				_autoShowTimer.Dispose();
				_autoHideTimer.Dispose();
				_visibleHost = null;
			}
			base.Dispose(disposing);
		}

		#region IEnumerable<ViewHost>

		public IEnumerator<ViewHost> GetEnumerator()
		{
			return _dockedHosts.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _dockedHosts.GetEnumerator();
		}

		#endregion
	}
}
