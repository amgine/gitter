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
using System.Collections.ObjectModel;

/// <summary>Collection which notifies user about any changes through events.</summary>
/// <typeparam name="T">Item type.</typeparam>
public class NotifyCollection<T> : Collection<T>
{
	#region Events

	/// <summary>Collection is about to be changed.</summary>
	public event EventHandler<NotifyCollectionEventArgs> Changing;
	/// <summary>Collection has changed.</summary>
	public event EventHandler<NotifyCollectionEventArgs> Changed;

	#endregion

	#region .ctor

	/// <summary>Create <see cref="NotifyCollection{T}"/>.</summary>
	public NotifyCollection()
		: base()
	{
	}

	/// <summary>Create <see cref="NotifyCollection{T}"/>.</summary>
	public NotifyCollection(IList<T> list)
		: base(list)
	{
	}

	#endregion

	#region Public Methods

	/// <summary>Sort collection using specified <paramref name="comparison"/>.</summary>
	/// <param name="comparison">Comparison to use for sorting.</param>
	/// <exception cref="T:System.NullReferenceException"><paramref name="comparison"/> == <c>null</c>.</exception>
	public void Sort(Comparison<T> comparison)
	{
		Verify.Argument.IsNotNull(comparison);

		int items = Items.Count;
		if(items < 2) return;
		var array = new T[items];
		Items.CopyTo(array, 0);
		RaiseChanging(0, items - 1, NotifyEvent.Clear);
		Items.Clear();
		RaiseChanged(0, items - 1, NotifyEvent.Clear);
		Array.Sort<T>(array, comparison);
		RaiseChanging(0, items - 1, NotifyEvent.Insert);
		for(int i = 0; i < items; ++i)
		{
			Items.Add(array[i]);
		}
		RaiseChanged(0, items - 1, NotifyEvent.Insert);
	}

	/// <summary>Sort collection using specified <paramref name="comparer"/>.</summary>
	/// <param name="comparer">Comparer to use for sorting.</param>
	/// <exception cref="T:System.NullReferenceException"><paramref name="comparer"/> == <c>null</c>.</exception>
	public void Sort(IComparer<T> comparer)
	{
		Verify.Argument.IsNotNull(comparer);

		int items = Items.Count;
		if(items < 2) return;
		var array = new T[items];
		Items.CopyTo(array, 0);
		RaiseChanging(0, items - 1, NotifyEvent.Clear);
		Items.Clear();
		RaiseChanged(0, items - 1, NotifyEvent.Clear);
		Array.Sort<T>(array, comparer);
		RaiseChanging(0, items - 1, NotifyEvent.Insert);
		for(int i = 0; i < items; ++i)
		{
			Items.Add(array[i]);
		}
		RaiseChanged(0, items - 1, NotifyEvent.Insert);
	}

	/// <summary>Adds range of items to this collection.</summary>
	/// <param name="list">Items to add.</param>
	/// <exception cref="T:System.NullReferenceException"><paramref name="list"/> == <c>null</c>.</exception>
	/// <exception cref="T:System.ArgumentException">
	/// Some of the items in <paramref name="list"/> didn't pass <see cref="VerifyItem"/> check.
	/// </exception>
	public virtual void AddRange(IEnumerable<T> list)
	{
		Verify.Argument.IsNotNull(list);

		int count = 0;
		foreach(var item in list)
		{
			Verify.Argument.IsTrue(VerifyItem(item), nameof(list), "List contains invalid items.");
			++count;
		}
		int start = Items.Count;
		int end = start + count - 1;
		RaiseChanging(start, end, NotifyEvent.Insert);
		foreach(var item in list)
		{
			AcquireItem(item);
			Items.Add(item);
		}
		RaiseChanged(start, end, NotifyEvent.Insert);
	}

