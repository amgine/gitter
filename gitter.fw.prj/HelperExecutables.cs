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

namespace gitter.Framework;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

public static class HelperExecutables
{
	/// <summary>Runs <paramref name="actions"/> with administrator privileges, requesting them by UAC if necessary.</summary>
	/// <param name="actions">Actions to execute.</param>
	public static void ExecuteWithAdministartorRights(string[] actions)
	{
		Verify.Argument.IsNotNull(actions);

		var path = Path.Combine(AppContext.BaseDirectory, @"gitter.uac.exe");
		using var administratorProcess = new Process()
		{
			StartInfo = new ProcessStartInfo(path, string.Join(" ", actions))
			{
				CreateNoWindow = true,
			},
		};
		administratorProcess.Start();
		administratorProcess.WaitForExit();
	}

	public static bool CheckIfUpdaterIsRunning()
	{
		using var _ = new Semaphore(0, 1, "gitter-updater", out var singleInstance);
		return !singleInstance;
	}

	public static bool CheckIfCanLaunchUpdater()
	{
		const string updaterExeName = @"gitter.updater.exe";

		var sourcePath = Path.GetDirectoryName(typeof(Utility).Assembly.Location) ?? "";
		return File.Exists(Path.Combine(sourcePath, updaterExeName)) && !CheckIfUpdaterIsRunning();
	}

	public static void LaunchUpdater(string cmdline)
	{
		const string tempPathName      = @"gitter-updater";
		const string updaterExeName    = @"gitter.updater.exe";
#if NETCOREAPP
		const string updaterDllName    = @"gitter.updater.dll";
		const string updaterConfigName = @"gitter.updater.runtimeconfig.json";
#else
		const string updaterConfigName = @"gitter.updater.exe.config";
#endif

		var sourcePath = Path.GetDirectoryName(typeof(HelperExecutables).Assembly.Location) ?? "";
		var targetPath = Path.Combine(Path.GetTempPath(), tempPathName);
		if(!Directory.Exists(targetPath))
		{
			Directory.CreateDirectory(targetPath);
		}
#if NETCOREAPP
		File.Copy(Path.Combine(sourcePath, updaterDllName),    Path.Combine(targetPath, updaterDllName),    overwrite: true);
#endif
		File.Copy(Path.Combine(sourcePath, updaterExeName),    Path.Combine(targetPath, updaterExeName),    overwrite: true);
		File.Copy(Path.Combine(sourcePath, updaterConfigName), Path.Combine(targetPath, updaterConfigName), overwrite: true);
		using var process = new Process
		{
			StartInfo = new ProcessStartInfo(Path.Combine(targetPath, updaterExeName), cmdline)
			{
				WindowStyle     = ProcessWindowStyle.Normal,
				CreateNoWindow  = false,
				LoadUserProfile = true,
			},
		};
		process.Start();
	}

	public static void CleanupUpdaterTemporaryFiles()
	{
		const string tempPathName = @"gitter-updater";
		var path = Path.Combine(Path.GetTempPath(), tempPathName);
		Directory.Delete(path, recursive: true);
	}
}
