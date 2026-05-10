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
using System.IO;
using System.Text.Json;

public static class IconPackManifestParser
{
	private static readonly JsonSerializerOptions _options = CreateOptions();

	private static JsonSerializerOptions CreateOptions()
	{
		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			ReadCommentHandling = JsonCommentHandling.Skip,
			AllowTrailingCommas = true,
		};
		options.Converters.Add(new IconDefinitionJsonConverter());
		return options;
	}

	public static IconPackManifest Parse(string json)
	{
		if(json is null) throw new ArgumentNullException(nameof(json));
		var manifest = JsonSerializer.Deserialize<IconPackManifest>(json, _options)
			?? throw new InvalidDataException("Icon pack manifest is empty.");
		return manifest;
	}

	public static IconPackManifest ParseFile(string path)
	{
		if(path is null) throw new ArgumentNullException(nameof(path));
		using var stream = File.OpenRead(path);
		var manifest = JsonSerializer.Deserialize<IconPackManifest>(stream, _options)
			?? throw new InvalidDataException($"Icon pack manifest at '{path}' is empty.");
		return manifest;
	}
}
