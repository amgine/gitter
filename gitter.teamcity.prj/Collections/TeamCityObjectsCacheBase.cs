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
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

public abstract class TeamCityObjectsCacheBase<T> : IEnumerable<T>
	where T : TeamCityObject
{
	private readonly Dictionary<string, T> _cache;
	private readonly TeamCityServiceContext _context;

	internal TeamCityObjectsCacheBase(TeamCityServiceContext context)
	{
		Verify.Argument.IsNotNull(context);

		_cache   = [];
		_context = context;
	}

	protected abstract T Create(XmlNode node);

	protected Dictionary<string, T> Cache => _cache;

	protected internal TeamCityServiceContext Context => _context;

	public LockType SyncRoot => _context.SyncRoot;

	internal T Lookup(XmlNode node)
	{
		Verify.Argument.IsNotNull(node);

		var id = TeamCityUtility.LoadString(node.Attributes?[TeamCityObject.IdProperty.XmlNodeName]);
		if(id is not { Length: not 0 }) throw new ArgumentException("Id is not defined.", nameof(node));

		T? obj;
		lock(SyncRoot)
		{
			if(!_cache.TryGetValue(id, out obj))
			{
				_cache.Add(id, obj = Create(node));
			}
			else
			{
				obj.Update(node);
			}
		}
		return obj;
	}

	protected internal async Task<T> FetchSingleItemAsync(string url, CancellationToken cancellationToken = default)
	{
		var xml = await Context
			.GetXmlAsync(url, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		return Lookup(xml.DocumentElement ?? throw new ApplicationException("XML is empty."));
	}

	protected async Task<List<T>> FetchItemsFromSinglePageAsync(string url, CancellationToken cancellationToken = default)
	{
		var xml = await Context
			.GetXmlAsync(url, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		var list = new List<T>();
		foreach(var item in Select(xml.DocumentElement ?? throw new ApplicationException("XML is empty.")))
		{
			list.Add(item);
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
