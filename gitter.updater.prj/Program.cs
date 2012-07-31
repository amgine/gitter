namespace gitter.Updater
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows.Forms;
	using System.Threading;

	static class Program
	{
		/// <summary>Available updaters.</summary>
		private static IEnumerable<IUpdateDriver> UpdateDrivers =
			new IUpdateDriver[]
			{
				new GitUpdateDriver(),
				new DeployDriver(),
			};

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			var cmdline = new CommandLine();
			bool singleInstance;
			Semaphore s = null;
			if(!cmdline.IsDefined("forcenewinstance"))
			{
				s = new Semaphore(0, 1, "gitter-updater", out singleInstance);
			}
			else
			{
				singleInstance = true;
			}
			try
			{
				if(singleInstance)
				{
					var driverName = cmdline["driver"];
					if(!string.IsNullOrEmpty(driverName))
					{
						var driver = UpdateDrivers.FirstOrDefault(d => d.Name == driverName);
						if(driver != null)
						{
							var process = driver.CreateProcess(cmdline);
							if(process != null)
							{
								if(cmdline.IsDefined("hidden"))
								{
									var monitor = new UpdateProcessMonitor();
									process.Update(monitor);
								}
								else
								{
									Application.EnableVisualStyles();
									Application.SetCompatibleTextRenderingDefault(false);
									Application.Run(new MainForm(process));
								}
							}
						}
					}
				}
			}
			finally
			{
				if(s != null)
				{
					s.Dispose();
				}
			}
		}
	}
}
