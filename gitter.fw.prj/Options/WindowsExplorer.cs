#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.IO;

using Microsoft.Win32;

static class WindowsExplorer
{
	private static string GetAppPath() => Path.Combine(Path.GetFullPath(AppContext.BaseDirectory), "gitter.exe");

	const string SubKeyName = @"gitter";

	public static bool IsIntegratedInExplorerContextMenu
	{
		get
		{
			try
			{
				using var key = Registry.ClassesRoot.OpenSubKey($@"Directory\shell\{SubKeyName}\command", writable: false);
				if(key is null) return false;
				var value = (string?)key.GetValue(null, string.Empty);
				if(string.IsNullOrEmpty(value)) return false;
				if(value!.EndsWith(" \"%1\""))
				{
					value = value.Substring(0, value.Length - 5);
					if(value.StartsWith("\"") && value.EndsWith("\""))
					{
						value = value.Substring(1, value.Length - 2);
						value = Path.GetFullPath(value);
						return value == GetAppPath();
					}
				}
				return false;
			}
			catch(Exception exc) when(!exc.IsCritical)
			{
				return false;
			}
		}
	}

	public static void IntegrateInExplorerContextMenu()
	{
		try
		{
			using var key = Registry.ClassesRoot.OpenSubKey(@"Directory\shell", writable: true);
			if(key is null) return;
			using var gitterKey = key.CreateSubKey(SubKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryOptions.None);
			gitterKey.SetValue(null, @"Open with gitter");
			using var commandKey = gitterKey.CreateSubKey("command", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryOptions.None);
			var appPath = GetAppPath();
			if(!appPath.StartsWith("\"") || !appPath.EndsWith("\""))
			{
				appPath = "\"" + appPath + "\"";
			}
			appPath += " \"%1\"";
			commandKey.SetValue(null, appPath);
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
		}
	}

	public static void RemoveFromExplorerContextMenu()
	{
		try
		{
			using var key = Registry.ClassesRoot.OpenSubKey(@"Directory\shell", writable: true);
			if(key is null) return;
			key.DeleteSubKeyTree(SubKeyName, throwOnMissingSubKey: false);
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
		}
	}
}
