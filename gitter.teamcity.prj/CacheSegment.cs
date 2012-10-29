namespace gitter.TeamCity
{
	using System;
	using System.Collections.Generic;

	public abstract class CacheSegment<T> : IEnumerable<T>
		where T : TeamCityObject
	{
		private readonly TeamCityObjectsCacheBase<T> _cache;

		internal CacheSegment(TeamCityObjectsCacheBase<T> cache)
		{
			Verify.Argument.IsNotNull(cache, "cache");

			_cache = cache;
		}

		public object SyncRoot
		{
			get { return _cache.SyncRoot; }
		}

		protected TeamCityServiceContext Context
		{
			get { return _cache.Context; }
		}

		public abstract void Refresh();

		protected abstract bool IsIncluded(T item);

		#region IEnumerable<T>

		public IEnumerator<T> GetEnumerator()
		{
			foreach(var item in _cache)
			{
				if(IsIncluded(item)) yield return item;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
