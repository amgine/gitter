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

using System.Collections.Generic;
using System.Text.Json.Serialization;

public sealed class IconPackManifest
{
	[JsonPropertyName("iconDefinitions")]
	public Dictionary<string, IconDefinition> IconDefinitions { get; set; } = new();

	[JsonPropertyName("file")]
	public string? File { get; set; }

	[JsonPropertyName("folder")]
	public string? Folder { get; set; }

	[JsonPropertyName("folderExpanded")]
	public string? FolderExpanded { get; set; }

	[JsonPropertyName("rootFolder")]
	public string? RootFolder { get; set; }

	[JsonPropertyName("rootFolderExpanded")]
	public string? RootFolderExpanded { get; set; }

	[JsonPropertyName("folderNames")]
	public Dictionary<string, string> FolderNames { get; set; } = new();

	[JsonPropertyName("folderNamesExpanded")]
	public Dictionary<string, string> FolderNamesExpanded { get; set; } = new();

	[JsonPropertyName("fileExtensions")]
	public Dictionary<string, string> FileExtensions { get; set; } = new();

	[JsonPropertyName("fileNames")]
	public Dictionary<string, string> FileNames { get; set; } = new();

	[JsonPropertyName("languageIds")]
	public Dictionary<string, string> LanguageIds { get; set; } = new();
}
