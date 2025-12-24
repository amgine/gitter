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

namespace gitter.Framework;

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Reflection;

/// <summary>Provides cached icon resources.</summary>
public sealed class CachedIconResources
{
	private readonly Dictionary<string, Icon> _cache = [];
	private readonly Assembly _assembly;
	private readonly string _prefix;

	public CachedIconResources(Assembly assembly, string prefix)
	{
		Verify.Argument.IsNotNull(assembly);

		_assembly = assembly;
		_prefix   = prefix;
	}

	public Icon? this[string name]
	{
		get
		{
			if(!_cache.TryGetValue(name, out var icon))
			{
				using(var stream = _assembly.GetManifestResourceStream(_prefix + "." + name + ".ico"))
				{
					icon = stream is not null ? new Icon(stream) : default;
				}
				if(icon is null) return default;
				_cache.Add(name, icon);
			}
			return icon;
		}
	}
}
