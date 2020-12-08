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
	using System.Windows.Forms;

	public sealed class ProcessOverlay : IProgress<OperationProgress>, IDisposable
	{
		private ProcessOverlayRenderer _renderer;
		private Font _font;
		private readonly Func<Rectangle> _getOverlayArea;

		private Timer _timer;

		public event EventHandler RepaintRequired;

		public ProcessOverlay(Control hostControl, Func<Rectangle> getOverlayArea)
		{
			HostControl = hostControl;
			_getOverlayArea = getOverlayArea;
			Rounding = 10.0f;
			InvalidateHost = true;
			DisableHost = true;

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
				if(Marquee)
				{
					form.SetTaskbarProgressState(TbpFlag.Indeterminate);
				}
				else
				{
					form.SetTaskbarProgressState(TbpFlag.Normal);
					form.SetTaskbarProgressValue(
						(long)(Value - Minimum),
						(long)(Maximum - Minimum));
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
			get => _font ?? HostControl?.Font ?? GitterApplication.FontManager.UIFont;
			set => _font = value;
		}

		public ProcessOverlayRenderer Renderer
		{
			get => _renderer ?? ProcessOverlayRenderer.Default;
			set => _renderer = value;
		}

		public int Minimum { get; set; }

		public int Maximum { get; set; }

		public int Value { get; set; }

		public string Title { get; set; }

		public string Message { get; set; }

		public bool Marquee { get; set; }

		public float Rounding { get; set; }

		public Control HostControl { get; internal set; }

		public bool InvalidateHost { get; set; }

		public bool DisableHost { get; set; }

		public bool IsVisible { get; private set; }

		public void OnPaint(Graphics graphics, Rectangle bounds)
		{
			if(IsVisible)
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
			RepaintRequired?.Invoke(this, EventArgs.Empty);
			if(InvalidateHost && HostControl != null && HostControl.Created && !HostControl.IsDisposed)
			{
				InvalidateHostControl();
			}
		}

		private void InvalidateHostControl()
		{
			var hostControl = HostControl;
			if(hostControl == null || hostControl.IsDisposed) return;
			if(hostControl.InvokeRequired)
			{
				try
				{
					hostControl.BeginInvoke(new MethodInvoker(InvalidateHostControl));
				}
				catch(ObjectDisposedException)
				{
				}
			}
			else
			{
				var rect = (_getOverlayArea == null) ? hostControl.ClientRectangle : _getOverlayArea();
				hostControl.Invalidate(rect);
			}
		}

		public event EventHandler Canceled;

		private void InvokeCanceled() => Canceled?.Invoke(this, EventArgs.Empty);

		public event EventHandler Started;

		private void InvokeStarted() => Started?.Invoke(this, EventArgs.Empty);

		public IAsyncResult CurrentContext { get; private set; }

		public string ActionName
		{
			get => Title;
			set
			{
				Title = value;
				Repaint();
			}
		}

		public bool CanCancel { get; set; }

		public bool IsCancelRequested => false;

		public void Start(IWin32Window parent, IAsyncResult context, bool blocking)
		{
			CurrentContext = context;
			if(DisableHost)
			{
				HostControl.Enabled = false;
			}
			IsVisible = true;
			_timer.Enabled = true;
			UpdateWin7ProgressBar();
			Repaint();
			InvokeStarted();
		}

		public void SetAction(string action)
		{
			Message = action;
			Repaint();
		}

		public void SetProgressRange(int min, int max)
		{
			Minimum = min;
			Maximum = max;
			UpdateWin7ProgressBar();
		}

		public void SetProgressRange(int min, int max, string action)
		{
			Minimum = min;
			Maximum = max;
			Message = action;
			UpdateWin7ProgressBar();
			Repaint();
		}

		public void SetProgress(int val)
		{
			Value = val;
			Marquee = false;
			UpdateWin7ProgressBar();
			Repaint();
		}

		public void SetProgress(int val, string action)
		{
			Value = val;
			Message = action;
			Marquee = false;
			UpdateWin7ProgressBar();
			Repaint();
		}

		public void SetProgressIndeterminate()
		{
			Marquee = true;
			UpdateWin7ProgressBar();
			Repaint();
		}

		public void ProcessCompleted()
		{
			CurrentContext = null;
			IsVisible = false;
			var timer = _timer;
			if(timer != null)
			{
				timer.Enabled = false;
			}
			StopWin7ProgressBar();
			if(DisableHost)
			{
				if(HostControl.Created && !HostControl.IsDisposed)
				{
					try
					{
						HostControl.BeginInvoke(new MethodInvoker(
							() =>
							{
								if(!HostControl.IsDisposed)
								{
									HostControl.Enabled = true;
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
			catch(Exception exc) when(!exc.IsCritical())
			{
			}
		}

		#region IProgress<OperationProgress> Members

		public void Report(OperationProgress progress)
		{
			var hostControl = HostControl;
			if(hostControl == null || hostControl.IsDisposed)
			{
				return;
			}
			if(hostControl.InvokeRequired)
			{
				try
				{
					hostControl.BeginInvoke(new Action<OperationProgress>(ReportCore), progress);
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
				if(DisableHost)
				{
					HostControl.Enabled = false;
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
