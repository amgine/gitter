namespace gitter.Framework
{
	using System;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>Dummy progress monitor.</summary>
	public sealed class NullMonitor : IAsyncProgressMonitor, IDisposable
	{
		#region Data

		private string _actionName;
		private bool _canCancel;
		private ManualResetEvent _evExit;
		private IAsyncResult _currentAction;

		#endregion

		/// <summary>Create <see cref="NullMonitor"/>.</summary>
		public NullMonitor()
		{
		}

		~NullMonitor()
		{
			Dispose(false);
		}

		#region IAsyncProgressMonitor Members

		public event EventHandler Cancelled
		{
			add { }
			remove { }
		}

		public event EventHandler Started;

		public IAsyncResult CurrentContext
		{
			get { return _currentAction; }
		}

		public string ActionName
		{
			get { return _actionName; }
			set { _actionName = value; }
		}

		public bool CanCancel
		{
			get { return _canCancel; }
			set { _canCancel = value; }
		}

		public void Start(IWin32Window parent, IAsyncResult context, bool blocking)
		{
			_currentAction = context;
			var handler = Started;
			if(handler != null) handler(this, EventArgs.Empty);
			if(blocking)
			{
				_evExit = new ManualResetEvent(false);
				_evExit.WaitOne();
			}
		}

		public void SetAction(string action)
		{
		}

		public void SetProgressRange(int min, int max)
		{
		}

		public void SetProgressRange(int min, int max, string action)
		{
		}

		public void SetProgress(int val)
		{
		}

		public void SetProgress(int val, string action)
		{
		}

		public void SetProgressIntermediate()
		{
		}

		public void ProcessCompleted()
		{
			if(_evExit != null)
				_evExit.Set();
			_currentAction = null;
		}

		#endregion
	
		#region IDisposable Members

		private void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_evExit != null)
				{
					_evExit.Close();
					_evExit = null;
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
