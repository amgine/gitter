namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;


	public sealed class ProcessOverlay : IAsyncProgressMonitor, IDisposable
	{
		private static readonly Color BackgroundColor = Color.FromArgb(225, 240, 240, 255);
		private static readonly Color BorderColor = Color.FromArgb(75, 75, 100);

		private static readonly Brush BackgroundBrush = new SolidBrush(BackgroundColor);
		private static readonly Pen BorderPen = new Pen(BorderColor, 2.0f);

		private static readonly Brush FontBrush = new SolidBrush(BorderColor);

		private static readonly StringFormat StringFormat = new StringFormat()
		{
			Alignment = StringAlignment.Center,
			LineAlignment = StringAlignment.Center,
			Trimming = StringTrimming.EllipsisCharacter,
			FormatFlags = StringFormatFlags.FitBlackBox,
		};

		private static readonly StringFormat TitleStringFormat = new StringFormat(StringFormat.GenericTypographic)
		{
			Alignment = StringAlignment.Near,
			LineAlignment = StringAlignment.Center,
			Trimming = StringTrimming.EllipsisCharacter,
			FormatFlags = StringFormatFlags.FitBlackBox,
		};

		private Control _hostControl;

		private int _max;
		private int _min;
		private int _value;
		private bool _marquee;
		private string _title;
		private string _message;
		private float _rounding;
		private bool _visible;
		private bool _canCancel;
		private IAsyncResult _context;
		private Font _font;
		private bool _invalidateHost;
		private bool _disableHost;

		private readonly Func<Rectangle> _getOverlayArea;

		private Timer _timer;

		public event EventHandler RepaintRequired;

		public ProcessOverlay(Control hostControl, Func<Rectangle> getOverlayArea)
		{
			_hostControl = hostControl;
			_getOverlayArea = getOverlayArea;
			_rounding = 10.0f;
			_invalidateHost = true;
			_disableHost = true;

			_timer = new Timer()
			{
				Interval = 1000/25,
				Enabled = false,
			};
			_timer.Tick += (sender, e) => Repaint();
		}

		public ProcessOverlay(Control hostControl)
			: this(hostControl, null)
		{
		}

		public ProcessOverlay()
			: this(null, null)
		{
		}

		private void UpdateWin7ProgressBar()
		{
			var form = GitterApplication.MainForm;
			if(form != null && !form.IsDisposed)
			{
				if(_marquee)
				{
					form.SetTaskbarProgressState(TbpFlag.Indeterminate);
				}
				else
				{
					form.SetTaskbarProgressState(TbpFlag.Normal);
					form.SetTaskbarProgressValue(
						(long)(_value - _min),
						(long)(_max - _min));
				}
			}
		}

		private static void StopWin7ProgressBar()
		{
			var form = GitterApplication.MainForm;
			if(form != null && !form.IsDisposed) form.SetTaskbarProgressState(TbpFlag.NoProgress);
		}

		private static Color ColorLERP(Color c1, Color c2, double position)
		{
			byte r1 = c1.R;
			byte r2 = c2.R;
			byte r = (byte)(r1 + (r2 - r1) * position);

			byte g1 = c1.G;
			byte g2 = c2.G;
			byte g = (byte)(g1 + (g2 - g1) * position);

			byte b1 = c1.B;
			byte b2 = c2.B;
			byte b = (byte)(b1 + (b2 - b1) * position);

			return Color.FromArgb(r, g, b);
		}

		private static void DrawIntermediateProgress(Graphics graphics, int x, int y, int w, int h)
		{
			const int n = 12;

			int cx = x + w / 2;
			int cy = y + h / 2;

			int r = (w < h ? w : h) / 2;

			long current = (DateTime.Now.Ticks / 1000000) % n;

			for(int i = 0; i < n; ++i)
			{
				var a = i * (Math.PI * 2) / n;
				var cos = Math.Cos(a);
				var sin = Math.Sin(a);
				float x1 = (float)(cx + cos * r / 3.0);
				float y1 = (float)(cy + sin * r / 3.0);
				float x2 = (float)(cx + cos * r);
				float y2 = (float)(cy + sin * r);

				Color color;
				if(i == current)
				{
					color = BorderColor;
				}
				else
				{
					if((current + 1) % n == i)
					{
						color = BackgroundColor;
					}
					else
					{
						var d = i - current;
						if(d < 0) d += n;
						d = n - d;
						var k = (double)d / (double)n;
						color = ColorLERP(BorderColor, BackgroundColor, k);
					}
				}

				using(var pen = new Pen(color, 2.0f))
				{
					graphics.DrawLine(pen, x1, y1, x2, y2);
				}
			}
		}

		public Font Font
		{
			get { return _font; }
			set { _font = value; }
		}

		public int Minimum
		{
			get { return _min; }
			set { _min = value; }
		}

		public int Maximum
		{
			get { return _max; }
			set { _max = value; }
		}

		public int Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		public bool Marquee
		{
			get { return _marquee; }
			set { _marquee = value; }
		}

		public float Rounding
		{
			get { return _rounding; }
			set { _rounding = value; }
		}

		public Control HostControl
		{
			get { return _hostControl; }
			internal set { _hostControl = value; }
		}

		public bool InvalidateHost
		{
			get { return _invalidateHost; }
			set { _invalidateHost = value; }
		}

		public bool DisableHost
		{
			get { return _disableHost; }
			set { _disableHost = value; }
		}

		public bool Visible
		{
			get { return _visible; }
		}

		public void OnPaint(Graphics graphics, Rectangle rect)
		{
			if(_visible)
			{
				var font = _font;
				if(font == null)
				{
					if(_hostControl != null)
						font = _hostControl.Font;
					else
						font = GitterApplication.FontManager.UIFont;
				}
				const int spacing = 10;
				using(var path = Utility.GetRoundedRectangle(rect, _rounding))
				{
					graphics.SmoothingMode = SmoothingMode.HighQuality;
					graphics.TextRenderingHint = Utility.TextRenderingHint;

					graphics.FillPath(BackgroundBrush, path);
					graphics.DrawPath(BorderPen, path);
				}
				var tw = GitterApplication.TextRenderer.MeasureText(
					graphics, _title, font, rect.Width, TitleStringFormat).Width;

				DrawIntermediateProgress(graphics, rect.X + (rect.Width - tw)/2 - 14 - 5, rect.Y + (rect.Height - 14) /2, 14, 14);
				var titleRect = new Rectangle(rect.X + (rect.Width - tw)/2, rect.Y, rect.Width - spacing*2 - 5 - 14, rect.Height);

				GitterApplication.TextRenderer.DrawText(
					graphics, _title, font, FontBrush, titleRect, TitleStringFormat);
				GitterApplication.TextRenderer.DrawText(
					graphics, _message, font, FontBrush, rect, StringFormat);
			}
		}

		public void DrawMessage(Graphics gx, Font font, Rectangle rect, string status)
		{
			if(rect.Height > 25)
			{
				using(var path = Utility.GetRoundedRectangle(rect, 10.0f))
				{
					gx.FillPath(BackgroundBrush, path);
					gx.DrawPath(BorderPen, path);
					GitterApplication.TextRenderer.DrawText(
						gx, status, font, FontBrush, rect, StringFormat);
				}
			}
		}

		public void DrawProgress(Graphics gx, Font font, Rectangle rect, string status)
		{
			using(var path = Utility.GetRoundedRectangle(rect, 10.0f))
			{
				gx.FillPath(BackgroundBrush, path);
				gx.DrawPath(BorderPen, path);
				GitterApplication.TextRenderer.DrawText(
					gx, status, font, FontBrush, rect, StringFormat);
			}
		}

		private void Repaint()
		{
			RepaintRequired.Raise(this);
			if(_invalidateHost && _hostControl != null && _hostControl.Created)
			{
				var rect = (_getOverlayArea == null) ? _hostControl.ClientRectangle : _getOverlayArea();
				if(_hostControl.InvokeRequired)
				{
					_hostControl.BeginInvoke(new Action<Rectangle>(_hostControl.Invalidate), rect);
				}
				else
				{
					_hostControl.Invalidate(rect);
				}
			}
		}

		#region IAsyncProgressMonitor Members

		public event EventHandler Cancelled;

		private void InvokeCancelled()
		{
			var handler = Cancelled;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		public event EventHandler Started;

		private void InvokeStarted()
		{
			var handler = Started;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		public IAsyncResult CurrentContext
		{
			get { return _context; }
		}

		public string ActionName
		{
			get { return _title; }
			set
			{
				_title = value;
				Repaint();
			}
		}

		public bool CanCancel
		{
			get { return _canCancel; }
			set { _canCancel = value; }
		}

		public void Start(IWin32Window parent, IAsyncResult context, bool blocking)
		{
			_context = context;
			if(_disableHost)
			{
				_hostControl.Enabled = false;
			}
			_visible = true;
			_timer.Enabled = true;
			UpdateWin7ProgressBar();
			Repaint();
			InvokeStarted();
		}

		public void SetAction(string action)
		{
			_message = action;
			Repaint();
		}

		public void SetProgressRange(int min, int max)
		{
			_min = min;
			_max = max;
			UpdateWin7ProgressBar();
		}

		public void SetProgressRange(int min, int max, string action)
		{
			_min = min;
			_max = max;
			_message = action;
			UpdateWin7ProgressBar();
			Repaint();
		}

		public void SetProgress(int val)
		{
			_value = val;
			_marquee = false;
			UpdateWin7ProgressBar();
			Repaint();
		}

		public void SetProgress(int val, string action)
		{
			_value = val;
			_message = action;
			_marquee = false;
			UpdateWin7ProgressBar();
			Repaint();
		}

		public void SetProgressIntermediate()
		{
			_marquee = true;
			UpdateWin7ProgressBar();
			Repaint();
		}

		public void ProcessCompleted()
		{
			_context = null;
			_visible = false;
			var timer = _timer;
			if(timer != null)
			{
				timer.Enabled = false;
			}
			StopWin7ProgressBar();
			if(_disableHost)
			{
				if(_hostControl.Created)
				{
					try
					{
						_hostControl.BeginInvoke(new MethodInvoker(() => _hostControl.Enabled = true));
					}
					catch { }
				}
			}
			try
			{
				Repaint();
			}
			catch { }
		}

		#endregion

		public void Dispose()
		{
			if(_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}
		}
	}
}
