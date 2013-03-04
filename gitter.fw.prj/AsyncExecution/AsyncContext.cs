namespace gitter.Framework
{
	using System;
	using System.Threading;

	/// <summary>Async execution thread hoster.</summary>
	internal abstract class AsyncContext : IAsyncResult, IDisposable
	{
		#region Data

		private readonly IAsyncProgressMonitor _monitor;
		private readonly bool _ownsMonitor;
		private volatile bool _isCompleted;
		private Thread _thread;
		private bool _isDisposed;
		private volatile Exception _exception;
		private ManualResetEvent _cancel;
		private ManualResetEvent _completed;

		private AsyncCallback _callback;
		private object _asyncState;

		#endregion

		/// <summary>Creates thread to execute related action.</summary>
		/// <returns>Created thread.</returns>
		protected abstract Thread CreateThreadCore();

		internal Thread CreateThread()
		{
			return _thread = CreateThreadCore();
		}

		#region .ctor

		internal AsyncContext(IAsyncProgressMonitor progressMonitor, bool ownsMonitor)
		{
			_monitor = progressMonitor;
			_ownsMonitor = ownsMonitor;
			_cancel = new ManualResetEvent(false);
			_completed = new ManualResetEvent(false);
		}

		#endregion

		~AsyncContext()
		{
			Dispose(false);
		}

		protected void SetCallback(AsyncCallback callback, object state)
		{
			_callback = callback;
			_asyncState = state;
		}

		protected void NotifyCompleted()
		{
			if(_completed != null)
			{
				_completed.Set();
			}
			_isCompleted = true;
			if(_monitor != null)
			{
				_monitor.ProcessCompleted();
			}
			if(_callback != null)
			{
				_callback(this);
			}
		}

		/// <summary>Thread which executes action.</summary>
		internal Thread Thread
		{
			get { return _thread; }
		}

		/// <summary>Use this monitor to report execution progress.</summary>
		public IAsyncProgressMonitor Monitor
		{
			get { return _monitor; }
		}

		/// <summary>Check this to see if action is requested to terminate by monitor.</summary>
		public ManualResetEvent Canceled
		{
			get { return _cancel; }
		}

		/// <summary>Completed event handle.</summary>
		internal ManualResetEvent Completed
		{
			get { return _completed; }
		}

		/// <summary>Determies if action is completed.</summary>
		public bool IsCompleted
		{
			get { return _isCompleted; }
		}

		/// <summary>Exception thrown by executing action.</summary>
		public Exception Exception
		{
			get { return _exception; }
			protected set { _exception = value; }
		}

		#region IAsyncResult Members

		/// <summary>
		/// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
		/// </summary>
		/// <value></value>
		/// <returns>A user-defined object that qualifies or contains information about an asynchronous operation.</returns>
		object IAsyncResult.AsyncState
		{
			get { return _asyncState; }
		}

		/// <summary>
		/// Gets a <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.
		/// </summary>
		/// <value></value>
		/// <returns>A <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.</returns>
		WaitHandle IAsyncResult.AsyncWaitHandle
		{
			get { return _completed; }
		}

		/// <summary>
		/// Gets a value that indicates whether the asynchronous operation completed synchronously.
		/// </summary>
		/// <value></value>
		/// <returns>true if the asynchronous operation completed synchronously; otherwise, false.</returns>
		bool IAsyncResult.CompletedSynchronously
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value that indicates whether the asynchronous operation has completed.
		/// </summary>
		/// <value></value>
		/// <returns>true if the operation is complete; otherwise, false.</returns>
		bool IAsyncResult.IsCompleted
		{
			get { return _isCompleted; }
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Gets a value indicating whether this instance is disposed.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
		/// </value>
		public bool IsDisposed
		{
			get { return _isDisposed; }
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if(disposing && !_isDisposed)
			{
				_isDisposed = true;
				if(_ownsMonitor)
				{
					var disp = _monitor as IDisposable;
					if(disp != null) disp.Dispose();
				}
				_callback = null;
				_asyncState = null;
				if(_cancel != null)
				{
					_cancel.Close();
					_cancel = null;
				}
				if(_completed != null)
				{
					_completed.Close();
					_completed = null;
				}
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
