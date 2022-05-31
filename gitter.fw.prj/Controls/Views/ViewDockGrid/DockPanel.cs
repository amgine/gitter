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
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

/// <summary>Hosts view hosts and applies grid layout.</summary>
[DesignerCategory("")]
public sealed class DockPanel : ContainerControl, IDockHost
{
	#region Data

	private readonly DockPanelDockMarkers _dockMarkers;

	#endregion

	#region .ctor

	/// <summary>Initializes a new instance of the <see cref="DockPanel"/> class.</summary>
	public DockPanel()
	{
		SuspendLayout();
		_dockMarkers = new DockPanelDockMarkers(this);

		SetStyle(ControlStyles.ContainerControl, true);

		AutoScaleMode = AutoScaleMode.None;
		RootControl  = RootHost = CreateRootHost();
		RootControl.Parent = this;
		ResumeLayout(true);
		BackColor = ViewManager.Renderer.BackgroundColor;

		DockElements<DockPanel>.Add(this);
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

	private ViewRenderer Renderer => ViewManager.Renderer;

	#endregion

	private ViewHost CreateRootHost()
		=> new(this, isRoot: true, isDocumentWell: true, views: null)
		{
			Bounds = GetRootBounds(Dpi.FromControl(this)),
		};

#if NET5_0_OR_GREATER
	[MemberNotNull(nameof(LeftSide))]
#endif
	private DockPanelSide SpawnLeftSide()
	{
		Verify.State.IsTrue(LeftSide is null);

		var dpi    = Dpi.FromControl(this);
		var bounds = GetLeftSideBounds(dpi);

		LeftSide = new DockPanelSide(this, AnchorStyles.Left)
		{
			Bounds = bounds,
			Parent = this,
		};

		if(TopSide     is not null) TopSide.Bounds     = GetTopSideBounds   (dpi);
		if(BottomSide  is not null) BottomSide.Bounds  = GetBottomSideBounds(dpi);
		if(RootControl is not null) RootControl.Bounds = GetRootBounds      (dpi);

		return LeftSide;
	}

#if NET5_0_OR_GREATER
	[MemberNotNull(nameof(TopSide))]
#endif
	private DockPanelSide SpawnTopSide()
	{
		Verify.State.IsTrue(TopSide is null);

		var dpi    = Dpi.FromControl(this);
		var bounds = GetTopSideBounds(dpi);

		TopSide = new DockPanelSide(this, AnchorStyles.Top)
		{
			Bounds = bounds,
			Parent = this,
		};

		if(LeftSide    is not null) LeftSide.Bounds    = GetLeftSideBounds (dpi);
		if(RightSide   is not null) RightSide.Bounds   = GetRightSideBounds(dpi);
		if(RootControl is not null) RootControl.Bounds = GetRootBounds     (dpi);

		return TopSide;
	}

#if NET5_0_OR_GREATER
	[MemberNotNull(nameof(RightSide))]
#endif
	private DockPanelSide SpawnRightSide()
	{
		Verify.State.IsTrue(RightSide is null);

		var dpi    = Dpi.FromControl(this);
		var bounds = GetRightSideBounds(dpi);

		RightSide = new DockPanelSide(this, AnchorStyles.Right)
		{
			Bounds = bounds,
			Parent = this,
		};

		if(TopSide     is not null) TopSide.Bounds     = GetTopSideBounds   (dpi);
		if(BottomSide  is not null) BottomSide.Bounds  = GetBottomSideBounds(dpi);
		if(RootControl is not null) RootControl.Bounds = GetRootBounds      (dpi);

		return RightSide;
	}

#if NET5_0_OR_GREATER
	[MemberNotNull(nameof(BottomSide))]
#endif
	private DockPanelSide SpawnBottomSide()
	{
		Verify.State.IsTrue(BottomSide is null);

		var dpi    = Dpi.FromControl(this);
		var bounds = GetBottomSideBounds(dpi);

		BottomSide = new DockPanelSide(this, AnchorStyles.Bottom)
		{
			Bounds = bounds,
			Parent = this,
		};

		if(LeftSide    is not null) LeftSide.Bounds    = GetLeftSideBounds (dpi);
		if(RightSide   is not null) RightSide.Bounds   = GetRightSideBounds(dpi);
		if(RootControl is not null) RootControl.Bounds = GetRootBounds     (dpi);

		return BottomSide;
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
				RemoveBottomSide();
				break;
			default:
				throw new ArgumentException(
					$"Unknown AnchorStyles value: {side}",
					nameof(side));
		}
	}

