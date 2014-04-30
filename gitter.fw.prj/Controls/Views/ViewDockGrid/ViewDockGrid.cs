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
	using System.Windows.Forms;
	using System.Drawing;
	using System.ComponentModel;

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
		private PopupNotificationsStack _popupsStack;

		private ViewDockSide _left;
		private ViewDockSide _top;
		private ViewDockSide _right;
		private ViewDockSide _bottom;
		private LinkedList<FloatingViewForm> _floatingViewForms;

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
			_floatingViewForms = new LinkedList<FloatingViewForm>();
			_popupsStack = new PopupNotificationsStack();

			SetStyle(ControlStyles.ContainerControl, true);

			SuspendLayout();
			_rootHost.Parent = this;
			ResumeLayout(true);

			lock(_grids)
			{
				_grids.AddLast(this);
			}

			BackColor = ViewManager.Renderer.BackgroundColor;
		}

		#endregion

		#region Properties

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor
		{
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}

		internal ViewHost RootHost
		{
			get { return _rootHost; }
		}

		internal Control RootControl
		{
			get { return _rootControl; }
			set { _rootControl = value; }
		}

		internal IEnumerable<FloatingViewForm> FloatingViewForms
		{
			get { return _floatingViewForms; }
		}

		private ViewRenderer Renderer
		{
			get { return ViewManager.Renderer; }
		}

		#endregion

		internal void AddFloatingForm(FloatingViewForm floatingViewForm)
		{
			Verify.Argument.IsNotNull(floatingViewForm, "floatingViewForm");

			_floatingViewForms.AddLast(floatingViewForm);
		}

		internal void RemoveFloatingForm(FloatingViewForm floatingViewForm)
		{
			Verify.Argument.IsNotNull(floatingViewForm, "floatingViewForm");

			_floatingViewForms.Remove(floatingViewForm);
		}

		private void SpawnLeftSide()
		{
			Verify.State.IsTrue(_left == null);

			var size = Size;
			var bounds = new Rectangle(
				0, ViewConstants.Spacing,
				Renderer.SideTabHeight, 0);

			int hspace = size.Width - (Renderer.SideTabHeight + ViewConstants.Spacing * 2);
			if(_right != null) hspace -= Renderer.SideTabHeight;

			if(_top != null)
			{
				if(_top.Width > hspace)
				{
					_top.Bounds = new Rectangle(
						Renderer.SideTabHeight + ViewConstants.Spacing, 0,
						hspace, Renderer.SideTabHeight);
				}
				else
				{
					_top.Left += Renderer.SideTabHeight;
				}
				bounds.Y += Renderer.SideTabHeight;
			}
			if(_bottom != null)
			{
				if(_bottom.Width > hspace)
				{
					_bottom.Bounds = new Rectangle(
						Renderer.SideTabHeight + ViewConstants.Spacing, size.Height - Renderer.SideTabHeight,
						hspace, Renderer.SideTabHeight);
				}
				else
				{
					_bottom.Left += Renderer.SideTabHeight;
				}
			}
			_rootControl.SetBounds(
				_rootControl.Left + Renderer.SideTabHeight, 0,
				_rootControl.Width - Renderer.SideTabHeight, 0,
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
				0, Renderer.SideTabHeight);

			int vspace = size.Height - Renderer.SideTabHeight - ViewConstants.Spacing * 2;
			if(_bottom != null) vspace -= Renderer.SideTabHeight;

			if(_left != null)
			{
				if(_left.Height > vspace)
				{
					_left.Bounds = new Rectangle(
						0, Renderer.SideTabHeight + ViewConstants.Spacing,
						Renderer.SideTabHeight, vspace);
				}
				else
				{
					_left.Top += Renderer.SideTabHeight;
				}
				bounds.X += Renderer.SideTabHeight;
			}
			if(_right != null)
			{
				if(_right.Height > vspace)
				{
					_right.Bounds = new Rectangle(
						size.Width - Renderer.SideTabHeight, Renderer.SideTabHeight + ViewConstants.Spacing,
						Renderer.SideTabHeight, vspace);
				}
				else
				{
					_right.Top += Renderer.SideTabHeight;
				}
			}
			_rootControl.SetBounds(
				0, _rootControl.Top + Renderer.SideTabHeight,
				0, _rootControl.Height - Renderer.SideTabHeight,
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
				size.Width - Renderer.SideTabHeight, ViewConstants.Spacing,
				Renderer.SideTabHeight, 0);

			int hspace = size.Width - Renderer.SideTabHeight - ViewConstants.Spacing * 2;
			if(_left != null) hspace -= Renderer.SideTabHeight;

			if(_top != null)
			{
				if(_top.Width > hspace)
				{
					_top.Width = hspace;
				}
				bounds.Y += Renderer.SideTabHeight;
			}
			if(_bottom != null)
			{
				if(_bottom.Width > hspace)
				{
					_bottom.Width = hspace;
				}
			}
			_rootControl.Width -= Renderer.SideTabHeight;
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
				ViewConstants.Spacing, size.Height - Renderer.SideTabHeight,
				0, Renderer.SideTabHeight);

			int vspace = size.Height - Renderer.SideTabHeight - ViewConstants.Spacing * 2;
			if(_top != null) vspace -= Renderer.SideTabHeight;

			if(_left != null)
			{
				if(_left.Height > vspace)
				{
					_left.Height = vspace;
				}
				bounds.X += Renderer.SideTabHeight;
			}
			if(_right != null)
			{
				if(_right.Height > vspace)
				{
					_right.Height = vspace;
				}
			}
			_rootControl.Height -= Renderer.SideTabHeight;
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
					RemoveLeftSide();
					break;
				case AnchorStyles.Top:
					RemoveTopSide();
					break;
				case AnchorStyles.Right:
					RemoveRightSide();
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

		private void RemoveAllSides()
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

		private void RemoveLeftSide()
		{
			if(_left != null)
			{
				_left.Parent = null;
				_left.Dispose();
				_left = null;

				var bounds = _rootControl.Bounds;
				bounds.X -= Renderer.SideTabHeight;
				bounds.Width += Renderer.SideTabHeight;
				_rootControl.Bounds = bounds;
				var hcs = Width - ViewConstants.Spacing * 2;
				if(_right != null) hcs -= Renderer.SideTabHeight;
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
							ViewConstants.Spacing, 0, len, Renderer.SideTabHeight,
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
							ViewConstants.Spacing, 0, len, Renderer.SideTabHeight,
							BoundsSpecified.X | BoundsSpecified.Width);
					}
				}
			}
		}

		private void RemoveTopSide()
		{
			if(_top != null)
			{
				_top.Parent = null;
				_top.Dispose();
				_top = null;

				var bounds = _rootControl.Bounds;
				bounds.Y -= Renderer.SideTabHeight;
				bounds.Height += Renderer.SideTabHeight;
				_rootControl.Bounds = bounds;
				var vcs = Height - Renderer.SideTabHeight * 2;
				if(_bottom != null) vcs -= Renderer.SideTabHeight;
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
							0, ViewConstants.Spacing, Renderer.SideTabHeight, len,
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
							0, ViewConstants.Spacing, Renderer.SideTabHeight, len,
							BoundsSpecified.Y | BoundsSpecified.Height);
					}
				}
			}
		}

		private void RemoveRightSide()
		{
			if(_right != null)
			{
				_right.Parent = null;
				_right.Dispose();
				_right = null;
				_rootControl.Width += Renderer.SideTabHeight;

				var hcs = Width - ViewConstants.Spacing * 2;
				if(_left != null) hcs -= Renderer.SideTabHeight;
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
				_rootControl.Height += Renderer.SideTabHeight;

				var vcs = Height - Renderer.SideTabHeight * 2;
				if(_bottom != null) vcs -= Renderer.SideTabHeight;
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
			return viewDockSide;
		}

		public PopupNotificationsStack PopupsStack
		{
			get { return _popupsStack; }
		}

		internal int HorizontalClientSpace
		{
			get
			{
				var w = Width - ViewConstants.Spacing * 2;
				if(_left != null) w -= Renderer.SideTabHeight;
				if(_right != null) w -= Renderer.SideTabHeight;
				return w;
			}
		}

		internal int VerticalClientSpace
		{
			get
			{
				var h = Height - ViewConstants.Spacing * 2;
				if(_top != null) h -= Renderer.SideTabHeight;
				if(_bottom != null) h -= Renderer.SideTabHeight;
				return h;
			}
		}

		private static ViewBase FindView(Control control, Guid guid, object viewModel, bool considerViewModel)
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
							if(considerViewModel)
							{
								if(object.Equals(view.ViewModel, viewModel))
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
					var view = FindView(child, guid, viewModel, considerViewModel);
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
				if(_popupsStack != null)
				{
					_popupsStack.Dispose();
					_popupsStack = null;
				}
				_rootControl = null;
				_floatingViewForms.Clear();
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
