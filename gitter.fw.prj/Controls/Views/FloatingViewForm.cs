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

#nullable enable

namespace gitter.Framework.Controls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

/// <summary>Hosts floating views.</summary>
[System.ComponentModel.ToolboxItem(defaultType: false)]
[System.ComponentModel.DesignerCategory("")]
public class FloatingViewForm : Form
{
	static class Constants
	{
		public const int HTTRANSPARENT = -1;

		public const int HTCAPTION = 2;
		public const int HTLEFT = 10;
		public const int HTRIGHT = 11;
		public const int HTTOP = 12;
		public const int HTTOPLEFT = 13;
		public const int HTTOPRIGHT = 14;
		public const int HTBOTTOM = 15;
		public const int HTBOTTOMLEFT = 16;
		public const int HTBOTTOMRIGHT = 17;

		public const int GA_ROOT = 2;
	}

	const int SC_MOUSEMOVE = 0xF012;

	private bool _isForceClosing;

	private ViewHostDockingProcess? _dockingProcess;

	internal static Point GetLocationFor(Control control)
	{
		var loc        = control.PointToScreen(Point.Empty);
		var borderSize = ViewManager.Renderer.FloatBorderSize.GetValue(Dpi.FromControl(control));
		loc.X -= borderSize.Width;
		loc.Y -= borderSize.Height;
		return loc;
	}

	/// <summary>Initializes a new instance of the <see cref="FloatingViewForm"/> class.</summary>
	/// <param name="control">Control to display.</param>
	public FloatingViewForm(Control control)
	{
		Verify.Argument.IsNotNull(control);

		var dpi = Dpi.FromControl(this);

		var minSize = new Size(
			width:  ViewConstants.MinimumHostWidth.GetValue(dpi),
			height: ViewConstants.MinimumHostHeight.GetValue(dpi));

		var borderSize = Renderer.FloatBorderSize.GetValue(dpi);

		if(control is ViewSplit)
		{
			EnterMulticontrolMode();
		}

#if !NET5_0_OR_GREATER
		Font			= GitterApplication.FontManager.UIFont.Font;
#endif
		Text			= control.Text;
		FormBorderStyle	= FormBorderStyle.None;
		BackColor		= Renderer.BackgroundColor;
		StartPosition	= FormStartPosition.Manual;
		Padding			= new Padding(borderSize.Width, borderSize.Height, borderSize.Width, borderSize.Height);
		Bounds			= GetBoundsForControl(control, dpi);
		if(control.Width < minSize.Width)
		{
			if(control.Height < minSize.Height)
			{
				control.Size = minSize;
			}
			else
			{
				control.Width = minSize.Width;
			}
		}
		else if(control.Height < minSize.Height)
		{
			control.Height = minSize.Height;
		}
		MinimumSize		= new Size(
			minSize.Width  + borderSize.Width  * 2,
			minSize.Height + borderSize.Height * 2);
		ShowInTaskbar	= false;
		ShowIcon		= false;
		ControlBox      = false;
		MinimizeBox		= false;
		MaximizeBox		= true;
		RootControl	    = control;
		UpdateLayout();

		DockElements<FloatingViewForm>.Add(this);
	}

	private ViewRenderer Renderer => ViewManager.Renderer;

	/// <inheritdoc/>
	protected override void OnActivated(EventArgs e)
	{
		base.OnActivated(e);
		BackColor = Renderer.AccentColor;
	}

	/// <inheritdoc/>
	protected override void OnDeactivate(EventArgs e)
	{
		base.OnDeactivate(e);
		BackColor = Renderer.BackgroundColor;
	}

	private Rectangle GetBoundsForControl(Control control, Dpi dpi)
	{
		Assert.IsNotNull(control);

		var loc        = control.PointToScreen(Point.Empty);
		var size       = control.Size;
		var borderSize = Renderer.FloatBorderSize.GetValue(dpi);
		loc.X -= borderSize.Width;
		loc.Y -= borderSize.Height;
		size.Width  += borderSize.Width  * 2;
		size.Height += borderSize.Height * 2;
		if(IsInMulticontrolMode)
		{
			var dy = Renderer.FloatTitleHeight.GetValue(dpi);
			loc.Y       -= dy;
			size.Height += dy;
		}
		return new Rectangle(loc, size);
	}

	internal bool IsInMulticontrolMode { get; private set; }

