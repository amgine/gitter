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

using System;
using System.Collections.Generic;
using System.Xml;

public abstract class TeamCityObjectsCacheBase<T> : IEnumerable<T>
	where T : TeamCityObject
{
	#region Data

	private readonly Dictionary<string, T> _cache;
	private readonly TeamCityServiceContext _context;

	#endregion

	internal TeamCityObjectsCacheBase(TeamCityServiceContext context)
	{
		Verify.Argument.IsNotNull(context);

		_cache = new Dictionary<string, T>();
		_context = context;
	}

	protected abstract T Create(XmlNode node);

	protected Dictionary<string, T> Cache => _cache;

	protected internal TeamCityServiceContext Context => _context;

	public object SyncRoot => _context.SyncRoot;

	internal T Lookup(XmlNode node)
	{
		Verify.Argument.IsNotNull(node);

		var id = TeamCityUtility.LoadString(node.Attributes[TeamCityObject.IdProperty.XmlNodeName]);
		T obj;
		lock(SyncRoot)
		{
			if(!_cache.TryGetValue(id, out obj))
			{
				obj = Create(node);
				_cache.Add(id, obj);
			}
			else
			{
				obj.Update(node);
			}
		}
		return obj;
	}

	protected internal T FetchSingleItem(string url)
	{
		var xml = Context.GetXml(url);
		return Lookup(xml.DocumentElement);
	}

	protected LinkedList<T> FetchItemsFromSinglePage(string url)
	{
		var xml = Context.GetXml(url);
		var list = new LinkedList<T>();
		foreach(var item in Select(xml.DocumentElement))
		{
			list.AddLast(item);
		}
		return list;
	}

	protected IEnumerable<T> Select(XmlNode node)
	{
		Verify.Argument.IsNotNull(node);

		foreach(XmlNode child in node.ChildNodes)
		{
			yield return Lookup(child);
		}
	}

	public T this[string id] => _cache[id];

	public int Count => _cache.Count;

	internal bool Remove(T item)
	{
		Verify.Argument.IsNotNull(item);

		lock(SyncRoot)
		{
			return _cache.Remove(item.Id);
		}
	}

	internal bool Remove(string id)
	{
		lock(SyncRoot)
		{
			return _cache.Remove(id);
		}
	}

	internal void Clear()
	{
		lock(SyncRoot)
		{
			_cache.Clear();
		}
	}

	#region IEnumerable

	public IEnumerator<T> GetEnumerator()
		=> _cache.Values.GetEnumerator();

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		=> _cache.Values.GetEnumerator();

	#endregion
}
