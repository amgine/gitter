namespace gitter.Framework
{
	using System;
	using System.Threading;

	internal sealed class AsyncActionContext<TData> : AsyncContext
	{
		#region Data

		private readonly Action<TData, IAsyncProgressMonitor> _action;
		private readonly TData _data;

		#endregion

		#region .ctor

		internal AsyncActionContext(Action<TData, IAsyncProgressMonitor> action, TData data, IAsyncProgressMonitor monitor, bool ownsMonitor)
			: base(monitor, ownsMonitor)
		{
			_action = action;
			_data = data;
		}

		internal AsyncActionContext(Action<TData, IAsyncProgressMonitor> action, TData data, IAsyncProgressMonitor monitor, bool ownsMonitor, AsyncCallback callback, object asyncState)
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
				_action(_data, Monitor);
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

		/// <summary>Input data for action.</summary>
		public TData Data
		{
			get { return _data; }
		}
	}
}
