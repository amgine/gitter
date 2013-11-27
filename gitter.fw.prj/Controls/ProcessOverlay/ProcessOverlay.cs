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

namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	public sealed class ProcessOverlay : IProgress<OperationProgress>, IDisposable
	{
		private Control _hostControl;
		private ProcessOverlayRenderer _renderer;

		private int _max;
		private int _min;
		private int _value;
		private bool _marquee;
		private string _title;
		private string _message;
		private float _rounding;
		private bool _isVisible;
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

		public Font Font
		{
			get
			{
				if(_font != null)
				{
					return _font;
				}
				if(_hostControl != null)
				{
					return _hostControl.Font;
				}
				return GitterApplication.FontManager.UIFont;
			}
			set { _font = value; }
		}

		public ProcessOverlayRenderer Renderer
		{
			get
			{
				if(_renderer != null)
				{
					return _renderer;
				}
				return ProcessOverlayRenderer.Default;
			}
			set { _renderer = value; }
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

		public bool IsVisible
		{
			get { return _isVisible; }
			private set { _isVisible = value; }
		}

		public void OnPaint(Graphics graphics, Rectangle bounds)
		{
			if(_isVisible)
			{
				Renderer.Paint(this, graphics, bounds);
			}
		}

		public void DrawMessage(Graphics graphics, Rectangle bounds, string status)
		{
			if(bounds.Height > 25)
			{
				Renderer.PaintMessage(this, graphics, bounds, status);
			}
		}

		private void Repaint()
		{
			RepaintRequired.Raise(this);
			if(_invalidateHost && _hostControl != null && _hostControl.Created && !_hostControl.IsDisposed)
			{
				InvalidateHostControl();
			}
		}

		private void InvalidateHostControl()
		{
			if(_hostControl.InvokeRequired)
			{
				try
				{
					_hostControl.BeginInvoke(new MethodInvoker(InvalidateHostControl));
				}
				catch(ObjectDisposedException)
				{
				}
			}
			else
			{
				var rect = (_getOverlayArea == null) ? _hostControl.ClientRectangle : _getOverlayArea();
				_hostControl.Invalidate(rect);
			}
		}

		public event EventHandler Canceled;

		private void InvokeCancelled()
		{
			var handler = Canceled;
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

		public bool IsCancelRequested
		{
			get { return false; }
		}

		public void Start(IWin32Window parent, IAsyncResult context, bool blocking)
		{
			_context = context;
			if(_disableHost)
			{
				_hostControl.Enabled = false;
			}
			_isVisible = true;
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

		public void SetProgressIndeterminate()
		{
			_marquee = true;
			UpdateWin7ProgressBar();
			Repaint();
		}

		public void ProcessCompleted()
		{
			_context = null;
			_isVisible = false;
			var timer = _timer;
			if(timer != null)
			{
				timer.Enabled = false;
			}
			StopWin7ProgressBar();
			if(_disableHost)
			{
				if(_hostControl.Created && !_hostControl.IsDisposed)
				{
					try
					{
						_hostControl.BeginInvoke(new MethodInvoker(
							() =>
							{
								if(!_hostControl.IsDisposed)
								{
									_hostControl.Enabled = true;
								}
							}));
					}
					catch(ObjectDisposedException)
					{
					}
				}
			}
			try
			{
				Repaint();
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
			}
		}

		#region IProgress<OperationProgress> Members

		public void Report(OperationProgress progress)
		{
			if(_hostControl.IsDisposed)
			{
				return;
			}
			if(_hostControl.InvokeRequired)
			{
				try
				{
					_hostControl.BeginInvoke(new Action<OperationProgress>(ReportCore), progress);
				}
				catch(ObjectDisposedException)
				{
				}
			}
			else
			{
				ReportCore(progress);
			}
		}

		private void ReportCore(OperationProgress progress)
		{
			if(progress.IsCompleted)
			{
				if(IsVisible)
				{
					ProcessCompleted();
				}
				return;
			}
			if(!IsVisible)
			{
				IsVisible = true;
				_timer.Enabled = true;
				if(_disableHost)
				{
					_hostControl.Enabled = false;
				}
			}
			Title = progress.ActionName;
			if(progress.IsIndeterminate)
			{
				SetProgressIndeterminate();
			}
			else
			{
				SetProgressRange(0, progress.MaxProgress);
				SetProgress(progress.CurrentProgress);
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if(_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}
		}

		#endregion
	}
}
