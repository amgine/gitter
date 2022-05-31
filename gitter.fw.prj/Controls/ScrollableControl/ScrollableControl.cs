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

namespace gitter.Framework.Controls;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Native;

/// <summary>User-drawn control which supports content scrolling in both directions.</summary>
[DesignerCategory("")]
public class ScrollableControl : Control
{
	#region Data

	private BorderStyle _borderStyle;

	private IGitterStyle _style;
	private IScrollBarWidget _vScrollBar;
	private IScrollBarWidget _hScrollBar;
	private bool _blockScrollRedraw;
	private int _vScrollPos;
	private int _hScrollPos;
	private int _maxVScrollPos;
	private int _maxHScrollPos;
	private int _vScrollDirection;
	private int _hScrollDirection;
	private Timer _scrollTimer;
	private bool _alwaysShowVScrollBar;
	private bool _alwaysShowHScrollBar;
	private bool _needVScroll;
	private bool _needHScroll;
	private bool _isMouseOver;
	private int _updateCount;

	/// <summary>Area for content rendering and scrolling.</summary>
	private Rectangle _contentArea;
	/// <summary>Client area (excludes borders).</summary>
	private Rectangle _clientArea;
	private Size _contentSize;

	#endregion

	#region Events

	private static readonly object StyleChangedEvent = new();

	public new event EventHandler StyleChanged
	{
		add    => Events.AddHandler    (StyleChangedEvent, value);
		remove => Events.RemoveHandler (StyleChangedEvent, value);
	}

	protected new virtual void OnStyleChanged(EventArgs e)
		=> ((EventHandler)Events[StyleChangedEvent])?.Invoke(this, e);

	#endregion

	#region .ctor

	/// <summary>Create <see cref="ScrollableControl"/>.</summary>
	public ScrollableControl()
	{
		_borderStyle = BorderStyle.Fixed3D;

		if(LicenseManager.UsageMode == LicenseUsageMode.Designtime)
		{
			_style = new MSVS2010Style();
		}

		SetStyle(
			ControlStyles.ContainerControl |
			ControlStyles.ResizeRedraw |
			ControlStyles.SupportsTransparentBackColor, false);
		SetStyle(
			ControlStyles.UserPaint |
			ControlStyles.AllPaintingInWmPaint |
			ControlStyles.Selectable |
			ControlStyles.OptimizedDoubleBuffer, true);

		CreateScrollBars();
		BackColor = SystemColors.Window;
	}

	#endregion

	#region Properties

	/// <summary>Gets or sets control style.</summary>
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IGitterStyle Style
	{
		get => _style ?? GitterApplication.Style;
		set
		{
			if(_style != value)
			{
				_style = value;
				CreateScrollBars();
				OnStyleChanged(EventArgs.Empty);
				Invalidate();
			}
		}
	}

	/// <summary>Control border style.</summary>
	[DefaultValue(BorderStyle.Fixed3D)]
	[Description("Control border style.")]
	public BorderStyle BorderStyle
	{
		get => _borderStyle;
		set
		{
			if(_borderStyle != value)
			{
				_borderStyle = value;
				RecomputeAreas();
				SetScrollBars(false);
				Invalidate();
			}
		}
	}

	/// <summary>Control background color.</summary>
	[DefaultValue(typeof(Color), "Window")]
	public override Color BackColor
	{
		get => base.BackColor;
		set => base.BackColor = value;
	}

	/// <summary>Always show vertical scroll bar.</summary>
	[DefaultValue(false)]
	[Description("Always show vertical scroll bar.")]
	public bool AlwaysShowVScrollBar
	{
		get => _alwaysShowVScrollBar;
		set
		{
			if(value != _alwaysShowVScrollBar)
			{
				_alwaysShowVScrollBar = value;
				RecomputeAreas();
				SetScrollBars(false);
				Invalidate();
			}
		}
	}

	/// <summary>Vertical scroll position.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int VScrollPos
	{
		get => _vScrollPos;
		set => DoVScroll(value);
	}