	private void RemoveAllSides()
	{
		SuspendLayout();
		if(LeftSide is not null)
		{
			LeftSide.Parent = null;
			LeftSide.Dispose();
			LeftSide = null;
		}
		if(TopSide is not null)
		{
			TopSide.Parent = null;
			TopSide.Dispose();
			TopSide = null;
		}
		if(RightSide is not null)
		{
			RightSide.Parent = null;
			RightSide.Dispose();
			RightSide = null;
		}
		if(BottomSide is not null)
		{
			BottomSide.Parent = null;
			BottomSide.Dispose();
			BottomSide = null;
		}
		if(RootControl is not null)
		{
			RootControl.Bounds = GetRootBounds(Dpi.FromControl(this));
		}
		ResumeLayout(performLayout: false);
	}

	private void RemoveLeftSide()
	{
		if(LeftSide is null) return;

		LeftSide.Parent = null;
		LeftSide.Dispose();
		LeftSide = null;

		var dpi = Dpi.FromControl(this);

		if(RootControl is not null) RootControl.Bounds = GetRootBounds      (dpi);
		if(TopSide     is not null) TopSide.Bounds     = GetTopSideBounds   (dpi);
		if(BottomSide  is not null) BottomSide.Bounds  = GetBottomSideBounds(dpi);
	}

	private void RemoveTopSide()
	{
		if(TopSide is null) return;

		TopSide.Parent = null;
		TopSide.Dispose();
		TopSide = null;

		var dpi = Dpi.FromControl(this);

		if(RootControl is not null) RootControl.Bounds = GetRootBounds     (dpi);
		if(LeftSide    is not null) LeftSide.Bounds    = GetLeftSideBounds (dpi);
		if(RightSide   is not null) RightSide.Bounds   = GetRightSideBounds(dpi);
	}

	private void RemoveRightSide()
	{
		if(RightSide is null) return;

		RightSide.Parent = null;
		RightSide.Dispose();
		RightSide = null;

		var dpi = Dpi.FromControl(this);

		if(RootControl is not null) RootControl.Bounds = GetRootBounds      (dpi);
		if(TopSide     is not null) TopSide.Bounds     = GetTopSideBounds   (dpi);
		if(BottomSide  is not null) BottomSide.Bounds  = GetBottomSideBounds(dpi);
	}

	private void RemoveBottomSide()
	{
		if(BottomSide is null) return;

		BottomSide.Parent = null;
		BottomSide.Dispose();
		BottomSide = null;

		var dpi = Dpi.FromControl(this);

		if(RootControl is not null) RootControl.Bounds = GetRootBounds     (dpi);
		if(LeftSide    is not null) LeftSide.Bounds    = GetLeftSideBounds (dpi);
		if(RightSide   is not null) RightSide.Bounds   = GetRightSideBounds(dpi);
	}

	internal DockPanelSide LeftSide { get; private set; }

	internal DockPanelSide RightSide { get; private set; }

	internal DockPanelSide TopSide { get; private set; }

	internal DockPanelSide BottomSide { get; private set; }

	private DockPanelSide GetCreateDockSide(AnchorStyles side)
	{
		DockPanelSide viewDockSide;
		switch(side)
		{
			case AnchorStyles.Left:
				if(LeftSide is null) SpawnLeftSide();
				viewDockSide = LeftSide;
				break;
			case AnchorStyles.Top:
				if(TopSide is null) SpawnTopSide();
				viewDockSide = TopSide;
				break;
			case AnchorStyles.Right:
				if(RightSide is null) SpawnRightSide();
				viewDockSide = RightSide;
				break;
			case AnchorStyles.Bottom:
				if(BottomSide is null) SpawnBottomSide();
				viewDockSide = BottomSide;
				break;
			default:
				throw new ArgumentException(
					"Unknown AnchorStyles value: {0}".UseAsFormat(side),
					"side");
		}
		return viewDockSide;
	}

	public PopupNotificationsStack PopupsStack { get; private set; } = new();

	internal int HorizontalClientSpace
	{
		get
		{
			var dpi = Dpi.FromControl(this);
			var w = Width - ViewConstants.Spacing.GetValue(dpi).Width * 2;
			if(LeftSide  is not null) w -= Renderer.SideTabSize.GetValue(dpi).Width;
			if(RightSide is not null) w -= Renderer.SideTabSize.GetValue(dpi).Width;
			return w;
		}
	}