	internal void EnterMulticontrolMode()
	{
		Verify.State.IsFalse(IsInMulticontrolMode);

		var dpi = Dpi.FromControl(this);
		SuspendLayout();
		if(WindowState == FormWindowState.Maximized)
		{
			Padding = new Padding(
				0,
				0 + Renderer.FloatTitleHeight.GetValue(dpi),
				0,
				0);
		}
		else
		{
			var borderSize = Renderer.FloatBorderSize.GetValue(dpi);
			Padding = new Padding(
				borderSize.Width,
				borderSize.Height + Renderer.FloatTitleHeight.GetValue(dpi),
				borderSize.Width,
				borderSize.Height);
		}
		IsInMulticontrolMode = true;
		ResumeLayout(performLayout: true);
	}

	internal void LeaveMulticontrolMode()
	{
		Verify.State.IsTrue(IsInMulticontrolMode);

		var dpi = Dpi.FromControl(this);
		if(WindowState == FormWindowState.Maximized)
		{
			Padding = new Padding(
				0,
				0,
				0,
				0);
		}
		else
		{
			var borderSize = Renderer.FloatBorderSize.GetValue(dpi);
			Padding = new Padding(
				borderSize.Width,
				borderSize.Height,
				borderSize.Width,
				borderSize.Height);
		}
		IsInMulticontrolMode = false;
	}

	/// <summary>Returns the root control of this <see cref="FloatingViewForm"/>.</summary>
	/// <value>Root control of this <see cref="FloatingViewForm"/>.</value>
	public Control? RootControl { get; internal set; }

	/// <inheritdoc/>
	protected override CreateParams CreateParams
	{
		get
		{
			const int CS_DROPSHADOW    = 0x00020000;
			const int WS_EX_TOOLWINDOW = 0x00000080;

			var cp = base.CreateParams;
			//cp.Style |= WS_SIZEBOX;
			cp.ClassStyle |= CS_DROPSHADOW;
			cp.ExStyle    |= WS_EX_TOOLWINDOW;
			return cp;
		}
	}

	/// <summary>Returns a sequence of all contained view hosts.</summary>
	/// <returns>Sequence of all contained view hosts.</returns>
	public IEnumerable<ViewHost> GetViewHosts()
		=> RootControl is not null
			? new ViewHostsSequence(RootControl)
			: Enumerable.Empty<ViewHost>();

	internal void UpdateMaximizeBounds()
	{
		var bounds = Screen.FromControl(this).WorkingArea;
		bounds.X = 0;
		bounds.Y = 0;
		MaximizedBounds = bounds;
	}

	/// <inheritdoc/>
	protected override void WndProc(ref Message m)
	{
		bool processed = false;
		switch((Native.WM)m.Msg)
		{
			case Native.WM.NCHITTEST:
				processed = OnNcHitTest(ref m);
				break;
			case Native.WM.SYSCOMMAND when m.WParam == (IntPtr)SC_MOUSEMOVE:
				if(!IsInMulticontrolMode && RootControl is ViewHost viewHost)
				{
					if(_dockingProcess is null)
					{
						_dockingProcess = new(viewHost);
					}
					else if(_dockingProcess.IsActive)
					{
						_dockingProcess.Cancel();
					}
					_dockingProcess.Start(Point.Empty);
				}
				break;
			case Native.WM.MOVE:
				if(_dockingProcess is not null)
				{
					_dockingProcess.Update(Point.Empty);
				}
				break;
			case Native.WM.EXITSIZEMOVE:
				if(_dockingProcess is not null)
				{
					_dockingProcess.Commit(Point.Empty);
					_dockingProcess.Dispose();
					_dockingProcess = null;
				}
				else if(!IsDisposed)
				{
					UpdateMaximizeBounds();
				}
				break;
		}
		if(!processed)
		{
			base.WndProc(ref m);
		}
	}

