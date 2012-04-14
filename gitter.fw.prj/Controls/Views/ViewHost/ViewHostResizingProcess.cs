namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	sealed class ViewHostResizingProcess : IMouseDragProcess, IDisposable
	{
		#region Data

		private readonly ViewHost _toolHost;
		private SplitterMarker _splitterMarker;
		private bool _isActive;
		private int _resizeOffset;
		private int _minimumPosition;
		private int _maximumPosition;

		#endregion

		#region .ctor

		public ViewHostResizingProcess(ViewHost toolHost)
		{
			if(toolHost == null) throw new ArgumentNullException("toolHost");
			_toolHost = toolHost;
		}

		#endregion

		#region Properties

		public ViewHost ToolHost
		{
			get { return _toolHost; }
		}

		public bool IsActive
		{
			get { return _isActive; }
		}

		#endregion

		public bool Start(Point location)
		{
			if(_isActive) throw new InvalidOperationException();
			if(_toolHost.Status != ViewHostStatus.AutoHide) throw new InvalidOperationException();
			if(!_toolHost.Visible) throw new InvalidOperationException();

			_isActive = true;
			var size = _toolHost.Size;
			Rectangle bounds;
			var side = _toolHost.DockSide;
			var grid = side.Grid;
			var rootCtlBounds = grid.RootControl.Bounds;
			Orientation orientation;
			switch(side.Side)
			{
				case AnchorStyles.Left:
					_minimumPosition = ViewConstants.MinimumHostWidth;
					_maximumPosition = grid.HorizontalClientSpace - ViewConstants.SideDockPanelBorderSize;
					bounds = new Rectangle(
						size.Width - ViewConstants.SideDockPanelBorderSize, 0,
						ViewConstants.SideDockPanelBorderSize, size.Height);
					_resizeOffset = location.X - bounds.X;
					orientation = Orientation.Horizontal;
					break;
				case AnchorStyles.Top:
					_minimumPosition = ViewConstants.MinimumHostHeight;
					_maximumPosition = grid.VerticalClientSpace - ViewConstants.SideDockPanelBorderSize;
					bounds = new Rectangle(
						0, size.Height - ViewConstants.SideDockPanelBorderSize,
						size.Width, ViewConstants.SideDockPanelBorderSize);
					_resizeOffset = location.Y - bounds.Y;
					orientation = Orientation.Vertical;
					break;
				case AnchorStyles.Right:
					_minimumPosition = size.Width - grid.HorizontalClientSpace;
					_maximumPosition = size.Width - ViewConstants.MinimumHostWidth;
					bounds = new Rectangle(
						0, 0,
						ViewConstants.SideDockPanelBorderSize, size.Height);
					_resizeOffset = location.X - bounds.X;
					orientation = Orientation.Horizontal;
					break;
				case AnchorStyles.Bottom:
					_minimumPosition = size.Height - grid.VerticalClientSpace;
					_maximumPosition = size.Height - ViewConstants.MinimumHostHeight;
					bounds = new Rectangle(
						0, 0,
						size.Width, ViewConstants.SideDockPanelBorderSize);
					_resizeOffset = location.Y - bounds.Y;
					orientation = Orientation.Vertical;
					break;
				default:
					throw new ApplicationException();
			}
			bounds.Location = _toolHost.PointToClient(bounds.Location);
			SpawnMarker(bounds, orientation);
			return true;
		}

		public void Update(Point location)
		{
			if(!_isActive) throw new InvalidOperationException();
			switch(_toolHost.DockSide.Orientation)
			{
				case Orientation.Vertical:
					{
						var x = location.X - _resizeOffset;
						if(x < _minimumPosition)
							x = _minimumPosition;
						else if(x > _maximumPosition)
							x = _maximumPosition;
						location = new Point(x, 0);
					}
					break;
				case Orientation.Horizontal:
					{
						var y = location.Y - _resizeOffset;
						if(y < _minimumPosition)
							y = _minimumPosition;
						else if(y > _maximumPosition)
							y = _maximumPosition;
						location = new Point(0, y);
					}
					break;
				default:
					throw new ApplicationException("Unexpected ToolDockSide.Orientation: " + _toolHost.DockSide.Orientation);
			}
			location = _toolHost.PointToScreen(location);
			_splitterMarker.Location = location;
		}

		public void Commit(Point e)
		{
			if(!_isActive) throw new InvalidOperationException();
			KillMarker();
			_isActive = false;
			switch(_toolHost.DockSide.Side)
			{
				case AnchorStyles.Left:
					{
						var x = e.X - _resizeOffset;
						if(x < _minimumPosition)
							x = _minimumPosition;
						else if(x > _maximumPosition)
							x = _maximumPosition;
						_toolHost.Width = x + ViewConstants.SideDockPanelBorderSize;
					}
					break;
				case AnchorStyles.Top:
					{
						var y = e.Y - _resizeOffset;
						if(y < _minimumPosition)
							y = _minimumPosition;
						else if(y > _maximumPosition)
							y = _maximumPosition;
						_toolHost.Height = y + ViewConstants.SideDockPanelBorderSize;
					}
					break;
				case AnchorStyles.Right:
					{
						var x = e.X - _resizeOffset;
						if(x < _minimumPosition)
							x = _minimumPosition;
						else if(x > _maximumPosition)
							x = _maximumPosition;
						var w = _toolHost.Width - x;
						var dw = _toolHost.Width - w;
						 _toolHost.SetBounds(_toolHost.Left + dw, 0, w, 0, BoundsSpecified.X | BoundsSpecified.Width);
					}
					break;
				case AnchorStyles.Bottom:
					{
						var y = e.Y - _resizeOffset;
						if(y < _minimumPosition)
							y = _minimumPosition;
						else if(y > _maximumPosition)
							y = _maximumPosition;
						var h = _toolHost.Height - y;
						var dh = _toolHost.Height - h;
						_toolHost.SetBounds(0, _toolHost.Top + dh, 0, h, BoundsSpecified.Y | BoundsSpecified.Height);
					}
					break;
				default:
					throw new ApplicationException("Unexpected ToolDockSide.Side: " + _toolHost.DockSide.Side);
			}
		}

		public void Cancel()
		{
			if(!_isActive) throw new InvalidOperationException();
			KillMarker();
			_isActive = false;
		}

		private void SpawnMarker(Rectangle bounds, Orientation orientation)
		{
			if(_splitterMarker != null) throw new InvalidOperationException();
			_splitterMarker = new SplitterMarker(bounds, orientation);
			_splitterMarker.Show();
		}

		private void KillMarker()
		{
			if(_splitterMarker != null)
			{
				_splitterMarker.Close();
				_splitterMarker.Dispose();
				_splitterMarker = null;
			}
		}

		public void Dispose()
		{
			if(_splitterMarker != null)
			{
				_splitterMarker.Dispose();
				_splitterMarker = null;
			}
			_isActive = false;
		}
	}
}