	/// <summary>Maximum vertical scroll position.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int MaxVScrollPos => _maxVScrollPos;

	/// <summary>Horizontal scroll position.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int HScrollPos
	{
		get => _hScrollPos;
		set => DoHScroll(value);
	}

	/// <summary>Maximum horizontal scroll position.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int MaxHScrollPos => _maxHScrollPos;

	/// <summary>Always show horizontal scroll bar.</summary>
	[DefaultValue(false)]
	[Description("Always show horizontal scroll bar.")]
	public bool AlwaysShowHScrollBar
	{
		get => _alwaysShowHScrollBar;
		set
		{
			if(value != _alwaysShowHScrollBar)
			{
				_alwaysShowHScrollBar = value;
				RecomputeAreas();
				SetScrollBars(false);
				Invalidate();
			}
		}
	}

	/// <summary>Horizontal scrollbar.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IScrollBarWidget HScrollBar => _hScrollBar;

	/// <summary>Vertical scrollbar.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IScrollBarWidget VScrollBar => _vScrollBar;

	/// <summary>Client area.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Rectangle ClientArea => _clientArea;

	/// <summary>Content area.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Rectangle ContentArea => _contentArea;

	/// <summary>Part of content which is visible.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Rectangle ContentWindow
		=> new Rectangle(_hScrollPos, _vScrollPos, _contentArea.Width, _contentArea.Height);

	/// <summary>Content size.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Size ContentSize => _contentSize;

	#endregion

	private int BorderSize => _borderStyle == BorderStyle.None ? 0 : 1;

	private IScrollBarWidget CreateScrollBar(Orientation orientation)
		=> LicenseManager.UsageMode == LicenseUsageMode.Runtime
			? Style.CreateScrollBar(orientation)
			: new SystemScrollBarAdapter(orientation);

	protected override bool ScaleChildren => false;

	private int _vscrollWidth;
	private int _hscrollHeight;

	private void UpdateScrollBarBounds()
	{
		_hscrollHeight = SystemInformation.GetHorizontalScrollBarHeightForDpi(DeviceDpi);
		_vscrollWidth  = SystemInformation.GetVerticalScrollBarWidthForDpi(DeviceDpi);

		if(_vScrollBar is not null)
		{
			_vScrollBar.Control.Bounds = GetVScrollBounds();
		}
		if(_hScrollBar is not null)
		{
			_hScrollBar.Control.Bounds = GetHScrollBounds();
		}
	}

	/// <inheritdoc/>
	protected override void OnDpiChangedAfterParent(EventArgs e)
	{
		UpdateScrollBarBounds();
		RecomputeAreas();
		base.OnDpiChangedAfterParent(e);
	}

	private void CreateScrollBars()
	{
		_hscrollHeight = SystemInformation.GetHorizontalScrollBarHeightForDpi(DeviceDpi);
		_vscrollWidth  = SystemInformation.GetVerticalScrollBarWidthForDpi(DeviceDpi);

		_vScrollBar?.Dispose();
		_hScrollBar?.Dispose();

		_vScrollBar = CreateScrollBar(Orientation.Vertical);
		_vScrollBar.Maximum = 1;
		_vScrollBar.Minimum = 0;
		_vScrollBar.SmallChange = 1;
		_vScrollBar.LargeChange = 1;
		_vScrollBar.Control.Enabled = false;
		_vScrollBar.Control.Bounds = GetVScrollBounds();
		_vScrollBar.ValueChanged += OnVScrollBarValueChanged;
		_vScrollBar.Scroll += OnVScrollBarScroll;
		_vScrollBar.Control.MouseCaptureChanged += OnScrollBarCaptureChanged;
		_vScrollBar.Control.MouseDown += OnScrollBarMouseDown;

		_hScrollBar = CreateScrollBar(Orientation.Horizontal);
		_hScrollBar.Maximum = 1;
		_hScrollBar.Minimum = 0;
		_hScrollBar.SmallChange = 1;
		_hScrollBar.LargeChange = 1;
		_hScrollBar.Control.Enabled = false;
		_hScrollBar.Control.Bounds = GetHScrollBounds();
		_hScrollBar.ValueChanged += OnHScrollBarValueChanged;
		_hScrollBar.Scroll += OnHScrollBarScroll;
		_hScrollBar.Control.MouseCaptureChanged += OnScrollBarCaptureChanged;
		_hScrollBar.Control.MouseDown += OnScrollBarMouseDown;
	}

