namespace gitter.Framework.Controls
{
	using System;
	using System.Windows.Forms;
	using System.Drawing;

	using gitter.Framework.Services;

	public abstract class DockMarker : Form
	{
		protected static readonly Color BorderColor = Color.FromArgb(99, 104, 113);

		#region Data

		private readonly DockMarkerButton[] _buttons;
		private readonly TrackingService<DockMarkerButton> _buttonHover;
		private readonly Point[] _borderPolygon;
		private bool _isHovered;
		private DockPositionMarker _positionMarker;
		private IDockHost _dockHost;
		private ViewHost _dockClient;

		#endregion

		protected DockMarker(IDockHost dockHost, ViewHost dockClient, DockMarkerButton[] buttons, Point[] border, Rectangle bounds)
		{
			if(dockHost == null) throw new ArgumentNullException("dockHost");
			if(dockClient == null) throw new ArgumentNullException("dockClient");

			_dockHost = dockHost;
			_dockClient = dockClient;

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

			MinimumSize = new Size(1, 1);
			StartPosition = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			ControlBox = false;
			MaximizeBox = false;
			MinimizeBox = false;
			Text = string.Empty;
			ShowIcon = false;
			ShowInTaskbar = false;
			Enabled = false;
			ImeMode = ImeMode.Disable;

			Bounds = bounds;
			AllowTransparency = true;
			Opacity = ViewConstants.OpacityNormal;

			_buttons = buttons;
			_buttonHover = new TrackingService<DockMarkerButton>();
			_buttonHover.Changed += OnButtonHoverChanged;

			_borderPolygon = border;
		}

		/// <summary>
		/// Displays the control to the user.
		/// </summary>
		public new void Show()
		{
			NativeMethods.ShowWindow(this.Handle, 8);
		}

		private void OnButtonHoverChanged(object sender, TrackingEventArgs<DockMarkerButton> e)
		{
			Invalidate(e.Item.Bounds);
			if(e.IsTracked)
			{
				var rect = _dockHost.GetDockBounds(_dockClient, e.Item.Type);
				if(rect.Width != 0 && rect.Height != 0)
				{
					SpawnDockPositionMarker(rect);
				}
			}
			else
			{
				KillDockPositionMarker();
			}
		}

		protected override void DefWndProc(ref Message m)
		{
			const int WM_MOUSEACTIVATE = 0x21;
			const int MA_NOACTIVATE = 0x0003;

			switch(m.Msg)
			{
				case WM_MOUSEACTIVATE:
					m.Result = (IntPtr)MA_NOACTIVATE;
					return;
			}
			base.DefWndProc(ref m);
		}

		protected override void OnShown(EventArgs e)
		{
			TopMost = true;
		}

		protected override bool ShowWithoutActivation
		{
			get { return true; }
		}

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

		/// <summary>
		/// Paints the background of the control.
		/// </summary>
		/// <param name="pevent">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains information about the control to paint.</param>
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

		/// <summary>
		/// Paints the control.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;
			var reg = Region;
			graphics.Clear(Color.White);
			using(var pen = new Pen(BorderColor))
			{
				graphics.DrawPolygon(pen, _borderPolygon);
				for(int i = 0; i < _buttons.Length; ++i)
				{
					_buttons[i].OnPaint(graphics, !_isHovered || _buttonHover.Index == i);
				}
			}
		}

		private int HitTestIndex(Point point)
		{
			for(int i = 0; i < _buttons.Length; ++i)
			{
				if(_buttons[i].Bounds.Contains(point))
					return i;
			}
			return -1;
		}

		public DockResult HitTest()
		{
			return HitTest(Control.MousePosition);
		}

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
			if(ClientRectangle.Contains(point) && (rgn == null || rgn.IsVisible(point)))
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

		public void UpdateHover()
		{
			UpdateHover(Control.MousePosition);
		}

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
			if(_positionMarker == null)
				SpawnDockPositionMarker(bounds);
			else
				_positionMarker.Bounds = bounds;
		}

		private void KillDockPositionMarker()
		{
			if(_positionMarker != null)
			{
				_positionMarker.Dispose();
				_positionMarker = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				KillDockPositionMarker();
			}
			base.Dispose(disposing);
		}
	}
}
