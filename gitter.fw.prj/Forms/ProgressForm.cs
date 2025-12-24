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

namespace gitter.Framework;

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework.Controls;
using gitter.Framework.Layout;

using Resources = gitter.Framework.Properties.Resources;

[DesignerCategory("")]
public partial class ProgressForm : Form, IProgress<OperationProgress>
{
	private bool _canCancel;
	private IAsyncResult? _context;
	private volatile bool _isCancelRequested;
	private CancellationTokenSource? _cancellationTokenSource;

	/// <summary>
	/// Monitor raises this event to stop monitored action execution.
	/// </summary>
	public event EventHandler? Canceled;

	/// <summary>
	/// Monitor raises this event when it is ready to receive Set() calls after Start() call.
	/// </summary>
	public event EventHandler? Started;

	protected virtual void OnStarted()
		=> Started?.Invoke(this, EventArgs.Empty);

	protected virtual void OnCanceled()
		=> Canceled?.Invoke(this, EventArgs.Empty);

	private readonly LabelControl _lblAction;
	private readonly IProgressBarWidget _progressBar;
	private readonly IButtonWidget _btnCancel;

	/// <summary>Initializes a new instance of the <see cref="ProgressForm"/> class.</summary>
	public ProgressForm()
	{
		SuspendLayout();

		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode = AutoScaleMode.Dpi;
		Font = GitterApplication.FontManager.UIFont;
		FormBorderStyle = FormBorderStyle.FixedDialog;
		ClientSize = new(389, 104);
		MaximizeBox = false;
		MinimizeBox = false;
		Name = nameof(ProgressForm);
		Text = "Progress";
		ShowIcon = false;
		ShowInTaskbar = false;
		StartPosition = FormStartPosition.CenterParent;

		var noMargin = DpiBoundValue.Constant(Padding.Empty);

		Panel pnlButtons;

		var style = GitterApplication.Style;
		var colors = style.Colors;

		BackColor = colors.Window;
		ForeColor = colors.WindowText;

		_lblAction   = new();
		_progressBar = style.ProgressBarFactory.Create();
		_btnCancel   = style.ButtonFactory.Create();

		_ = new ControlLayout(this)
		{
			Content = new Grid(
				rows:
				[
					SizeSpec.Everything(),
					Application.RenderWithVisualStyles ? SizeSpec.Absolute(1) : SizeSpec.Nothing(),
					SizeSpec.Absolute(39),
				],
				content:
				[
					new GridContent(new Grid(
						padding: DpiBoundValue.Padding(new Padding(12)),
						rows:
						[
							SizeSpec.Absolute(18),
							SizeSpec.Absolute(2),
							SizeSpec.Absolute(18),
						],
						content:
						[
							new GridContent(new ControlContent(_lblAction,   marginOverride: noMargin), row: 0),
							new GridContent(new WidgetContent (_progressBar, marginOverride: noMargin), row: 2),
						]), row: 0),
						new GridContent(new ControlContent(new Panel
						{
							BackColor = colors.WindowFooterSeparator,
							Parent    = this,
						},
						marginOverride: noMargin,
						horizontalContentAlignment: HorizontalContentAlignment.Stretch,
						verticalContentAlignment:   VerticalContentAlignment.Stretch),
						row: 1),
					new GridContent(new ControlContent(pnlButtons = new Panel
					{
						BackColor = colors.WindowFooter,
						Parent    = this,
					},
					marginOverride: noMargin,
					horizontalContentAlignment: HorizontalContentAlignment.Stretch,
					verticalContentAlignment:   VerticalContentAlignment.Stretch),
					row: 2),
				]),
		};

		const int ButtonHeight = 23;
		const int TopMargin = 8;
		const int RightMargin = 6;

		_ = new ControlLayout(pnlButtons)
		{
			Content = new Grid(
				rows:
				[
					SizeSpec.Absolute(TopMargin),
					SizeSpec.Absolute(ButtonHeight),
					SizeSpec.Everything(),
				],
				columns:
				[
					SizeSpec.Everything(),
					SizeSpec.Absolute(75),
					SizeSpec.Absolute(RightMargin),
				],
				content:
				[
					new GridContent(new WidgetContent(_btnCancel, marginOverride: noMargin), column: 1, row: 1),
				]),
		};

		_lblAction.Parent   = this;
		_progressBar.Parent = this;
		_btnCancel.Parent   = pnlButtons;

		_btnCancel.Text = Resources.StrCancel;
		_lblAction.Text = string.Empty;
		_canCancel = true;

		ResumeLayout(performLayout: false);
		PerformLayout();
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(_cancellationTokenSource is not null)
		{
			_cancellationTokenSource.Dispose();
			_cancellationTokenSource = null;
		}
		StopWin7ProgressBar();
		base.Dispose(disposing);
	}

