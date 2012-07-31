namespace gitter.Updater
{
	using System;

	public interface IUpdateDriver
	{
		string Name { get; }

		IUpdateProcess CreateProcess(CommandLine cmdline);
	}

	public interface IUpdateProcess
	{
		void BeginUpdate(UpdateProcessMonitor monitor);

		void Update(UpdateProcessMonitor monitor);
	}
}
