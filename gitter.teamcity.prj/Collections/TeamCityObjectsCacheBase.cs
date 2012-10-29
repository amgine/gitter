namespace gitter.TeamCity
{
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
			Verify.Argument.IsNotNull(context, "context");

			_cache = new Dictionary<string, T>();
			_context = context;
		}

		protected abstract T Create(XmlNode node);

		protected Dictionary<string, T> Cache
		{
			get { return _cache; }
		}

		protected internal TeamCityServiceContext Context
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
			Verify.Argument.IsNotNull(node, "node");

			foreach(XmlNode child in node.ChildNodes)
			{
				yield return Lookup(child);
			}
		}

		public T this[string id]
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
