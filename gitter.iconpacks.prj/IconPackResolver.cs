#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Linq;

/// <summary>
/// Resolves a file or folder name to an icon-definition name following the
/// VSCode icon-theme resolution order:
///   files:   fileNames[exact] -> fileExtensions[longest] -> languageIds[lang] -> file (default)
///   folders: folderNames[exact] -> folder default
/// </summary>
public sealed class IconPackResolver
{
	private readonly IconPackManifest _manifest;
	private readonly Dictionary<string, string> _fileNamesIgnoreCase;
	private readonly Dictionary<string, string> _folderNamesIgnoreCase;
	private readonly Dictionary<string, string> _folderNamesExpandedIgnoreCase;
	private readonly List<KeyValuePair<string, string>> _extensionsLongestFirst;

	public IconPackResolver(IconPackManifest manifest)
	{
		_manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));

		_fileNamesIgnoreCase           = ToIgnoreCase(manifest.FileNames);
		_folderNamesIgnoreCase         = ToIgnoreCase(manifest.FolderNames);
		_folderNamesExpandedIgnoreCase = ToIgnoreCase(manifest.FolderNamesExpanded);
		_extensionsLongestFirst        = manifest.FileExtensions
			.OrderByDescending(static kv => kv.Key.Length)
			.ToList();
	}

	public IconPackManifest Manifest => _manifest;

	/// <summary>Returns the icon-definition name for a file, or null if no default is configured.</summary>
	public string? ResolveFile(string fileName, string? languageId = null)
	{
		if(fileName is null) throw new ArgumentNullException(nameof(fileName));
		var name = Path.GetFileName(fileName);
		if(name.Length == 0) name = fileName;

		if(_fileNamesIgnoreCase.TryGetValue(name, out var byName)) return byName;

		var lower = name.ToLowerInvariant();
		foreach(var kv in _extensionsLongestFirst)
		{
			var ext = kv.Key;
			if(ext.Length == 0) continue;
			if(lower.Length > ext.Length &&
				lower[lower.Length - ext.Length - 1] == '.' &&
				lower.EndsWith(ext, StringComparison.Ordinal))
			{
				return kv.Value;
			}
		}

		if(!string.IsNullOrEmpty(languageId) &&
			_manifest.LanguageIds.TryGetValue(languageId!, out var byLang))
		{
			return byLang;
		}

		return _manifest.File;
	}

	/// <summary>Returns the icon-definition name for a folder, or null if no default is configured.</summary>
	public string? ResolveFolder(string folderName, bool expanded = false)
	{
		if(folderName is null) throw new ArgumentNullException(nameof(folderName));
		var name = Path.GetFileName(folderName.TrimEnd('/', '\\'));
		if(name.Length == 0) name = folderName;

		if(expanded)
		{
			if(_folderNamesExpandedIgnoreCase.TryGetValue(name, out var byNameExp)) return byNameExp;
			if(_folderNamesIgnoreCase        .TryGetValue(name, out var byName))    return byName;
			return _manifest.FolderExpanded ?? _manifest.Folder;
		}
		else
		{
			if(_folderNamesIgnoreCase.TryGetValue(name, out var byName)) return byName;
			return _manifest.Folder;
		}
	}

	/// <summary>Returns the icon-path for a resolved icon-def name, or null if the def is missing or has no path.</summary>
	public string? ResolveIconPath(string? iconDefName)
	{
		if(string.IsNullOrEmpty(iconDefName)) return null;
		return _manifest.IconDefinitions.TryGetValue(iconDefName!, out var def) ? def.IconPath : null;
	}

	private static Dictionary<string, string> ToIgnoreCase(Dictionary<string, string> source)
	{
		var dict = new Dictionary<string, string>(source.Count, StringComparer.OrdinalIgnoreCase);
		foreach(var kv in source) dict[kv.Key] = kv.Value;
		return dict;
	}
}
