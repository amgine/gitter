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
using System.Reflection;

public enum IconPackSource
{
	Bundled,
	User,
}

public sealed class IconPackDescriptor
{
	public IconPackDescriptor(string name, IconPackSource source, string? directory, Assembly? assembly, string? resourcePrefix)
	{
		Name           = name ?? throw new ArgumentNullException(nameof(name));
		Source         = source;
		Directory      = directory;
		Assembly       = assembly;
		ResourcePrefix = resourcePrefix;
	}

	public string         Name           { get; }
	public IconPackSource Source         { get; }
	public string?        Directory      { get; }
	public Assembly?      Assembly       { get; }
	public string?        ResourcePrefix { get; }

	public IconPack Load() => Source switch
	{
		IconPackSource.Bundled when Assembly is not null && ResourcePrefix is not null
			=> IconPackLoader.LoadFromEmbeddedResources(Assembly, ResourcePrefix, Name),
		IconPackSource.User when Directory is not null
			=> IconPackLoader.LoadFromDirectory(Directory, Name),
		_ => throw new InvalidOperationException($"Cannot load descriptor: source={Source}, dir={Directory}, asm={Assembly?.FullName}"),
	};

	public static IconPackDescriptor ForBundled(string name, Assembly assembly, string resourcePrefix)
		=> new(name, IconPackSource.Bundled, directory: null, assembly: assembly, resourcePrefix: resourcePrefix);

	public static IconPackDescriptor ForUser(string name, string directory)
		=> new(name, IconPackSource.User, directory: directory, assembly: null, resourcePrefix: null);
}
