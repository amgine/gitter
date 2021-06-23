﻿#region Copyright Notice
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
	using System.ComponentModel;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	/// <summary>Control for hosting <see cref="T:ViewBase"/> controls.</summary>
	[ToolboxItem(false)]
	public sealed class ViewHost : Control, IDockHost
	{
		#region Static

		private static readonly LinkedList<ViewHost> _viewHosts = new();

		public static IEnumerable<ViewHost> ViewHosts => _viewHosts;

		#endregion

		#region Data

		private readonly ViewDockGrid _grid;
		private readonly bool _isRoot;
		private readonly List<ViewBase> _views;
		private readonly Panel _viewContainer;
		private readonly ViewHostDockMarkers _dockMarkers;
		private readonly ViewHostDockingProcess _dockingProcess;
		private readonly ViewHostResizingProcess _resizingProcess;
		private bool _isDocumentWell;
		private bool _isActive;
		private ViewHostHeader _header;
		private ViewHostTabs _tabs;
		private ViewHostFooter _footer;
		private ViewBase _activeView;
		private ViewHostStatus _status;
		private Form _ownerForm;
		private ViewDockSide _dockSide;

		private bool _readyToMove;
		private int _mdX;
		private int _mdY;

		#endregion

		#region Events

		private static readonly object ActiveViewChangedEvent = new();
		/// <summary>Occurs when active view changes.</summary>
		public event EventHandler ActiveViewChanged
		{
			add    => Events.AddHandler    (ActiveViewChangedEvent, value);
			remove => Events.RemoveHandler (ActiveViewChangedEvent, value);
		}

		private static readonly object StatusChangedEvent = new();
		/// <summary>Occurs when status changes.</summary>
		public event EventHandler StatusChanged
		{
			add    => Events.AddHandler    (StatusChangedEvent, value);
			remove => Events.RemoveHandler (StatusChangedEvent, value);
		}

		private static readonly object ViewAddedEvent = new();
		/// <summary>Occurs when view is added to this host.</summary>
		public event EventHandler<ViewEventArgs> ViewAdded
		{
			add    => Events.AddHandler    (ViewAddedEvent, value);
			remove => Events.RemoveHandler (ViewAddedEvent, value);
		}

		private static readonly object ViewRemovedEvent = new();
		/// <summary>Occurs when view is removed from this host.</summary>
		public event EventHandler<ViewEventArgs> ViewRemoved
		{
			add    => Events.AddHandler    (ViewRemovedEvent, value);
			remove => Events.RemoveHandler (ViewRemovedEvent, value);
		}

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="ViewHost"/> class.</summary>
		/// <param name="grid">Host grid.</param>
		/// <param name="isRoot">if set to <c>true</c> disables host auto-destruction on losing all hosted views.</param>
		/// <param name="isDocumentWell">if set to <c>true</c> uses advanced layout features for hosting variable size documents.</param>
		/// <param name="views">Hosted views.</param>
		internal ViewHost(ViewDockGrid grid, bool isRoot, bool isDocumentWell, IEnumerable<ViewBase> views)
		{
			Verify.Argument.IsNotNull(grid, nameof(grid));

			SetStyle(ControlStyles.ContainerControl, true);
			SetStyle(
				ControlStyles.SupportsTransparentBackColor |
				ControlStyles.Selectable,
				false);

			_grid = grid;
			_isRoot = isRoot;
			_isDocumentWell = isDocumentWell;
			_views = new List<ViewBase>();
			var size = new Size(ViewConstants.MinimumHostWidth, ViewConstants.MinimumHostHeight);
			if(views is not null)
			{
				foreach(var view in views)
				{
					Verify.Argument.IsTrue(view is not null, nameof(views), "List of views contains invalid arguments.");

					_views.Add(view);
					view.TextChanged += OnViewTextChanged;
					view.Host = this;
					var ts = view.Size;
					if(ts.Width > size.Width)
					{
						size.Width = ts.Width;
					}
					if(ts.Height > size.Height)
					{
						size.Height = ts.Height;
					}
				}
			}
			if(_views.Count > 0)
			{
				if(isDocumentWell)
				{
					size.Height += Renderer.TabHeight +
								   Renderer.TabFooterHeight +
								   Renderer.FooterHeight;
				}
				else
				{
					size.Height += Renderer.HeaderHeight;
					if(_views.Count > 1)
					{
						size.Height += Renderer.TabHeight;
					}
				}
			}

			_dockMarkers     = new ViewHostDockMarkers    (this);
			_dockingProcess  = new ViewHostDockingProcess (this);
			_resizingProcess = new ViewHostResizingProcess(this);

			BackColor = Renderer.BackgroundColor;

			SuspendLayout();

			if(_views.Count != 0)
			{
				_activeView = _views[0];
				if(isDocumentWell)
				{
					SpawnTabs();
					SpawnFooter();
				}
				else
				{
					SpawnHeader();
					if(_views.Count > 1)
					{
						SpawnTabs();
					}
				}
			}

			_viewContainer = new Panel();

			if(_activeView is not null)
			{
				_activeView.Bounds = _viewContainer.ClientRectangle;
				_activeView.Dock   = DockStyle.Fill;
				_activeView.Parent = _viewContainer;
			}

			Size = size;

			_viewContainer.Parent = this;

			ResumeLayout(true);

			lock(_viewHosts)
			{
				_viewHosts.AddLast(this);
			}

			UpdateChildBounds();
		}

		#endregion

		private ViewRenderer Renderer => ViewManager.Renderer;

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.ParentChanged"/> event.</summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			UpdateOwnerForm();
		}

		private void UpdateOwnerForm()
		{
			if(TopLevelControl is Form form)
			{
				if(_ownerForm != form)
				{
					if(_ownerForm is not null)
					{
						_ownerForm.Activated -= OnOwnerFormActivated;
						_ownerForm.Deactivate -= OnOwnerFormDeactivated;
					}
					_ownerForm = form;
					form.Activated += OnOwnerFormActivated;
					form.Deactivate += OnOwnerFormDeactivated;
					_isActive = form.Focused;
				}
			}
			else
			{
				if(_ownerForm is not null)
				{
					_ownerForm.Activated  -= OnOwnerFormActivated;
					_ownerForm.Deactivate -= OnOwnerFormDeactivated;
					_ownerForm = null;
					_isActive = false;
				}
			}
		}

		private void OnOwnerFormActivated(object sender, EventArgs e)
		{
			if(ContainsFocus)
			{
				_isActive = true;
				InvalidateHelpers();
			}
		}

		private void OnOwnerFormDeactivated(object sender, EventArgs e)
		{
			if(_isActive)
			{
				_isActive = false;
				InvalidateHelpers();
			}
		}

		/// <summary>Gets a value indicating whether this <see cref="ViewHost"/> is resizing.</summary>
		/// <value><c>true</c> if this instance is resizing; otherwise, <c>false</c>.</value>
		public bool IsResizing => _resizingProcess.IsActive;

		/// <summary>Gets a value indicating whether this <see cref="ViewHost"/> is moving.</summary>
		/// <value><c>true</c> if this instance is moving; otherwise, <c>false</c>.</value>
		public bool IsMoving => _dockingProcess.IsActive;

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave"/> event.</summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if(Status == ViewHostStatus.AutoHide)
			{
				Cursor = Cursors.Default;
			}
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.MouseEnter"/> event.</summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			if(Status == ViewHostStatus.AutoHide)
			{
				Cursor = DockSide.Orientation switch
				{
					Orientation.Vertical   => Cursors.SizeWE,
					Orientation.Horizontal => Cursors.SizeNS,
					_ => throw new ApplicationException($"Unexpected ViewDockSide.Orientation: {DockSide.Orientation}"),
				};
			}
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"/> event.</summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if(Status == ViewHostStatus.AutoHide)
			{
				if(e.Button == MouseButtons.Left)
				{
					_resizingProcess.Start(e.Location);
				}
			}
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"/> event.</summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if(_resizingProcess.IsActive)
			{
				_resizingProcess.Update(e.Location);
			}
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.</summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if((e.Button & MouseButtons.Left) == MouseButtons.Left && _resizingProcess.IsActive)
			{
				_resizingProcess.Commit(e.Location);
			}
		}

		private void UpdateChildBounds()
		{
			//var converter = new DpiConverter(this);
			var cb = ClientRectangle;

			if(DockSide is not null)
			{
				var delta = ViewConstants.SideDockPanelBorderSize;
				//delta = converter.ConvertY(delta);
				switch(DockSide.Side)
				{
					case AnchorStyles.Top:
						cb.Height -= delta;
						break;
					case AnchorStyles.Bottom:
						cb.Y      += delta;
						cb.Height -= delta;
						break;
					case AnchorStyles.Left:
						cb.Width -= delta;
						break;
					case AnchorStyles.Right:
						cb.X     += delta;
						cb.Width -= delta;
						break;
				}
			}
			if(_header is not null)
			{
				var headerHeight = Renderer.HeaderHeight;
				//headerHeight = converter.ConvertY(headerHeight);
				_header.Bounds = new Rectangle(cb.X, cb.Y, cb.Width, headerHeight);
				cb.Y      += headerHeight;
				cb.Height -= headerHeight;
			}
			if(_tabs is not null)
			{
				var tabHeight = Renderer.TabHeight;
				//tabHeight = converter.ConvertY(tabHeight);
				if(_isDocumentWell)
				{
					tabHeight += Renderer.TabFooterHeight;
					_tabs.Bounds = new Rectangle(
						cb.X, cb.Y,
						cb.Width, tabHeight);
					cb.Y += tabHeight;
				}
				else
				{
					_tabs.Bounds = new Rectangle(
						cb.X, cb.Y + cb.Height - tabHeight,
						cb.Width, tabHeight);
				}
				cb.Height -= tabHeight;
			}
			if(_footer is not null)
			{
				var footerHeight = Renderer.FooterHeight;
				//footerHeight = converter.ConvertY(footerHeight);
				_footer.Bounds = new Rectangle(
					cb.X, cb.Y + cb.Height - footerHeight,
					cb.Width, footerHeight);
				cb.Height -= footerHeight;
			}
			_viewContainer.Bounds = cb;
		}

		/// <summary>Creates <see cref="ViewHostHeader"/> control.</summary>
		private void SpawnHeader()
		{
			Verify.State.IsTrue(_header is null, "Header is already spawned.");

			if(Renderer.HeaderHeight > 0)
			{
				_header = new ViewHostHeader(this)
				{
					Parent = this,
				};
				ResetHeaderButtons();
				_header.MouseDown += OnHeaderMouseDown;
				_header.MouseMove += OnHeaderMouseMove;
				_header.MouseUp += OnHeaderMouseUp;
				_header.MouseDoubleClick += OnHeaderMouseDoubleClick;
				_header.HeaderButtonClick += OnHeaderButtonClick;
			}
		}

		/// <summary>Destroy header control.</summary>
		private void RemoveHeader()
		{
			if(_header is not null)
			{
				_header.Parent = null;
				_header.MouseDown -= OnHeaderMouseDown;
				_header.MouseMove -= OnHeaderMouseMove;
				_header.MouseUp -= OnHeaderMouseUp;
				_header.MouseDoubleClick -= OnHeaderMouseDoubleClick;
				_header.HeaderButtonClick -= OnHeaderButtonClick;
				_header.Dispose();
				_header = null;
			}
		}

		/// <summary>Creates <see cref="ViewHostFooter"/> control.</summary>
		private void SpawnFooter()
		{
			Verify.State.IsTrue(_footer is null, "Footer is already spawned.");

			var footerHeight = Renderer.FooterHeight;
			if(footerHeight > 0)
			{
				_footer = new ViewHostFooter(this)
				{
					Parent = this,
				};
			}
		}

		/// <summary>Destroys footer control.</summary>
		private void RemoveFooter()
		{
			if(_footer is not null)
			{
				_footer.Parent = null;
				_footer.Dispose();
				_footer = null;
			}
		}

		private static AnchorStyles GetRelativeSide(ViewSplit viewSplit, int index, int side)
		{
			var item = viewSplit[index];
			var isDw = item switch
			{
				ViewHost  th => th.IsDocumentWell,
				ViewSplit ts => ts.ContainsDocumentWell,
				_ => false,
			};
			if(!isDw) return AnchorStyles.None;
			return viewSplit.Orientation switch
			{
				Orientation.Horizontal => side == -1 ? AnchorStyles.Right  : AnchorStyles.Left,
				Orientation.Vertical   => side == -1 ? AnchorStyles.Bottom : AnchorStyles.Top,
				_ => throw new ApplicationException(),
			};
		}

		private static AnchorStyles GetRelativeSide(ViewSplit viewSplit, Control control)
		{
			var index = viewSplit.IndexOf(control);
			for(int i = 0; i < index; ++i)
			{
				var anchor = GetRelativeSide(viewSplit, i, -1);
				if(anchor != AnchorStyles.None) return anchor;
			}
			for(int i = index + 1; i < viewSplit.Count; ++i)
			{
				var anchor = GetRelativeSide(viewSplit, i, 1);
				if(anchor != AnchorStyles.None) return anchor;
			}
			if(viewSplit.Parent is ViewSplit parent)
			{
				return GetRelativeSide(parent, viewSplit);
			}
			return AnchorStyles.None;
		}

		public AnchorStyles GetRelativeSide()
		{
			if(_isDocumentWell) return AnchorStyles.None;
			if(_status == ViewHostStatus.Floating) return AnchorStyles.None;
			return Parent is ViewSplit parent
				? GetRelativeSide(parent, this)
				: AnchorStyles.None;
		}

		/// <summary>Pins this <see cref="ViewHost"/>.</summary>
		/// <exception cref="InvalidOperationException">This <see cref="ViewHost"/> is not in auto-hide mode.</exception>
		public void Pin()
		{
			Verify.State.IsTrue(_status == ViewHostStatus.AutoHide);

			var side = _dockSide.Side;
			var grid = _dockSide.Grid;
			Undock();
			var dockResult = side switch
			{
				AnchorStyles.Left   => DockResult.Left,
				AnchorStyles.Top    => DockResult.Top,
				AnchorStyles.Right  => DockResult.Right,
				AnchorStyles.Bottom => DockResult.Bottom,
				_ => throw new ApplicationException($"Unexpected ViewDockGridSide.Side: {side}"),
			};
			grid.PerformDock(this, dockResult);
		}

		internal void UnpinFromLeft()
		{
			var w = Width;
			_grid.PerformDock(this, DockResult.AutoHideLeft);
			_dockSide = _grid.LeftSide;
			Width = w + ViewConstants.SideDockPanelBorderSize;
			if(_tabs != null)
			{
				RemoveTabs();
				_viewContainer.Height += Renderer.TabHeight;
			}
			_header.Width = w;
			_viewContainer.Width = w;
		}

		internal void UnpinFromTop()
		{
			var h = Height - Renderer.HeaderHeight;
			_grid.PerformDock(this, DockResult.AutoHideTop);
			_dockSide = _grid.TopSide;
			RemoveTabs();
			Height += ViewConstants.SideDockPanelBorderSize;
			_viewContainer.Height = h;
		}

		internal void UnpinFromRight()
		{
			var w = Width;
			_grid.PerformDock(this, DockResult.AutoHideRight);
			_dockSide = _grid.RightSide;
			_viewContainer.SuspendLayout();
			if(_tabs == null)
			{
				_viewContainer.SetBounds(
					ViewConstants.SideDockPanelBorderSize, 0, w, 0,
					BoundsSpecified.X | BoundsSpecified.Width);
			}
			else
			{
				_viewContainer.SetBounds(
					ViewConstants.SideDockPanelBorderSize, 0, w, Height - Renderer.HeaderHeight,
					BoundsSpecified.X | BoundsSpecified.Width | BoundsSpecified.Height);
				RemoveTabs();
			}
			Width += ViewConstants.SideDockPanelBorderSize;
			_header.SetBounds(
				ViewConstants.SideDockPanelBorderSize, 0, w, 0,
				BoundsSpecified.X | BoundsSpecified.Width);
			_viewContainer.ResumeLayout(true);
		}

		internal void UnpinFromBottom()
		{
			var h = Height;
			_grid.PerformDock(this, DockResult.AutoHideBottom);
			_dockSide = _grid.BottomSide;
			_viewContainer.SuspendLayout();
			var headerHeight = Renderer.HeaderHeight;
			if(_tabs == null)
			{
				_viewContainer.Top = headerHeight +
					ViewConstants.SideDockPanelBorderSize;
			}
			else
			{
				_viewContainer.SetBounds(
					0, ViewConstants.SideDockPanelBorderSize + headerHeight,
					0, h - headerHeight,
					BoundsSpecified.Y | BoundsSpecified.Height);
				RemoveTabs();
			}
			_header.Top = ViewConstants.SideDockPanelBorderSize;
			Height += ViewConstants.SideDockPanelBorderSize;
			_viewContainer.ResumeLayout(true);
		}

		/// <summary>Unpins this <see cref="ViewHost"/>.</summary>
		/// <exception cref="InvalidOperationException">This <see cref="ViewHost"/> is not docked.</exception>
		public void Unpin()
		{
			Verify.State.IsFalse(_isDocumentWell);
			Verify.State.IsTrue(_status == ViewHostStatus.Docked);

			var side = GetRelativeSide();
			if(side == AnchorStyles.None) return;
			Undock();
			switch(side)
			{
				case AnchorStyles.Left:
					UnpinFromLeft();
					break;
				case AnchorStyles.Top:
					UnpinFromTop();
					break;
				case AnchorStyles.Right:
					UnpinFromRight();
					break;
				case AnchorStyles.Bottom:
					UnpinFromBottom();
					break;
				default:
					throw new ApplicationException();
			}
			Status = ViewHostStatus.AutoHide;
		}

		private void MaximizeFloatingForm()
		{
			if(_status == ViewHostStatus.Floating)
			{
				_header.SetAvailableButtons(new[]
				{
					ViewButtonType.Normalize,
					ViewButtonType.Close,
				});
				((Form)TopLevelControl).WindowState = FormWindowState.Maximized;
			}
		}

		private void NormalizeFloatingForm()
		{
			if(_status == ViewHostStatus.Floating)
			{
				_header.SetAvailableButtons(new[]
					{
						ViewButtonType.Maximize,
						ViewButtonType.Close,
					});
				((Form)TopLevelControl).WindowState = FormWindowState.Normal;
			}
		}

		private void OnHeaderMouseDoubleClick(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				if(_status == ViewHostStatus.Floating)
				{
					var f = (Form)TopLevelControl;
					if(f.WindowState == FormWindowState.Maximized)
					{
						NormalizeFloatingForm();
					}
					else
					{
						MaximizeFloatingForm();
					}
				}
				else
				{
					var owner = GetRootOwnerForm();
					var loc = PointToScreen(Point.Empty);
					loc.X -= Renderer.FloatBorderSize;
					loc.Y -= Renderer.FloatBorderSize;
					var form = PrepareFloatingMode();
					form.Location = loc;
					form.Show(owner);
				}
			}
		}

		private void OnHeaderButtonClick(object sender, ViewButtonClickEventArgs e)
		{
			switch(e.Button)
			{
				case ViewButtonType.Close:
					if(_status == ViewHostStatus.Floating)
					{
						((Form)TopLevelControl).Close();
					}
					else
					{
						if(_activeView != null)
						{
							var view = _activeView;
							var index = _views.IndexOf(view);
							if(index == _views.Count - 1)
							{
								--index;
							}
							else
							{
								++index;
							}
							if(index >= 0)
							{
								SetActiveView(_views[index]);
							}
							view.Close();
						}
					}
					break;
				case ViewButtonType.Pin:
					Pin();
					Activate();
					break;
				case ViewButtonType.Unpin:
					Unpin();
					break;
				case ViewButtonType.Maximize:
					MaximizeFloatingForm();
					break;
				case ViewButtonType.Normalize:
					NormalizeFloatingForm();
					break;
			}
		}

		/// <summary>This host should not be destroyed if all hosted views are removed.</summary>
		public bool IsRoot => _isRoot;

		/// <summary>This is a document well - advanced view host which has tabs instead of title bar.</summary>
		public bool IsDocumentWell => _isDocumentWell;

		/// <summary>Host specified view.</summary>
		/// <param name="view">View to host.</param>
		public void AddView(ViewBase view)
		{
			Verify.Argument.IsNotNull(view, nameof(view));
			Verify.Argument.IsTrue(view.Host == null, nameof(view), "View is already hosted.");

			_views.Add(view);
			view.Host = this;
			if(_isDocumentWell)
			{
				if(_views.Count == 1)
				{
					SpawnTabs();
					SpawnFooter();
					_activeView = view;
					_activeView.Bounds = _viewContainer.ClientRectangle;
					_activeView.Dock   = DockStyle.Fill;
					_activeView.Parent = _viewContainer;
					Events.Raise(ActiveViewChangedEvent, this);
				}
				else
				{
					_tabs.AddView(view);
				}
			}
			else
			{
				if(_views.Count == 1)
				{
					SpawnHeader();
					_activeView = view;
					_activeView.Bounds = _viewContainer.ClientRectangle;
					_activeView.Dock   = DockStyle.Fill;
					_activeView.Parent = _viewContainer;
					Events.Raise(ActiveViewChangedEvent, this);
				}
				else if(_views.Count == 2)
				{
					SpawnTabs();
				}
				else
				{
					_tabs.AddView(view);
				}
			}
			UpdateChildBounds();
			view.TextChanged += OnViewTextChanged;
			Events.Raise(ViewAddedEvent, this, new ViewEventArgs(view));
		}

		/// <summary>Number of hosted views.</summary>
		public int ViewsCount => _views.Count;

		/// <summary>Returns hosted view.</summary>
		/// <param name="index">View index.</param>
		/// <returns>Hosted view with specified index.</returns>
		public ViewBase GetView(int index) => _views[index];

		public bool Contains(ViewBase view) => _views.Contains(view);

		public int IndexOf(ViewBase view) => _views.IndexOf(view);

		/// <summary>Set active view.</summary>
		/// <param name="view">View to activate.</param>
		public void SetActiveView(ViewBase view)
		{
			Verify.Argument.IsNotNull(view, nameof(view));
			Verify.Argument.IsTrue(view.Host == this, nameof(view), "View is hosted in another host.");

			if(_activeView != view)
			{
				var old = _activeView;
				_activeView = view;
				_activeView.Bounds = _viewContainer.ClientRectangle;
				_activeView.Dock   = DockStyle.Fill;
				_activeView.Parent = _viewContainer;
				if(old is not null)
				{
					old.Parent = null;
				}
				_header?.Invalidate();
				if(_tabs is not null)
				{
					_tabs.EnsureVisible(view);
					_tabs.Invalidate();
				}
				Events.Raise(ActiveViewChangedEvent, this);
			}
		}

		private void PreventActiveViewDispose()
		{
			if(_activeView is not null)
			{
				_activeView.Parent = null;
			}
		}

		/// <summary>Remove view from host.</summary>
		/// <param name="view">View to remove.</param>
		/// <remarks>
		/// Removing last view from non-host control destroys host.
		/// Host container gets updated and sibling hosts resize accordingly.
		/// </remarks>
		internal void RemoveView(ViewBase view)
		{
			Verify.Argument.IsNotNull(view, nameof(view));
			Verify.Argument.IsTrue(view.Host == this, nameof(view), "View is not hosted in this ViewHost.");
			int index = _views.IndexOf(view);
			Assert.AreNotEqual(index, -1);

			_views.RemoveAt(index);
			view.TextChanged -= OnViewTextChanged;
			view.Host = null;
			if(_isDocumentWell)
			{
				if(_views.Count == 0)
				{
					if(view == _activeView)
					{
						view.Parent = null;
					}
					if(!_isRoot)
					{
						Undock();
						Dispose();
						Status = ViewHostStatus.Disposed;
					}
					else
					{
						SuspendLayout();
						RemoveTabs();
						RemoveFooter();
						UpdateChildBounds();
						ResumeLayout(performLayout: false);
					}
				}
				else
				{
					_tabs.RemoveView(view);
					if(_activeView == view)
					{
						bool focused = view.ContainsFocus;
						if(index >= _views.Count)
						{
							index = _views.Count - 1;
						}
						if(focused)
						{
							Activate(_views[index]);
						}
						else
						{
							SetActiveView(_views[index]);
						}
						view.Parent = null;
					}
				}
			}
			else
			{
				if(_views.Count == 0)
				{
					if(view == _activeView)
					{
						view.Parent = null;
					}
					if(!_isRoot)
					{
						Undock();
						Dispose();
						Status = ViewHostStatus.Disposed;
					}
					else
					{
						RemoveHeader();
						UpdateChildBounds();
					}
				}
				else
				{
					if(_status != ViewHostStatus.AutoHide)
					{
						if(_views.Count == 1)
						{
							RemoveTabs();
							UpdateChildBounds();
						}
						else
						{
							_tabs.RemoveView(view);
						}
					}
					if(_activeView == view)
					{
						bool focused = view.ContainsFocus;
						if(index >= _views.Count)
						{
							index = _views.Count - 1;
						}
						if(focused)
						{
							Activate(_views[index]);
						}
						else
						{
							SetActiveView(_views[index]);
						}
						view.Parent = null;
					}
				}
			}
			Events.Raise(ViewRemovedEvent, this, new ViewEventArgs(view));
		}

		/// <summary>Creates <see cref="ViewHostTabs"/> control.</summary>
		private void SpawnTabs()
		{
			Verify.State.IsTrue(_tabs is null, "Tabs are already spawned.");

			_tabs = new ViewHostTabs(this, _isDocumentWell ? AnchorStyles.Top : AnchorStyles.Bottom)
			{
				Parent = this
			};
		}

		/// <summary>Destroy tabs control.</summary>
		private void RemoveTabs()
		{
			if(_tabs is not null)
			{
				_tabs.Parent = null;
				_tabs.Dispose();
				_tabs = null;
			}
		}

		private void OnHeaderMouseDown(object sender, MouseEventArgs e)
		{
			switch(e.Button)
			{
				case MouseButtons.Left:
					if(_status == ViewHostStatus.Floating)
					{
						var form = (Form)TopLevelControl;
						if(form.WindowState != FormWindowState.Normal) return;
					}
					_mdX = e.X;
					_mdY = e.Y;
					_readyToMove = true;
					break;
			}
		}

		private void OnHeaderMouseMove(object sender, MouseEventArgs e)
		{
			bool moving = false;
			if(_readyToMove)
			{
				if(_status != ViewHostStatus.Floating)
				{
					if(Math.Abs(e.X - _mdX) > 6 || Math.Abs(e.Y - _mdY) > 6)
					{
						_readyToMove = false;
						moving = true;
						GoFloatingMode();
					}
				}
				else
				{
					_readyToMove = false;
					moving = true;
				}
			}
			if(moving || _dockingProcess.IsActive)
			{
				var p = TopLevelControl;
				if(p != null)
				{
					int dx = e.X - _mdX;
					int dy = e.Y - _mdY;
					if(dx != 0 || dy != 0)
					{
						var loc = p.Location;
						loc.Offset(dx, dy);
						p.Location = loc;
					}
				}
				var location = e.Location;
				location.X += _header.Left;
				location.Y += _header.Top;
				if(_dockingProcess.IsActive)
				{
					_dockingProcess.Update(location);
				}
				else
				{
					_dockingProcess.Start(location);
				}
			}
		}

		private void OnHeaderMouseUp(object sender, MouseEventArgs e)
		{
			if(_dockingProcess.IsActive)
			{
				var location = e.Location;
				location.X += _header.Left;
				location.Y += _header.Top;
				_dockingProcess.Commit(location);
			}
			_readyToMove = false;
		}

		private void OnViewTextChanged(object sender, EventArgs e)
		{
			var view = (ViewBase)sender;
			_header?.Invalidate();
			_tabs?.InvalidateTab(view);
		}

		private void InvalidateHelpers()
		{
			_tabs?.Invalidate();
			_footer?.Invalidate();
			_header?.Invalidate();
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.Enter"/> event.</summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			_isActive = true;
			InvalidateHelpers();
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.Leave"/> event.</summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave(e);
			_isActive = false;
			InvalidateHelpers();
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Control.Resize"/> event.</summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateChildBounds();
			if(_tabs is not null && _activeView is not null)
			{
				_tabs.EnsureVisible(_activeView);
			}
		}

		public void Activate()
		{
			if(_activeView is not null)
			{
				if(_activeView.Focus())
				{
					_isActive = true;
				}
			}
		}

		public void Activate(ViewBase view)
		{
			Verify.Argument.IsNotNull(view, nameof(view));
			Verify.Argument.IsTrue(view.Host == this, nameof(view), "View is not hosted in this ViewHost.");

			SetActiveView(view);
			_activeView.Focus();
		}

		/// <summary>This view host is active.</summary>
		public bool IsActive => ContainsFocus;

		/// <summary>Undocks this <see cref="ViewHost"/>.</summary>
		private void Undock()
		{
			switch(Status)
			{
				case ViewHostStatus.AutoHide:
					{
						_dockSide.RemoveHost(this);
						switch(_dockSide.Side)
						{
							case AnchorStyles.Left or AnchorStyles.Right:
								Width -= ViewConstants.SideDockPanelBorderSize;
								break;
							case AnchorStyles.Top or AnchorStyles.Bottom:
								Height -= ViewConstants.SideDockPanelBorderSize;
								break;
							default:
								throw new ApplicationException($"Unexpected ViewDockSide.Side: {_dockSide.Side}");
						}
						_dockSide = null;
						if(_views.Count > 1) SpawnTabs();
						UpdateChildBounds();
					}
					break;
				case ViewHostStatus.DockedOnFloat:
				case ViewHostStatus.Docked:
					{
						var parent = Parent;
						if(parent is not null)
						{
							parent.DisableRedraw();
							try
							{
								if(parent is ViewSplit split)
								{
									split.Remove(this);
									return;
								}
								else
								{
									throw new ApplicationException($"Unexpected ViewHost.Parent: {parent}");
								}
							}
							finally
							{
								if(!parent.IsDisposed)
								{
									parent.EnableRedraw();
									parent.RedrawWindow();
								}
							}
						}
					}
					break;
				case ViewHostStatus.Floating:
					{
						var parent = (Form)TopLevelControl;
						Parent = null;
						parent.Close();
						parent.Dispose();
						Anchor = ViewConstants.AnchorDefault;
					}
					break;
				case ViewHostStatus.Offscreen:
				case ViewHostStatus.Disposed:
					throw new InvalidOperationException();
				default:
					throw new ApplicationException();
			}
			Status = ViewHostStatus.Offscreen;
		}

		public void StartMoving()
		{
			int d = Renderer.FloatBorderSize + Renderer.HeaderHeight / 2;
			StartMoving(d, d);
		}

		public void StartMoving(int dx, int dy)
		{
			if(_header is null) return;

			var pos = Control.MousePosition;
			pos.Offset(-dx, -dy);
			_mdX = dx - Renderer.FloatBorderSize;
			_mdY = dy - Renderer.FloatBorderSize;
			Parent.Location = pos;
			_dockingProcess.Start(new Point(dx, dy));
			_header.Capture = true;
		}

		/// <summary>Undock and embed into floating form.</summary>
		/// <returns>Floating form.</returns>
		internal FloatingViewForm PrepareFloatingMode()
		{
			if(_status != ViewHostStatus.Offscreen) Undock();
			var floatingForm = new FloatingViewForm(_grid, this);
			Location = new Point(
				Renderer.FloatBorderSize,
				Renderer.FloatBorderSize);
			Anchor = ViewConstants.AnchorAll;
			Parent = floatingForm;
			Status = ViewHostStatus.Floating;
			return floatingForm;
		}

		/// <summary>Gets the root owner form.</summary>
		/// <returns>Root owner form.</returns>
		public Form GetRootOwnerForm()
		{
			if(TopLevelControl is Form form)
			{
				while(form.Owner is not null)
				{
					form = form.Owner;
				}
				return form;
			}
			return null;
		}

		/// <summary>Undock and go floating.</summary>
		/// <param name="parent">Parent form.</param>
		/// <returns>Floating form.</returns>
		public Form GoFloatingMode(IWin32Window parent)
		{
			var floatingForm = PrepareFloatingMode();
			floatingForm.Show(parent);
			return floatingForm;
		}

		/// <summary>Undock and go floating.</summary>
		/// <returns>Floating form.</returns>
		public Form GoFloatingMode() => GoFloatingMode(GetRootOwnerForm());

		/// <summary>Destroy floating host form and get ready to dock.</summary>
		private void ReturnFromFloatingMode()
		{
			if(Status == ViewHostStatus.Floating)
			{
				var p = (Form)TopLevelControl;
				Parent = null;
				p.Close();
				p.Dispose();
				Anchor = ViewConstants.AnchorDefault;
				Status = ViewHostStatus.Offscreen;
			}
		}

		/// <summary>Gets the hosting <see cref="ViewDockGrid"/>.</summary>
		/// <value>Hosting <see cref="ViewDockGrid"/>.</value>
		internal ViewDockGrid Grid => _grid;

		/// <summary>Host dock status.</summary>
		public ViewHostStatus Status
		{
			get => _status;
			internal set
			{
				if(_status != value)
				{
					_status = value;
					if(_header is not null) ResetHeaderButtons();
					Events.Raise(StatusChangedEvent, this);
				}
			}
		}

		private void ResetHeaderButtons()
		{
			if(_header is null) return;
			switch(Status)
			{
				case ViewHostStatus.AutoHide:
					_header.SetAvailableButtons(ViewButtonType.Pin, ViewButtonType.Close);
					break;
				case ViewHostStatus.Floating:
					switch(((Form)TopLevelControl).WindowState)
					{
						case FormWindowState.Maximized:
							_header.SetAvailableButtons(ViewButtonType.Normalize, ViewButtonType.Close);
							break;
						case FormWindowState.Normal:
							_header.SetAvailableButtons(ViewButtonType.Maximize, ViewButtonType.Close);
							break;
					}
					break;
				case ViewHostStatus.Docked:
					_header.SetAvailableButtons(ViewButtonType.Unpin, ViewButtonType.Close);
					break;
				case ViewHostStatus.DockedOnFloat:
					_header.SetAvailableButtons(ViewButtonType.Close);
					break;
			}
		}

		/// <summary>Returns collection of hosted views.</summary>
		public IEnumerable<ViewBase> Views => _views;

		/// <summary>Active view.</summary>
		/// <remarks>
		/// Host can have only one active view.
		/// Root host has no active view if it hosts no views.
		/// </remarks>
		public ViewBase ActiveView => _activeView;

		/// <summary>Active view's caption.</summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text
		{
			get => _activeView is not null ? _activeView.Text : string.Empty;
			set => throw new InvalidOperationException();
		}

		/// <summary>Dock inside another host as tabbed views.</summary>
		/// <param name="viewHost">Host to dock into.</param>
		/// <remarks>
		/// This ViewHost will be destroyed in the process of docking.
		/// </remarks>
		internal void DockInto(ViewHost viewHost)
		{
			Verify.Argument.IsNotNull(viewHost, nameof(viewHost));
			Verify.Argument.IsTrue(viewHost != this, nameof(viewHost), "Cannot dock to itself.");

			ReturnFromFloatingMode();
			PreventActiveViewDispose();
			foreach(var view in Views)
			{
				view.Host = null;
				viewHost.AddView(view);
			}
			if(ActiveView != null)
			{
				viewHost.SetActiveView(_activeView);
				_activeView = null;
			}
			Dispose();
			Status = ViewHostStatus.Disposed;
		}

		/// <summary>Gets the <see cref="ViewDockSide"/> which hosts this <see cref="ViewHost"/> in auto-hide state.</summary>
		/// <value><see cref="ViewDockSide"/> which hosts this <see cref="ViewHost"/> in auto-hide state.</value>
		internal ViewDockSide DockSide
		{
			get => _dockSide;
			set => _dockSide = value;
		}

		/// <summary>Dock to side of specified host.</summary>
		/// <param name="origin">Relative control.</param>
		/// <param name="side">Side to dock into.</param>
		internal void DockToSide(Control origin, AnchorStyles side)
		{
			bool wasActive = ActiveView != null && IsActive;
			ReturnFromFloatingMode();
			ViewSplit.Replace(origin, this, side);
			Status = ViewHostStatus.Docked;
			if(wasActive)
			{
				SelectNextControl(this, true, true, true, false);
			}
		}

		/// <summary>Converts this <see cref="ViewHost"/> to document well.</summary>
		private void ConvertToDocumentWell()
		{
			if(!IsDocumentWell)
			{
				_isDocumentWell = true;
				SuspendLayout();
				RemoveHeader();
				RemoveTabs();
				var size = Size;
				SpawnTabs();
				SpawnFooter();
				UpdateChildBounds();
				ResumeLayout(performLayout: true);
			}
		}

		/// <summary>Dock to side of specified host as document.</summary>
		/// <param name="origin">Relative control.</param>
		/// <param name="side">Side to dock into.</param>
		internal void DockToSideAsDocument(Control origin, AnchorStyles side)
		{
			bool wasActive = ActiveView != null && IsActive;
			ReturnFromFloatingMode();
			ConvertToDocumentWell();
			ViewSplit.Replace(origin, this, side);
			Status = ViewHostStatus.Docked;
			if(wasActive)
			{
				SelectNextControl(this, true, true, true, false);
			}
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> and its child controls and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_ownerForm != null)
				{
					_ownerForm.Activated  -= OnOwnerFormActivated;
					_ownerForm.Deactivate -= OnOwnerFormDeactivated;
					_ownerForm = null;
				}
				foreach(var view in _views)
				{
					view.TextChanged -= OnViewTextChanged;
				}
				RemoveHeader();
				RemoveTabs();
				RemoveFooter();
				_dockingProcess.Dispose();
				_resizingProcess.Dispose();
				_viewContainer.Dispose();
				_dockMarkers.Dispose();
				_dockSide = null;
				_status = ViewHostStatus.Disposed;
			}
			lock(_viewHosts)
			{
				_viewHosts.Remove(this);
			}
			base.Dispose(disposing);
		}

		#region IDockHost

		/// <summary>Provides dock helper markers.</summary>
		public IDockMarkerProvider DockMarkers => _dockMarkers;

		/// <summary>
		/// Determines if <see cref="ViewHost"/> can be docked into this <see cref="IDockHost"/>.
		/// </summary>
		/// <param name="viewHost"><see cref="ViewHost"/> to dock.</param>
		/// <param name="dockResult">Position for docking.</param>
		/// <returns>true if docking is possible.</returns>
		public bool CanDock(ViewHost viewHost, DockResult dockResult)
		{
			Verify.Argument.IsNotNull(viewHost, nameof(viewHost));

			switch(dockResult)
			{
				case DockResult.Left:
				case DockResult.Top:
				case DockResult.Right:
				case DockResult.Bottom:
				case DockResult.Fill:
					return true;
				case DockResult.DocumentLeft:
				case DockResult.DocumentTop:
				case DockResult.DocumentRight:
				case DockResult.DocumentBottom:
					return _isDocumentWell;
				default:
					return false;
			}
		}

		/// <summary>
		/// Docks <paramref name="viewHost"/> into this <see cref="IDockHost"/>.
		/// </summary>
		/// <param name="viewHost"><see cref="ViewHost"/> to dock.</param>
		/// <param name="dockResult">Position for docking.</param>
		public void PerformDock(ViewHost viewHost, DockResult dockResult)
		{
			Verify.Argument.IsNotNull(viewHost, nameof(viewHost));

			switch(dockResult)
			{
				case DockResult.Left:
					viewHost.DockToSide(this, AnchorStyles.Left);
					if(_status == ViewHostStatus.DockedOnFloat || _status == ViewHostStatus.Floating)
					{
						viewHost.Status = ViewHostStatus.DockedOnFloat;
					}
					break;
				case DockResult.Top:
					viewHost.DockToSide(this, AnchorStyles.Top);
					if(_status == ViewHostStatus.DockedOnFloat || _status == ViewHostStatus.Floating)
					{
						viewHost.Status = ViewHostStatus.DockedOnFloat;
					}
					break;
				case DockResult.Right:
					viewHost.DockToSide(this, AnchorStyles.Right);
					if(_status == ViewHostStatus.DockedOnFloat || _status == ViewHostStatus.Floating)
					{
						viewHost.Status = ViewHostStatus.DockedOnFloat;
					}
					break;
				case DockResult.Bottom:
					viewHost.DockToSide(this, AnchorStyles.Bottom);
					if(_status == ViewHostStatus.DockedOnFloat || _status == ViewHostStatus.Floating)
					{
						viewHost.Status = ViewHostStatus.DockedOnFloat;
					}
					break;
				case DockResult.DocumentLeft:
					viewHost.DockToSideAsDocument(this, AnchorStyles.Left);
					break;
				case DockResult.DocumentTop:
					viewHost.DockToSideAsDocument(this, AnchorStyles.Top);
					break;
				case DockResult.DocumentRight:
					viewHost.DockToSideAsDocument(this, AnchorStyles.Right);
					break;
				case DockResult.DocumentBottom:
					viewHost.DockToSideAsDocument(this, AnchorStyles.Bottom);
					break;
				case DockResult.Fill:
					viewHost.DockInto(this);
					Activate();
					break;
				default:
					throw new ArgumentException($"Unsupported DockResult value: {dockResult}", nameof(dockResult));
			}
		}

		/// <summary>Get bounding rectangle for docked view.</summary>
		/// <param name="viewHost">Tested <see cref="ViewHost"/>.</param>
		/// <param name="dockResult">Position for docking.</param>
		/// <returns>Bounding rectangle for docked view.</returns>
		public Rectangle GetDockBounds(ViewHost viewHost, DockResult dockResult)
		{
			Verify.Argument.IsNotNull(viewHost, nameof(viewHost));

			Rectangle bounds;
			var size = Size;
			switch(dockResult)
			{
				case DockResult.Left:
				case DockResult.DocumentLeft:
					{
						var w1 = viewHost.Width;
						var w2 = size.Width;
						ViewSplit.OptimalSizes(w2, ViewConstants.MinimumHostWidth, ref w1, ref w2);
						bounds = new Rectangle(0, 0, w1, size.Height);
					}
					break;
				case DockResult.Top:
				case DockResult.DocumentTop:
					{
						var h1 = viewHost.Height;
						var h2 = size.Height;
						ViewSplit.OptimalSizes(h2, ViewConstants.MinimumHostHeight, ref h1, ref h2);
						bounds = new Rectangle(0, 0, size.Width, h1);
					}
					break;
				case DockResult.Right:
				case DockResult.DocumentRight:
					{
						var w1 = size.Width;
						var w2 = viewHost.Width;
						ViewSplit.OptimalSizes(w1, ViewConstants.MinimumHostWidth, ref w1, ref w2);
						bounds = new Rectangle(size.Width - w2, 0, w2, size.Height);
					}
					break;
				case DockResult.Bottom:
				case DockResult.DocumentBottom:
					{
						var h1 = size.Height;
						var h2 = viewHost.Height;
						ViewSplit.OptimalSizes(h1, ViewConstants.MinimumHostHeight, ref h1, ref h2);
						bounds = new Rectangle(0, size.Height - h2, size.Width, h2);
					}
					break;
				case DockResult.Fill:
					bounds = new Rectangle(0, 0, size.Width, size.Height);
					bounds.Intersect(_viewContainer.Bounds);
					break;
				default:
					throw new ArgumentException($"Unsupported DockResult value: {dockResult}", nameof(dockResult));
			}
			return RectangleToScreen(bounds);
		}

		#endregion
	}
}
