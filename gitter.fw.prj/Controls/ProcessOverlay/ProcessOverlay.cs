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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

public sealed class ProcessOverlay : IProgress<OperationProgress>, IDisposable
{
	private ProcessOverlayRenderer? _renderer;
	private Font? _font;
	private readonly Func<Rectangle>? _getOverlayArea;

	private System.Windows.Forms.Timer? _animationTimer;
	private TaskbarProgressClient? _trackbarProgressClient;
	private bool _isVisible;

	public event EventHandler? RepaintRequired;

	public ProcessOverlay(Control? hostControl, Func<Rectangle>? getOverlayArea)
	{
		HostControl = hostControl;
		_getOverlayArea = getOverlayArea;
		Rounding = 10.0f;
		InvalidateHost = true;
		DisableHost = true;
	}

	public ProcessOverlay(Control hostControl)
		: this(hostControl, null)
	{
	}

	public ProcessOverlay()
		: this(null, null)
	{
	}

	sealed class SharedTaskbarProgress
	{
		private int _count;

		public static SharedTaskbarProgress Instance { get; } = new();

		public void Increment()
		{
			if(Interlocked.Increment(ref _count) == 1)
			{
				if(GitterApplication.MainForm is { IsDisposed: false } form)
				{
					form.SetTaskbarProgressState(TbpFlag.Indeterminate);
				}
			}
		}

		public void Decrement(bool reset = true)
		{
			if(Interlocked.Decrement(ref _count) == 0)
			{
				if(reset && GitterApplication.MainForm is { IsDisposed: false } form)
				{
					form.SetTaskbarProgressState(TbpFlag.NoProgress);
				}
			}
		}
	}

	sealed class TaskbarProgressClient
	{
		private enum State
		{
			NoProgress,
			Scheduled,
			Indeterminate,
			Set,
		}

		private readonly SharedTaskbarProgress _progress;
		private readonly LockType _syncRoot = new();
		private State _state;
		private System.Threading.Timer? _timer;

		internal TaskbarProgressClient(SharedTaskbarProgress progress)
		{
			_progress = progress;
		}

		public TaskbarProgressClient() : this(SharedTaskbarProgress.Instance)
		{
		}

		public void SetIndeterminate()
		{
			lock(_syncRoot)
			{
				if(_state != State.NoProgress) return;
				_state = State.Scheduled;
				if(_timer is null)
				{
					_timer = new(OnTimerTick, null, 100, Timeout.Infinite);
				}
				else
				{
					_timer.Change(100, Timeout.Infinite);
				}
			}
		}

		public void Set(long current, long total)
		{
			lock(_syncRoot)
			{
				if(_state != State.NoProgress)
				{
					if(_state == State.Scheduled)
					{
						_timer?.Change(Timeout.Infinite, Timeout.Infinite);
					}
					if(_state == State.Indeterminate)
					{
						_progress.Decrement(reset: false);
					}
					_state = State.Set;
				}
				if(GitterApplication.MainForm is { IsDisposed: false } form)
				{
					form.SetTaskbarProgressState(TbpFlag.Normal);
					form.SetTaskbarProgressValue(current, total);
				}
			}
		}

		private void OnTimerTick(object? state)
		{
			lock(_syncRoot)
			{
				if(_state != State.Scheduled) return;
				_progress.Increment();
				_state = State.Indeterminate;
			}
		}

		public void Reset()
		{
			lock(_syncRoot)
			{
				if(_state == State.NoProgress) return;

				if(_state == State.Scheduled)
				{
					_timer?.Change(Timeout.Infinite, Timeout.Infinite);
				}
				if(_state == State.Indeterminate)
				{
					_progress.Decrement();
				}
				if(_state == State.Set)
				{
					if(GitterApplication.MainForm is { IsDisposed: false } form)
					{
						form.SetTaskbarProgressState(TbpFlag.NoProgress);
					}
				}
				_state = State.NoProgress;
			}
		}