	private static void RunAsModalWithTask(IWin32Window? parent, Form dialog, Task task)
	{
		Assert.IsNotNull(task);

		if(task.IsCompleted || task.IsFaulted || task.IsCanceled)
		{
			if(!dialog.IsDisposed)
			{
				dialog.Dispose();
			}
			TaskUtility.PropagateFaultedStates(task);
		}
		var continuation = task.ContinueWith(
			t =>
			{
				if(!dialog.IsDisposed)
				{
					dialog.Close();
					dialog.Dispose();
				}
				TaskUtility.PropagateFaultedStates(t);
			},
			TaskScheduler.FromCurrentSynchronizationContext());
		dialog.ShowDialog(parent);
		TaskUtility.PropagateFaultedStates(task);
	}

	private static T RunAsModalWithTask<T>(IWin32Window? parent, Form dialog, Task<T> task)
	{
		Assert.IsNotNull(task);

		if(task.IsCompleted || task.IsFaulted || task.IsCanceled)
		{
			if(!dialog.IsDisposed)
			{
				dialog.Dispose();
			}
			return TaskUtility.UnwrapResult(task);
		}
		var continuation = task.ContinueWith(
			t =>
			{
				if(!dialog.IsDisposed)
				{
					dialog.Close();
					dialog.Dispose();
				}
				return TaskUtility.UnwrapResult(task);
			},
			TaskScheduler.FromCurrentSynchronizationContext());
		dialog.ShowDialog(parent);
		return TaskUtility.UnwrapResult(task);
	}

	public static void MonitorTaskAsModalWindow(IWin32Window? parent, string windowTitle, Func<IProgress<OperationProgress>, Task> func)
	{
		Verify.Argument.IsNotNull(func);

		var dialog = new ProgressForm()
		{
			Text = windowTitle,
		};
		dialog.SetCanCancel(false);
		var task = func(dialog);
		RunAsModalWithTask(parent, dialog, task);
	}

	public static void MonitorTaskAsModalWindow(IWin32Window? parent, string windowTitle, Func<IProgress<OperationProgress>, CancellationToken, Task> func)
	{
		Verify.Argument.IsNotNull(func);

		var dialog = new ProgressForm()
		{
			Text = windowTitle,
		};
		dialog.SetCanCancel(true);
		var task = func(dialog, dialog.CancellationToken);
		RunAsModalWithTask(parent, dialog, task);
	}

	public static T MonitorTaskAsModalWindow<T>(IWin32Window? parent, string windowTitle, Func<IProgress<OperationProgress>, CancellationToken, Task<T>> func)
	{
		Verify.Argument.IsNotNull(func);

		var dialog = new ProgressForm()
		{
			Text = windowTitle,
		};
		dialog.SetCanCancel(true);
		var task = func(dialog, dialog.CancellationToken);
		return RunAsModalWithTask(parent, dialog, task);
	}

	private void UpdateWin7ProgressBar()
	{
		if(GitterApplication.MainForm is { IsDisposed: false } form)
		{
			if(_progressBar.IsIndeterminate)
			{
				form.SetTaskbarProgressState(TbpFlag.Indeterminate);
			}
			else
			{
				form.SetTaskbarProgressState(TbpFlag.Normal);
				form.SetTaskbarProgressValue(
					(long)(_progressBar.Value - _progressBar.Minimum),
					(long)(_progressBar.Maximum - _progressBar.Minimum));
			}
		}
	}

	private void StopWin7ProgressBar()
	{
		if(GitterApplication.MainForm is { IsDisposed: false } form)
		{
			form.SetTaskbarProgressState(TbpFlag.NoProgress);
		}
	}

	/// <inheritdoc/>
	protected override void OnHandleCreated(EventArgs e)
	{
		this.EnableImmersiveDarkModeIfNeeded();
		base.OnHandleCreated(e);
	}

	/// <inheritdoc/>
	protected override void OnShown(EventArgs e)
	{
		base.OnShown(e);
		OnStarted();
		UpdateWin7ProgressBar();
	}

	/// <summary>
	/// Starts monitor.
	/// </summary>
	/// <param name="parent">Reference to parent window which is related to executing action.</param>
	/// <param name="context">Execution context.</param>
	/// <param name="blocking">Block calling thread until ProcessCompleted() is called.</param>
	public void Start(IWin32Window parent, IAsyncResult context, bool blocking)
	{
		_context = context;
		if(blocking)
		{
			ShowDialog(parent);
		}
		else
		{
			Show(parent);
		}
	}

	private CancellationToken CancellationToken
		=> _cancellationTokenSource is not null
			? _cancellationTokenSource.Token
			: CancellationToken.None;

	/// <summary>
	/// Executing action.
	/// </summary>
	/// <value></value>
	public IAsyncResult? CurrentContext => _context;

	/// <summary>
	/// Async action name.
	/// </summary>
	/// <value></value>
	public string ActionName
	{
		get => Text;
		set
		{
			if(!IsDisposed)
			{
				if(InvokeRequired)
				{
					try
					{
						BeginInvoke(new Action<string>(
							text =>
							{
								if(!IsDisposed)
								{
									Text = text;
								}
							}), value);
					}
					catch(ObjectDisposedException)
					{
					}
				}
				else
				{
					Text = value;
				}
			}
		}
	}