	public void ScrollToTop()
	{
		if(_vScrollPos != 0)
		{
			_vScrollBar.Value = 0;
		}
	}

	public void ScrollToBottom()
	{
		if(_vScrollPos != _maxVScrollPos)
		{
			_vScrollBar.Value = _maxVScrollPos;
		}
	}

	public void ScrollToTopLeft()
	{
		if(_vScrollPos != 0)
		{
			_vScrollBar.Value = 0;
		}
		if(_hScrollPos != 0)
		{
			_hScrollBar.Value = 0;
		}
	}

	public void ScrollToBottomLeft()
	{
		if(_vScrollPos != _maxVScrollPos)
		{
			_vScrollBar.Value = _maxVScrollPos;
		}
		if(_hScrollPos != 0)
		{
			_hScrollBar.Value = 0;
		}
	}

	public void ScrollUp() => ScrollItems(1);

	public void ScrollDown() => ScrollItems(-1);

	/// <summary>Disable control redraw events.</summary>
	public void BeginUpdate()
	{
		++_updateCount;
		if(_updateCount == 1)
		{
			this.DisableRedraw();
		}
	}

	/// <summary>Enable control redraw events.</summary>
	/// <param name="redraw">Repaint control.</param>
	public virtual void EndUpdate(bool redraw = true)
	{
		Verify.State.IsTrue(_updateCount > 0);

		--_updateCount;
		if(_updateCount == 0)
		{
			this.EnableRedraw();
			if(redraw) this.RedrawWindow();
		}
	}

	public bool IsUpdating => _updateCount != 0;

	protected int UpdateCounter => _updateCount;

	protected bool IsMouseOver => _isMouseOver;

	private Rectangle GetClientArea()
		=> GetClientArea(_vScrollBar.Control.Parent != null, _hScrollBar.Control.Parent != null);

	private Rectangle GetClientArea(bool vScrollBar, bool hScrollBar)
	{
		var client = ClientRectangle;
		int scrollbarOffset = 0;
		if(_borderStyle != BorderStyle.None)
		{
			client.Inflate(-2, -2);
		}
		else
		{
			client.Inflate(-1, -1);
			scrollbarOffset = 1;
		}
		if(vScrollBar)
		{
			client.Width -= _vScrollBar.Control.Width - scrollbarOffset;
		}
		if(hScrollBar)
		{
			client.Height -= _hScrollBar.Control.Height - scrollbarOffset;
		}
		return client;
	}

	protected void NotifyContentSizeChanged()
	{
		if(Created)
		{
			RecomputeAreas();
			SetScrollBars(false);
			Invalidate(ClientArea);
		}
	}

