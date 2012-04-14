namespace gitter.UAC
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Runtime.Remoting;
	using System.Runtime.Remoting.Channels;
	using System.Runtime.Remoting.Channels.Ipc;

	static class Program
	{
		private static RemoteExecutor _executor;
		private const string RemotingChannelName = "gitter.UAC";
		private const string RemotingObjectName = "RemoteExecutor.rem";

		/// <summary>The main entry point for the application.</summary>
		[STAThread]
		private static void Main()
		{
			bool isFirstInstance;
			using(var mutex = new Mutex(true, "gitter-uac-instance", out isFirstInstance))
			{
				if(isFirstInstance)
				{
					var args = Environment.GetCommandLineArgs();
					if(args == null || args.Length < 2 || args[1] != "--remoting")
						return;

					ChannelServices.RegisterChannel(
						new IpcChannel(
							new Dictionary<string, string>
							{
								{ "portName", RemotingChannelName }
							},
							new BinaryClientFormatterSinkProvider(),
							new BinaryServerFormatterSinkProvider()
							{
								TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full,
							}),
						false);

					RemotingConfiguration.RegisterWellKnownServiceType(
						typeof(RemoteExecutor), RemotingObjectName, WellKnownObjectMode.Singleton);

					_executor = (RemoteExecutor)Activator.GetObject(typeof(RemoteExecutor),
						string.Format(@"ipc://{0}/{1}", RemotingChannelName, RemotingObjectName));
					_executor.ExitEvent.WaitOne();
					_executor.ExitEvent.Close();
				}
			}
		}
	}
}
