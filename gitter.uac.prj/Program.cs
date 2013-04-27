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
