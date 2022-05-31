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
using System.Windows.Forms;

/// <summary>Collection with auto-sorting &amp; change notification.</summary>
/// <typeparam name="T">Item type.</typeparam>
public class NotifySortedCollection<T> : NotifyCollection<T>
{
	#region Data

	private Comparison<T> _comparison;
	private SortOrder _sortOrder;

	#endregion

	#region .ctor

	/// <summary>Create <see cref="NotifySortedCollection{T}"/>.</summary>
	public NotifySortedCollection()
	{
		_sortOrder = SortOrder.Ascending;
	}

	#endregion

	#region Properties

	/// <summary>Comparison used when sorting items.</summary>
	public Comparison<T> Comparison
	{
		get => _comparison;
		set
		{
			if(_comparison != value)
			{
				_comparison = value;
				if(_comparison is not null)
				{
					switch(_sortOrder)
					{
						case SortOrder.Ascending:
							Sort(_comparison);
							break;
						case SortOrder.Descending:
							Sort(InvertedComparison);
							break;
					}
				}
			}
		}
	}

	/// <summary>Item sort order.</summary>
	public SortOrder SortOrder
	{
		get => _sortOrder;
		set
		{
			if(_sortOrder != value)
			{
				var oldOrder = _sortOrder;
				_sortOrder = value;
				if(_sortOrder != SortOrder.None && _comparison != null)
				{
					if(oldOrder == SortOrder.None)
					{
						switch(_sortOrder)
						{
							case SortOrder.Ascending:
								Sort(_comparison);
								break;
							case SortOrder.Descending:
								Sort(InvertedComparison);
								break;
						}
					}
					else
					{
						InvertOrder();
					}
				}
			}
		}
	}

	#endregion

	#region Methods

	/// <summary>Inverts <see cref="M:Comparison"/>.</summary>
	/// <param name="item1">First compared item.</param>
	/// <param name="item2">Second compared item.</param>
	/// <returns>Comparison result.</returns>
	private int InvertedComparison(T item1, T item2)
		=> _comparison(item2, item1);

	/// <summary>Inverts item order in collection.</summary>
	private void InvertOrder()
	{
		var items = Items.Count;
		var arr = new T[items];
		RaiseChanging(0, items - 1, NotifyEvent.Clear);
		Items.CopyTo(arr, 0);
		Items.Clear();
		RaiseChanged(0, items, NotifyEvent.Clear);
		RaiseChanging(0, items - 1, NotifyEvent.Insert);
		for(int i = items - 1; i >= 0; --i)
		{
			Items.Add(arr[i]);
		}
		RaiseChanged(0, items - 1, NotifyEvent.Insert);
	}

	/// <summary>Gets new index for <paramref name="item"/>.</summary>
	/// <param name="index">Suggested index.</param>
	/// <param name="item">Inserted item.</param>
	/// <returns>New index for <paramref name="item"/>.</returns>
	private int GetNewIndex(int index, T item)
	{
		switch(_sortOrder)
		{
			case SortOrder.None:
				return index;
			case SortOrder.Ascending:
				for(int i = Items.Count - 1; i >= 0; --i)
				{
					if(_comparison(Items[i], item) <= 0)
						return i + 1;
				}
				return 0;
			case SortOrder.Descending:
				for(int i = 0; i < Items.Count; ++i)
				{
					if(_comparison(Items[i], item) <= 0)
						return i;
				}
				return Items.Count;
			default:
				return index;
		}
	}

	/// <summary>Adds range of items to this collection.</summary>
	/// <param name="list">Items to add.</param>
	/// <exception cref="T:System.NullReferenceException"><paramref name="list"/> == <c>null</c>.</exception>
	/// <exception cref="T:System.ArgumentException">
	/// Some of the items in <paramref name="list"/> didn't pass <see cref="VerifyItem"/> check.
	/// </exception>
	public override void AddRange(IEnumerable<T> list)
	{
		if(_comparison is null)
		{
			base.AddRange(list);
		}
		else
		{
			foreach(var item in list)
			{
				var index = GetNewIndex(Items.Count, item);
				base.InsertItem(index, item);
			}
		}
	}

	/// <summary>Inserts range of items. Index may be altered by <see cref="M:Comparison"/>.</summary>
	/// <param name="index">Starting index.</param>
	/// <param name="list">Items to insert.</param>
	/// <exception cref="T:System.NullReferenceException"><paramref name="list"/> == <c>null</c>.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
	/// <exception cref="T:System.ArgumentException">
	/// Some of the items in <paramref name="list"/> didn't pass <see cref="VerifyItem"/> check.
	/// </exception>
	public override void InsertRange(int index, IEnumerable<T> list)
	{
		if(_comparison is null)
		{
			base.InsertRange(index, list);
		}
		else
		{
			int id = index;
			foreach(var item in list)
			{
				var newIndex = GetNewIndex(id++, item);
				base.InsertItem(newIndex, item);
			}
		}
	}

	/// <summary>Called on insert item request.</summary>
	/// <param name="index">Item index.</param>
	/// <param name="item">Inserted item.</param>
	/// <exception cref="T:System.ArgumentException"><paramref name="item"/> didn't pass <see cref="VerifyItem"/> check.</exception>
	protected override void InsertItem(int index, T item)
	{
		if(_comparison is null)
			base.InsertItem(index, item);
		else
			base.InsertItem(GetNewIndex(index, item), item);
	}

	/// <summary>Called on set item request.</summary>
	/// <param name="index">Item index.</param>
	/// <param name="item">Inserted item.</param>
	/// <exception cref="T:System.ArgumentException"><paramref name="item"/> didn't pass <see cref="VerifyItem"/> check.</exception>
	protected override void SetItem(int index, T item)
	{
		if(_comparison is null)
		{
			base.SetItem(index, item);
		}
		else
		{
			var index2 = GetNewIndex(index, item);
			if(index == index2)
			{
				base.SetItem(index2, item);
			}
			else
			{
				base.RemoveAt(index);
				base.InsertItem(index2, item);
			}
		}
	}

	#endregion
}
