#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.IO;
using System.Threading;

using Resources = gitter.Updater.Properties.Resources;

abstract class UpdateProcessBase(string targetDirectory) : IUpdateProcess
{
	protected const string MainBinaryName = "gitter.exe";

	public string TargetDirectory { get; } = targetDirectory;

	protected UpdateProcessMonitor Monitor { get; private set; } = default!;

	public bool IsUpdating { get; set; }

	protected virtual void NotifyInitializing(UpdateProcessMonitor monitor)
	{
		monitor.Stage = Resources.StrInitializing + "...";
	}

	public void BeginUpdate(UpdateProcessMonitor monitor)
	{
		if(IsUpdating) throw new InvalidOperationException();

		NotifyInitializing(monitor);

		Monitor = monitor;
		ThreadPool.QueueUserWorkItem(_ => Update(monitor), null);
	}

	public void Update(UpdateProcessMonitor monitor)
	{
		NotifyInitializing(monitor);

		IsUpdating = true;
		Monitor = monitor;
		try
		{
			UpdateProc();
		}
		catch(Exception exc)
		{
			if(Monitor.CancelRequested)
			{
				Monitor.ReportCancelled();
			}
			else
			{
				Monitor.ReportFailure("Unexpected error:\n" + exc.Message);
			}
		}
		finally
		{
			Cleanup();
			Monitor = null!;
			IsUpdating = false;
		}
	}

	protected abstract void UpdateProc();

	protected abstract void Cleanup();

	protected void KillAllGitterProcesses()
	{
		Utility.KillAllGitterProcesses(TargetDirectory);
	}

	protected void StartApplication()
	{
		try
		{
			var exeName = Path.Combine(TargetDirectory, MainBinaryName);
			Utility.StartApplication(exeName);
		}
		catch
		{
		}
	}

	protected void InstallApplication(string from)
	{
		var m = Monitor.MaximumProgress;

		Monitor.Stage = "Installing application...";
		Monitor.CurrentProgress = m - 2;
		KillAllGitterProcesses();
		if(Monitor.CancelRequested)
		{
			Monitor.ReportCancelled();
			return;
		}
		Monitor.CanCancel = false;
		if(!Utility.Deploy(from, TargetDirectory))
		{
			Monitor.ReportFailure("Failed to deploy build results.");
		}

		Monitor.Stage = "Cleaning up temporary files...";
		Monitor.CurrentProgress = m - 1;
		Cleanup();

		Monitor.CurrentProgress = m;
		Monitor.Stage = "Launching application...";
		StartApplication();
		Monitor.ReportSuccess();
	}
}
