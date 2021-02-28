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

namespace gitter.Updater
{
	using System;
	using System.IO;

	using Resources = gitter.Updater.Properties.Resources;

	abstract class UpdateProcessBase : IUpdateProcess
	{
		protected const string MainBinaryName = "gitter.exe";

		private IAsyncResult _currentProcess;

		protected UpdateProcessBase(string targetDirectory)
		{
			TargetDirectory = targetDirectory;
		}

		public string TargetDirectory { get; }

		protected UpdateProcessMonitor Monitor { get; private set; }

		public bool IsUpdating => _currentProcess != null;

		protected virtual void NotifyInitializing(UpdateProcessMonitor monitor)
		{
			monitor.Stage = Resources.StrInitializing + "...";
		}

		public void BeginUpdate(UpdateProcessMonitor monitor)
		{
			if(_currentProcess != null) throw new InvalidOperationException();

			NotifyInitializing(monitor);

			Action proc = UpdateProc;
			Monitor = monitor;
			_currentProcess = proc.BeginInvoke(UpdateProcCallback, proc);
		}

		public void Update(UpdateProcessMonitor monitor)
		{
			NotifyInitializing(monitor);

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
				Monitor = null;
			}
		}

		private void UpdateProcCallback(IAsyncResult ar)
		{
			var proc = (Action)ar.AsyncState;
			try
			{
				proc.EndInvoke(ar);
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
			}
			Monitor = null;
			_currentProcess = null;
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
}
