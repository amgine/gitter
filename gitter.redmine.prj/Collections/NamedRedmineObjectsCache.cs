namespace gitter.Redmine
{
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
}
