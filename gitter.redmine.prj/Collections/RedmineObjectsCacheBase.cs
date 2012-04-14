namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;

	public abstract class RedmineObjectsCacheBase<T> : IEnumerable<T>
		where T : RedmineObject
	{
		#region Data

		private readonly Dictionary<int, T> _cache;
		private readonly RedmineServiceContext _context;

		#endregion

		internal RedmineObjectsCacheBase(RedmineServiceContext context)
		{
			if(context == null) throw new ArgumentNullException("context");

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
			if(node == null) throw new ArgumentNullException("node");

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
			if(node == null) throw new ArgumentNullException("node");

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
			if(item == null) throw new ArgumentNullException("item");

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
