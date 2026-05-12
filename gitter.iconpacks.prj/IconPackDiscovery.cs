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

namespace gitter.IconPacks;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public static class IconPackDiscovery
{
	public const string DefaultBundledPackName     = "default";
	public const string DefaultBundledResourceRoot = "gitter.IconPacks.Resources.packs.default";

	/// <summary>Returns the default user packs directory: %APPDATA%/Gitter/icon-packs.</summary>
	public static string GetDefaultUserPacksDirectory()
	{
		var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		return Path.Combine(appData, "Gitter", "icon-packs");
	}

	/// <summary>Ensures the user packs directory exists. Creates it on first run.</summary>
	public static string EnsureUserPacksDirectory(string? path = null)
	{
		var dir = path ?? GetDefaultUserPacksDirectory();
		Directory.CreateDirectory(dir);
		return dir;
	}

	public static IReadOnlyList<IconPackDescriptor> Discover(string? userPacksDir = null, Assembly? bundledAssembly = null)
	{
		var list = new List<IconPackDescriptor>();
		list.Add(IconPackDescriptor.ForBundled(
			DefaultBundledPackName,
			bundledAssembly ?? typeof(IconPackDiscovery).Assembly,
			DefaultBundledResourceRoot));

		var dir = userPacksDir ?? GetDefaultUserPacksDirectory();
		if(Directory.Exists(dir))
		{
			foreach(var sub in Directory.EnumerateDirectories(dir))
			{
				var manifest = Path.Combine(sub, IconPackLoader.ManifestFileName);
				if(File.Exists(manifest))
				{
					var name = new DirectoryInfo(sub).Name;
					list.Add(IconPackDescriptor.ForUser(name, sub));
				}
			}
		}
		return list;
	}
}
