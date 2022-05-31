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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Resources = gitter.Updater.Properties.Resources;

sealed class DeployDriver : IUpdateDriver
{
	public string Name => "deploy";

	public IUpdateProcess CreateProcess(CommandLine cmdline)
	{
		var source = cmdline["source"];
		if(source == null) return null;
		var target = cmdline["target"];
		if(target == null) return null;
		return new DeployProcess(source, target);
	}
}

sealed class DeployProcess : IUpdateProcess
{
	private readonly string _source;
	private readonly string _target;
	private IAsyncResult _currentProcess;
	private UpdateProcessMonitor _monitor;

	public DeployProcess(string source, string target)
	{
		_source = source;
		_target = target;
	}

	public void BeginUpdate(UpdateProcessMonitor monitor)
	{
		if(_currentProcess != null) throw new InvalidOperationException();

		monitor.Stage = Resources.StrInitializing + "...";
		monitor.MaximumProgress = 10;
		Action proc = UpdateProc;

		_monitor = monitor;
		_currentProcess = proc.BeginInvoke(UpdateProcCallback, proc);
	}

	public void Update(UpdateProcessMonitor monitor)
	{
		_monitor = monitor;
		UpdateProc();
		_monitor = null;
	}

	private void UpdateProc()
	{
		try
		{
			if(_monitor.CancelRequested)
			{
				_monitor.ReportCancelled();
				return;
			}
			_monitor.Stage = "Closing running application instances...";
			Utility.KillAllGitterProcesses(_target);
			_monitor.CurrentProgress = 3;
			if(_monitor.CancelRequested)
			{
				_monitor.ReportCancelled();
				return;
			}
			_monitor.CanCancel = false;
			_monitor.Stage = "Installing application...";
			Utility.CopyDirectoryContent(_source, _target);
			_monitor.CurrentProgress = _monitor.MaximumProgress;
			_monitor.ReportSuccess();
		}
		catch(Exception exc)
		{
			_monitor.ReportFailure("Unexpected error:\n" + exc.Message);
		}
	}

	public bool IsUpdating => _currentProcess != null;

	private void UpdateProcCallback(IAsyncResult ar)
	{
		var proc = (Action)ar.AsyncState;
		try
		{
			proc.EndInvoke(ar);
		}
		catch(Exception exc)
		{
			_monitor.ReportFailure("Unexpected error:\n" + exc.Message);
		}
		_monitor = null;
		_currentProcess = null;
	}
}