	/// <summary>Starts window move process.</summary>
	public void DragMove()
	{
		if(!IsHandleCreated) return;
		_ = Native.User32.SendMessage(Handle, Native.WM.SYSCOMMAND, (IntPtr)SC_MOUSEMOVE, IntPtr.Zero);
		if(!IsHandleCreated) return;
		_ = Native.User32.SendMessage(Handle, Native.WM.LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
	}

	private Point _originalMousePosition;

	/// <summary>Start window move process after it was displayed.</summary>
	public void DragOnShown()
	{
		_originalMousePosition = Control.MousePosition;
		VisibleChanged += OnVisibleChanged;
	}

	private void OnVisibleChanged(object? sender, EventArgs e)
	{
		if(sender is not FloatingViewForm { Visible: true } f) return;
		f.VisibleChanged -= OnVisibleChanged;
		f.BeginInvoke(() =>
		{
			var p1 = Control.MousePosition;
			var dx = p1.X - _originalMousePosition.X;
			var dy = p1.Y - _originalMousePosition.Y;
			if(dx != 0 || dy != 0)
			{
				var location = f.Location;
				location.X += dx;
				location.Y += dy;
				f.Location = location;
			}
			f.RootControl?.Focus();
			f.DragMove();
		});
	}

	private bool OnNcHitTest(ref Message m)
	{
		int x = (short)Native.Macro.LOWORD(m.LParam);
		int y = (short)Native.Macro.HIWORD(m.LParam);

		var point = PointToClient(new Point(x, y));
		var rc = ClientRectangle;
		bool isMaximized = WindowState == FormWindowState.Maximized;

		if(IsInMulticontrolMode)
		{
			var dpi = Dpi.FromControl(this);
			if(isMaximized)
			{
				if((new Rectangle(
					rc.X, rc.Y,
					rc.Width, Renderer.FloatTitleHeight.GetValue(dpi))).Contains(point))
				{
					m.Result = (IntPtr)Constants.HTCAPTION;
					return true;
				}
			}
			else
			{
				var borderSize = Renderer.FloatBorderSize.GetValue(dpi);
				if((new Rectangle(
					rc.X + borderSize.Width,
					rc.Y + borderSize.Height,
					rc.Width - borderSize.Width * 2,
					Renderer.FloatTitleHeight.GetValue(dpi))).Contains(point))
				{
					m.Result = (IntPtr)Constants.HTCAPTION;
					return true;
				}
			}
		}

		if(isMaximized) return false;

		var grip = new GripBounds(rc);
		if(grip.TopLeft.Contains(point))
		{
			m.Result = (IntPtr)Constants.HTTOPLEFT;
			return true;
		}
		if(grip.TopRight.Contains(point))
		{
			m.Result = (IntPtr)Constants.HTTOPRIGHT;
			return true;
		}
		if(grip.Top.Contains(point))
		{
			m.Result = (IntPtr)Constants.HTTOP;
			return true;
		}
		if(grip.BottomLeft.Contains(point))
		{
			m.Result = (IntPtr)Constants.HTBOTTOMLEFT;
			return true;
		}
		if(grip.BottomRight.Contains(point))
		{
			m.Result = (IntPtr)Constants.HTBOTTOMRIGHT;
			return true;
		}
		if(grip.Bottom.Contains(point))
		{
			m.Result = (IntPtr)Constants.HTBOTTOM;
			return true;
		}
		if(grip.Left.Contains(point))
		{
			m.Result = (IntPtr)Constants.HTLEFT;
			return true;
		}
		if(grip.Right.Contains(point))
		{
			m.Result = (IntPtr)Constants.HTRIGHT;
			return true;
		}
		return false;
	}

	/// <inheritdoc/>
	protected override void OnStyleChanged(EventArgs e)
	{
		MaximizedBounds = Screen.GetWorkingArea(this);
		base.OnStyleChanged(e);
	}

	private Rectangle GetRootControlBounds(Dpi dpi)
	{
		var rc = ClientRectangle;
		if(IsInMulticontrolMode)
		{
			var dy = Renderer.FloatTitleHeight.GetValue(dpi);
			rc.Y      += dy;
			rc.Height -= dy;
		}
		if(WindowState != FormWindowState.Maximized)
		{
			var borderSize = Renderer.FloatBorderSize.GetValue(dpi);
			rc.X += borderSize.Width;
			rc.Y += borderSize.Height;
			rc.Width  -= borderSize.Width  * 2;
			rc.Height -= borderSize.Height * 2;
		}
		return rc;
	}

	internal void UpdateLayout()
	{
		var dpi = Dpi.FromControl(this);
		if(RootControl is not null)
		{
			RootControl.Bounds = GetRootControlBounds(dpi);
		}
		if(WindowState != FormWindowState.Maximized)
		{
			if(Renderer.FloatCornerRadius is not null)
			{
				var cornderRadius = Renderer.FloatCornerRadius.GetValue(dpi);
				if(cornderRadius is { Width: > 0, Height: > 0 })
				{
					Region = GraphicsUtility.GetRoundedRegion(ClientRectangle, cornderRadius.Width);
				}
			}
		}
	}

#if NETCOREAPP || NET48_OR_GREATER
	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	protected override void OnDpiChangedAfterParent(EventArgs e)
	{
		UpdateLayout();
		base.OnDpiChangedAfterParent(e);
	}
#endif

	/// <inheritdoc/>
	protected override void OnResize(EventArgs e)
	{
		UpdateLayout();
		base.OnResize(e);
	}

	internal void ForceClose()
	{
		_isForceClosing = true;
		Close();
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			DockElements<FloatingViewForm>.Remove(this);
			_dockingProcess?.Dispose();
			RootControl = null;
		}
		base.Dispose(disposing);
	}
}
