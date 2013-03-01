namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Resources;

	public class CachedResources<T>
	{
		#region Data

		private readonly Dictionary<string, T> _cache;
		private readonly ResourceManager _manager;

		#endregion

		#region .ctor

		public CachedResources(ResourceManager manager)
		{
			Verify.Argument.IsNotNull(manager, "manager");

			_manager = manager;
			_cache = new Dictionary<string, T>();
		}

		#endregion

		public T this[string name]
		{
			get
			{
				T resource;
				if(!_cache.TryGetValue(name, out resource))
				{
					resource = (T)_manager.GetObject(name);
					_cache.Add(name, resource);
				}
				return resource;
			}
		}
	}
}
