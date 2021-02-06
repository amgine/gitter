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

		private static readonly LinkedList<ViewDockGrid> _grids = new();

		internal static IEnumerable<ViewDockGrid> Grids => _grids;

		#endregion

		#region Data

		private readonly GridDockMarkers _dockMarkers;
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
			RootHost = new ViewHost(this, true, true, null)
			{
				Bounds = bounds,
				Anchor = ViewConstants.AnchorAll,
			};
			RootControl = RootHost;
			_floatingViewForms = new LinkedList<FloatingViewForm>();
			PopupsStack = new PopupNotificationsStack();

			SetStyle(ControlStyles.ContainerControl, true);

			SuspendLayout();
			RootHost.Parent = this;
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
			get => base.BackColor;
			set => base.BackColor = value;
		}

		internal ViewHost RootHost { get; }

		internal Control RootControl { get; set; }

		internal IEnumerable<FloatingViewForm> FloatingViewForms
			=> _floatingViewForms;

		private ViewRenderer Renderer => ViewManager.Renderer;

		#endregion

		internal void AddFloatingForm(FloatingViewForm floatingViewForm)
		{
			Verify.Argument.IsNotNull(floatingViewForm, nameof(floatingViewForm));

			_floatingViewForms.AddLast(floatingViewForm);
		}

		internal void RemoveFloatingForm(FloatingViewForm floatingViewForm)
		{
			Verify.Argument.IsNotNull(floatingViewForm, nameof(floatingViewForm));

			_floatingViewForms.Remove(floatingViewForm);
		}

		private void SpawnLeftSide()
		{
			Verify.State.IsTrue(LeftSide == null);

			var size = Size;
			var bounds = new Rectangle(
				0, ViewConstants.Spacing,
				Renderer.SideTabHeight, 0);

			int hspace = size.Width - (Renderer.SideTabHeight + ViewConstants.Spacing * 2);
			if(RightSide != null) hspace -= Renderer.SideTabHeight;

			if(TopSide != null)
			{
				if(TopSide.Width > hspace)
				{
					TopSide.Bounds = new Rectangle(
						Renderer.SideTabHeight + ViewConstants.Spacing, 0,
						hspace, Renderer.SideTabHeight);
				}
				else
				{
					TopSide.Left += Renderer.SideTabHeight;
				}
				bounds.Y += Renderer.SideTabHeight;
			}
			if(BottomSide != null)
			{
				if(BottomSide.Width > hspace)
				{
					BottomSide.Bounds = new Rectangle(
						Renderer.SideTabHeight + ViewConstants.Spacing, size.Height - Renderer.SideTabHeight,
						hspace, Renderer.SideTabHeight);
				}
				else
				{
					BottomSide.Left += Renderer.SideTabHeight;
				}
			}
			RootControl.SetBounds(
				RootControl.Left + Renderer.SideTabHeight, 0,
				RootControl.Width - Renderer.SideTabHeight, 0,
				BoundsSpecified.X | BoundsSpecified.Width);
			LeftSide = new ViewDockSide(this, AnchorStyles.Left)
			{
				Anchor = AnchorStyles.Left | AnchorStyles.Top,
				Bounds = bounds,
				Parent = this,
			};
		}

		private void SpawnTopSide()
		{
			Verify.State.IsTrue(TopSide == null);

			var size = Size;
			var bounds = new Rectangle(
				ViewConstants.Spacing, 0,
				0, Renderer.SideTabHeight);

			int vspace = size.Height - Renderer.SideTabHeight - ViewConstants.Spacing * 2;
			if(BottomSide != null) vspace -= Renderer.SideTabHeight;

			if(LeftSide != null)
			{
				if(LeftSide.Height > vspace)
				{
					LeftSide.Bounds = new Rectangle(
						0, Renderer.SideTabHeight + ViewConstants.Spacing,
						Renderer.SideTabHeight, vspace);
				}
				else
				{
					LeftSide.Top += Renderer.SideTabHeight;
				}
				bounds.X += Renderer.SideTabHeight;
			}
			if(RightSide != null)
			{
				if(RightSide.Height > vspace)
				{
					RightSide.Bounds = new Rectangle(
						size.Width - Renderer.SideTabHeight, Renderer.SideTabHeight + ViewConstants.Spacing,
						Renderer.SideTabHeight, vspace);
				}
				else
				{
					RightSide.Top += Renderer.SideTabHeight;
				}
			}
			RootControl.SetBounds(
				0, RootControl.Top + Renderer.SideTabHeight,
				0, RootControl.Height - Renderer.SideTabHeight,
				BoundsSpecified.Y | BoundsSpecified.Height);
			TopSide = new ViewDockSide(this, AnchorStyles.Top)
			{
				Anchor = AnchorStyles.Left | AnchorStyles.Top,
				Bounds = bounds,
				Parent = this,
			};
		}

		private void SpawnRightSide()
		{
			Verify.State.IsTrue(RightSide == null);

			var size = Size;
			var bounds = new Rectangle(
				size.Width - Renderer.SideTabHeight, ViewConstants.Spacing,
				Renderer.SideTabHeight, 0);

			int hspace = size.Width - Renderer.SideTabHeight - ViewConstants.Spacing * 2;
			if(LeftSide != null) hspace -= Renderer.SideTabHeight;

			if(TopSide != null)
			{
				if(TopSide.Width > hspace)
				{
					TopSide.Width = hspace;
				}
				bounds.Y += Renderer.SideTabHeight;
			}
			if(BottomSide != null)
			{
				if(BottomSide.Width > hspace)
				{
					BottomSide.Width = hspace;
				}
			}
			RootControl.Width -= Renderer.SideTabHeight;
			RightSide = new ViewDockSide(this, AnchorStyles.Right)
			{
				Anchor = AnchorStyles.Right | AnchorStyles.Top,
				Bounds = bounds,
				Parent = this,
			};
		}

		private void SpawnBottomSide()
		{
			Verify.State.IsTrue(BottomSide == null);

			var size = Size;
			var bounds = new Rectangle(
				ViewConstants.Spacing, size.Height - Renderer.SideTabHeight,
				0, Renderer.SideTabHeight);

			int vspace = size.Height - Renderer.SideTabHeight - ViewConstants.Spacing * 2;
			if(TopSide != null) vspace -= Renderer.SideTabHeight;

			if(LeftSide != null)
			{
				if(LeftSide.Height > vspace)
				{
					LeftSide.Height = vspace;
				}
				bounds.X += Renderer.SideTabHeight;
			}
			if(RightSide != null)
			{
				if(RightSide.Height > vspace)
				{
					RightSide.Height = vspace;
				}
			}
			RootControl.Height -= Renderer.SideTabHeight;
			BottomSide = new ViewDockSide(this, AnchorStyles.Bottom)
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
			if(LeftSide != null)
			{
				LeftSide.Parent = null;
				LeftSide.Dispose();
				LeftSide = null;
			}
			if(TopSide != null)
			{
				TopSide.Parent = null;
				TopSide.Dispose();
				TopSide = null;
			}
			if(RightSide != null)
			{
				RightSide.Parent = null;
				RightSide.Dispose();
				RightSide = null;
			}
			if(BottomSide != null)
			{
				BottomSide.Parent = null;
				BottomSide.Dispose();
				BottomSide = null;
			}
			RootControl.SetBounds(
				ViewConstants.Spacing, ViewConstants.Spacing,
				Width - ViewConstants.Spacing * 2, Height - ViewConstants.Spacing * 2,
				BoundsSpecified.All);
			ResumeLayout(true);
		}

		private void RemoveLeftSide()
		{
			if(LeftSide != null)
			{
				LeftSide.Parent = null;
				LeftSide.Dispose();
				LeftSide = null;

				var bounds = RootControl.Bounds;
				bounds.X -= Renderer.SideTabHeight;
				bounds.Width += Renderer.SideTabHeight;
				RootControl.Bounds = bounds;
				var hcs = Width - ViewConstants.Spacing * 2;
				if(RightSide != null) hcs -= Renderer.SideTabHeight;
				if(TopSide != null)
				{
					var w = TopSide.Width;
					var len = TopSide.OptimalLength;
					if(w >= len)
					{
						TopSide.Left = ViewConstants.Spacing;
					}
					else
					{
						if(len > hcs) len = hcs;
						TopSide.SetBounds(
							ViewConstants.Spacing, 0, len, Renderer.SideTabHeight,
							BoundsSpecified.X | BoundsSpecified.Width);
					}
				}
				if(BottomSide != null)
				{
					var w = BottomSide.Height;
					var len = BottomSide.OptimalLength;
					if(w >= len)
					{
						BottomSide.Left = ViewConstants.Spacing;
					}
					else
					{
						if(len > hcs) len = hcs;
						BottomSide.SetBounds(
							ViewConstants.Spacing, 0, len, Renderer.SideTabHeight,
							BoundsSpecified.X | BoundsSpecified.Width);
					}
				}
			}
		}

		private void RemoveTopSide()
		{
			if(TopSide != null)
			{
				TopSide.Parent = null;
				TopSide.Dispose();
				TopSide = null;

				var bounds = RootControl.Bounds;
				bounds.Y -= Renderer.SideTabHeight;
				bounds.Height += Renderer.SideTabHeight;
				RootControl.Bounds = bounds;
				var vcs = Height - Renderer.SideTabHeight * 2;
				if(BottomSide != null) vcs -= Renderer.SideTabHeight;
				if(LeftSide != null)
				{
					var h = LeftSide.Height;
					var len = LeftSide.OptimalLength;
					if(h >= len)
					{
						LeftSide.Top = ViewConstants.Spacing;
					}
					else
					{
						if(len > vcs) len = vcs;
						LeftSide.SetBounds(
							0, ViewConstants.Spacing, Renderer.SideTabHeight, len,
							BoundsSpecified.Y | BoundsSpecified.Height);
					}
				}
				if(RightSide != null)
				{
					var h = RightSide.Height;
					var len = RightSide.OptimalLength;
					if(h >= len)
					{
						RightSide.Top = ViewConstants.Spacing;
					}
					else
					{
						if(len > vcs) len = vcs;
						RightSide.SetBounds(
							0, ViewConstants.Spacing, Renderer.SideTabHeight, len,
							BoundsSpecified.Y | BoundsSpecified.Height);
					}
				}
			}
		}

		private void RemoveRightSide()
		{
			if(RightSide != null)
			{
				RightSide.Parent = null;
				RightSide.Dispose();
				RightSide = null;
				RootControl.Width += Renderer.SideTabHeight;

				var hcs = Width - ViewConstants.Spacing * 2;
				if(LeftSide != null) hcs -= Renderer.SideTabHeight;
				if(TopSide != null)
				{
					var w = TopSide.Width;
					var len = TopSide.OptimalLength;
					if(w < len)
					{
						if(len > hcs) len = hcs;
						TopSide.Width = len;
					}
				}
				if(BottomSide != null)
				{
					var w = BottomSide.Height;
					var len = BottomSide.OptimalLength;
					if(w < len)
					{
						if(len > hcs) len = hcs;
						BottomSide.Width = len;
					}
				}
			}
		}

		private void KillBottomSide()
		{
			if(BottomSide != null)
			{
				BottomSide.Parent = null;
				BottomSide.Dispose();
				BottomSide = null;
				RootControl.Height += Renderer.SideTabHeight;

				var vcs = Height - Renderer.SideTabHeight * 2;
				if(BottomSide != null) vcs -= Renderer.SideTabHeight;
				if(LeftSide != null)
				{
					var h = LeftSide.Height;
					var len = LeftSide.OptimalLength;
					if(h < len)
					{
						if(len > vcs) len = vcs;
						LeftSide.Height = len;
					}
				}
				if(RightSide != null)
				{
					var h = RightSide.Height;
					var len = RightSide.OptimalLength;
					if(h < len)
					{
						if(len > vcs) len = vcs;
						RightSide.Height = len;
					}
				}
			}
		}

		internal ViewDockSide LeftSide { get; private set; }

		internal ViewDockSide RightSide { get; private set; }

		internal ViewDockSide TopSide { get; private set; }

		internal ViewDockSide BottomSide { get; private set; }

		private ViewDockSide GetCreateDockSide(AnchorStyles side)
		{
			ViewDockSide viewDockSide;
			switch(side)
			{
				case AnchorStyles.Left:
					if(LeftSide == null) SpawnLeftSide();
					viewDockSide = LeftSide;
					break;
				case AnchorStyles.Top:
					if(TopSide == null) SpawnTopSide();
					viewDockSide = TopSide;
					break;
				case AnchorStyles.Right:
					if(RightSide == null) SpawnRightSide();
					viewDockSide = RightSide;
					break;
				case AnchorStyles.Bottom:
					if(BottomSide == null) SpawnBottomSide();
					viewDockSide = BottomSide;
					break;
				default:
					throw new ArgumentException(
						"Unknown AnchorStyles value: {0}".UseAsFormat(side),
						"side");
			}
			return viewDockSide;
		}

		public PopupNotificationsStack PopupsStack { get; private set; }

		internal int HorizontalClientSpace
		{
			get
			{
				var w = Width - ViewConstants.Spacing * 2;
				if(LeftSide != null) w -= Renderer.SideTabHeight;
				if(RightSide != null) w -= Renderer.SideTabHeight;
				return w;
			}
		}

		internal int VerticalClientSpace
		{
			get
			{
				var h = Height - ViewConstants.Spacing * 2;
				if(TopSide != null) h -= Renderer.SideTabHeight;
				if(BottomSide != null) h -= Renderer.SideTabHeight;
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
			RootHost.AddView(view);
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
			if(LeftSide != null)
			{
				var h = LeftSide.Height;
				if(h > vspace)
				{
					LeftSide.Height = vspace;
				}
				else
				{
					if(h < LeftSide.OptimalLength)
					{
						h = LeftSide.OptimalLength;
						if(h > vspace) h = vspace;
						LeftSide.Height = h;
					}
				}
			}
			if(TopSide != null)
			{
				var w = TopSide.Width;
				if(w > hspace)
				{
					TopSide.Width = hspace;
				}
				else
				{
					if(w < TopSide.OptimalLength)
					{
						w = TopSide.OptimalLength;
						if(w > hspace) w = hspace;
						TopSide.Width = w;
					}
				}
			}
			if(RightSide != null)
			{
				var h = RightSide.Height;
				if(h > vspace)
				{
					RightSide.Height = vspace;
				}
				else
				{
					if(h < RightSide.OptimalLength)
					{
						h = RightSide.OptimalLength;
						if(h > vspace) h = vspace;
						RightSide.Height = h;
					}
				}
			}
			if(BottomSide != null)
			{
				var w = BottomSide.Width;
				if(w > hspace)
				{
					BottomSide.Width = hspace;
				}
				else
				{
					if(w < BottomSide.OptimalLength)
					{
						w = BottomSide.OptimalLength;
						if(w > hspace) w = hspace;
						BottomSide.Width = w;
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
				if(LeftSide != null)
				{
					LeftSide.Dispose();
					LeftSide = null;
				}
				if(TopSide != null)
				{
					TopSide.Dispose();
					TopSide = null;
				}
				if(RightSide != null)
				{
					RightSide.Dispose();
					RightSide = null;
				}
				if(BottomSide != null)
				{
					BottomSide.Dispose();
					BottomSide = null;
				}
				if(PopupsStack != null)
				{
					PopupsStack.Dispose();
					PopupsStack = null;
				}
				RootControl = null;
				_floatingViewForms.Clear();
			}
			base.Dispose(disposing);
		}

		#region IDockHost

		/// <summary>Provides dock helper markers to determine dock position (<see cref="DockResult"/>).</summary>
		public IDockMarkerProvider DockMarkers => _dockMarkers;

		/// <summary>Determines if <see cref="ViewHost"/> cn be docked into this <see cref="IDockHost"/>.</summary>
		/// <param name="viewHost"><see cref="ViewHost"/> to dock.</param>
		/// <param name="dockResult">Position for docking.</param>
		/// <returns>true if docking is possible.</returns>
		public bool CanDock(ViewHost viewHost, DockResult dockResult)
		{
			Verify.Argument.IsNotNull(viewHost, nameof(viewHost));

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
			Verify.Argument.IsNotNull(viewHost, nameof(viewHost));
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
						nameof(dockResult));
			}
		}

		/// <summary>Get bounding rectangle for docked view.</summary>
		/// <param name="viewHost">Tested <see cref="ViewHost"/>.</param>
		/// <param name="dockResult">Position for docking.</param>
		/// <returns>Bounding rectangle for docked view.</returns>
		public Rectangle GetDockBounds(ViewHost viewHost, DockResult dockResult)
		{
			Verify.Argument.IsNotNull(viewHost, nameof(viewHost));

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
						"Unsupported DockResult value: {0}".UseAsFormat(dockResult),
						nameof(dockResult));
			}
			bounds.Offset(rootBounds.X, rootBounds.Y);
			return RectangleToScreen(bounds);
		}

		#endregion
	}
}
