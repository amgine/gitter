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

namespace gitter.Framework.Controls;

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
	protected abstract ISynchronizeInvoke? SynchronizeInvoke { get; }

	#endregion

	#region Methods

	public void AddSafe(T item)
	{
		if(SynchronizeInvoke is not { InvokeRequired: true } sync)
		{
			Add(item);
		}
		else
		{
			try
			{
				sync.BeginInvoke(new Action<T>(Add), [item]);
			}
			catch(ObjectDisposedException)
			{
			}
		}
	}

	public void AddRangeSafe(IEnumerable<T> list)
	{
		if(SynchronizeInvoke is not { InvokeRequired: true } sync)
		{
			AddRange(list);
		}
		else
		{
			try
			{
				sync.BeginInvoke(new Action<IEnumerable<T>>(AddRange), [list]);
			}
			catch(ObjectDisposedException)
			{
			}
		}
	}

	public void SetSafe(int index, T item)
	{
		if(SynchronizeInvoke is not { InvokeRequired: true } sync)
		{
			this[index] = item;
		}
		else
		{
			try
			{
				sync.BeginInvoke(new Action<int, T>(SetSafe), [index, item]);
			}
			catch(ObjectDisposedException)
			{
			}
		}
	}

	public void InsertSafe(int index, T item)
	{
		if(SynchronizeInvoke is not { InvokeRequired: true } sync)
		{
			Insert(index, item);
		}
		else
		{
			try
			{
				sync.BeginInvoke(new Action<int, T>(Insert), [index, item]);
			}
			catch(ObjectDisposedException)
			{
			}
		}
	}

	public void RemoveSafe(T item)
	{
		if(SynchronizeInvoke is not { InvokeRequired: true } sync)
		{
			Remove(item);
		}
		else
		{
			try
			{
				sync.BeginInvoke(new Func<T, bool>(Remove), [item]);
			}
			catch(ObjectDisposedException)
			{
			}
		}
	}

	public void RemoveAtSafe(int index)
	{
		if(SynchronizeInvoke is not { InvokeRequired: true } sync)
		{
			RemoveAt(index);
		}
		else
		{
			try
			{
				sync.BeginInvoke(new Action<int>(RemoveAt), [index]);
			}
			catch(ObjectDisposedException)
			{
			}
		}
	}

	public void RemoveRangeSafe(int index, int count)
	{
		if(SynchronizeInvoke is not { InvokeRequired: true } sync)
		{
			RemoveRange(index, count);
		}
		else
		{
			try
			{
				sync.BeginInvoke(new Action<int, int>(RemoveRange), [index, count]);
			}
			catch(ObjectDisposedException)
			{
			}
		}
	}

	public void ClearSafe()
	{
		if(SynchronizeInvoke is not { InvokeRequired: true } sync)
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
		if(SynchronizeInvoke is not { InvokeRequired: true } sync)
		{
			InsertSortedFromTop(item, comparer);
		}
		else
		{
			try
			{
				sync.BeginInvoke(
					new Func<T, Func<T, T, int>, int>(InsertSortedFromTop),
					[item, comparer]);
			}
			catch(ObjectDisposedException)
			{
			}
		}
	}

	public void InsertSortedFromBottomSafe(T item, Func<T, T, int> comparer)
	{
		if(SynchronizeInvoke is not { InvokeRequired: true } sync)
		{
			InsertSortedFromBottom(item, comparer);
		}
		else
		{
			try
			{
				sync.BeginInvoke(
					new Func<T, Func<T, T, int>, int>(InsertSortedFromBottom),
					[item, comparer]);
			}
			catch(ObjectDisposedException)
			{
			}
		}
	}

	#endregion
}