	protected void RecomputeAreas()
	{
		var clientArea = GetClientArea(_alwaysShowVScrollBar, _alwaysShowHScrollBar);
		var contentArea = GetContentArea(clientArea);
		var contentSize = MeasureContent(contentArea);
		if(contentSize.Height > contentArea.Height)
		{
			// vertical scrollbar is needed
			_needVScroll = true;
			// check horizontal scrollbar
			if(contentSize.Width > contentArea.Width)
			{
				// horizontal scrollbar is needed
				if(!_alwaysShowVScrollBar && !_alwaysShowHScrollBar)
				{
					// give control a chance to refit content and prevent horizontal scrollbar display,
					// if both scrollbars are not forced.
					clientArea = GetClientArea(true, false);
					contentArea = GetContentArea(clientArea);
					contentSize = MeasureContent(contentArea);
					if(contentSize.Width > contentArea.Width)
					{
						clientArea = GetClientArea(true, true);
						contentArea = GetContentArea(clientArea);
						_needHScroll = true;
					}
				}
				else
				{
					// horizontal scrollbar is forced
					clientArea = GetClientArea(true, true);
					contentArea = GetContentArea(clientArea);
					_needHScroll = true;
				}
			}
			else
			{
				// horizontal scrollbar is not needed, though it may still be enabled
				if(!_alwaysShowVScrollBar)
				{
					clientArea = GetClientArea(true, _alwaysShowHScrollBar);
					contentArea = GetContentArea(clientArea);
					contentSize = MeasureContent(contentArea);
					if(contentSize.Width > contentArea.Width)
					{
						clientArea = GetClientArea(true, true);
						contentArea = GetContentArea(clientArea);
						_needHScroll = true;
					}
					else
					{
						_needHScroll = false;
					}
				}
				else
				{
					_needHScroll = false;
				}
			}
		}
		else
		{
			// vertical scrollbar is not needed
			if(contentSize.Width > contentArea.Width)
			{
				// horizontal scroll is needed
				if(!_alwaysShowHScrollBar)
				{
					clientArea = GetClientArea(_alwaysShowVScrollBar, true);
					contentArea = GetContentArea(clientArea);
					if(contentSize.Height > contentArea.Height)
					{
						contentSize = MeasureContent(contentArea);
					}
				}
				if(contentSize.Height > contentArea.Height)
				{
					// vscroll is needed because of horizontal scrollbar
					clientArea = GetClientArea(true, true);
					contentArea = GetContentArea(clientArea);
					_needVScroll = true;
				}
				else
				{
					_needVScroll = false;
				}
				_needHScroll = true;
			}
			else
			{
				// content fits in the area
				_needHScroll = false;
				_needVScroll = false;
			}
		}
		_contentSize = contentSize;
		if(_contentArea != contentArea)
		{
			_clientArea = clientArea;
			_contentArea = contentArea;
			OnContentAreaChanged();
		}
	}

	protected virtual void UpdateHover(int x, int y)
	{
	}

	protected virtual void OnContentAreaChanged()
	{
	}

	protected virtual int GetVScrollSmallChange() => 10;

	protected virtual int GetHScrollSmallChange() => 10;

	protected bool SetScrollBars(bool allowRedraw = true)
		=> SetScrollBars(MeasureContent(_contentArea), allowRedraw);

	protected bool SetScrollBars(Size contentSize, bool allowRedraw)
	{
		var r1 = SetVScrollBar(contentSize, allowRedraw);
		var r2 = SetHScrollBar(contentSize, allowRedraw);
		return r1 || r2;
	}

	protected bool SetVScrollBar(bool allowRedraw)
	{
		var contentSize = MeasureContent(_contentArea);
		return SetVScrollBar(contentSize, allowRedraw);
	}

	private Rectangle GetVScrollBounds()
	{
		int borderSize = BorderSize;
		var size       = Size;
		if(_needHScroll || _alwaysShowHScrollBar)
		{
			size.Height -= _hscrollHeight;
		}
		return new Rectangle(
			size.Width - _vscrollWidth - borderSize, borderSize,
			_vscrollWidth, size.Height - borderSize * 2);
	}

	private Rectangle GetHScrollBounds()
	{
		int borderSize = BorderSize;
		var size       = Size;
		if(_needVScroll || _alwaysShowVScrollBar)
		{
			size.Width -= _vscrollWidth;
		}
		return new Rectangle(
			borderSize, size.Height - _hscrollHeight - borderSize,
			size.Width - borderSize * 2, _hscrollHeight);
	}

