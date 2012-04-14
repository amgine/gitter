namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;

	public abstract class RedmineObjectsCache<T> : RedmineObjectsCacheBase<T>
		where T : RedmineObject
	{
		internal RedmineObjectsCache(RedmineServiceContext context)
			: base(context)
		{
		}

		protected abstract T Create(int id);

		internal T Lookup(int id)
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
}
