namespace gitter.Updater
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	sealed class DeployDriver : IUpdateDriver
	{
		public string Name
		{
			get { return "deploy"; }
		}

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

		public DeployProcess(string source, string target)
		{
			_source = source;
			_target = target;
		}

		public void Begin(UpdateProcessMonitor monitor)
		{
			if(_currentProcess != null) throw new InvalidOperationException();

			monitor.Stage = "Initializing...";
			monitor.MaximumProgress = 10;
			Action<UpdateProcessMonitor> proc = UpdateProc;

			_currentProcess = proc.BeginInvoke(monitor, UpdateProcCallback, monitor);
		}

		private void UpdateProc(UpdateProcessMonitor monitor)
		{
			try
			{
				if(monitor.CancelRequested)
				{
					monitor.ReportCancelled();
					return;
				}
				monitor.Stage = "Closing running app instances...";
				Utility.KillAllGitterProcesses(_target);
				monitor.CurrentProgress = 3;
				if(monitor.CancelRequested)
				{
					monitor.ReportCancelled();
					return;
				}
				monitor.CanCancel = false;
				monitor.Stage = "Installing application...";
				Utility.CopyDirectoryContent(_source, _target);
				monitor.CurrentProgress = monitor.MaximumProgress;
				monitor.ReportSuccess();
			}
			catch(Exception exc)
			{
				monitor.ReportFailure("Unexpected error:\n" + exc.Message);
			}
		}

		public bool IsUpdating
		{
			get { return _currentProcess != null; }
		}

		private void UpdateProcCallback(IAsyncResult ar)
		{
			_currentProcess = null;
		}
	}
}