	protected bool SetVScrollBar(Size contentSize, bool allowRedraw)
	{
		var msp = contentSize.Height - _contentArea.Height;
		bool res = false;
		if(msp > 0)
		{
			msp = TransformMaxVScrollPos(msp);
			_maxVScrollPos = msp;
			var lc = _contentArea.Height;
			if(lc < 0) lc = 0;
			if(_vScrollBar.Control.Parent is null)
			{
				_vScrollBar.Control.Bounds = GetVScrollBounds();
				_vScrollBar.Control.Parent = this;
				RecomputeAreas();
				if(allowRedraw)
				{
					Invalidate();
					res = true;
				}
			}
			else
			{
				_vScrollBar.Control.Bounds = GetVScrollBounds();
			}
			_vScrollBar.Control.Enabled = true;
			_vScrollBar.SmallChange = GetVScrollSmallChange();
			_vScrollBar.LargeChange = lc;
			_vScrollBar.Maximum = _maxVScrollPos + lc - 1;
			if(_vScrollBar.Value > _maxVScrollPos)
			{
				_blockScrollRedraw = !allowRedraw;
				_vScrollBar.Value = _maxVScrollPos;
				_blockScrollRedraw = false;
			}
		}
		else
		{
			if((_vScrollBar.Control.Parent == null && _alwaysShowVScrollBar) || (_vScrollBar.Control.Parent != null && !_alwaysShowVScrollBar))
			{
				_vScrollPos = 0;
				if(_alwaysShowVScrollBar)
				{
					_vScrollBar.Control.Bounds = GetVScrollBounds();
					_vScrollBar.Control.Parent = this;
				}
				else
				{
					_vScrollBar.Control.Parent = null;
				}
				RecomputeAreas();
				if(allowRedraw)
				{
					Invalidate();
					res = true;
				}
				_maxVScrollPos = 0;
				_vScrollBar.Value = 0;
				_vScrollBar.Control.Enabled = false;
			}
			else
			{
				var b = GetVScrollBounds();
				_vScrollBar.Control.Bounds = b;
				if(_maxVScrollPos != 0)
				{
					_maxVScrollPos = 0;
					_blockScrollRedraw = !allowRedraw;
					_vScrollBar.Value = 0;
					_blockScrollRedraw = false;
					_vScrollBar.Control.Enabled = false;
				}
			}
		}
		return res;
	}

	protected bool SetHScrollBar(bool allowRedraw)
	{
		var contentSize = MeasureContent(_contentArea);
		return SetHScrollBar(contentSize, allowRedraw);
	}

	protected bool SetHScrollBar(Size contentSize, bool allowRedraw)
	{
		var msp = contentSize.Width - _contentArea.Width;
		bool res = false;
		if(msp > 0)
		{
			msp = TransformMaxHScrollPos(msp);
			_maxHScrollPos = msp;
			var lc = _contentArea.Width;
			if(lc < 0) lc = 0;
			if(_hScrollBar.Control.Parent is null)
			{
				_hScrollBar.Control.Bounds = GetHScrollBounds();
				_hScrollBar.Control.Parent = this;
				RecomputeAreas();
				if(allowRedraw)
				{
					Invalidate();
					res = true;
				}
			}
			else
			{
				_hScrollBar.Control.Bounds = GetHScrollBounds();
			}
			_hScrollBar.Control.Enabled = true;
			_hScrollBar.SmallChange = GetHScrollSmallChange();
			_hScrollBar.LargeChange = lc;
			_hScrollBar.Maximum = _maxHScrollPos + lc - 1;
			if(_hScrollBar.Value > _maxHScrollPos)
			{
				_blockScrollRedraw = !allowRedraw;
				_hScrollBar.Value = _maxHScrollPos;
				_blockScrollRedraw = false;
			}
		}
		else
		{
			if((_hScrollBar.Control.Parent is null && _alwaysShowHScrollBar) || (_hScrollBar.Control.Parent is not null && !_alwaysShowHScrollBar))
			{
				_hScrollPos = 0;
				if(_alwaysShowHScrollBar)
				{
					_hScrollBar.Control.Bounds = GetHScrollBounds();
					_hScrollBar.Control.Parent = this;
				}
				else
				{
					_hScrollBar.Control.Parent = null;
				}
				RecomputeAreas();
				if(allowRedraw)
				{
					Invalidate();
					res = true;
				}
				_maxHScrollPos = 0;
				_hScrollBar.Value = 0;
				_hScrollBar.Control.Enabled = false;
			}
			else
			{
				_hScrollBar.Control.Bounds = GetHScrollBounds();
				if(_maxHScrollPos != 0)
				{
					_maxHScrollPos = 0;
					_blockScrollRedraw = !allowRedraw;
					_hScrollBar.Value = 0;
					_blockScrollRedraw = false;
					_hScrollBar.Control.Enabled = false;
				}
			}
		}
		return res;
	}

