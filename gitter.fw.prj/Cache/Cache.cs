namespace gitter.Framework
{
	using System;

	public abstract class Cache<T>
	{
		public abstract bool IsCached { get; }

		public abstract void Invalidate();

		public abstract T Value { get; }

		public static implicit operator T(Cache<T> cache)
		{
			return cache.Value;
		}
	}
}
