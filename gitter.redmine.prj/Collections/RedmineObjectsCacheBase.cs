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

#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;

public abstract class RedmineObjectsCacheBase<T> : IEnumerable<T>
	where T : RedmineObject
{
	internal RedmineObjectsCacheBase(RedmineServiceContext context)
	{
		Verify.Argument.IsNotNull(context);

		Context = context;
	}

	protected abstract T Create(XmlNode node);

	protected Dictionary<int, T> Cache { get; } = [];

	protected RedmineServiceContext Context { get; }

	public LockType SyncRoot => Context.SyncRoot;

	internal T Lookup(XmlNode node)
	{
		Verify.Argument.IsNotNull(node);

		var id = RedmineUtility.LoadInt(node[RedmineObject.IdProperty.XmlNodeName]);
		lock(SyncRoot)
		{
			if(!Cache.TryGetValue(id, out var obj))
			{
				Cache.Add(id, obj = Create(node));
			}
			else
			{
				obj.Update(node);
			}
			return obj;
		}
	}

	protected internal async Task<T> FetchSingleItemAsync(string url, CancellationToken cancellationToken = default)
	{
		var xml = await Context
			.GetXmlAsync(url, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		var root = xml?.DocumentElement ?? throw new ApplicationException("XML is empty");
		return Lookup(root);
	}

	protected async Task<List<T>> FetchItemsFromAllPagesAsync(string url, CancellationToken cancellationToken = default)
	{
		var list = new List<T>();
		await Context.GetAllDataPagesAsync(url,
			xml =>
			{
				var root = xml.DocumentElement;
				if(root is null) return;
				foreach(var item in Select(root))
				{
					list.Add(item);
				}
			}, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		return list;
	}

	protected async Task<List<T>> FetchItemsFromSinglePageAsync(string url, CancellationToken cancellationToken = default)
	{
		var xml = await Context
			.GetXmlAsync(url, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		var root = xml?.DocumentElement ?? throw new ApplicationException("XML is empty");
		var list = new List<T>();
		foreach(var item in Select(root))
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

	public T this[int id] => Cache[id];

	public int Count => Cache.Count;

	internal bool Remove(T item)
	{
		Verify.Argument.IsNotNull(item);

		lock(SyncRoot)
		{
			return Cache.Remove(item.Id);
		}
	}

	internal bool Remove(int id)
	{
		lock(SyncRoot)
		{
			return Cache.Remove(id);
		}
	}

	internal void Clear()
	{
		lock(SyncRoot)
		{
			Cache.Clear();
		}
	}

	#region IEnumerable

	public IEnumerator<T> GetEnumerator()
		=> Cache.Values.GetEnumerator();

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		=> Cache.Values.GetEnumerator();

	#endregion
}