	protected virtual Rectangle GetContentArea(Rectangle clientArea) => clientArea;

	protected virtual bool HScrollAffectsClientArea => true;

	protected virtual bool VScrollAffectsClientArea => false;

	protected virtual Size MeasureContent() => Size.Empty;

	protected virtual Size MeasureContent(Rectangle contentArea) => MeasureContent();

	protected virtual int TransformVScrollPos(int position) => position;

	protected virtual int TransformHScrollPos(int position) => position;

	protected virtual int TransformMaxVScrollPos(int position) => position;

	protected virtual int TransformMaxHScrollPos(int position) => position;

	protected virtual void OnVScroll(int dy)
	{
	}

	protected virtual void OnHScroll(int dx)
	{
	}

	private void ScrollY(int dy)
	{
		var hwnd = Handle;
		var scroll = VScrollAffectsClientArea ?
			new RECT(_clientArea) :
			new RECT(_contentArea);
		User32.ScrollWindowEx(
			hwnd,
			0, dy,
			ref scroll, ref scroll,
			IntPtr.Zero, IntPtr.Zero,
			0x0002);
	}

	private void ScrollX(int dx)
	{
		var hwnd = Handle;
		var scroll = HScrollAffectsClientArea ?
			new RECT(_clientArea) :
			new RECT(_contentArea);
		User32.ScrollWindowEx(
			hwnd,
			dx, 0,
			ref scroll, ref scroll,
			IntPtr.Zero, IntPtr.Zero,
			0x0002);
	}

	private static int ClampScrollPosition(int position, int maxPosition)
	{
		if(position < 0) return 0;
		if(position > maxPosition) return maxPosition;
		return position;
	}

	protected void DoVScroll(int newPosition)
	{
		newPosition = ClampScrollPosition(newPosition, _maxVScrollPos);
		newPosition = TransformVScrollPos(newPosition);
		if(newPosition != _vScrollPos)
		{
			if(!_blockScrollRedraw)
			{
				Update();
			}

			int dy = _vScrollPos - newPosition;
			_vScrollPos = newPosition;

			OnVScroll(dy);

			if(!_blockScrollRedraw)
			{
				ScrollY(dy);
			}
		}
	}

	private void DoHScroll(int newPosition)
	{
		newPosition = ClampScrollPosition(newPosition, _maxHScrollPos);
		newPosition = TransformHScrollPos(newPosition);
		if(newPosition != _hScrollPos)
		{
			if(!_blockScrollRedraw)
			{
				Update();
			}

			int dx = _hScrollPos - newPosition;
			_hScrollPos = newPosition;

			OnHScroll(dx);

			if(!_blockScrollRedraw)
			{
				ScrollX(dx);
			}
		}
	}

	protected void StartScrollTimer(int vDirection)
	{
		StartScrollTimer(vDirection, 0);
	}

	protected void StartScrollTimer(int vDirection, int hDirection)
	{
		_vScrollDirection = vDirection;
		_hScrollDirection = hDirection;
		if(_scrollTimer == null)
		{
			_scrollTimer = new Timer()
			{
				Interval = 150,
			};
			_scrollTimer.Tick += OnScrollTimerTick;
			_scrollTimer.Enabled = true;
		}
	}

	protected void StopScrollTimer()
	{
		if(_scrollTimer != null)
		{
			_scrollTimer.Tick -= OnScrollTimerTick;
			_scrollTimer.Enabled = false;
			_scrollTimer.Dispose();
			_scrollTimer = null;
		}
	}

