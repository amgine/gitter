namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;
	using System.Drawing;

	/// <summary>Hosts view hosts and applies grid layout.</summary>
	public sealed class ViewDockGrid : Control, IDockHost
	{
		#region Static

		private static readonly LinkedList<ViewDockGrid> _grids;

		internal static IEnumerable<ViewDockGrid> Grids
		{
			get { return _grids; }
		}

		/// <summary>Initializes the <see cref="ViewDockGrid"/> class.</summary>
		static ViewDockGrid()
		{
			_grids = new LinkedList<ViewDockGrid>();
		}

		#endregion

		#region Data

		private readonly ViewHost _rootHost;
		private readonly GridDockMarkers _dockMarkers;
		private Control _rootControl;

		private ViewDockSide _left;
		private ViewDockSide _top;
		private ViewDockSide _right;
		private ViewDockSide _bottom;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="ViewDockGrid"/> class.</summary>
		public ViewDockGrid()
		{
			_dockMarkers = new GridDockMarkers(this);
			var bounds = ClientRectangle;
			bounds.X += ViewConstants.Spacing;
			bounds.Y += ViewConstants.Spacing;
			bounds.Width -= ViewConstants.Spacing * 2;
			bounds.Height -= ViewConstants.Spacing * 2;
			_rootHost = new ViewHost(this, true, true, null)
			{
				Bounds = bounds,
				Anchor = ViewConstants.AnchorAll,
			};
			_rootControl = _rootHost;

			SetStyle(ControlStyles.ContainerControl, true);

			SuspendLayout();
			_rootHost.Parent = this;
			ResumeLayout(true);

			lock(_grids)
			{
				_grids.AddLast(this);
			}
		}

		#endregion

		#region Properties

		internal ViewHost RootHost
		{
			get { return _rootHost; }
		}

		internal Control RootControl
		{
			get { return _rootControl; }
			set { _rootControl = value; }
		}

		#endregion

		private void SpawnLeftSide()
		{
			Verify.State.IsTrue(_left == null);

			var size = Size;
			var bounds = new Rectangle(
				0, ViewConstants.Spacing,
				ViewConstants.SideTabHeight, 0);

			int hspace = size.Width - (ViewConstants.SideTabHeight + ViewConstants.Spacing * 2);
			if(_right != null) hspace -= ViewConstants.SideTabHeight;

			if(_top != null)
			{
				if(_top.Width > hspace)
				{
					_top.Bounds = new Rectangle(
						ViewConstants.SideTabHeight + ViewConstants.Spacing, 0,
						hspace, ViewConstants.SideTabHeight);
				}
				else
				{
					_top.Left += ViewConstants.SideTabHeight;
				}
				bounds.Y += ViewConstants.SideTabHeight;
			}
			if(_bottom != null)
			{
				if(_bottom.Width > hspace)
				{
					_bottom.Bounds = new Rectangle(
						ViewConstants.SideTabHeight + ViewConstants.Spacing, size.Height - ViewConstants.SideTabHeight,
						hspace, ViewConstants.SideTabHeight);
				}
				else
				{
					_bottom.Left += ViewConstants.SideTabHeight;
				}
			}
			_rootControl.SetBounds(
				_rootControl.Left + ViewConstants.SideTabHeight, 0,
				_rootControl.Width - ViewConstants.SideTabHeight, 0,
				BoundsSpecified.X | BoundsSpecified.Width);
			_left = new ViewDockSide(this, AnchorStyles.Left)
			{
				Anchor = AnchorStyles.Left | AnchorStyles.Top,
				Bounds = bounds,
				Parent = this,
			};
		}

		private void SpawnTopSide()
		{
			Verify.State.IsTrue(_top == null);

			var size = Size;
			var bounds = new Rectangle(
				ViewConstants.Spacing, 0,
				0, ViewConstants.SideTabHeight);

			int vspace = size.Height - ViewConstants.SideTabHeight - ViewConstants.Spacing * 2;
			if(_bottom != null) vspace -= ViewConstants.SideTabHeight;

			if(_left != null)
			{
				if(_left.Height > vspace)
				{
					_left.Bounds = new Rectangle(
						0, ViewConstants.SideTabHeight + ViewConstants.Spacing,
						ViewConstants.SideTabHeight, vspace);
				}
				else
				{
					_left.Top += ViewConstants.SideTabHeight;
				}
				bounds.X += ViewConstants.SideTabHeight;
			}
			if(_right != null)
			{
				if(_right.Height > vspace)
				{
					_right.Bounds = new Rectangle(
						size.Width - ViewConstants.SideTabHeight, ViewConstants.SideTabHeight + ViewConstants.Spacing,
						ViewConstants.SideTabHeight, vspace);
				}
				else
				{
					_right.Top += ViewConstants.SideTabHeight;
				}
			}
			_rootControl.SetBounds(
				0, _rootControl.Top + ViewConstants.SideTabHeight,
				0, _rootControl.Height - ViewConstants.SideTabHeight,
				BoundsSpecified.Y | BoundsSpecified.Height);
			_top = new ViewDockSide(this, AnchorStyles.Top)
			{
				Anchor = AnchorStyles.Left | AnchorStyles.Top,
				Bounds = bounds,
				Parent = this,
			};
		}

		private void SpawnRightSide()
		{
			Verify.State.IsTrue(_right == null);

			var size = Size;
			var bounds = new Rectangle(
				size.Width - ViewConstants.SideTabHeight, ViewConstants.Spacing,
				ViewConstants.SideTabHeight, 0);

			int hspace = size.Width - ViewConstants.SideTabHeight - ViewConstants.Spacing * 2;
			if(_left != null) hspace -= ViewConstants.SideTabHeight;

			if(_top != null)
			{
				if(_top.Width > hspace)
				{
					_top.Width = hspace;
				}
				bounds.Y += ViewConstants.SideTabHeight;
			}
			if(_bottom != null)
			{
				if(_bottom.Width > hspace)
				{
					_bottom.Width = hspace;
				}
			}
			_rootControl.Width -= ViewConstants.SideTabHeight;
			_right = new ViewDockSide(this, AnchorStyles.Right)
			{
				Anchor = AnchorStyles.Right | AnchorStyles.Top,
				Bounds = bounds,
				Parent = this,
			};
		}

		private void SpawnBottomSide()
		{
			Verify.State.IsTrue(_bottom == null);

			var size = Size;
			var bounds = new Rectangle(
				ViewConstants.Spacing, size.Height - ViewConstants.SideTabHeight,
				0, ViewConstants.SideTabHeight);

			int vspace = size.Height - ViewConstants.SideTabHeight - ViewConstants.Spacing * 2;
			if(_top != null) vspace -= ViewConstants.SideTabHeight;

			if(_left != null)
			{
				if(_left.Height > vspace)
				{
					_left.Height = vspace;
				}
				bounds.X += ViewConstants.SideTabHeight;
			}
			if(_right != null)
			{
				if(_right.Height > vspace)
				{
					_right.Height = vspace;
				}
			}
			_rootControl.Height -= ViewConstants.SideTabHeight;
			_bottom = new ViewDockSide(this, AnchorStyles.Bottom)
			{
				Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
				Bounds = bounds,
				Parent = this,
			};
		}

		internal void KillSide(AnchorStyles side)
		{
			switch(side)
			{
				case AnchorStyles.Left:
					KillLeftSide();
					break;
				case AnchorStyles.Top:
					KillTopSide();
					break;
				case AnchorStyles.Right:
					KillRightSide();
					break;
				case AnchorStyles.Bottom:
					KillBottomSide();
					break;
				default:
					throw new ArgumentException(
						"Unknown AnchorStyles value: {0}".UseAsFormat(side),
						"side");
			}
		}

		private void KillAllSides()
		{
			SuspendLayout();
			if(_left != null)
			{
				_left.Parent = null;
				_left.Dispose();
				_left = null;
			}
			if(_top != null)
			{
				_top.Parent = null;
				_top.Dispose();
				_top = null;
			}
			if(_right != null)
			{
				_right.Parent = null;
				_right.Dispose();
				_right = null;
			}
			if(_bottom != null)
			{
				_bottom.Parent = null;
				_bottom.Dispose();
				_bottom = null;
			}
			RootControl.SetBounds(
				ViewConstants.Spacing, ViewConstants.Spacing,
				Width - ViewConstants.Spacing * 2, Height - ViewConstants.Spacing * 2,
				BoundsSpecified.All);
			ResumeLayout(true);
		}

		private void KillLeftSide()
		{
			if(_left != null)
			{
				_left.Parent = null;
				_left.Dispose();
				_left = null;

				var bounds = _rootControl.Bounds;
				bounds.X -= ViewConstants.SideTabHeight;
				bounds.Width += ViewConstants.SideTabHeight;
				_rootControl.Bounds = bounds;
				var hcs = Width - ViewConstants.Spacing * 2;
				if(_right != null) hcs -= ViewConstants.SideTabHeight;
				if(_top != null)
				{
					var w = _top.Width;
					var len = _top.OptimalLength;
					if(w >= len)
					{
						_top.Left = ViewConstants.Spacing;
					}
					else
					{
						if(len > hcs) len = hcs;
						_top.SetBounds(
							ViewConstants.Spacing, 0, len, ViewConstants.SideTabHeight,
							BoundsSpecified.X | BoundsSpecified.Width);
					}
				}
				if(_bottom != null)
				{
					var w = _bottom.Height;
					var len = _bottom.OptimalLength;
					if(w >= len)
					{
						_bottom.Left = ViewConstants.Spacing;
					}
					else
					{
						if(len > hcs) len = hcs;
						_bottom.SetBounds(
							ViewConstants.Spacing, 0, len, ViewConstants.SideTabHeight,
							BoundsSpecified.X | BoundsSpecified.Width);
					}
				}
			}
		}

		private void KillTopSide()
		{
			if(_top != null)
			{
				_top.Parent = null;
				_top.Dispose();
				_top = null;

				var bounds = _rootControl.Bounds;
				bounds.Y -= ViewConstants.SideTabHeight;
				bounds.Height += ViewConstants.SideTabHeight;
				_rootControl.Bounds = bounds;
				var vcs = Height - ViewConstants.SideTabHeight * 2;
				if(_bottom != null) vcs -= ViewConstants.SideTabHeight;
				if(_left != null)
				{
					var h = _left.Height;
					var len = _left.OptimalLength;
					if(h >= len)
					{
						_left.Top = ViewConstants.Spacing;
					}
					else
					{
						if(len > vcs) len = vcs;
						_left.SetBounds(
							0, ViewConstants.Spacing, ViewConstants.SideTabHeight, len,
							BoundsSpecified.Y | BoundsSpecified.Height);
					}
				}
				if(_right != null)
				{
					var h = _right.Height;
					var len = _right.OptimalLength;
					if(h >= len)
					{
						_right.Top = ViewConstants.Spacing;
					}
					else
					{
						if(len > vcs) len = vcs;
						_right.SetBounds(
							0, ViewConstants.Spacing, ViewConstants.SideTabHeight, len,
							BoundsSpecified.Y | BoundsSpecified.Height);
					}
				}
			}
		}

		private void KillRightSide()
		{
			if(_right != null)
			{
				_right.Parent = null;
				_right.Dispose();
				_right = null;
				_rootControl.Width += ViewConstants.SideTabHeight;

				var hcs = Width - ViewConstants.Spacing * 2;
				if(_left != null) hcs -= ViewConstants.SideTabHeight;
				if(_top != null)
				{
					var w = _top.Width;
					var len = _top.OptimalLength;
					if(w < len)
					{
						if(len > hcs) len = hcs;
						_top.Width = len;
					}
				}
				if(_bottom != null)
				{
					var w = _bottom.Height;
					var len = _bottom.OptimalLength;
					if(w < len)
					{
						if(len > hcs) len = hcs;
						_bottom.Width = len;
					}
				}
			}
		}

		private void KillBottomSide()
		{
			if(_bottom != null)
			{
				_bottom.Parent = null;
				_bottom.Dispose();
				_bottom = null;
				_rootControl.Height += ViewConstants.SideTabHeight;

				var vcs = Height - ViewConstants.SideTabHeight * 2;
				if(_bottom != null) vcs -= ViewConstants.SideTabHeight;
				if(_left != null)
				{
					var h = _left.Height;
					var len = _left.OptimalLength;
					if(h < len)
					{
						if(len > vcs) len = vcs;
						_left.Height = len;
					}
				}
				if(_right != null)
				{
					var h = _right.Height;
					var len = _right.OptimalLength;
					if(h < len)
					{
						if(len > vcs) len = vcs;
						_right.Height = len;
					}
				}
			}
		}

		internal ViewDockSide LeftSide
		{
			get { return _left; }
		}

		internal ViewDockSide RightSide
		{
			get { return _right; }
		}

		internal ViewDockSide TopSide
		{
			get { return _top; }
		}

		internal ViewDockSide BottomSide
		{
			get { return _bottom; }
		}

		private ViewDockSide GetCreateDockSide(AnchorStyles side)
		{
			ViewDockSide viewDockSide;
			switch(side)
			{
				case AnchorStyles.Left:
					if(_left == null) SpawnLeftSide();
					viewDockSide = _left;
					break;
				case AnchorStyles.Top:
					if(_top == null) SpawnTopSide();
					viewDockSide = _top;
					break;
				case AnchorStyles.Right:
					if(_right == null) SpawnRightSide();
					viewDockSide = _right;
					break;
				case AnchorStyles.Bottom:
					if(_bottom == null) SpawnBottomSide();
					viewDockSide = _bottom;
					break;
				default:
					throw new ArgumentException(
						"Unknown AnchorStyles value: {0}".UseAsFormat(side),
						"side");
			}
			Assert.AreEqual(viewDockSide.Side, side);
			return viewDockSide;
		}

		internal int HorizontalClientSpace
		{
			get
			{
				var w = Width - ViewConstants.Spacing * 2;
				if(_left != null) w -= ViewConstants.SideTabHeight;
				if(_right != null) w -= ViewConstants.SideTabHeight;
				return w;
			}
		}

		internal int VerticalClientSpace
		{
			get
			{
				var h = Height - ViewConstants.Spacing * 2;
				if(_top != null) h -= ViewConstants.SideTabHeight;
				if(_bottom != null) h -= ViewConstants.SideTabHeight;
				return h;
			}
		}

		private static ViewBase FindView(Control control, Guid guid, IDictionary<string, object> parameters, bool considerParameters)
		{
			foreach(Control child in control.Controls)
			{
				var viewHost = child as ViewHost;
				if(viewHost != null)
				{
					foreach(var view in viewHost.Views)
					{
						if(view.Guid == guid)
						{
							if(considerParameters)
							{
								if(view.ParametersIdentical(parameters))
								{
									return view;
								}
								else
								{
									continue;
								}
							}
							return view;
						}
					}
				}
				else
				{
					var view = FindView(child, guid, parameters, considerParameters);
					if(view != null) return view;
				}
			}
			return null;
		}

		public ViewBase FindView(Guid guid)
		{
			return FindView(this, guid, null, false);
		}

		public ViewBase FindView(Guid guid, IDictionary<string, object> parameters)
		{
			return FindView(this, guid, parameters, true);
		}

		public void DockSide(AnchorStyles side, ViewBase view, bool autoHide)
		{
			DockSide(side, new ViewHost(this, false, false, new[] { view }), autoHide);
		}

		internal void DockSide(AnchorStyles side, ViewHost host, bool autoHide)
		{
			host.DockToSide(RootControl, side);
		}

		public void DockRoot(ViewBase view)
		{
			_rootHost.AddView(view);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.Resize"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			var vspace = VerticalClientSpace;
			var hspace = HorizontalClientSpace;
			if(_left != null)
			{
				var h = _left.Height;
				if(h > vspace)
				{
					_left.Height = vspace;
				}
				else
				{
					if(h < _left.OptimalLength)
					{
						h = _left.OptimalLength;
						if(h > vspace) h = vspace;
						_left.Height = h;
					}
				}
			}
			if(_top != null)
			{
				var w = _top.Width;
				if(w > hspace)
				{
					_top.Width = hspace;
				}
				else
				{
					if(w < _top.OptimalLength)
					{
						w = _top.OptimalLength;
						if(w > hspace) w = hspace;
						_top.Width = w;
					}
				}
			}
			if(_right != null)
			{
				var h = _right.Height;
				if(h > vspace)
				{
					_right.Height = vspace;
				}
				else
				{
					if(h < _right.OptimalLength)
					{
						h = _right.OptimalLength;
						if(h > vspace) h = vspace;
						_right.Height = h;
					}
				}
			}
			if(_bottom != null)
			{
				var w = _bottom.Width;
				if(w > hspace)
				{
					_bottom.Width = hspace;
				}
				else
				{
					if(w < _bottom.OptimalLength)
					{
						w = _bottom.OptimalLength;
						if(w > hspace) w = hspace;
						_bottom.Width = w;
					}
				}
			}
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> and its child controls and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			lock(_grids)
			{
				_grids.Remove(this);
			}
			if(disposing)
			{
				_dockMarkers.Dispose();
				if(_left != null)
				{
					_left.Dispose();
					_left = null;
				}
				if(_top != null)
				{
					_top.Dispose();
					_top = null;
				}
				if(_right != null)
				{
					_right.Dispose();
					_right = null;
				}
				if(_bottom != null)
				{
					_bottom.Dispose();
					_bottom = null;
				}
				_rootControl = null;
			}
			base.Dispose(disposing);
		}

		#region IDockHost

		/// <summary>Provides dock helper markers to determine dock position (<see cref="DockResult"/>).</summary>
		public IDockMarkerProvider DockMarkers
		{
			get { return _dockMarkers; }
		}

		/// <summary>Determines if <see cref="ViewHost"/> cn be docked into this <see cref="IDockHost"/>.</summary>
		/// <param name="viewHost"><see cref="ViewHost"/> to dock.</param>
		/// <param name="dockResult">Position for docking.</param>
		/// <returns>true if docking is possible.</returns>
		public bool CanDock(ViewHost viewHost, DockResult dockResult)
		{
			Verify.Argument.IsNotNull(viewHost, "viewHost");

			if(viewHost.IsDocumentWell || (viewHost.ViewsCount == 1 && viewHost.GetView(0).IsDocument))
			{
				return false;
			}
			switch(dockResult)
			{
				case DockResult.Left:
				case DockResult.Top:
				case DockResult.Right:
				case DockResult.Bottom:
					return true;
				default:
					return false;
			}
		}

		/// <summary>Docks <paramref name="viewHost"/> into this <see cref="IDockHost"/>.</summary>
		/// <param name="viewHost"><see cref="ViewHost"/> to dock.</param>
		/// <param name="dockResult">Position for docking.</param>
		public void PerformDock(ViewHost viewHost, DockResult dockResult)
		{
			Verify.Argument.IsNotNull(viewHost, "viewHost");
			Verify.Argument.IsFalse(viewHost.IsDocumentWell, "viewHost");
			Verify.Argument.IsFalse(viewHost.ViewsCount == 1 && viewHost.GetView(0).IsDocument, "viewHost");

			switch(dockResult)
			{
				case DockResult.Left:
					DockSide(AnchorStyles.Left, viewHost, false);
					viewHost.Status = ViewHostStatus.Docked;
					break;
				case DockResult.Top:
					DockSide(AnchorStyles.Top, viewHost, false);
					viewHost.Status = ViewHostStatus.Docked;
					break;
				case DockResult.Right:
					DockSide(AnchorStyles.Right, viewHost, false);
					viewHost.Status = ViewHostStatus.Docked;
					break;
				case DockResult.Bottom:
					DockSide(AnchorStyles.Bottom, viewHost, false);
					viewHost.Status = ViewHostStatus.Docked;
					break;
				case DockResult.AutoHideLeft:
					GetCreateDockSide(AnchorStyles.Left).AddHost(viewHost);
					break;
				case DockResult.AutoHideTop:
					GetCreateDockSide(AnchorStyles.Top).AddHost(viewHost);
					break;
				case DockResult.AutoHideRight:
					GetCreateDockSide(AnchorStyles.Right).AddHost(viewHost);
					break;
				case DockResult.AutoHideBottom:
					GetCreateDockSide(AnchorStyles.Bottom).AddHost(viewHost);
					break;
				default:
					throw new ArgumentException(
						"Unsupported DockResult value: {0}".UseAsFormat(dockResult),
						"dockResult");
			}
		}

		/// <summary>Get bounding rectangle for docked view.</summary>
		/// <param name="viewHost">Tested <see cref="ViewHost"/>.</param>
		/// <param name="dockResult">Position for docking.</param>
		/// <returns>Bounding rectangle for docked view.</returns>
		public Rectangle GetDockBounds(ViewHost viewHost, DockResult dockResult)
		{
			Verify.Argument.IsNotNull(viewHost, "viewHost");

			var rootBounds = RootControl.Bounds;
			Rectangle bounds;
			switch(dockResult)
			{
				case DockResult.Left:
					{
						var w1 = viewHost.Width;
						var w2 = rootBounds.Width;
						ViewSplit.OptimalSizes(w2, ViewConstants.MinimumHostWidth, ref w1, ref w2);
						bounds = new Rectangle(0, 0, w1, rootBounds.Height);
					}
					break;
				case DockResult.Top:
					{
						var h1 = viewHost.Height;
						var h2 = rootBounds.Height;
						ViewSplit.OptimalSizes(h2, ViewConstants.MinimumHostHeight, ref h1, ref h2);
						bounds = new Rectangle(0, 0, rootBounds.Width, h1);
					}
					break;
				case DockResult.Right:
					{
						var w1 = rootBounds.Width;
						var w2 = viewHost.Width;
						ViewSplit.OptimalSizes(w1, ViewConstants.MinimumHostWidth, ref w1, ref w2);
						bounds = new Rectangle(rootBounds.Width - w2, 0, w2, rootBounds.Height);
					}
					break;
				case DockResult.Bottom:
					{
						var h1 = rootBounds.Height;
						var h2 = viewHost.Height;
						ViewSplit.OptimalSizes(h1, ViewConstants.MinimumHostHeight, ref h1, ref h2);
						bounds = new Rectangle(0, rootBounds.Height - h2, rootBounds.Width, h2);
					}
					break;
				default:
					throw new ArgumentException(
						"Unsuported DockResult value: {0}".UseAsFormat(dockResult),
						"dockResult");
			}
			bounds.Offset(rootBounds.X, rootBounds.Y);
			return RectangleToScreen(bounds);
		}

		#endregion
	}
}
