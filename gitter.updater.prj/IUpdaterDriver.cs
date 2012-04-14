namespace gitter.Updater
{
	public interface IUpdateDriver
	{
		string Name { get; }

		IUpdateProcess CreateProcess(CommandLine cmdline);
	}

	public interface IUpdateProcess
	{
		void Begin(UpdateProcessMonitor monitor);
	}
}
