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

namespace gitter.Redmine
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using System.Xml;

	using gitter.Framework;

	public abstract class RedmineObjectsCacheBase<T> : IEnumerable<T>
		where T : RedmineObject
	{
		#region Data

		private readonly Dictionary<int, T> _cache;
		private readonly RedmineServiceContext _context;

		#endregion

		internal RedmineObjectsCacheBase(RedmineServiceContext context)
		{
			Verify.Argument.IsNotNull(context, "context");

			_cache = new Dictionary<int, T>();
			_context = context;
		}

		protected abstract T Create(XmlNode node);

		protected Dictionary<int, T> Cache
		{
			get { return _cache; }
		}

		protected RedmineServiceContext Context
		{
			get { return _context; }
		}

		public object SyncRoot
		{
			get { return _context.SyncRoot; }
		}

		internal T Lookup(XmlNode node)
		{
			Verify.Argument.IsNotNull(node, "node");

			var id = RedmineUtility.LoadInt(node[RedmineObject.IdProperty.XmlNodeName]);
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

		protected LinkedList<T> FetchItemsFromAllPages(string url)
		{
			var list = new LinkedList<T>();
			Context.GetAllDataPages(url,
				xml =>
				{
					foreach(var item in Select(xml.DocumentElement))
					{
						list.AddLast(item);
					}
				});
			return list;
		}

		protected Task<LinkedList<T>> FetchItemsFromAllPagesAsync(string url, CancellationToken cancellationToken)
		{
			var list = new LinkedList<T>();
			return Context
				.GetAllDataPagesAsync(url,
				xml =>
				{
					foreach(var item in Select(xml.DocumentElement))
					{
						list.AddLast(item);
					}
				},
				cancellationToken)
				.ContinueWith(
				t =>
				{
					TaskUtility.PropagateFaultedStates(t);
					return list;
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
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
			Verify.Argument.IsNotNull(node, "node");

			foreach(XmlNode child in node.ChildNodes)
			{
				yield return Lookup(child);
			}
		}

		public T this[int id]
		{
			get { return _cache[id]; }
		}

		public int Count
		{
			get { return _cache.Count; }
		}

		internal bool Remove(T item)
		{
			Verify.Argument.IsNotNull(item, "item");

			lock(SyncRoot)
			{
				return _cache.Remove(item.Id);
			}
		}

		internal bool Remove(int id)
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
		{
			return _cache.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _cache.Values.GetEnumerator();
		}

		#endregion
	}
}
