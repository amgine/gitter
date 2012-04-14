namespace gitter.Updater
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public sealed class UpdateProcessMonitor
	{
		public event EventHandler StageChanged;
		public event EventHandler MaximumProgressChanged;
		public event EventHandler CurrentProgressChanged;
		public event EventHandler UpdateCancelled;
		public event EventHandler<UpdateFailedEventArgs> UpdateFailed;
		public event EventHandler UpdateSuccessful;
		public event EventHandler CanCancelChanged;

		private string _stage;
		private int _maximumProgress;
		private int _currentProgress;
		private bool _canCancel;

		public UpdateProcessMonitor()
		{
			_canCancel = true;
		}

		public string Stage
		{
			get { return _stage; }
			set
			{
				_stage = value;
				var handler = StageChanged;
				if(handler != null) handler(this, EventArgs.Empty);
			}
		}

		public int MaximumProgress
		{
			get { return _maximumProgress; }
			set
			{
				_maximumProgress = value;
				var handler = MaximumProgressChanged;
				if(handler != null) handler(this, EventArgs.Empty);
			}
		}

		public int CurrentProgress
		{
			get { return _currentProgress; }
			set
			{
				_currentProgress = value;
				var handler = CurrentProgressChanged;
				if(handler != null) handler(this, EventArgs.Empty);
			}
		}

		public void ReportFailure(string error)
		{
			var handler = UpdateFailed;
			if(handler != null) handler(this, new UpdateFailedEventArgs(error));
		}

		public void ReportSuccess()
		{
			var handler = UpdateSuccessful;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		public void ReportCancelled()
		{
			var handler = UpdateCancelled;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		public bool CanCancel
		{
			get { return _canCancel; }
			set
			{
				if(_canCancel != value)
				{
					_canCancel = value;
					var handler = CanCancelChanged;
					if(handler != null) handler(this, EventArgs.Empty);
				}
			}
		}

		public bool CancelRequested
		{
			get;
			set;
		}
	}

	public sealed class UpdateFailedEventArgs : EventArgs
	{
		private readonly string _message;

		public UpdateFailedEventArgs(string message)
		{
			_message = message;
		}

		public string Message
		{
			get { return _message; }
		}
	}
}
