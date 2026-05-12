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
using System.IO;
using System.Reflection;

public static class IconPackLoader
{
	public const string ManifestFileName = "icon-theme.json";

	private static readonly string[] _candidateManifestPaths =
	[
		"icon-theme.json",
		"dist/icon-theme.json",
		"dist/material-icons.json",
		"material-icons.json",
	];

	/// <summary>Loads a pack from a directory. Looks for the manifest at several
	/// well-known locations; icon paths are resolved relative to the manifest's
	/// own directory (per VSCode convention).</summary>
	public static IconPack LoadFromDirectory(string packDir, string? name = null)
	{
		if(packDir is null) throw new ArgumentNullException(nameof(packDir));
		if(!Directory.Exists(packDir))
			throw new DirectoryNotFoundException($"Icon pack directory not found: {packDir}");

		var manifestPath = FindManifest(packDir)
			?? throw new FileNotFoundException(
				$"Manifest not found in '{packDir}' (looked for {string.Join(", ", _candidateManifestPaths)}).");
		var manifestDir = Path.GetDirectoryName(manifestPath) ?? packDir;

		var manifest = IconPackManifestParser.ParseFile(manifestPath);
		var packName = name ?? new DirectoryInfo(packDir).Name;

		Stream? Loader(string relPath)
		{
			var full = Path.GetFullPath(Path.Combine(manifestDir, NormalizeRelative(relPath)));
			return File.Exists(full) ? File.OpenRead(full) : null;
		}

		return new IconPack(packName, manifest, Loader);
	}

	private static string? FindManifest(string packDir)
	{
		foreach(var candidate in _candidateManifestPaths)
		{
			var full = Path.Combine(packDir, candidate.Replace('/', Path.DirectorySeparatorChar));
			if(File.Exists(full)) return full;
		}
		return null;
	}

	/// <summary>Loads a pack from embedded resources where every entry is prefixed with <paramref name="resourcePrefix"/>.</summary>
	public static IconPack LoadFromEmbeddedResources(Assembly assembly, string resourcePrefix, string name)
	{
		if(assembly is null)       throw new ArgumentNullException(nameof(assembly));
		if(resourcePrefix is null) throw new ArgumentNullException(nameof(resourcePrefix));
		if(name is null)           throw new ArgumentNullException(nameof(name));

		var manifestKey = MakeResourceName(resourcePrefix, ManifestFileName);
		using var manifestStream = assembly.GetManifestResourceStream(manifestKey)
			?? throw new FileNotFoundException($"Embedded manifest not found: {manifestKey}");
		using var sr = new StreamReader(manifestStream);
		var manifest = IconPackManifestParser.Parse(sr.ReadToEnd());

		Stream? Loader(string relPath)
		{
			var key = MakeResourceName(resourcePrefix, relPath);
			return assembly.GetManifestResourceStream(key);
		}

		return new IconPack(name, manifest, Loader);
	}

	private static string NormalizeRelative(string path)
	{
		var p = path;
		if(p.StartsWith("./",  StringComparison.Ordinal)) p = p.Substring(2);
		if(p.StartsWith(".\\", StringComparison.Ordinal)) p = p.Substring(2);
		return p.Replace('/', Path.DirectorySeparatorChar);
	}

	private static string MakeResourceName(string prefix, string relativePath)
	{
		var p = relativePath;
		if(p.StartsWith("./",  StringComparison.Ordinal)) p = p.Substring(2);
		if(p.StartsWith(".\\", StringComparison.Ordinal)) p = p.Substring(2);
		// Embedded resource names use '.' as separator and preserve case for filenames.
		var parts = p.Split('/', '\\');
		return prefix + "." + string.Join(".", parts);
	}
}
