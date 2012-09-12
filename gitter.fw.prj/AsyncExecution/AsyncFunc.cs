namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>Represents a function which can be invoked sync/async with progress monitoring.</summary>
	public static class AsyncFunc
	{
		/// <summary>Create new <see cref="AsyncFunc&lt;TData, TResult&gt;"/>.</summary>
		/// <typeparam name="TData">Type of action input data.</typeparam>
		/// <typeparam name="TResult">Function return type.</typeparam>
		/// <param name="data">Input data.</param>
		/// <param name="func">Function.</param>
		/// <param name="funcName">Function name.</param>
		/// <param name="funcDetails">Function details.</param>
		/// <param name="canCancel">Function supports cancelling.</param>
		/// <returns>Created <see cref="AsyncFunc&lt;TData, TResult&gt;"/>.</returns>
		public static AsyncFunc<TData, TResult> Create<TData, TResult>(TData data, Func<TData, IAsyncProgressMonitor, TResult> func, string funcName, string funcDetails, bool canCancel)
		{
			return new AsyncFunc<TData, TResult>(data, func, funcName, funcDetails, canCancel);
		}

		/// <summary>Create new <see cref="AsyncFunc&lt;TData, TResult&gt;"/>.</summary>
		/// <typeparam name="TData">Type of action input data.</typeparam>
		/// <typeparam name="TResult">Function return type.</typeparam>
		/// <param name="data">Input data.</param>
		/// <param name="func">Function.</param>
		/// <param name="funcName">Function name.</param>
		/// <param name="funcDetails">Function details.</param>
		/// <returns>Created <see cref="AsyncFunc&lt;TData, TResult&gt;"/>.</returns>
		public static AsyncFunc<TData, TResult> Create<TData, TResult>(TData data, Func<TData, IAsyncProgressMonitor, TResult> func, string funcName, string funcDetails)
		{
			return new AsyncFunc<TData, TResult>(data, func, funcName, funcDetails);
		}
	}

	/// <summary>Represents a function which can be invoked sync/async with progress monitoring.</summary>
	/// <typeparam name="TData">Function parameter.</typeparam>
	/// <typeparam name="TResult">Function result.</typeparam>
	public sealed class AsyncFunc<TData, TResult> : IAsyncFunc<TResult>
	{
		#region Data

		private readonly Func<TData, IAsyncProgressMonitor, TResult> _func;
		private readonly string _funcName;
		private readonly string _funcDetails;
		private readonly bool _canCancel;
		private readonly TData _data;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="AsyncFunc&lt;TData, TResult&gt;"/>.</summary>
		/// <param name="data">Function input data.</param>
		/// <param name="func">Function.</param>
		/// <param name="funcName">Function name.</param>
		/// <param name="funcDetails">Function details.</param>
		/// <param name="canCancel">Function can be cancelled.</param>
		public AsyncFunc(TData data, Func<TData, IAsyncProgressMonitor, TResult> func, string funcName, string funcDetails, bool canCancel)
		{
			Verify.Argument.IsNotNull(func, "func");

			_data = data;
			_funcName = funcName;
			_funcDetails = funcDetails;
			_func = func;
			_canCancel = canCancel;
		}

		/// <summary>Create <see cref="AsyncFunc&lt;TData, TResult&gt;"/>.</summary>
		/// <param name="data">Function input data.</param>
		/// <param name="func">Function.</param>
		/// <param name="funcName">Function name.</param>
		/// <param name="funcDetails">Function details.</param>
		public AsyncFunc(TData data, Func<TData, IAsyncProgressMonitor, TResult> func, string funcName, string funcDetails)
			: this(data, func, funcName, funcDetails, false)
		{
		}

		#endregion

		#region Invoke()

		private TResult InvokeCore(IWin32Window parent, IAsyncProgressMonitor monitor, bool ownsMonitor)
		{
			monitor.ActionName = _funcName;
			monitor.CanCancel = _canCancel;
			monitor.SetAction(_funcDetails);
			monitor.SetProgressIntermediate();

			TResult result;

			using(var context = new AsyncFuncContext<TData, TResult>(_func, _data, monitor, false))
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

				result = context.Result;
			}

			return result;
		}

		/// <summary>Executes action synchronously.</summary>
		/// <typeparam name="TMonitor">Type of monitor to use.</typeparam>
		/// <param name="parent">Parent window reference.</param>
		/// <returns>Result.</returns>
		public TResult Invoke<TMonitor>(IWin32Window parent)
			where TMonitor : IAsyncProgressMonitor, new()
		{
			return InvokeCore(parent, new TMonitor(), true);
		}

		/// <summary>Executes action synchronously.</summary>
		/// <returns>Result.</returns>
		public TResult Invoke()
		{
			using(var mon = new NullMonitor())
			{
				return InvokeCore(null, mon, false);
			}
		}

		/// <summary>Executes action synchronously.</summary>
		/// <param name="parent">Parent window reference.</param>
		/// <param name="monitor">Monitor to use.</param>
		/// <returns>Result.</returns>
		public TResult Invoke(IWin32Window parent, IAsyncProgressMonitor monitor)
		{
			Verify.Argument.IsNotNull(monitor, "monitor");

			return InvokeCore(parent, monitor, false);
		}

		public IAsyncResult BeginInvoke<TMonitor>(IWin32Window parent, AsyncCallback callback, object asyncState)
			where TMonitor : IAsyncProgressMonitor, new()
		{
			var monitor = new TMonitor()
			{
				ActionName = _funcName,
				CanCancel = _canCancel,
			};
			monitor.SetAction(_funcDetails);
			monitor.SetProgressIntermediate();

			var context = new AsyncFuncContext<TData, TResult>(_func, _data, monitor, true, callback, asyncState);

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

			monitor.ActionName = _funcName;
			monitor.CanCancel = _canCancel;
			monitor.SetAction(_funcDetails);
			monitor.SetProgressIntermediate();

			var context = new AsyncFuncContext<TData, TResult>(_func, _data, monitor, false, callback, asyncState);

			monitor.Started += OnMonitorStarted;

			if(_canCancel)
			{
				monitor.Cancelled += OnMonitorCancelled;
			}

			monitor.Start(parent, context, false);

			return context;
		}

		public TResult EndInvoke(IAsyncResult asyncResult)
		{
			Verify.Argument.IsNotNull(asyncResult, "asyncResult");
			var context = asyncResult as AsyncFuncContext<TData, TResult>;
			Verify.Argument.IsTrue(context != null, "asyncResult",
				"Supplied IAsyncResult was not generated by this AsyncFunc.");
			Verify.Argument.IsFalse(context.IsDisposed, "asyncResult",
				"Supplied IAsyncResult is disposed.");

			if(!context.IsCompleted)
			{
				context.Completed.WaitOne();
			}
			if(_canCancel)
			{
				context.Monitor.Cancelled -= OnMonitorCancelled;
			}
			var exc = context.Exception;
			if(exc != null)
			{
				context.Dispose();
				exc.Data["OriginalStackTrace"] = exc.StackTrace;
				throw exc;
			}
			else
			{
				var res = context.Result;
				context.Dispose();
				return res;
			}
		}

		#endregion

		#region Properties

		public TData Data
		{
			get { return _data; }
		}

		public Func<TData, IAsyncProgressMonitor, TResult> Func
		{
			get { return _func; }
		}

		public string FuncName
		{
			get { return _funcName; }
		}

		public string FuncDetails
		{
			get { return _funcDetails; }
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

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="AsyncFunc&lt;TData, TResult&gt;"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="AsyncFunc&lt;TData, TResult&gt;"/>.</returns>
		public override string ToString()
		{
			return _funcName;
		}

		#endregion
	}
}
