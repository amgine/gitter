namespace gitter.Framework
{
	using System;
	using System.Resources;
	using System.Collections.Generic;
	using System.Drawing;

	/// <summary>Provides cached icon resources.</summary>
	public sealed class CachedIconResources
	{
		private readonly Dictionary<string, Icon> _cache = new Dictionary<string, Icon>();
		private readonly ResourceManager _manager;

		public CachedIconResources(ResourceManager manager)
		{
			Verify.Argument.IsNotNull(manager, "manager");

			_manager = manager;
		}

		public Icon this[string name]
		{
			get
			{
				Icon bmp;
				if(!_cache.TryGetValue(name, out bmp))
				{
					bmp = (Icon)_manager.GetObject(name);
					_cache.Add(name, bmp);
				}
				return bmp;
			}
		}
	}
}
