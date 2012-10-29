namespace gitter.TeamCity
{
	public abstract class TeamCityObjectsCache<T> : TeamCityObjectsCacheBase<T>
		where T : TeamCityObject
	{
		internal TeamCityObjectsCache(TeamCityServiceContext context)
			: base(context)
		{
		}

		protected abstract T Create(string id);

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