		public void Dispose()
		{
			lock(_syncRoot)
			{
				if(_state == State.Indeterminate)
				{
					_progress.Decrement();
				}
				else if(_state == State.Set)
				{
					if(GitterApplication.MainForm is { IsDisposed: false } form)
					{
						form.SetTaskbarProgressState(TbpFlag.NoProgress);
					}
				}
				_state = State.NoProgress;
				if(_timer is not null)
				{
					_timer.Dispose();
					_timer = null;
				}
			}
		}
	}

	private void UpdateWin7ProgressBar()
	{
		_trackbarProgressClient ??= new();
		if(Marquee)
		{
			_trackbarProgressClient.SetIndeterminate();
		}
		else
		{
			_trackbarProgressClient.Set((long)(Value - Minimum), (long)(Maximum - Minimum));
		}
	}

	private void StopWin7ProgressBar()
	{
		_trackbarProgressClient?.Reset();
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

	public string? Title { get; set; }

	public string? Message { get; set; }

	public bool Marquee { get; set; }

	public float Rounding { get; set; }

	public Control? HostControl { get; internal set; }

	public bool InvalidateHost { get; set; }

	public bool DisableHost { get; set; }

	public bool IsVisible
	{
		get => _isVisible;
		set
		{
			if(_isVisible == value) return;

			_isVisible = value;
			if(value)
			{
				StartAnimationTimer();
			}
			else
			{
				StopAnimationTimer();
				StopWin7ProgressBar();
			}
		}
	}

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
		if(InvalidateHost && HostControl is { Created: true, IsDisposed: false })
		{
			InvalidateHostControl();
		}
	}

	private void InvalidateHostControl()
	{
		var hostControl = HostControl;
		if(hostControl is null || hostControl.IsDisposed) return;
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
			var rect = _getOverlayArea is null ? hostControl.ClientRectangle : _getOverlayArea();
			hostControl.Invalidate(rect);
		}
	}

	public event EventHandler? Canceled;

	private void InvokeCanceled() => Canceled?.Invoke(this, EventArgs.Empty);

	public event EventHandler? Started;

	private void InvokeStarted() => Started?.Invoke(this, EventArgs.Empty);

	public IAsyncResult? CurrentContext { get; private set; }

	public string? ActionName
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

	private void StartAnimationTimer()
	{
		if(_animationTimer is null)
		{
			_animationTimer = new()
			{
				Interval = 1000 / 25,
			};
			_animationTimer.Tick += OnAninmationTimerTick;
		}
		else
		{
			_animationTimer.Enabled = true;
		}
	}

	private void StopAnimationTimer()
	{
		if(_animationTimer is not null)
		{
			_animationTimer.Enabled = false;
		}
	}

	private void OnAninmationTimerTick(object? sender, EventArgs e)
	{
		Repaint();
	}

	public void Start(IWin32Window parent, IAsyncResult context, bool blocking)
	{
		CurrentContext = context;
		if(DisableHost && HostControl is not null)
		{
			HostControl.Enabled = false;
		}
		IsVisible = true;
		StartAnimationTimer();
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
		var timer = _animationTimer;
		if(timer is not null)
		{
			timer.Enabled = false;
		}
		StopWin7ProgressBar();
		if(DisableHost && HostControl is not null)
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
		catch(Exception exc) when(!exc.IsCritical)
		{
		}
	}

	#region IProgress<OperationProgress> Members

	public void Report(OperationProgress progress)
	{
		var hostControl = HostControl;
		if(hostControl is not { IsDisposed: false })
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
			StartAnimationTimer();
			if(DisableHost && HostControl is not null)
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
		if(_trackbarProgressClient is not null)
		{
			_trackbarProgressClient.Dispose();
			_trackbarProgressClient = default;
		}
		if(_animationTimer is not null)
		{
			_animationTimer.Dispose();
			_animationTimer = null;
		}
	}

	#endregion
}
