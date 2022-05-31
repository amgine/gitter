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

namespace gitter.Updater;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

static class Program
{
	/// <summary>Available updaters.</summary>
	private static readonly IReadOnlyList<IUpdateDriver> UpdateDrivers =
		new IUpdateDriver[]
		{
			new GitUpdateDriver(),
			new DeployDriver(),
			new DownloadAndUnzipDriver(),
		};

	/// <summary>The main entry point for the application.</summary>
	[STAThread]
	static void Main()
	{
		var cmdline = new CommandLine();
		bool singleInstance;
		var s = default(Semaphore);
		if(!cmdline.IsDefined(@"forcenewinstance"))
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
				if(cmdline[@"driver"] is { Length: > 0 } driverName)
				{
					var process = UpdateDrivers.FirstOrDefault(d => d.Name == driverName)?.CreateProcess(cmdline);
					if(process is null) return;

					if(cmdline.IsDefined(@"hidden"))
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
		finally
		{
			s?.Dispose();
		}
	}
}
