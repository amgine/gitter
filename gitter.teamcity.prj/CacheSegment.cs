#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
			Verify.Argument.IsNotNull(cache, nameof(cache));

			_cache = cache;
		}

		public object SyncRoot => _cache.SyncRoot;

		protected TeamCityServiceContext Context => _cache.Context;

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
