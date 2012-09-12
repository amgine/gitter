namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>Represents a method which can be invoked sync/async with progress monitoring.</summary>
	public static class AsyncAction
	{
		/// <summary>Create new <see cref="AsyncAction&lt;TData&gt;"/>.</summary>
		/// <typeparam name="TData">Type of action input data.</typeparam>
		/// <param name="data">Input data.</param>
		/// <param name="action">Action.</param>
		/// <param name="actionName">Action name.</param>
		/// <param name="actionDetails">Action details.</param>
		/// <param name="canCancel">Acton supports cancelling.</param>
		/// <returns>Created <see cref="AsyncAction&lt;TData&gt;"/>.</returns>
		public static AsyncAction<TData> Create<TData>(TData data, Action<TData, IAsyncProgressMonitor> action, string actionName, string actionDetails, bool canCancel)
		{
			return new AsyncAction<TData>(data, action, actionName, actionDetails, canCancel);
		}

		/// <summary>Create new <see cref="AsyncAction&lt;TData&gt;"/>.</summary>
		/// <typeparam name="TData">Type of action input data.</typeparam>
		/// <param name="data">Input data.</param>
		/// <param name="action">Action.</param>
		/// <param name="actionName">Action name.</param>
		/// <param name="actionDetails">Action details.</param>
		/// <returns>Created <see cref="AsyncAction&lt;TData&gt;"/>.</returns>
		public static AsyncAction<TData> Create<TData>(TData data, Action<TData, IAsyncProgressMonitor> action, string actionName, string actionDetails)
		{
			return new AsyncAction<TData>(data, action, actionName, actionDetails);
		}
	}

	/// <summary>Represents a method which can be invoked sync/async with progress monitoring.</summary>
	/// <typeparam name="TData">Method parameter type.</typeparam>
	public sealed class AsyncAction<TData> : IAsyncAction
	{
		#region Data

		private readonly Action<TData, IAsyncProgressMonitor> _action;
		private readonly string _actionName;
		private readonly string _actionDetails;
		private readonly bool _canCancel;
		private readonly TData _data;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="AsyncAction&ltTData&gt;"/>.</summary>
		/// <param name="data">Action input data.</param>
		/// <param name="action">Action.</param>
		/// <param name="actionName">Action name.</param>
		/// <param name="actionDetails">Action details.</param>
		/// <param name="canCancel">Action can be cancelled.</param>
		public AsyncAction(TData data, Action<TData, IAsyncProgressMonitor> action, string actionName, string actionDetails, bool canCancel)
		{
			Verify.Argument.IsNotNull(action, "action");

			_data = data;
			_actionName = actionName;
			_actionDetails = actionDetails;
			_action = action;
			_canCancel = canCancel;
		}

		/// <summary>Create <see cref="AsyncAction&ltTData&gt;"/>.</summary>
		/// <param name="data">Action input data.</param>
		/// <param name="action">Action.</param>
		/// <param name="actionName">Action name.</param>
		/// <param name="actionDetails">Action details.</param>
		public AsyncAction(TData data, Action<TData, IAsyncProgressMonitor> action, string actionName, string actionDetails)
			: this(data, action, actionName, actionDetails, false)
		{
		}

		#endregion

		#region Invoke()

		private void InvokeCore(IWin32Window parent, IAsyncProgressMonitor monitor, bool ownsMonitor)
		{
			monitor.ActionName = _actionName;
			monitor.CanCancel = _canCancel;
			monitor.SetAction(_actionDetails);
			monitor.SetProgressIntermediate();

			using(var context = new AsyncActionContext<TData>(_action, _data, monitor, ownsMonitor))
			{
				monitor.Started += OnMonitorStarted;

				if(_canCancel)
				{
					monitor.Cancelled += OnMonitorCancelled;
				}

				monitor.Start(parent, context, true);

				if(_canCancel)
				{
					monitor.Cancelled -= OnMonitorCancelled;
				}

				var exc = context.Exception;
				if(exc != null)
				{
					exc.Data["OriginalStackTrace"] = exc.StackTrace;
					throw exc;
				}
			}
		}

		/// <summary>Executes action synchronously.</summary>
		/// <typeparam name="TMonitor">Type of monitor to use.</typeparam>
		/// <param name="parent">Parent window reference.</param>
		/// <returns>Result.</returns>
		public void Invoke<TMonitor>(IWin32Window parent)
			where TMonitor : IAsyncProgressMonitor, new()
		{
			InvokeCore(parent, new TMonitor(), true);
		}

		/// <summary>Executes action synchronously.</summary>
		/// <returns>Result.</returns>
		public void Invoke()
		{
			using(var mon = new NullMonitor())
			{
				InvokeCore(null, mon, false);
			}
		}

		/// <summary>Executes action synchronously.</summary>
		/// <param name="parent">Parent window reference.</param>
		/// <param name="monitor">Monitor to use.</param>
		/// <returns>Result.</returns>
		public void Invoke(IWin32Window parent, IAsyncProgressMonitor monitor)
		{
			Verify.Argument.IsNotNull(monitor, "monitor");

			InvokeCore(parent, monitor, false);
		}

		public IAsyncResult BeginInvoke<TMonitor>(IWin32Window parent, AsyncCallback callback, object asyncState)
			where TMonitor : IAsyncProgressMonitor, new()
		{
			var monitor = new TMonitor()
			{
				ActionName = _actionName,
				CanCancel = _canCancel,
			};
			monitor.SetAction(_actionDetails);
			monitor.SetProgressIntermediate();

			var context = new AsyncActionContext<TData>(_action, _data, monitor, true, callback, asyncState);

			monitor.Started += OnMonitorStarted;
			if(_canCancel)
			{
				monitor.Cancelled += OnMonitorCancelled;
			}

			monitor.Start(parent, context, false);

			return context;
		}

		public IAsyncResult BeginInvoke(IWin32Window parent, IAsyncProgressMonitor monitor, AsyncCallback callback, object asyncState)
		{
			Verify.Argument.IsNotNull(monitor, "monitor");

			monitor.ActionName = _actionName;
			monitor.CanCancel = _canCancel;
			monitor.SetAction(_actionDetails);
			monitor.SetProgressIntermediate();

			var context = new AsyncActionContext<TData>(_action, _data, monitor, false, callback, asyncState);

			monitor.Started += OnMonitorStarted;
			if(_canCancel)
			{
				monitor.Cancelled += OnMonitorCancelled;
			}

			monitor.Start(parent, context, false);

			return context;
		}

		public void EndInvoke(IAsyncResult asyncResult)
		{
			Verify.Argument.IsNotNull(asyncResult, "asyncResult");
			var context = asyncResult as AsyncActionContext<TData>;
			Verify.Argument.IsTrue(context != null, "asyncResult",
				"Supplied IAsyncResult was not generated by this AsyncAction.");
			Verify.Argument.IsFalse(context.IsDisposed, "asyncResult",
				"Supplied IAsyncResult is disposed.");

			var monitor = context.Monitor;
			if(!context.IsCompleted)
			{
				context.Completed.WaitOne();
			}
			if(_canCancel)
			{
				monitor.Cancelled -= OnMonitorCancelled;
			}
			var exc = context.Exception;
			context.Dispose();
			if(exc != null)
			{
				exc.Data["OriginalStackTrace"] = exc.StackTrace;
				throw exc;
			}
		}

		#endregion

		#region Properties
		
		/// <summary>Action input data.</summary>
		public TData Data
		{
			get { return _data; }
		}

		/// <summary>Action.</summary>
		public Action<TData, IAsyncProgressMonitor> Action
		{
			get { return _action; }
		}

		/// <summary>Action name.</summary>
		public string ActionName
		{
			get { return _actionName; }
		}

		/// <summary>Action details.</summary>
		public string ActionDetails
		{
			get { return _actionDetails; }
		}

		#endregion

		#region Event Handlers

		private static void OnMonitorStarted(object sender, EventArgs e)
		{
			var monitor = (IAsyncProgressMonitor)sender;
			var context = (AsyncContext)monitor.CurrentContext;
			monitor.Started -= OnMonitorStarted;
			var thread = context.CreateThread();
			thread.Start();
		}

		private static void OnMonitorCancelled(object sender, EventArgs e)
		{
			var monitor = (IAsyncProgressMonitor)sender;
			var context = (AsyncContext)monitor.CurrentContext;
			context.Cancelled.Set();
		}

		#endregion

		#region Overrides

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="AsyncAction&ltTData&gt;"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="AsyncAction&ltTData&gt;"/>.</returns>
		public override string ToString()
		{
			return _actionName;
		}

		#endregion
	}
}
