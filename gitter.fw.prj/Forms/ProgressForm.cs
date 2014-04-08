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

namespace gitter.Framework
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	public partial class ProgressForm : Form, IProgress<OperationProgress>
	{
		#region Data

		private bool _canCancel;
		private IAsyncResult _context;
		private volatile bool _isCancelRequested;
		private CancellationTokenSource _cancellationTokenSource;

		#endregion

		#region Events

		/// <summary>
		/// Monitor raises this event to stop monitored action execution.
		/// </summary>
		public event EventHandler Canceled;

		/// <summary>
		/// Monitor raises this event when it is ready to receive Set() calls after Start() call.
		/// </summary>
		public event EventHandler Started;

		protected virtual void OnStarted()
		{
			var handler = Started;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		protected virtual void OnCanceled()
		{
			var handler = Canceled;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		/// <summary>Initializes a new instance of the <see cref="ProgressForm"/> class.</summary>
		public ProgressForm()
		{
			InitializeComponent();

			if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
			{
				Font = GitterApplication.FontManager.UIFont;
			}
			else
			{
				Font = SystemFonts.MessageBoxFont;
			}

			ClientSize = new Size(ClientSize.Width, _pnlContainer.Height + panel1.Height);

			_btnCancel.Text = Resources.StrCancel;

			_lblAction.Text = string.Empty;
			_canCancel = true;

			if(!Application.RenderWithVisualStyles)
			{
				_pnlContainer.BackColor = SystemColors.Control;
				_pnlLine.BackColor = SystemColors.Control;

				int d = _btnCancel.Top - _pnlContainer.Bottom;

				_pnlContainer.Height += d;
				ClientSize = new Size(ClientSize.Width, ClientSize.Height - d);
			}
		}

		private static void RunAsModalWithTask(IWin32Window parent, Form dialog, Task task)
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

		private static T RunAsModalWithTask<T>(IWin32Window parent, Form dialog, Task<T> task)
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

		public static void MonitorTaskAsModalWindow(IWin32Window parent, string windowTitle, Func<IProgress<OperationProgress>, Task> func)
		{
			Verify.Argument.IsNotNull(func, "func");

			var dialog = new ProgressForm()
			{
				Text = windowTitle,
			};
			dialog.SetCanCancel(false);
			var task = func(dialog);
			RunAsModalWithTask(parent, dialog, task);
		}

		public static void MonitorTaskAsModalWindow(IWin32Window parent, string windowTitle, Func<IProgress<OperationProgress>, CancellationToken, Task> func)
		{
			Verify.Argument.IsNotNull(func, "func");

			var dialog = new ProgressForm()
			{
				Text = windowTitle,
			};
			dialog.SetCanCancel(true);
			var task = func(dialog, dialog.CancellationToken);
			RunAsModalWithTask(parent, dialog, task);
		}

		public static T MonitorTaskAsModalWindow<T>(IWin32Window parent, string windowTitle, Func<IProgress<OperationProgress>, CancellationToken, Task<T>> func)
		{
			Verify.Argument.IsNotNull(func, "func");

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
			var form = GitterApplication.MainForm;
			if(form != null && !form.IsDisposed)
			{
				if(_progressBar.Style == ProgressBarStyle.Marquee)
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
			var form = GitterApplication.MainForm;
			if(form != null && !form.IsDisposed)
			{
				form.SetTaskbarProgressState(TbpFlag.NoProgress);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Shown"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			OnStarted();
			UpdateWin7ProgressBar();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Closed" /> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected override void OnClosed(EventArgs e)
		{
			StopWin7ProgressBar();
			base.OnClosed(e);
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
		{
			get
			{
				if(_cancellationTokenSource == null)
				{
					return CancellationToken.None;
				}
				return _cancellationTokenSource.Token;
			}
		}

		/// <summary>
		/// Executing action.
		/// </summary>
		/// <value></value>
		public IAsyncResult CurrentContext
		{
			get { return _context; }
		}

		/// <summary>
		/// Async action name.
		/// </summary>
		/// <value></value>
		public string ActionName
		{
			get { return Text; }
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

		/// <summary>Determines if action can be cancelled.</summary>
		public bool CanCancel
		{
			get { return _canCancel; }
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
		public bool IsCancelRequested
		{
			get { return _isCancelRequested; }
		}

		/// <summary>Sets the can cancel.</summary>
		/// <param name="value">if set to <c>true</c> [value].</param>
		private void SetCanCancel(bool value)
		{
			if(!IsDisposed)
			{
				if(_canCancel != value)
				{
					_btnCancel.Enabled = value;
					_canCancel = value;
					if(value)
					{
						if(_cancellationTokenSource == null)
						{
							_cancellationTokenSource = new CancellationTokenSource();
						}
					}
					else
					{
						if(_cancellationTokenSource != null)
						{
							_cancellationTokenSource.Dispose();
							_cancellationTokenSource = null;
						}
					}
				}
			}
		}

		/// <summary>Sets the name of currently executed step.</summary>
		/// <param name="action">Step name.</param>
		public void SetAction(string action)
		{
			if(!IsDisposed)
			{
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
		}

		/// <summary>
		/// Sets progress range information.
		/// </summary>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Maximum.</param>
		public void SetProgressRange(int min, int max)
		{
			if(!IsDisposed)
			{
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
					_progressBar.Style = ProgressBarStyle.Continuous;
					_progressBar.Minimum = min;
					_progressBar.Maximum = max;
					UpdateWin7ProgressBar();
				}
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
			if(!IsDisposed)
			{
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
		}

		/// <summary>
		/// Sets current progress.
		/// </summary>
		/// <param name="val">Progress.</param>
		public void SetProgress(int val)
		{
			if(!IsDisposed)
			{
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
		}

		/// <summary>
		/// Sets current progress and step name.
		/// </summary>
		/// <param name="val">Progress.</param>
		/// <param name="action">Step name.</param>
		public void SetProgress(int val, string action)
		{
			if(!IsDisposed)
			{
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
		}

		private void SetProgressCore(int current, int maximum)
		{
			if(IsDisposed) return;

			_progressBar.Style = ProgressBarStyle.Continuous;
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
			if(!IsDisposed)
			{
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
		}

		private void SetProgressIndeterminateCore()
		{
			if(IsDisposed) return;

			if(_progressBar.Style != ProgressBarStyle.Marquee)
			{
				_progressBar.Style = ProgressBarStyle.Marquee;
				UpdateWin7ProgressBar();
			}
		}

		private void SetActionNameCore(string actionName)
		{
			if(IsDisposed) return;

			_lblAction.Text = actionName;
		}

		/// <summary>
		/// Notifies that action is completed or cancelled and monitor must be shut down.
		/// </summary>
		public void ProcessCompleted()
		{
			_context = null;
			if(!IsDisposed)
			{
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
		}

		public void Report(OperationProgress progress)
		{
			if(!IsDisposed)
			{
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
				if(_cancellationTokenSource != null)
				{
					_cancellationTokenSource.Cancel();
				}
				OnCanceled();
			}
		}
	}
}
