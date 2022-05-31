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
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;

public static class Utility
{
	/// <summary>Checks if process is running with administrator privileges.</summary>
	public static bool IsRunningWithAdministratorRights
	{
		get
		{
			try
			{
				using var identity = WindowsIdentity.GetCurrent();
				var pricipal = new WindowsPrincipal(identity);
				return pricipal.IsInRole(WindowsBuiltInRole.Administrator);
			}
			catch
			{
				return false;
			}
		}
	}

	public static void EnsureDirectoryDoesNotExist(string path)
	{
		if(Directory.Exists(path))
		{
			RemoveDirectoryRecursively(path);
		}
	}

	public static void EnsureDirectoryExists(string path)
	{
		if(Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}

	public static void EnsureDirectoryIsEmpty(string path)
	{
		if(Directory.Exists(path))
		{
			RemoveDirectoryRecursively(path);
			Directory.CreateDirectory(path);
		}
	}

	private static void RemoveDirectoryRecursively(string dir)
	{
		var di = new DirectoryInfo(dir);
		try
		{
			DropProtectingFlags(di);
		}
		catch
		{
		}
		Directory.Delete(dir, true);
	}

	private static void DropProtectingFlags(DirectoryInfo di)
	{
		const FileAttributes drop = FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System;

		var dattrs = di.Attributes;
		if((dattrs & drop) != (FileAttributes)0)
		{
			dattrs &= ~drop;
			di.Attributes = dattrs;
		}
		foreach(var file in di.EnumerateFiles())
		{
			var attrs = file.Attributes;
			if((attrs & drop) != (FileAttributes)0)
			{
				attrs &= ~drop;
				file.Attributes = attrs;
			}
		}
		foreach(var d in di.EnumerateDirectories())
		{
			DropProtectingFlags(d);
		}
	}

	public static void CopyDirectoryContent(string source, string target)
	{
		CopyDirectoryContent(new DirectoryInfo(source), target);
	}

	public static void CopyDirectoryContent(DirectoryInfo source, string target)
	{
		EnsureDirectoryExists(target);
		foreach(var file in source.EnumerateFiles())
		{
			file.CopyTo(Path.Combine(target, file.Name), true);
		}
		foreach(var d in source.EnumerateDirectories())
		{
			CopyDirectoryContent(d, Path.Combine(target, d.Name));
		}
	}

	public static void StartApplication(string fileName)
	{
		using(var p = new Process())
		{
			p.StartInfo = new ProcessStartInfo()
			{
				FileName = fileName,
				WorkingDirectory = Path.GetDirectoryName(fileName),
			};
			p.Start();
		}
	}

	public static void KillAllGitterProcesses(string targetDirectory)
	{
		try
		{
			var di = new DirectoryInfo(targetDirectory);
			var processes = Process.GetProcesses();
			foreach(var p in processes)
			{
				bool kill = p.ProcessName == "gitter";
				if(!kill)
				{
					try
					{
						var dir = Path.GetDirectoryName(p.MainModule.FileName);
						var d = new DirectoryInfo(dir);
						while(d != null)
						{
							if(di.FullName == d.FullName)
							{
								kill = true;
								break;
							}
							d = d.Parent;
						}
					}
					catch
					{
					}
				}
				if(kill)
				{
					bool closed = false;
					try
					{
						closed = p.CloseMainWindow();
					}
					catch
					{
					}
					try
					{
						if(!closed || !p.WaitForExit(2000))
						{
							p.Kill();
						}
					}
					catch
					{
					}
				}
			}
		}
		catch
		{
		}
	}

	public static bool HasWriteAccess(string directory)
	{
		const string FileName = ".write-test";
		var fullPath = Path.Combine(directory, FileName);
		try
		{
			using(var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
			{
				fs.WriteByte(0);
			}
			File.Delete(fullPath);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool Deploy(string from, string to)
	{
		if(IsRunningWithAdministratorRights || HasWriteAccess(to))
		{
			var source = new DirectoryInfo(from);
			Utility.CopyDirectoryContent(source, to);
			return true;
		}
		else
		{
			var cmdline = new StringBuilder();
			cmdline.Append(@"/forcenewinstance");
			cmdline.Append(' ');
			cmdline.Append(@"/hidden");
			cmdline.Append(' ');
			cmdline.Append(@"/driver:deploy");
			cmdline.Append(' ');
			cmdline.Append('\"');
			cmdline.Append(@"/source:" + from);
			cmdline.Append('\"');
			cmdline.Append(' ');
			cmdline.Append('\"');
			cmdline.Append(@"/target:" + to);
			cmdline.Append('\"');
			string exeFileName;
			using(var p = Process.GetCurrentProcess())
			{
				exeFileName = p.MainModule.FileName;
			}
			using(var p = new Process()
				{
					StartInfo = new ProcessStartInfo(exeFileName, cmdline.ToString())
					{
						CreateNoWindow = true,
						Verb = "runas",
					},
				})
			{
				p.Start();
				p.WaitForExit();
				return p.ExitCode == 0;
			}
		}
	}
}