	#region Event Handlers

	private void OnVScrollBarScroll(object sender, ScrollEventArgs e)
	{
		DoVScroll(e.NewValue);
	}

	private void OnVScrollBarValueChanged(object sender, EventArgs e)
	{
		DoVScroll(_vScrollBar.Value);
	}

	private void OnHScrollBarScroll(object sender, ScrollEventArgs e)
	{
		DoHScroll(e.NewValue);
	}

	private void OnHScrollBarValueChanged(object sender, EventArgs e)
	{
		DoHScroll(_hScrollBar.Value);
	}

	private void OnScrollBarCaptureChanged(object sender, EventArgs e)
	{
		var control = (Control)sender;
		if(control.Capture) Focus();
	}

	private void OnScrollBarMouseDown(object sender, MouseEventArgs e)
	{
		Focus();
	}

	private void OnScrollTimerTick(object sender, EventArgs e)
	{
		if(_vScrollDirection > 0)
		{
			int d = _vScrollDirection;
			if(_vScrollPos + d >= _maxVScrollPos)
			{
				_vScrollBar.Value = _maxVScrollPos;
				_vScrollDirection = 0;
			}
			else
			{
				_vScrollBar.Value += d;
			}
		}
		else if(_vScrollDirection < 0)
		{
			int d = _vScrollDirection;
			if(_vScrollPos + d <= 0)
			{
				_vScrollBar.Value = 0;
				_vScrollDirection = 0;
			}
			else
			{
				_vScrollBar.Value += d;
			}
		}
		if(_hScrollDirection > 0)
		{
			int d = _hScrollDirection;
			if(_hScrollPos + d >= _maxHScrollPos)
			{
				_hScrollBar.Value = _maxHScrollPos;
				_hScrollDirection = 0;
			}
			else
			{
				_vScrollBar.Value += d;
			}
		}
		else if(_hScrollDirection < 0)
		{
			int d = _hScrollDirection;
			if(_hScrollPos + d <= 0)
			{
				_hScrollBar.Value = 0;
				_hScrollDirection = 0;
			}
			else
			{
				_hScrollBar.Value += d;
			}
		}
		if(_hScrollDirection == 0 && _vScrollDirection == 0)
		{
			StopScrollTimer();
		}
	}

	#endregion

	#region Overrides

	/// <inheritdoc/>
	protected sealed override void OnPaintBackground(PaintEventArgs pevent)
	{
	}

	private void PaintNonClient(Gdi gdi, Rectangle clip)
	{
		int h = Height;
		int w = Width;
		if(_borderStyle == BorderStyle.None)
		{
			if(clip.X <= 0 || clip.Y <= 0 || clip.Right >= w || clip.Bottom >= h)
			{
				gdi.Rectangle(BackColor, new Rectangle(0, 0, w, h));
			}
		}
		else
		{
			if((_borderStyle == BorderStyle.FixedSingle) || Application.RenderWithVisualStyles)
			{
				if(clip.X <= 0 || clip.Y <= 0 || clip.Right >= w || clip.Height >= h)
				{
					gdi.Rectangle(SystemColors.ControlDark, new Rectangle(0, 0, w, h));
					gdi.Rectangle(BackColor, new Rectangle(1, 1, w - 2, h - 2));
				}
			}
			else
			{
				if(clip.X <= 0 || clip.Y <= 0 || clip.Right >= w || clip.Height >= h)
				{
					gdi.Line(SystemColors.ControlDark, 0, 1, 0, h - 2);
					gdi.Line(SystemColors.ControlDark, 0, 0, w - 2, 0);
					gdi.Line(SystemColors.ControlDarkDark, 1, 2, 1, h - 3);
					gdi.Line(SystemColors.ControlDarkDark, 1, 1, w - 3, 1);

					gdi.Line(SystemColors.ControlLightLight, 0, h - 1, w - 1, h - 1);
					gdi.Line(SystemColors.ControlLightLight, w - 1, 0, w - 1, h - 1);
					gdi.Line(SystemColors.ControlLight, 1, h - 2, w - 2, h - 2);
					gdi.Line(SystemColors.ControlLight, w - 2, 1, w - 2, h - 2);
				}
			}
		}
		if(_vScrollBar.Control.Parent is not null && _hScrollBar.Control.Parent is not null)
		{
			var rcSpacing = Rectangle.Intersect(clip,
				new Rectangle(_vScrollBar.Control.Left, _vScrollBar.Control.Bottom, _vScrollBar.Control.Width, _hScrollBar.Control.Height));
			if(rcSpacing is { Width: > 0, Height: > 0 })
			{
				gdi.Fill(Style.Colors.ScrollBarSpacing, rcSpacing);
			}
		}
	}

