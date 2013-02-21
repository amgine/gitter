namespace gitter.Framework
{
	using System;
	using System.Threading;

	internal sealed class AsyncFuncContext<TData, TResult> : AsyncContext
	{
		#region Data

		private readonly Func<TData, IAsyncProgressMonitor, TResult> _action;
		private readonly TData _data;
		private TResult _result;

		#endregion

		#region .ctor

		internal AsyncFuncContext(Func<TData, IAsyncProgressMonitor, TResult> action, TData data, IAsyncProgressMonitor monitor, bool ownsMonitor)
			: base(monitor, ownsMonitor)
		{
			_action = action;
			_data = data;
		}

		internal AsyncFuncContext(Func<TData, IAsyncProgressMonitor, TResult> action, TData data, IAsyncProgressMonitor monitor, bool ownsMonitor, AsyncCallback callback, object asyncState)
			: base(monitor, ownsMonitor)
		{
			_action = action;
			_data = data;
			SetCallback(callback, asyncState);
		}

		#endregion

		/// <summary>Creates thread to execute related action.</summary>
		/// <returns>Created thread.</returns>
		protected override Thread CreateThreadCore()
		{
			return new Thread(ThreadProc)
			{
				IsBackground = true,
			};
		}

		private void ThreadProc()
		{
			try
			{
				_result = _action(_data, Monitor);
			}
#if !DEBUG
			catch(Exception exc)
			{
				Exception = exc;
			}
#endif
			finally
			{
				NotifyCompleted();
			}
		}

		/// <summary>Input data for function.</summary>
		public TData Data
		{
			get { return _data; }
		}

		/// <summary>Result.</summary>
		public TResult Result
		{
			get { return _result; }
		}
	}
}
