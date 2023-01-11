#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework.Services;

[System.ComponentModel.DesignerCategory("")]
[System.ComponentModel.ToolboxItem(defaultType: false)]
public abstract class DockMarker : Form
{
	private readonly DockMarkerButton[] _buttons;
	private readonly TrackingService<DockMarkerButton> _buttonHover;
	private readonly IDpiBoundValue<Point[]> _borderPolygon;
	private readonly IDockHost _dockHost;
	private readonly ViewHost _viewHost;
	private DockPositionMarker? _positionMarker;
	private bool _isHovered;

	protected DockMarker(IDockHost dockHost, ViewHost viewHost, DockMarkerButton[] buttons, IDpiBoundValue<Point[]> border, Rectangle bounds)
	{
		Verify.Argument.IsNotNull(dockHost);
		Verify.Argument.IsNotNull(viewHost);
		Verify.Argument.IsNotNull(buttons);
		Verify.Argument.IsNotNull(border);

		SetStyle(
			ControlStyles.ContainerControl |
			ControlStyles.Selectable |
			ControlStyles.ResizeRedraw |
			ControlStyles.SupportsTransparentBackColor,
			false);
		SetStyle(
			ControlStyles.UserPaint |
			ControlStyles.AllPaintingInWmPaint |
			ControlStyles.OptimizedDoubleBuffer,
			true);

		_dockHost         = dockHost;
		_viewHost         = viewHost;

		MinimumSize       = new Size(1, 1);
		StartPosition     = FormStartPosition.Manual;
		FormBorderStyle   = FormBorderStyle.None;
		ControlBox        = false;
		MaximizeBox       = false;
		MinimizeBox       = false;
		Text              = string.Empty;
		ShowIcon          = false;
		ShowInTaskbar     = false;
		Enabled           = false;
		ImeMode           = ImeMode.Disable;
		BackColor         = Renderer.DockMarkerBackgroundColor;
		Bounds            = bounds;
		AllowTransparency = true;
		Opacity           = ViewConstants.OpacityNormal;

		_borderPolygon    = border;
		_buttons          = buttons;
		_buttonHover      = new TrackingService<DockMarkerButton>(OnButtonHoverChanged);
	}

	private ViewRenderer Renderer => ViewManager.Renderer;

	/// <summary>Displays the control to the user.</summary>
	public new void Show()
	{
		var handle = Handle;

		const int SW_SHOWNA     = 8;

		_ = Native.User32.ShowWindow(handle, SW_SHOWNA);
		_ = Native.User32.RedrawWindow(handle, IntPtr.Zero, IntPtr.Zero, Native.RedrawWindowFlags.UpdateNow);
	}

	private void OnButtonHoverChanged(object? sender, TrackingEventArgs<DockMarkerButton> e)
	{
		Invalidate(e.Item!.Bounds);
		if(e.IsTracked)
		{
			var bounds = _dockHost.GetDockBounds(_viewHost, e.Item.Type);
			if(bounds is { Width: > 0, Height: > 0 })
			{
				SpawnDockPositionMarker(bounds);
			}
		}
		else
		{
			KillDockPositionMarker();
		}
	}

	/// <inheritdoc/>
	protected override void DefWndProc(ref Message m)
	{
		const int MA_NOACTIVATE = 0x0003;

		switch((Native.WM)m.Msg)
		{
			case Native.WM.MOUSEACTIVATE:
				m.Result = (IntPtr)MA_NOACTIVATE;
				return;
		}
		base.DefWndProc(ref m);
	}

	/// <inheritdoc/>
	protected override void OnShown(EventArgs e)
	{
		TopMost = true;
	}

	/// <inheritdoc/>
	protected override bool ShowWithoutActivation => true;

	/// <inheritdoc/>
	protected override CreateParams CreateParams
	{
		get
		{
			const int WS_EX_NOACTIVATE = 0x08000000;

			var baseParams = base.CreateParams;
			baseParams.ExStyle |= WS_EX_NOACTIVATE;
			return baseParams;
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintBackground(PaintEventArgs pevent)
	{
	}

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Assert.IsNotNull(e);

		var graphics = e.Graphics;
		using(var brush = SolidBrushCache.Get(Renderer.DockMarkerBackgroundColor))
		{
			graphics.FillRectangle(brush, e.ClipRectangle);
		}
		var control = (Control)_dockHost;
		using(var pen = new Pen(Renderer.DockMarkerBorderColor))
		{
			var dpi  = Dpi.FromControl(control);
			graphics.DrawPolygon(pen, _borderPolygon.GetValue(dpi));
		}
		for(int i = 0; i < _buttons.Length; ++i)
		{
			_buttons[i].OnPaint(control, graphics, !_isHovered || _buttonHover.Index == i);
		}
	}

	private int HitTestIndex(Point point)
	{
		for(int i = 0; i < _buttons.Length; ++i)
		{
			if(_buttons[i].Bounds.Contains(point))
			{
				return i;
			}
		}
		return -1;
	}

	public DockResult HitTest() => HitTest(Control.MousePosition);

	public DockResult HitTest(Point point)
	{
		point = PointToClient(point);
		for(int i = 0; i < _buttons.Length; ++i)
		{
			if(_buttons[i].Bounds.Contains(point))
				return _buttons[i].Type;
		}
		return DockResult.None;
	}

	public DockResult HitTest(int x, int y)
	{
		for(int i = 0; i < _buttons.Length; ++i)
		{
			if(_buttons[i].Bounds.Contains(x, y))
				return _buttons[i].Type;
		}
		return DockResult.None;
	}

	public bool UpdateHover(Point point)
	{
		point = PointToClient(point);
		var rgn = Region;
		if(ClientRectangle.Contains(point) && (rgn is null || rgn.IsVisible(point)))
		{
			if(!_isHovered)
			{
				Opacity = ViewConstants.OpacityHover;
				Invalidate();
				_isHovered = true;
			}
			var index = HitTestIndex(point);
			if(index == -1)
			{
				_buttonHover.Drop();
				return false;
			}
			else
			{
				_buttonHover.Track(index, _buttons[index]);
				return true;
			}
		}
		else
		{
			Unhover();
			return false;
		}
	}

	public void UpdateHover() => UpdateHover(Control.MousePosition);

	public void Unhover()
	{
		if(_isHovered)
		{
			Opacity = ViewConstants.OpacityNormal;
			Invalidate();
			_isHovered = false;
			_buttonHover.Drop();
		}
	}

	private void SpawnDockPositionMarker(Rectangle bounds)
	{
		_positionMarker = new DockPositionMarker(bounds);
		_positionMarker.Show();
	}

	private void UpdateDockPositionMarker(Rectangle bounds)
	{
		if(_positionMarker is null)
		{
			SpawnDockPositionMarker(bounds);
		}
		else
		{
			_positionMarker.Bounds = bounds;
		}
	}

	private void KillDockPositionMarker()
	{
		if(_positionMarker is not null)
		{
			_positionMarker.Dispose();
			_positionMarker = null;
		}
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			KillDockPositionMarker();
		}
		base.Dispose(disposing);
	}
}
