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

namespace gitter.TeamCity;

public abstract class NamedTeamCityObjectsCache<T> : TeamCityObjectsCacheBase<T>
	where T : NamedTeamCityObject
{
	internal NamedTeamCityObjectsCache(TeamCityServiceContext context)
		: base(context)
	{
	}

	protected abstract T Create(string id, string name);

	protected abstract T Create(string id);

	internal T Lookup(string id, string name)
	{
		T obj;
		lock(SyncRoot)
		{
			if(!Cache.TryGetValue(id, out obj))
			{
				obj = name is not null
					? Create(id, name)
					: Create(id);
				Cache.Add(id, obj);
			}
			else
			{
				if(name is not null)
				{
					obj.Name = name;
				}
			}
		}
		return obj;
	}

	internal T Lookup(string id)
	{
		T obj;
		lock(SyncRoot)
		{
			if(!Cache.TryGetValue(id, out obj))
			{
				obj = Create(id);
				Cache.Add(id, obj);
			}
		}
		return obj;
	}
}
