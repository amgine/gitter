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

namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	/// <summary>Collection with auto-sorting, change notification &amp; thread-safe operations invoked on host control.</summary>
	/// <typeparam name="T">Item type.</typeparam>
	public abstract class SafeNotifySortedCollection<T> : NotifySortedCollection<T>
	{
		#region .ctor

		/// <summary>Create <see cref="SafeNotifySortedCollection{T}"/>.</summary>
		public SafeNotifySortedCollection()
		{
		}

		#endregion

		#region Properties

		/// <summary>Object which is used to synchronize all *Safe() calls.</summary>
		protected abstract ISynchronizeInvoke SynchronizeInvoke { get; }

		#endregion

		#region Methods

		public void AddSafe(T item)
		{
			var sync = SynchronizeInvoke;
			if(sync == null || !sync.InvokeRequired)
			{
				Add(item);
			}
			else
			{
				try
				{
					sync.BeginInvoke(new Action<T>(Add), new object[] { item });
				}
				catch(ObjectDisposedException)
				{
				}
			}
		}

		public void AddRangeSafe(IEnumerable<T> list)
		{
			var sync = SynchronizeInvoke;
			if(sync == null || !sync.InvokeRequired)
			{
				AddRange(list);
			}
			else
			{
				try
				{
					sync.BeginInvoke(new Action<IEnumerable<T>>(AddRange), new object[] { list });
				}
				catch(ObjectDisposedException)
				{
				}
			}
		}

		public void SetSafe(int index, T item)
		{
			var sync = SynchronizeInvoke;
			if(sync == null || !sync.InvokeRequired)
			{
				this[index] = item;
			}
			else
			{
				try
				{
					sync.BeginInvoke(new Action<int, T>(SetSafe), new object[] { index, item });
				}
				catch(ObjectDisposedException)
				{
				}
			}
		}

		public void InsertSafe(int index, T item)
		{
			var sync = SynchronizeInvoke;
			if(sync == null || !sync.InvokeRequired)
			{
				Insert(index, item);
			}
			else
			{
				try
				{
					sync.BeginInvoke(new Action<int, T>(Insert), new object[] { index, item });
				}
				catch(ObjectDisposedException)
				{
				}
			}
		}

		public void RemoveSafe(T item)
		{
			var sync = SynchronizeInvoke;
			if(sync == null || !sync.InvokeRequired)
			{
				Remove(item);
			}
			else
			{
				try
				{
					sync.BeginInvoke(new Func<T, bool>(Remove), new object[] { item });
				}
				catch(ObjectDisposedException)
				{
				}
			}
		}

		public void RemoveAtSafe(int index)
		{
			var control = SynchronizeInvoke;
			if(control == null || !control.InvokeRequired)
			{
				RemoveAt(index);
			}
			else
			{
				try
				{
					control.BeginInvoke(new Action<int>(RemoveAt), new object[] { index });
				}
				catch(ObjectDisposedException)
				{
				}
			}
		}

		public void RemoveRangeSafe(int index, int count)
		{
			var sync = SynchronizeInvoke;
			if(sync == null || !sync.InvokeRequired)
			{
				RemoveRange(index, count);
			}
			else
			{
				try
				{
					sync.BeginInvoke(new Action<int, int>(RemoveRange), new object[] { index, count});
				}
				catch(ObjectDisposedException)
				{
				}
			}
		}

		public void ClearSafe()
		{
			var sync = SynchronizeInvoke;
			if(sync == null || !sync.InvokeRequired)
			{
				Clear();
			}
			else
			{
				try
				{
					sync.BeginInvoke(new Action(Clear), null);
				}
				catch(ObjectDisposedException)
				{
				}
			}
		}

		public void InsertSortedFromTopSafe(T item, Func<T, T, int> comparer)
		{
			var sync = SynchronizeInvoke;
			if(sync == null || !sync.InvokeRequired)
			{
				InsertSortedFromTop(item, comparer);
			}
			else
			{
				try
				{
					sync.BeginInvoke(
						new Func<T, Func<T, T, int>, int>(InsertSortedFromTop),
						new object[] { item, comparer });
				}
				catch(ObjectDisposedException)
				{
				}
			}
		}

		public void InsertSortedFromBottomSafe(T item, Func<T, T, int> comparer)
		{
			var sync = SynchronizeInvoke;
			if(sync == null || !sync.InvokeRequired)
			{
				InsertSortedFromBottom(item, comparer);
			}
			else
			{
				try
				{
					sync.BeginInvoke(
						new Func<T, Func<T, T, int>, int>(InsertSortedFromBottom),
						new object[] { item, comparer });
				}
				catch(ObjectDisposedException)
				{
				}
			}
		}

		#endregion
	}
}
