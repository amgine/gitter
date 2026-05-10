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

using gitter.Framework;

/// <summary>
/// Application-startup wiring for icon packs. Call <see cref="Initialize"/> once
/// during program initialization to install an <see cref="IconPackFileIconProvider"/>
/// loaded with the bundled default pack and to ensure the user packs directory exists.
/// </summary>
public static class IconPackBootstrap
{
	private static IconPackFileIconProvider? _provider;
	private static IconPackDescriptor[]      _descriptors = Array.Empty<IconPackDescriptor>();

	public static IconPackFileIconProvider? Provider   => _provider;
	public static IconPackDescriptor[]      Available  => _descriptors;
	public static IconPack?                 ActivePack => _provider?.Pack;

	public static void Initialize()
	{
		try
		{
			IconPackDiscovery.EnsureUserPacksDirectory();
		}
		catch(Exception ex)
		{
			IconPackLog.Warn("Failed to create user packs directory", ex);
		}

		try
		{
			var found = IconPackDiscovery.Discover();
			_descriptors = new IconPackDescriptor[found.Count];
			for(var i = 0; i < found.Count; i++) _descriptors[i] = found[i];
		}
		catch(Exception ex)
		{
			IconPackLog.Warn("Pack discovery failed", ex);
			_descriptors = Array.Empty<IconPackDescriptor>();
		}

		IconPack? pack = null;
		foreach(var desc in _descriptors)
		{
			try { pack = desc.Load(); break; }
			catch(Exception ex) { IconPackLog.Warn($"Failed to load pack '{desc.Name}'", ex); }
		}

		var fallback = FileIconProvider.Current;
		_provider = new IconPackFileIconProvider(pack, fallback);
		FileIconProvider.Current = _provider;
	}

	public static bool TryActivate(string packName)
	{
		if(_provider is null) return false;
		foreach(var desc in _descriptors)
		{
			if(string.Equals(desc.Name, packName, StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					var pack = desc.Load();
					var old  = _provider.Pack;
					_provider.Pack = pack;
					old?.Dispose();
					return true;
				}
				catch(Exception ex)
				{
					IconPackLog.Warn($"Failed to activate pack '{packName}'", ex);
					return false;
				}
			}
		}
		return false;
	}
}