	/// <summary>Override this to paint control content.</summary>
	/// <param name="e"><see cref="PaintEventArgs"/>.</param>
	protected virtual void OnPaintClientArea(PaintEventArgs e)
	{
	}

	/// <inheritdoc/>
	protected sealed override void OnPaint(PaintEventArgs e)
	{
		Assert.IsNotNull(e);

		using(var gdi = e.Graphics.AsGdi())
		{
			gdi.Fill(BackColor, e.ClipRectangle);
			PaintNonClient(gdi, e.ClipRectangle);
		}
		OnPaintClientArea(e);
	}

	/// <inheritdoc/>
	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		UpdateScrollBarBounds();
		if(Visible)
		{
			RecomputeAreas();
			SetScrollBars(false);
			Invalidate();
		}
	}

	/// <inheritdoc/>
	protected override void OnVisibleChanged(EventArgs e)
	{
		base.OnVisibleChanged(e);
		if(Created && Visible)
		{
			RecomputeAreas();
			SetScrollBars(false);
			Invalidate();
		}
	}

	/// <inheritdoc/>
	protected override void OnMouseEnter(EventArgs e)
	{
		_isMouseOver = true;
		base.OnMouseEnter(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		_isMouseOver = false;
		base.OnMouseLeave(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		Focus();
	}

	/// <inheritdoc/>
	protected override void OnMouseWheel(MouseEventArgs e)
	{
		if(ClientRectangle.Contains(e.Location))
		{
			ScrollItems(e.Delta);
			UpdateHover(e.X, e.Y);
		}
		base.OnMouseWheel(e);
	}

	private void ScrollItems(int delta)
	{
		int lines = SystemInformation.MouseWheelScrollLines;
		int scrollpos = VScrollPos;
		if(lines < 0)
		{
			int h = VScrollAffectsClientArea ? ClientArea.Height : ContentArea.Height;
			h = Math.Max(GetVScrollSmallChange(), h);
			scrollpos -= h * Math.Sign(delta);
		}
		else
		{
			scrollpos -= GetVScrollSmallChange() * Math.Sign(delta) * lines;
		}
		scrollpos = ClampScrollPosition(scrollpos, MaxVScrollPos);
		scrollpos = TransformVScrollPos(scrollpos);
		_vScrollBar.Value = scrollpos;
	}

	/// <inheritdoc/>
	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		if(Created && Visible)
		{
			RecomputeAreas();
			SetScrollBars(false);
			Invalidate();
		}
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			StopScrollTimer();

			_vScrollBar.ValueChanged -= OnVScrollBarValueChanged;
			_vScrollBar.Scroll -= OnVScrollBarScroll;
			_vScrollBar.Control.MouseCaptureChanged -= OnScrollBarCaptureChanged;
			_vScrollBar.Dispose();

			_hScrollBar.ValueChanged -= OnVScrollBarValueChanged;
			_hScrollBar.Scroll -= OnVScrollBarScroll;
			_hScrollBar.Control.MouseCaptureChanged -= OnScrollBarCaptureChanged;
			_hScrollBar.Dispose();
		}
		base.Dispose(disposing);
	}

	#endregion
}