	internal int VerticalClientSpace
	{
		get
		{
			var dpi = Dpi.FromControl(this);
			var h = Height - ViewConstants.Spacing.GetValue(dpi).Height * 2;
			if(TopSide    is not null) h -= Renderer.SideTabSize.GetValue(dpi).Height;
			if(BottomSide is not null) h -= Renderer.SideTabSize.GetValue(dpi).Height;
			return h;
		}
	}

	private static ViewBase FindView(Control control, Guid guid, object viewModel, bool considerViewModel)
	{
		foreach(Control child in control.Controls)
		{
			if(child is ViewHost viewHost)
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
				if(view is not null) return view;
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

	private void UpdateChildrenBounds()
	{
		var dpi = Dpi.FromControl(this);
		if(RootControl is not null) RootControl.Bounds = GetRootBounds      (dpi);
		if(LeftSide    is not null) LeftSide.Bounds    = GetLeftSideBounds  (dpi);
		if(TopSide     is not null) TopSide.Bounds     = GetTopSideBounds   (dpi);
		if(RightSide   is not null) RightSide.Bounds   = GetRightSideBounds (dpi);
		if(BottomSide  is not null) BottomSide.Bounds  = GetBottomSideBounds(dpi);
	}

	/// <inheritdoc/>
	protected override void OnSizeChanged(EventArgs e)
	{
		UpdateChildrenBounds();
		base.OnSizeChanged(e);
	}

#if NETCOREAPP || NET48_OR_GREATER
	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	protected override void OnDpiChangedAfterParent(EventArgs e)
	{
		UpdateChildrenBounds();
		base.OnDpiChangedAfterParent(e);
	}
#endif

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			DockElements<DockPanel>.Remove(this);
			_dockMarkers.Dispose();
			if(LeftSide is not null)
			{
				LeftSide.Dispose();
				LeftSide = null;
			}
			if(TopSide is not null)
			{
				TopSide.Dispose();
				TopSide = null;
			}
			if(RightSide is not null)
			{
				RightSide.Dispose();
				RightSide = null;
			}
			if(BottomSide is not null)
			{
				BottomSide.Dispose();
				BottomSide = null;
			}
			if(PopupsStack is not null)
			{
				PopupsStack.Dispose();
				PopupsStack = null;
			}
			RootControl = null;
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
		Verify.Argument.IsNotNull(viewHost);

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
		Verify.Argument.IsNotNull(viewHost);
		Verify.Argument.IsFalse(viewHost.IsDocumentWell, nameof(viewHost));
		Verify.Argument.IsFalse(viewHost.ViewsCount == 1 && viewHost.GetView(0).IsDocument, nameof(viewHost));

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

	private Rectangle GetRootBounds(Dpi dpi)
	{
		var bounds  = ClientRectangle;
		var spacing = ViewConstants.Spacing.GetValue(dpi);

		bounds.X      += spacing.Width;
		bounds.Y      += spacing.Height;
		bounds.Width  -= spacing.Width  * 2;
		bounds.Height -= spacing.Height * 2;

		var sideTabSize = Renderer.SideTabSize.GetValue(dpi);

		if(LeftSide is not null)
		{
			bounds.X     += sideTabSize.Width;
			bounds.Width -= sideTabSize.Width;
		}
		if(RightSide is not null)
		{
			bounds.Width -= sideTabSize.Width;
		}
		if(TopSide is not null)
		{
			bounds.Y      += sideTabSize.Height;
			bounds.Height -= sideTabSize.Height;
		}
		if(BottomSide is not null)
		{
			bounds.Height -= sideTabSize.Height;
		}

		if(bounds.Width  < 0) bounds.Width  = 0;
		if(bounds.Height < 0) bounds.Height = 0;

		return bounds;
	}

	private Rectangle GetLeftSideBounds(Dpi dpi)
	{
		var cs          = ClientSize;
		var spacing     = ViewConstants.Spacing.GetValue(dpi);
		var sideTabSize = Renderer.SideTabSize.GetValue(dpi);
		var bounds      = new Rectangle(
			0,
			spacing.Height,
			sideTabSize.Width,
			cs.Height - spacing.Height * 2);

		if(TopSide is not null)
		{
			bounds.Y      += sideTabSize.Height;
			bounds.Height -= sideTabSize.Height;
		}
		if(BottomSide is not null)
		{
			bounds.Height -= sideTabSize.Height;
		}

		if(bounds.Width  < 0) bounds.Width  = 0;
		if(bounds.Height < 0) bounds.Height = 0;

		return bounds;
	}

	private Rectangle GetTopSideBounds(Dpi dpi)
	{
		var cs          = ClientSize;
		var spacing     = ViewConstants.Spacing.GetValue(dpi);
		var sideTabSize = Renderer.SideTabSize.GetValue(dpi);
		var bounds      = new Rectangle(
			spacing.Width,
			0,
			cs.Width - spacing.Width * 2,
			sideTabSize.Height);

		if(LeftSide is not null)
		{
			bounds.X     += sideTabSize.Width;
			bounds.Width -= sideTabSize.Width;
		}
		if(RightSide is not null)
		{
			bounds.Width -= sideTabSize.Width;
		}

		if(bounds.Width  < 0) bounds.Width  = 0;
		if(bounds.Height < 0) bounds.Height = 0;

		return bounds;
	}

	private Rectangle GetBottomSideBounds(Dpi dpi)
	{
		var cs          = ClientSize;
		var spacing     = ViewConstants.Spacing.GetValue(dpi);
		var sideTabSize = Renderer.SideTabSize.GetValue(dpi);
		var bounds      = new Rectangle(
			spacing.Width,
			cs.Height - sideTabSize.Height,
			cs.Width  - spacing.Width * 2,
			sideTabSize.Height);

		if(LeftSide is not null)
		{
			bounds.X     += sideTabSize.Width;
			bounds.Width -= sideTabSize.Width;
		}
		if(RightSide is not null)
		{
			bounds.Width -= sideTabSize.Width;
		}

		if(bounds.Width  < 0) bounds.Width  = 0;
		if(bounds.Height < 0) bounds.Height = 0;

		return bounds;
	}

	private Rectangle GetRightSideBounds(Dpi dpi)
	{
		var cs          = ClientSize;
		var spacing     = ViewConstants.Spacing.GetValue(dpi);
		var sideTabSize = Renderer.SideTabSize.GetValue(dpi);
		var bounds      = new Rectangle(
			cs.Width - sideTabSize.Width,
			spacing.Height,
			sideTabSize.Width,
			cs.Height - spacing.Height * 2);

		if(TopSide is not null)
		{
			bounds.Y      += sideTabSize.Height;
			bounds.Height -= sideTabSize.Height;
		}
		if(BottomSide is not null)
		{
			bounds.Height -= sideTabSize.Height;
		}

		if(bounds.Width  < 0) bounds.Width  = 0;
		if(bounds.Height < 0) bounds.Height = 0;

		return bounds;
	}

	/// <summary>Get bounding rectangle for docked view.</summary>
	/// <param name="viewHost">Tested <see cref="ViewHost"/>.</param>
	/// <param name="dockResult">Position for docking.</param>
	/// <returns>Bounding rectangle for docked view.</returns>
	public Rectangle GetDockBounds(ViewHost viewHost, DockResult dockResult)
	{
		Verify.Argument.IsNotNull(viewHost);

		var rootBounds = RootControl is not null
			? RootControl.Bounds
			: GetRootBounds(Dpi.FromControl(this));
		Rectangle bounds;
		switch(dockResult)
		{
			case DockResult.Left:
				{
					var w1 = viewHost.Width;
					var w2 = rootBounds.Width;
					ViewSplit.OptimalSizes(this, w2, ViewConstants.MinimumHostWidth, ref w1, ref w2);
					bounds = new Rectangle(0, 0, w1, rootBounds.Height);
				}
				break;
			case DockResult.Top:
				{
					var h1 = viewHost.Height;
					var h2 = rootBounds.Height;
					ViewSplit.OptimalSizes(this, h2, ViewConstants.MinimumHostHeight, ref h1, ref h2);
					bounds = new Rectangle(0, 0, rootBounds.Width, h1);
				}
				break;
			case DockResult.Right:
				{
					var w1 = rootBounds.Width;
					var w2 = viewHost.Width;
					ViewSplit.OptimalSizes(this, w1, ViewConstants.MinimumHostWidth, ref w1, ref w2);
					bounds = new Rectangle(rootBounds.Width - w2, 0, w2, rootBounds.Height);
				}
				break;
			case DockResult.Bottom:
				{
					var h1 = rootBounds.Height;
					var h2 = viewHost.Height;
					ViewSplit.OptimalSizes(this, h1, ViewConstants.MinimumHostHeight, ref h1, ref h2);
					bounds = new Rectangle(0, rootBounds.Height - h2, rootBounds.Width, h2);
				}
				break;
			default:
				throw new ArgumentException(
					$"Unsupported {nameof(DockResult)} value: {dockResult}",
					nameof(dockResult));
		}
		bounds.Offset(rootBounds.X, rootBounds.Y);
		return RectangleToScreen(bounds);
	}

	#endregion
}
