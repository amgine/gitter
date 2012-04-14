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
			bool singleInstance = false;
			using(var s = new Semaphore(0, 1, "gitter-updater", out singleInstance))
			{
				if(singleInstance)
				{
					var cmdline = new CommandLine();
					var driverName = cmdline["driver"];
					if(!string.IsNullOrEmpty(driverName))
					{
						var driver = UpdateDrivers.FirstOrDefault(d => d.Name == driverName);
						if(driver != null)
						{
							var process = driver.CreateProcess(cmdline);
							if(process != null)
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
	}
}
