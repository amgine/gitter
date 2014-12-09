using System;
using System.Collections.Generic;

namespace gitter.Framework.Controls
{
	public static class ListExtensions
	{
		public static T FindPrevious<T>(this IList<T> items, int currentIndex, Predicate<T> filter)
		{
			while (--currentIndex != -1)
			{
				var item = items[currentIndex];
				if (filter(item))
				{
					return item;
				}
			}
			return default(T);
		}
		public static T FindNext<T>(this IList<T> items, int currentIndex, Predicate<T> filter)
		{
			while (++currentIndex != items.Count)
			{
				var item = items[currentIndex];
				if (filter(item))
				{
					return item;
				}
			}
			return default(T);
		}
	}
}