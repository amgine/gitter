namespace gitter.Updater
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using Resources = gitter.Updater.Properties.Resources;

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
				_monitor.Stage = "Closing running app instances...";
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

		public bool IsUpdating
		{
			get { return _currentProcess != null; }
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
				_monitor.ReportFailure("Unexpected error:\n" + exc.Message);
			}
			_monitor = null;
			_currentProcess = null;
		}
	}
}