	/// <summary>Inserts range of items.</summary>
	/// <param name="index">Starting index.</param>
	/// <param name="list">Items to insert.</param>
	/// <exception cref="T:System.NullReferenceException"><paramref name="list"/> == <c>null</c>.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
	/// <exception cref="T:System.ArgumentException">
	/// Some of the items in <paramref name="list"/> didn't pass <see cref="VerifyItem"/> check.
	/// </exception>
	public virtual void InsertRange(int index, IEnumerable<T> list)
	{
		Verify.Argument.IsNotNull(list);
		Verify.Argument.IsValidIndex(index, Items.Count, nameof(index));

		int count = 0;
		foreach(var item in list)
		{
			Verify.Argument.IsTrue(VerifyItem(item), nameof(list), "List contains invalid items.");
			++count;
		}

		int start = index;
		int end = start + count - 1;
		RaiseChanging(start, end, NotifyEvent.Insert);
		int id = start;
		foreach(var item in list)
		{
			AcquireItem(item);
			Items.Insert(id++, item);
		}
		RaiseChanged(start, end, NotifyEvent.Insert);
	}

	/// <summary>Removes <paramref name="count"/> items starting from <paramref name="index"/>.</summary>
	/// <param name="index">Index to start removing items from.</param>
	/// <param name="count">Number of items to remove.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	/// <paramref name="index"/> or <paramref name="count"/> is out of range.
	/// </exception>
	public void RemoveRange(int index, int count)
	{
		Verify.Argument.IsValidIndex(index, Items.Count, nameof(index));
		Verify.Argument.IsValidIndex(count, Items.Count - index + 1, nameof(count));

		int start = index;
		int end = index + count - 1;
		RaiseChanging(start, end, NotifyEvent.Remove);
		while(count != 0)
		{
			FreeItem(Items[start]);
			Items.RemoveAt(start);
			--count;
		}
		RaiseChanged(start, end, NotifyEvent.Remove);
	}

	/// <summary>Insert <paramref name="item"/>, assuming collection is sorted, starting checks from start.</summary>
	/// <param name="item">Item to add.</param>
	/// <param name="comparison">Comparison to use.</param>
	/// <returns>Item index.</returns>
	/// <exception cref="T:System.NullReferenceException"><paramref name="comparison"/> == <c>null</c>.</exception>
	/// <exception cref="T:System.ArgumentException"><paramref name="item"/> didn't pass <see cref="VerifyItem"/> check.</exception>
	public int InsertSortedFromTop(T item, Func<T, T, int> comparison)
	{
		Verify.Argument.IsNotNull(comparison);

		for(int i = 0; i < Items.Count; ++i)
		{
			if(comparison(Items[i], item) == -1)
			{
				Insert(i, item);
				return i;
			}
		}
		int id = Items.Count;
		Add(item);
		return id;
	}

	/// <summary>Insert <paramref name="item"/>, assuming collection is sorted, starting checks from end.</summary>
	/// <param name="item">Item to add.</param>
	/// <param name="comparison">Comparison to use.</param>
	/// <returns>Item index.</returns>
	/// <exception cref="T:System.NullReferenceException"><paramref name="comparison"/> == <c>null</c>.</exception>
	/// <exception cref="T:System.ArgumentException"><paramref name="item"/> didn't pass <see cref="VerifyItem"/> check.</exception>
	public int InsertSortedFromBottom(T item, Func<T, T, int> comparison)
	{
		Verify.Argument.IsNotNull(comparison);

		for(int i = Items.Count - 1; i > 0; --i)
		{
			if(comparison(Items[i], item) == 1)
			{
				Insert(i + 1, item);
				return i;
			}
		}
		Insert(0, item);
		return 0;
	}

	#endregion

	#region Protected Methods

	/// <summary>Raise <see cref="Changing"/> event.</summary>
	/// <param name="index">Index of the item which is changing.</param>
	/// <param name="notifyEvent">Change type.</param>
	protected void RaiseChanging(int index, NotifyEvent notifyEvent)
		=> Changing?.Invoke(this, new NotifyCollectionEventArgs(index, notifyEvent));

