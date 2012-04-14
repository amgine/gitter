namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	/// <summary>User-drawn control which supports content scrolling in both directions.</summary>
	public class ScrollableControl : Control
	{
		#region Data

		private BorderStyle _borderStyle;

		private VScrollBar _vScrollBar;
		private HScrollBar _hScrollBar;
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
		private bool _mouseHovered;
		private int _updateCount;

		/// <summary>Area for content rendering and scrolling.</summary>
		private Rectangle _contentArea;
		/// <summary>Client area (excludes borders).</summary>
		private Rectangle _clientArea;
		private Size _contentSize;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="ScrollableControl"/>.</summary>
		public ScrollableControl()
		{
			_borderStyle = BorderStyle.Fixed3D;

			var scrollWidth = SystemInformation.VerticalScrollBarWidth;
			_vScrollBar = new VScrollBar()
			{
				Maximum = 1,
				Minimum = 0,
				SmallChange = 1,
				LargeChange = 1,
				Enabled = false,
				Bounds = new Rectangle(Width - scrollWidth - 2, 2, scrollWidth, Height - 4),
				Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
			};
			_vScrollBar.ValueChanged += OnVScrollBarValueChanged;
			_vScrollBar.Scroll += OnVScrollBarScroll;
			_vScrollBar.MouseCaptureChanged += OnScrollBarCaptureChanged;

			var scrollHeight = SystemInformation.HorizontalScrollBarHeight;
			_hScrollBar = new HScrollBar()
			{
				Maximum = 1,
				Minimum = 0,
				SmallChange = 1,
				LargeChange = 1,
				Enabled = false,
				Bounds = new Rectangle(2, Height - 2 - scrollHeight, Width - 4, scrollHeight),
				Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
			};
			_hScrollBar.ValueChanged += OnHScrollBarValueChanged;
			_hScrollBar.Scroll += OnHScrollBarScroll;
			_hScrollBar.MouseCaptureChanged += OnScrollBarCaptureChanged;

			SetStyle(
				ControlStyles.ContainerControl |
				ControlStyles.ResizeRedraw |
				ControlStyles.SupportsTransparentBackColor, false);
			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.Selectable |
				ControlStyles.OptimizedDoubleBuffer, true);

			BackColor = SystemColors.Window;
		}

		#endregion

		#region Properties

		/// <summary>Control border style.</summary>
		[DefaultValue(BorderStyle.Fixed3D)]
		[Description("Control border style.")]
		public BorderStyle BorderStyle
		{
			get { return _borderStyle; }
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
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}

		/// <summary>Always show vertical scroll bar.</summary>
		[DefaultValue(false)]
		[Description("Always show vertical scroll bar.")]
		public bool AlwaysShowVScrollBar
		{
			get { return _alwaysShowVScrollBar; }
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
			get { return _vScrollPos; }
			set { DoVScroll(value); }
		}

		/// <summary>Maximum vertical scroll position.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int MaxVScrollPos
		{
			get { return _maxVScrollPos; }
		}

		/// <summary>Horizontal scroll position.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int HScrollPos
		{
			get { return _hScrollPos; }
			set { DoHScroll(value); }
		}

		/// <summary>Maximum horizontal scroll position.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int MaxHScrollPos
		{
			get { return _maxHScrollPos; }
		}

		/// <summary>Always show horizontal scroll bar.</summary>
		[DefaultValue(false)]
		[Description("Always show horizontal scroll bar.")]
		public bool AlwaysShowHScrollBar
		{
			get { return _alwaysShowHScrollBar; }
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
		protected HScrollBar HScrollBar
		{
			get { return _hScrollBar; }
		}

		/// <summary>Vertical scrollbar.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected VScrollBar VScrollBar
		{
			get { return _vScrollBar; }
		}

		/// <summary>Client area.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle ClientArea
		{
			get { return _clientArea; }
		}

		/// <summary>Content area.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle ContentArea
		{
			get { return _contentArea; }
		}

		/// <summary>Part of content which is visible.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle ContentWindow
		{
			get { return new Rectangle(_hScrollPos, _vScrollPos, _contentArea.Width, _contentArea.Height); }
		}

		/// <summary>Content size.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size ContentSize
		{
			get { return _contentSize; }
		}

		#endregion

		public void ScrollToTop()
		{
			if(_vScrollPos != 0)
				_vScrollBar.Value = 0;
		}

		public void ScrollToBottom()
		{
			if(_vScrollPos != _maxVScrollPos)
				_vScrollBar.Value = _maxVScrollPos;
		}

		public void ScrollToTopLeft()
		{
			if(_vScrollPos != 0)
				_vScrollBar.Value = 0;
			if(_hScrollPos != 0)
				_hScrollBar.Value = 0;
		}

		public void ScrollToBottomLeft()
		{
			if(_vScrollPos != _maxVScrollPos)
				_vScrollBar.Value = _maxVScrollPos;
			if(_hScrollPos != 0)
				_hScrollBar.Value = 0;
		}


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
		protected virtual void EndUpdate(bool redraw)
		{
			if(_updateCount == 0)
				throw new InvalidOperationException();
			--_updateCount;
			if(_updateCount == 0)
			{
				this.EnableRedraw();
				if(redraw) this.RedrawWindow();
			}
		}

		/// <summary>Enable control redraw events.</summary>
		public void EndUpdate()
		{
			EndUpdate(true);
		}

		public bool IsUpdating
		{
			get { return _updateCount != 0; }
		}

		protected int UpdateCounter
		{
			get { return _updateCount; }
		}

		protected bool MouseHovered
		{
			get { return _mouseHovered; }
		}

		private Rectangle GetClientArea()
		{
			return GetClientArea(_vScrollBar.Parent != null, _hScrollBar.Parent != null);
		}

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
				client.Width -= _vScrollBar.Width - scrollbarOffset;
			if(hScrollBar)
				client.Height -= _hScrollBar.Height - scrollbarOffset;
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

		protected virtual int GetVScrollSmallChange()
		{
			return 1;
		}

		protected virtual int GetHScrollSmallChange()
		{
			return 1;
		}

		protected bool SetScrollBars()
		{
			return SetScrollBars(true);
		}

		protected bool SetScrollBars(bool allowRedraw)
		{
			return SetScrollBars(MeasureContent(_contentArea), allowRedraw);
		}

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
			int offset = _borderStyle == BorderStyle.None ? 0 : 2;
			var bounds = new Rectangle(Width - _vScrollBar.Width - offset, offset, _vScrollBar.Width, Height - offset * 2);
			if(_needHScroll || _alwaysShowHScrollBar)
				bounds.Height -= _hScrollBar.Height;
			return bounds;
		}

		private Rectangle GetHScrollBounds()
		{
			int offset = _borderStyle == BorderStyle.None ? 0 : 2;
			var bounds = new Rectangle(offset, Height - _hScrollBar.Height - offset, Width - offset * 2, _hScrollBar.Height);
			if(_needVScroll || _alwaysShowVScrollBar)
				bounds.Width -= _vScrollBar.Width;
			return bounds;
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
				if(_vScrollBar.Parent == null)
				{
					_vScrollBar.Bounds = GetVScrollBounds();
					_vScrollBar.Parent = this;
					RecomputeAreas();
					if(allowRedraw)
					{
						Invalidate();
						res = true;
					}
				}
				else
				{
					var b = GetVScrollBounds();
					if(_vScrollBar.Bounds != b)
						_vScrollBar.Bounds = b;
				}
				_vScrollBar.Enabled = true;
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
				if((_vScrollBar.Parent == null && _alwaysShowVScrollBar) || (_vScrollBar.Parent != null && !_alwaysShowVScrollBar))
				{
					_vScrollPos = 0;
					if(_alwaysShowVScrollBar)
					{
						_vScrollBar.Bounds = GetVScrollBounds();
						_vScrollBar.Parent = this;
					}
					else
					{
						_vScrollBar.Parent = null;
					}
					RecomputeAreas();
					if(allowRedraw)
					{
						Invalidate();
						res = true;
					}
					_maxVScrollPos = 0;
					_vScrollBar.Value = 0;
					_vScrollBar.Enabled = false;
				}
				else
				{
					var b = GetVScrollBounds();
					if(_vScrollBar.Bounds != b)
						_vScrollBar.Bounds = b;
					if(_maxVScrollPos != 0)
					{
						_maxVScrollPos = 0;
						_blockScrollRedraw = !allowRedraw;
						_vScrollBar.Value = 0;
						_blockScrollRedraw = false;
						_vScrollBar.Enabled = false;
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
				if(_hScrollBar.Parent == null)
				{
					_hScrollBar.Bounds = GetHScrollBounds();
					_hScrollBar.Parent = this;
					RecomputeAreas();
					if(allowRedraw)
					{
						Invalidate();
						res = true;
					}
				}
				else
				{
					var b = GetHScrollBounds();
					if(_hScrollBar.Bounds != b)
						_hScrollBar.Bounds = b;
				}
				_hScrollBar.Enabled = true;
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
				if((_hScrollBar.Parent == null && _alwaysShowHScrollBar) || (_hScrollBar.Parent != null && !_alwaysShowHScrollBar))
				{
					_hScrollPos = 0;
					if(_alwaysShowHScrollBar)
					{
						_hScrollBar.Bounds = GetHScrollBounds();
						_hScrollBar.Parent = this;
					}
					else
					{
						_hScrollBar.Parent = null;
					}
					RecomputeAreas();
					if(allowRedraw)
					{
						Invalidate();
						res = true;
					}
					_maxHScrollPos = 0;
					_hScrollBar.Value = 0;
					_hScrollBar.Enabled = false;
				}
				else
				{
					var b = GetHScrollBounds();
					if(_hScrollBar.Bounds != b)
						_hScrollBar.Bounds = b;
					if(_maxHScrollPos != 0)
					{
						_maxHScrollPos = 0;
						_blockScrollRedraw = !allowRedraw;
						_hScrollBar.Value = 0;
						_blockScrollRedraw = false;
						_hScrollBar.Enabled = false;
					}
				}
			}
			return res;
		}

		protected virtual Rectangle GetContentArea(Rectangle clientArea)
		{
			return clientArea;
		}

		protected virtual bool HScrollAffectsClientArea
		{
			get { return true; }
		}

		protected virtual bool VScrollAffectsClientArea
		{
			get { return false; }
		}

		protected virtual Size MeasureContent()
		{
			return Size.Empty;
		}

		protected virtual Size MeasureContent(Rectangle contentArea)
		{
			return MeasureContent();
		}

		protected virtual int TransformVScrollPos(int position)
		{
			return position;
		}

		protected virtual int TransformHScrollPos(int position)
		{
			return position;
		}

		protected virtual int TransformMaxVScrollPos(int position)
		{
			return position;
		}

		protected virtual int TransformMaxHScrollPos(int position)
		{
			return position;
		}

		protected virtual void OnVScroll(int dy)
		{
		}

		protected virtual void OnHScroll(int dx)
		{
		}

		private void ScrollY(int dy)
		{
			var hwnd = Handle;
			var hdc = NativeMethods.GetDC(hwnd);
			NativeMethods.RECT scroll, clip;
			if(VScrollAffectsClientArea)
			{
				scroll = new NativeMethods.RECT(_clientArea);
				clip = new NativeMethods.RECT(_clientArea);
			}
			else
			{
				scroll = new NativeMethods.RECT(_contentArea);
				clip = new NativeMethods.RECT(_contentArea);
			}
			NativeMethods.RECT invalid;
			NativeMethods.ScrollDC(hdc, 0, dy, ref scroll, ref clip, IntPtr.Zero, out invalid);
			NativeMethods.ReleaseDC(hwnd, hdc);
			Invalidate(Rectangle.FromLTRB(invalid.left, invalid.top, invalid.right, invalid.bottom));
		}

		private void ScrollX(int dx)
		{
			var hwnd = Handle;
			var hdc = NativeMethods.GetDC(hwnd);
			NativeMethods.RECT scroll, clip;
			if(HScrollAffectsClientArea)
			{
				scroll = new NativeMethods.RECT(_clientArea);
				clip = new NativeMethods.RECT(_clientArea);
			}
			else
			{
				scroll = new NativeMethods.RECT(_contentArea);
				clip = new NativeMethods.RECT(_contentArea);
			}
			NativeMethods.RECT invalid;
			NativeMethods.ScrollDC(hdc, dx, 0, ref scroll, ref clip, IntPtr.Zero, out invalid);
			NativeMethods.ReleaseDC(hwnd, hdc);
			Invalidate(Rectangle.FromLTRB(invalid.left, invalid.top, invalid.right, invalid.bottom));
		}

		protected void DoVScroll(int newPosition)
		{
			if(newPosition < 0) newPosition = 0;
			else if(newPosition > _maxVScrollPos) newPosition = _maxVScrollPos;
			newPosition = TransformVScrollPos(newPosition);
			if(newPosition == _vScrollPos) return;

			int dy = _vScrollPos - newPosition;
			_vScrollPos = newPosition;

			OnVScroll(dy);

			if(_blockScrollRedraw) return;

			ScrollY(dy);
		}

		private void DoHScroll(int newPosition)
		{
			if(newPosition < 0) newPosition = 0;
			else if(newPosition > _maxHScrollPos) newPosition = _maxHScrollPos;
			newPosition = TransformHScrollPos(newPosition);
			if(newPosition == _hScrollPos) return;

			int dx = _hScrollPos - newPosition;
			_hScrollPos = newPosition;

			OnHScroll(dx);

			if(_blockScrollRedraw) return;

			ScrollX(dx);
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
				StopScrollTimer();
		}

		#endregion

		#region Overrides

		protected sealed override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

		private void PaintNonClient(PaintEventArgs e)
		{
			var gx = e.Graphics;
			var clip = e.ClipRectangle;
			int h = Height;
			int w = Width;
			if(_borderStyle == BorderStyle.None)
			{
				if(clip.X <= 0 || clip.Y <= 0 || clip.Right >= w || clip.Height >= h)
				{
					gx.DrawRectangle(SystemPens.Window, 0, 0, w - 1, h - 1);
				}
			}
			else
			{
				if((_borderStyle == BorderStyle.FixedSingle) || Application.RenderWithVisualStyles)
				{
					if(clip.X <= 0 || clip.Y <= 0 || clip.Right >= w || clip.Height >= h)
					{
						gx.DrawRectangle(SystemPens.ControlDark, 0, 0, w - 1, h - 1);
						gx.DrawRectangle(SystemPens.Window, 1, 1, w - 3, h - 3);
					}
				}
				else
				{
					if(clip.X <= 0 || clip.Y <= 0 || clip.Right >= w || clip.Height >= h)
					{
						gx.DrawLine(SystemPens.ControlDark, 0, 1, 0, h - 2);
						gx.DrawLine(SystemPens.ControlDark, 0, 0, w - 2, 0);
						gx.DrawLine(SystemPens.ControlDarkDark, 1, 2, 1, h - 3);
						gx.DrawLine(SystemPens.ControlDarkDark, 1, 1, w - 3, 1);

						gx.DrawLine(SystemPens.ControlLightLight, 0, h - 1, w - 1, h - 1);
						gx.DrawLine(SystemPens.ControlLightLight, w - 1, 0, w - 1, h - 1);
						gx.DrawLine(SystemPens.ControlLight, 1, h - 2, w - 2, h - 2);
						gx.DrawLine(SystemPens.ControlLight, w - 2, 1, w - 2, h - 2);
					}
				}
			}
		}

		/// <summary>Override this to paint control content.</summary>
		/// <param name="e"><see cref="PaintEventArgs"/>.</param>
		protected virtual void OnPaintClientArea(PaintEventArgs e)
		{
		}

		protected sealed override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(SystemBrushes.Window, e.ClipRectangle);
			PaintNonClient(e);
			OnPaintClientArea(e);
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			if(Visible)
			{
				RecomputeAreas();
				SetScrollBars(false);
				Invalidate();
			}
		}

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

		protected override void OnMouseEnter(EventArgs e)
		{
			_mouseHovered = true;
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			_mouseHovered = false;
			base.OnMouseLeave(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Focus();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if(ClientRectangle.Contains(e.Location))
			{
				int lines = SystemInformation.MouseWheelScrollLines;
				int scrollpos = VScrollPos;
				if(lines < 0)
				{
					int h = VScrollAffectsClientArea ? ClientArea.Height : ContentArea.Height;
					h = Math.Max(GetVScrollSmallChange(), h);
					scrollpos -= h * Math.Sign(e.Delta);
				}
				else
				{
					scrollpos -= GetVScrollSmallChange() * Math.Sign(e.Delta) * lines;
				}
				scrollpos = TransformVScrollPos(scrollpos);
				if(scrollpos < 0)
					scrollpos = 0;
				else if(scrollpos > MaxVScrollPos)
					scrollpos = MaxVScrollPos;
				_vScrollBar.Value = scrollpos;

				UpdateHover(e.X, e.Y);
			}
			base.OnMouseWheel(e);
		}

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

		protected override void Dispose(bool disposing)
		{
			if(disposing && !IsDisposed)
			{
				StopScrollTimer();

				if(!_vScrollBar.IsDisposed)
				{
					_vScrollBar.Dispose();
					_vScrollBar.ValueChanged -= OnVScrollBarValueChanged;
					_vScrollBar.Scroll -= OnVScrollBarScroll;
					_vScrollBar.MouseCaptureChanged -= OnScrollBarCaptureChanged;
				}

				if(!_hScrollBar.IsDisposed)
				{
					_hScrollBar.Dispose();
					_hScrollBar.ValueChanged -= OnVScrollBarValueChanged;
					_hScrollBar.Scroll -= OnVScrollBarScroll;
					_hScrollBar.MouseCaptureChanged -= OnScrollBarCaptureChanged;
				}
			}
			base.Dispose(disposing);
		}

		#endregion
	}
}