	/// <summary>Determines if action can be canceled.</summary>
	public bool CanCancel
	{
		get => _canCancel;
		set
		{
			if(!IsDisposed)
			{
				if(InvokeRequired)
				{
					try
					{
						BeginInvoke(new Action<bool>(SetCanCancel), value);
					}
					catch(ObjectDisposedException)
					{
					}
				}
				else
				{
					SetCanCancel(value);
				}
			}
		}
	}

	/// <summary>Returns <c>true</c> if operation must be terminated.</summary>
	public bool IsCancelRequested => _isCancelRequested;

	/// <summary>Sets the can cancel.</summary>
	/// <param name="value">if set to <c>true</c> [value].</param>
	private void SetCanCancel(bool value)
	{
		if(IsDisposed) return;
		if(_canCancel == value) return;

		_btnCancel.Enabled = value;
		_canCancel = value;
		if(value)
		{
			_cancellationTokenSource ??= new CancellationTokenSource();
		}
		else
		{
			if(_cancellationTokenSource is not null)
			{
				_cancellationTokenSource.Dispose();
				_cancellationTokenSource = null;
			}
		}
	}

	/// <summary>Sets the name of currently executed step.</summary>
	/// <param name="action">Step name.</param>
	public void SetAction(string action)
	{
		if(IsDisposed) return;

		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new Action<string>(SetAction), action);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			_lblAction.Text = action;
		}
	}

	/// <summary>
	/// Sets progress range information.
	/// </summary>
	/// <param name="min">Minimum.</param>
	/// <param name="max">Maximum.</param>
	public void SetProgressRange(int min, int max)
	{
		if(IsDisposed) return;

		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new Action<int, int>(SetProgressRange), min, max);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			_progressBar.IsIndeterminate = false;
			_progressBar.Minimum = min;
			_progressBar.Maximum = max;
			UpdateWin7ProgressBar();
		}
	}

	/// <summary>
	/// Sets progress range information and current step name.
	/// </summary>
	/// <param name="min">Minimum.</param>
	/// <param name="max">Maximum.</param>
	/// <param name="action">Step name.</param>
	public void SetProgressRange(int min, int max, string action)
	{
		if(IsDisposed) return;

		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new Action<int, int, string>(SetProgressRange), min, max, action);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			_progressBar.Minimum = min;
			_progressBar.Maximum = max;
			_lblAction.Text = action;
			UpdateWin7ProgressBar();
		}
	}

	/// <summary>
	/// Sets current progress.
	/// </summary>
	/// <param name="val">Progress.</param>
	public void SetProgress(int val)
	{
		if(IsDisposed) return;

		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new Action<int>(SetProgress), val);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			_progressBar.Value = val;
			UpdateWin7ProgressBar();
		}
	}

	/// <summary>
	/// Sets current progress and step name.
	/// </summary>
	/// <param name="val">Progress.</param>
	/// <param name="action">Step name.</param>
	public void SetProgress(int val, string action)
	{
		if(IsDisposed) return;

		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new Action<int, string>(SetProgress), val, action);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			_progressBar.Value = val;
			_lblAction.Text = action;
			UpdateWin7ProgressBar();
		}
	}

	private void SetProgressCore(int current, int maximum)
	{
		if(IsDisposed) return;

		_progressBar.IsIndeterminate = false;
		_progressBar.Minimum = 0;
		_progressBar.Maximum = maximum;
		_progressBar.Value = current;
		UpdateWin7ProgressBar();
	}

	/// <summary>
	/// Sets progress as unknown.
	/// </summary>
	public void SetProgressIndeterminate()
	{
		if(IsDisposed) return;

		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new Action(SetProgressIndeterminateCore));
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			SetProgressIndeterminateCore();
		}
	}

	private void SetProgressIndeterminateCore()
	{
		if(IsDisposed) return;

		if(!_progressBar.IsIndeterminate)
		{
			_progressBar.IsIndeterminate = true;
			UpdateWin7ProgressBar();
		}
	}

	private void SetActionNameCore(string actionName)
	{
		if(IsDisposed) return;

		_lblAction.Text = actionName;
	}

	/// <summary>
	/// Notifies that action is completed or canceled and monitor must be shut down.
	/// </summary>
	public void ProcessCompleted()
	{
		_context = null;
		if(IsDisposed) return;

		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new Action(ProcessCompleted));
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			Close();
		}
	}

	public void Report(OperationProgress progress)
	{
		if(IsDisposed) return;

		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new Action<OperationProgress>(ReportCore), progress);
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
		if(IsDisposed) return;

		SetActionNameCore(progress.ActionName);
		if(progress.IsIndeterminate)
		{
			SetProgressIndeterminate();
		}
		else
		{
			SetProgressCore(progress.CurrentProgress, progress.MaxProgress);
		}
	}

	private void OnCancelClick(object sender, EventArgs e)
	{
		if(!_isCancelRequested)
		{
			_btnCancel.Enabled = false;
			_isCancelRequested = true;
			try
			{
				_cancellationTokenSource?.Cancel();
			}
			catch(ObjectDisposedException)
			{
			}
			OnCanceled();
		}
	}
}
