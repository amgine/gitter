namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Security.Permissions;
	using System.Runtime.InteropServices;
	using System.Windows.Forms.VisualStyles;

	/// <summary>Represents a pop-up window.</summary>
	[ToolboxItem(false)]
	public partial class Popup : ToolStripDropDown
	{
		#region Data

		private VisualStyleRenderer _sizeGripRenderer;

		private Control _content;
		private PopupAnimations _popupAnimation;
		private PopupAnimations _closeAnimation;
		private int _animationDuration;
		private bool _focusOnPopup = true;
		private bool _acceptAlt = true;

		private ToolStripControlHost _host;
		private Control _ownerControl;
		private Popup _ownerPopup;
		private Popup _childPopup;
		private bool _resizableTop;
		private bool _resizableLeft;

		private bool _isChildPopupOpened;
		private bool _resizable;

		private Size _minimumSize;
		private Size _maximumSize;

		#endregion

		#region Properties

		public Control Content
		{
			get { return _content; }
		}

		public PopupAnimations PopupAnimation
		{
			get { return _popupAnimation; }
			set { _popupAnimation = value; }
		}

		public PopupAnimations CloseAnimation
		{
			get { return _closeAnimation; }
			set { _closeAnimation = value; }
		}

		public int AnimationDuration
		{
			get { return _animationDuration; }
			set { _animationDuration = value; }
		}

		[DefaultValue(true)]
		public bool FocusOnPopup
		{
			get { return _focusOnPopup; }
			set { _focusOnPopup = value; }
		}

		[DefaultValue(true)]
		public bool AcceptAlt
		{
			get { return _acceptAlt; }
			set { _acceptAlt = value; }
		}

		public bool Resizable
		{
			get { return _resizable && !_isChildPopupOpened; }
			set { _resizable = value; }
		}

		public new Size MinimumSize
		{
			get { return _minimumSize; }
			set { _minimumSize = value; }
		}

		public new Size MaximumSize
		{
			get { return _maximumSize; }
			set { _maximumSize = value; }
		}

		#endregion

		#region .ctor

		public Popup(Control content)
		{
			if(content == null) throw new ArgumentNullException("content");

			_content = content;
			_popupAnimation = PopupAnimations.SystemDefault;
			_closeAnimation = PopupAnimations.None;
			_animationDuration = 100;
			_isChildPopupOpened = false;
			
			AutoSize = false;
			DoubleBuffered = true;
			ResizeRedraw = true;
			
			_host = new ToolStripControlHost(content)
			{
				Padding = Padding.Empty,
				Margin = Padding.Empty,
			};
			Padding = Padding.Empty;
			Margin = Padding.Empty;

			_minimumSize = content.MinimumSize;
			content.MinimumSize = content.Size;
			_maximumSize = content.MaximumSize;
			content.MaximumSize = content.Size;
			Size = content.Size;
			content.Location = Point.Empty;

			content.TabStop = true;
			TabStop = true;

			Items.Add(_host);

			content.Disposed += (sender, e) =>
				{
					content = null;
					Dispose(true);
				};
			content.RegionChanged += (sender, e) =>
				{
					UpdateRegion();
				};
			content.Paint += (sender, e) =>
				{
					PaintSizeGrip(e);
				};
			
			UpdateRegion();
		}

		#endregion

		#region Overrides

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_content != null)
				{
					var c = _content;
					_content = null;
					c.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				var cp = base.CreateParams;
				cp.ExStyle |= NativeMethods.WS_EX_NOACTIVATE;
				return cp;
			}
		}

		#endregion

		#region Methods

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if((Visible && PopupAnimation == PopupAnimations.None) || (!Visible && CloseAnimation == PopupAnimations.None))
			{
				return;
			}
			NativeMethods.AnimationFlags flags = Visible ? NativeMethods.AnimationFlags.Roll : NativeMethods.AnimationFlags.Hide;
			PopupAnimations _flags = Visible ? PopupAnimation : CloseAnimation;
			if(_flags == PopupAnimations.SystemDefault)
			{
				if(SystemInformation.IsMenuAnimationEnabled)
				{
					if(SystemInformation.IsMenuFadeEnabled)
					{
						_flags = PopupAnimations.Blend;
					}
					else
					{
						_flags = PopupAnimations.Slide | (Visible ? PopupAnimations.TopToBottom : PopupAnimations.BottomToTop);
					}
				}
				else
				{
					_flags = PopupAnimations.None;
				}
			}
			if((_flags & (PopupAnimations.Blend | PopupAnimations.Center | PopupAnimations.Roll | PopupAnimations.Slide)) == PopupAnimations.None)
			{
				return;
			}
			if(_resizableTop) // popup is “inverted”, so the animation must be
			{
				if((_flags & PopupAnimations.BottomToTop) != PopupAnimations.None)
				{
					_flags = (_flags & ~PopupAnimations.BottomToTop) | PopupAnimations.TopToBottom;
				}
				else if((_flags & PopupAnimations.TopToBottom) != PopupAnimations.None)
				{
					_flags = (_flags & ~PopupAnimations.TopToBottom) | PopupAnimations.BottomToTop;
				}
			}
			if(_resizableLeft) // popup is “inverted”, so the animation must be
			{
				if((_flags & PopupAnimations.RightToLeft) != PopupAnimations.None)
				{
					_flags = (_flags & ~PopupAnimations.RightToLeft) | PopupAnimations.LeftToRight;
				}
				else if((_flags & PopupAnimations.LeftToRight) != PopupAnimations.None)
				{
					_flags = (_flags & ~PopupAnimations.LeftToRight) | PopupAnimations.RightToLeft;
				}
			}
			flags = flags | (NativeMethods.AnimationFlags.Mask & (NativeMethods.AnimationFlags)(int)_flags);
			NativeMethods.AnimateWindow(this, AnimationDuration, flags);
		}

		/// <summary>
		/// Processes a dialog box key.
		/// </summary>
		/// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys" /> values that represents the key to process.</param>
		/// <returns>
		/// true if the key was processed by the control; otherwise, false.
		/// </returns>
		[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		protected override bool ProcessDialogKey(Keys keyData)
		{
			if(_acceptAlt && ((keyData & Keys.Alt) == Keys.Alt))
			{
				if((keyData & Keys.F4) != Keys.F4)
				{
					return false;
				}
				else
				{
					this.Close();
				}
			}
			bool processed = base.ProcessDialogKey(keyData);
			if(!processed && (keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift)))
			{
				bool backward = (keyData & Keys.Shift) == Keys.Shift;
				this.Content.SelectNextControl(null, !backward, true, true, true);
			}
			return processed;
		}

		protected void UpdateRegion()
		{
			if(this.Region != null)
			{
				this.Region.Dispose();
				this.Region = null;
			}
			if(_content != null && _content.Region != null)
			{
				this.Region = _content.Region.Clone();
			}
		}

		public void Show(Control control)
		{
			if(control == null) throw new ArgumentNullException("control");
			Show(control, control.ClientRectangle);
		}

		/// <summary>
		/// Shows the pop-up window below the specified area of the specified control.
		/// </summary>
		/// <param name="control">The control used to compute screen location of specified area.</param>
		/// <param name="area">The area of control below which the pop-up will be shown.</param>
		/// <remarks>
		/// When there is no space below specified area, the pop-up control is shown above it.
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="control"/> is <code>null</code>.</exception>
		public void Show(Control control, Rectangle area)
		{
			if(control == null) throw new ArgumentNullException("control");

			SetOwnerItem(control);

			_resizableTop = _resizableLeft = false;
			var location = control.PointToScreen(new Point(area.Left, area.Top + area.Height));
			var screen = Screen.FromControl(control).WorkingArea;
			if(location.X + Size.Width > (screen.Left + screen.Width))
			{
				_resizableLeft = true;
				location.X = (screen.Left + screen.Width) - Size.Width;
			}
			if(location.Y + Size.Height > (screen.Top + screen.Height))
			{
				_resizableTop = true;
				location.Y -= Size.Height + area.Height;
			}
			location = control.PointToClient(location);
			Show(control, location, ToolStripDropDownDirection.BelowRight);
		}

		private void SetOwnerItem(Control control)
		{
			if(control == null)
			{
				return;
			}
			Popup popupControl = control as Popup;
			if(popupControl != null)
			{
				_ownerPopup = popupControl;
				_ownerPopup._childPopup = this;
				OwnerItem = popupControl.Items[0];
				return;
			}
			else if(_ownerControl == null)
			{
				_ownerControl = control;
			}
			if(control.Parent != null)
			{
				SetOwnerItem(control.Parent);
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			if(_content != null)
			{
				_content.MinimumSize = Size;
				_content.MaximumSize = Size;
				_content.Size = Size;
				_content.Location = Point.Empty;
			}
			base.OnSizeChanged(e);
		}

		protected override void OnOpening(CancelEventArgs e)
		{
			if(_content == null || _content.IsDisposed || _content.Disposing)
			{
				e.Cancel = true;
				return;
			}
			UpdateRegion();
			base.OnOpening(e);
		}

		protected override void OnOpened(EventArgs e)
		{
			if(_ownerPopup != null)
			{
				_ownerPopup._isChildPopupOpened = true;
			}
			if(_focusOnPopup)
			{
				_content.Focus();
			}
			base.OnOpened(e);
		}

		protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
		{
			_ownerControl = null;
			if(_ownerPopup != null)
			{
				_ownerPopup._isChildPopupOpened = false;
			}
			base.OnClosed(e);
		}

		#endregion

		#region Resizing

		protected override void WndProc(ref Message m)
		{
			if(InternalProcessResizing(ref m, false))
			{
				return;
			}
			base.WndProc(ref m);
		}

		public bool ProcessResizing(ref Message m)
		{
			return InternalProcessResizing(ref m, true);
		}

		private bool InternalProcessResizing(ref Message m, bool contentControl)
		{
			switch((WindowsMessage)m.Msg)
			{
				case WindowsMessage.WM_NCACTIVATE:
					if(m.WParam != IntPtr.Zero && _childPopup != null && _childPopup.Visible)
					{
						_childPopup.Hide();
					}
					break;
			}
			if(!Resizable)
			{
				return false;
			}
			switch((WindowsMessage)m.Msg)
			{
				case WindowsMessage.WM_NCHITTEST:
					return OnNcHitTest(ref m, contentControl);
				case WindowsMessage.WM_GETMINMAXINFO:
					return OnGetMinMaxInfo(ref m);
			}
			return false;
		}

		private bool OnGetMinMaxInfo(ref Message m)
		{
			var minmax = (NativeMethods.MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.MINMAXINFO));
			if(!_maximumSize.IsEmpty)
			{
				minmax.maxTrackSize = _maximumSize;
			}
			minmax.minTrackSize = _minimumSize;
			Marshal.StructureToPtr(minmax, m.LParam, false);
			return true;
		}

		private bool OnNcHitTest(ref Message m, bool contentControl)
		{
			int x = NativeMethods.LOWORD(m.LParam);
			int y = NativeMethods.HIWORD(m.LParam);

			var clientLocation = PointToClient(new Point(x, y));
			var gripBounds = new GripBounds(contentControl ? _content.ClientRectangle : ClientRectangle);
			IntPtr transparent = new IntPtr(NativeMethods.HTTRANSPARENT);

			if(_resizableTop)
			{
				if(_resizableLeft && gripBounds.TopLeft.Contains(clientLocation))
				{
					m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOPLEFT;
					return true;
				}
				if(!_resizableLeft && gripBounds.TopRight.Contains(clientLocation))
				{
					m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOPRIGHT;
					return true;
				}
				if(gripBounds.Top.Contains(clientLocation))
				{
					m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOP;
					return true;
				}
			}
			else
			{
				if(_resizableLeft && gripBounds.BottomLeft.Contains(clientLocation))
				{
					m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOMLEFT;
					return true;
				}
				if(!_resizableLeft && gripBounds.BottomRight.Contains(clientLocation))
				{
					m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOMRIGHT;
					return true;
				}
				if(gripBounds.Bottom.Contains(clientLocation))
				{
					m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOM;
					return true;
				}
			}
			if(_resizableLeft && gripBounds.Left.Contains(clientLocation))
			{
				m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTLEFT;
				return true;
			}
			if(!_resizableLeft && gripBounds.Right.Contains(clientLocation))
			{
				m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTRIGHT;
				return true;
			}
			return false;
		}

		private void PaintSizeGrip(PaintEventArgs e)
		{
			const int gripWidth = 0x10;
			const int gripHeight = 0x10;

			if(e == null || e.Graphics == null || !_resizable)
				return;

			var clientSize = _content.ClientSize;
			using(var gripImage = new Bitmap(gripWidth, gripHeight))
			{
				using(var graphics = Graphics.FromImage(gripImage))
				{
					if(Application.RenderWithVisualStyles)
					{
						if(_sizeGripRenderer == null)
						{
							_sizeGripRenderer = new VisualStyleRenderer(VisualStyleElement.Status.Gripper.Normal);
						}
						_sizeGripRenderer.DrawBackground(graphics, new Rectangle(0, 0, gripWidth, gripHeight));
					}
					else
					{
						ControlPaint.DrawSizeGrip(graphics, _content.BackColor, 0, 0, gripWidth, gripHeight);
					}
				}
				var gs = e.Graphics.Save();
				e.Graphics.ResetTransform();
				if(_resizableTop)
				{
					if(_resizableLeft)
					{
						e.Graphics.RotateTransform(180);
						e.Graphics.TranslateTransform(-clientSize.Width, -clientSize.Height);
					}
					else
					{
						e.Graphics.ScaleTransform(1, -1);
						e.Graphics.TranslateTransform(0, -clientSize.Height);
					}
				}
				else if(_resizableLeft)
				{
					e.Graphics.ScaleTransform(-1, 1);
					e.Graphics.TranslateTransform(-clientSize.Width, 0);
				}
				e.Graphics.DrawImage(gripImage, clientSize.Width - gripWidth, clientSize.Height - gripHeight + 1, gripWidth, gripHeight);
				e.Graphics.Restore(gs);
			}
		}

		#endregion
	}
}
