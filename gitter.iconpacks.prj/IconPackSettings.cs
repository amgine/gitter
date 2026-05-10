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

/// <summary>Simple file-based persistence for the user's active icon pack choice.</summary>
public static class IconPackSettings
{
	public static string GetActivePackFilePath()
		=> Path.Combine(IconPackDiscovery.GetDefaultUserPacksDirectory(), "active.txt");

	public static string? LoadActivePackName()
	{
		try
		{
			var path = GetActivePackFilePath();
			if(!File.Exists(path)) return null;
			var text = File.ReadAllText(path).Trim();
			return string.IsNullOrEmpty(text) ? null : text;
		}
		catch(Exception ex)
		{
			IconPackLog.Warn("Failed to read active pack setting", ex);
			return null;
		}
	}

	public static void SaveActivePackName(string? packName)
	{
		try
		{
			IconPackDiscovery.EnsureUserPacksDirectory();
			var path = GetActivePackFilePath();
			File.WriteAllText(path, packName ?? string.Empty);
		}
		catch(Exception ex)
		{
			IconPackLog.Warn("Failed to write active pack setting", ex);
		}
	}
}
