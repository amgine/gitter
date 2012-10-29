namespace gitter.TeamCity
{
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
					if(name != null)
					{
						obj = Create(id, name);
					}
					else
					{
						obj = Create(id);
					}
					Cache.Add(id, obj);
				}
				else
				{
					if(name != null)
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
}
