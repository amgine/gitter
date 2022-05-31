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

namespace gitter.Redmine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

public abstract class NamedRedmineObjectsCache<T> : RedmineObjectsCacheBase<T>
	where T : NamedRedmineObject
{
	internal NamedRedmineObjectsCache(RedmineServiceContext context)
		: base(context)
	{
	}

	protected abstract T Create(int id, string name);

	internal T Lookup(int id, string name)
	{
		T obj;
		lock(SyncRoot)
		{
			if(!Cache.TryGetValue(id, out obj))
			{
				obj = Create(id, name);
				Cache.Add(id, obj);
			}
			else
			{
				obj.Name = name;
			}
		}
		return obj;
	}
}
