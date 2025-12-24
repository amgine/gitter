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

namespace gitter.Updater;

using System;

public sealed class UpdateProcessMonitor
{
	public event EventHandler? StageChanged;
	public event EventHandler? MaximumProgressChanged;
	public event EventHandler? CurrentProgressChanged;
	public event EventHandler? UpdateCancelled;
	public event EventHandler<UpdateFailedEventArgs>? UpdateFailed;
	public event EventHandler? UpdateSuccessful;
	public event EventHandler? CanCancelChanged;

	private string? _stage;
	private int _maximumProgress;
	private int _currentProgress;
	private bool _canCancel;

	public UpdateProcessMonitor()
	{
		_canCancel = true;
	}

	public string? Stage
	{
		get => _stage;
		set
		{
			_stage = value;
			StageChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public int MaximumProgress
	{
		get => _maximumProgress;
		set
		{
			_maximumProgress = value;
			MaximumProgressChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public int CurrentProgress
	{
		get => _currentProgress;
		set
		{
			_currentProgress = value;
			CurrentProgressChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public void ReportFailure(string error)
	{
		UpdateFailed?.Invoke(this, new UpdateFailedEventArgs(error));
	}

	public void ReportSuccess()
	{
		UpdateSuccessful?.Invoke(this, EventArgs.Empty);
	}

	public void ReportCancelled()
	{
		UpdateCancelled?.Invoke(this, EventArgs.Empty);
	}

	public bool CanCancel
	{
		get => _canCancel;
		set
		{
			if(_canCancel != value)
			{
				_canCancel = value;
				CanCancelChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public bool CancelRequested { get; set; }
}

public sealed class UpdateFailedEventArgs(string message) : EventArgs
{
	public string Message { get; } = message;
}
