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