	/// <summary>Raise <see cref="Changing"/> event.</summary>
	/// <param name="startIndex">Index of the first item in a changing range.</param>
	/// <param name="endIndex">Index of the last item in a changing range.</param>
	/// <param name="notifyEvent">Change type.</param>
	protected void RaiseChanging(int startIndex, int endIndex, NotifyEvent notifyEvent)
		=> Changing?.Invoke(this, new NotifyCollectionEventArgs(startIndex, endIndex, notifyEvent));

	/// <summary>Raise <see cref="Changed"/> event.</summary>
	/// <param name="index">Index of the item which is changed.</param>
	/// <param name="notifyEvent">Change type.</param>
	protected void RaiseChanged(int index, NotifyEvent notifyEvent)
		=> Changed?.Invoke(this, new NotifyCollectionEventArgs(index, notifyEvent));

	/// <summary>Raise <see cref="Changed"/> event.</summary>
	/// <param name="startIndex">Index of the first item in a changed range.</param>
	/// <param name="endIndex">Index of the last item in a changed range.</param>
	/// <param name="notifyEvent">Change type.</param>
	protected void RaiseChanged(int startIndex, int endIndex, NotifyEvent notifyEvent)
		=> Changed?.Invoke(this, new NotifyCollectionEventArgs(startIndex, endIndex, notifyEvent));

	/// <summary>Called when item is removed from collection.</summary>
	/// <param name="item">Removed item.</param>
	protected virtual void FreeItem(T item)
	{
	}

	/// <summary>Checks if item can be added to collection.</summary>
	/// <param name="item">Item which is about to be added to collection.</param>
	/// <returns>true if <paramref name="item"/> can be added to this collection.</returns>
	protected virtual bool VerifyItem(T item)
	{
		return true;
	}

	/// <summary>Called when item is added to collection.</summary>
	/// <param name="item">Added item.</param>
	protected virtual void AcquireItem(T item)
	{
	}

	#endregion

	#region Overrides

	/// <summary>Called on <see cref="Clear"/> request.</summary>
	protected override void ClearItems()
	{
		int n = Count - 1;
		RaiseChanging(0, n, NotifyEvent.Clear);
		for(int i = 0; i < Items.Count; ++i)
		{
			FreeItem(Items[i]);
		}
		base.ClearItems();
		RaiseChanged(0, n, NotifyEvent.Clear);
	}

	/// <summary>Called on set item request.</summary>
	/// <param name="index">Item index.</param>
	/// <param name="item">Inserted item.</param>
	/// <exception cref="T:System.ArgumentException"><paramref name="item"/> didn't pass <see cref="VerifyItem"/> check.</exception>
	protected override void SetItem(int index, T item)
	{
		Verify.Argument.IsTrue(VerifyItem(item), nameof(item));

		RaiseChanging(index, NotifyEvent.Set);
		FreeItem(Items[index]);
		AcquireItem(item);
		base.SetItem(index, item);
		RaiseChanged(index, NotifyEvent.Set);
	}

	/// <summary>Called on insert item request.</summary>
	/// <param name="index">Item index.</param>
	/// <param name="item">Inserted item.</param>
	/// <exception cref="T:System.ArgumentException"><paramref name="item"/> didn't pass <see cref="VerifyItem"/> check.</exception>
	protected override void InsertItem(int index, T item)
	{
		Verify.Argument.IsTrue(VerifyItem(item), nameof(item));

		AcquireItem(item);
		RaiseChanging(index, NotifyEvent.Insert);
		base.InsertItem(index, item);
		RaiseChanged(index, NotifyEvent.Insert);
	}

	/// <summary>Called on remove item request.</summary>
	/// <param name="index">Item index.</param>
	protected override void RemoveItem(int index)
	{
		RaiseChanging(index, NotifyEvent.Remove);
		FreeItem(Items[index]);
		base.RemoveItem(index);
		RaiseChanged(index, NotifyEvent.Remove);
	}

	#endregion
}
