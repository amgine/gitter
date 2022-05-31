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

public class NotifyCollectionBinding<T> : IDisposable
{
	private readonly CustomListBoxItemsCollection _itemsCollection;
	private readonly NotifyCollection<T> _boundCollection;
	private readonly Converter<T, CustomListBoxItem> _itemConverter;

	public NotifyCollectionBinding(
		CustomListBoxItemsCollection itemsCollection,
		NotifyCollection<T> boundCollection,
		Converter<T, CustomListBoxItem> itemConverter)
	{
		Verify.Argument.IsNotNull(itemsCollection);
		Verify.Argument.IsNotNull(boundCollection);
		Verify.Argument.IsNotNull(itemConverter);

		_itemsCollection = itemsCollection;
		_boundCollection = boundCollection;
		_itemConverter = itemConverter;

		_itemsCollection.Clear();
		foreach(var item in _boundCollection)
		{
			_itemsCollection.Add(_itemConverter(item));
		}
		_boundCollection.Changed += OnChanged;
	}

	private void OnChanged(object sender, NotifyCollectionEventArgs e)
	{
		switch(e.Event)
		{
			case NotifyEvent.Insert:
				for(int i = e.StartIndex; i <= e.EndIndex; ++i)
				{
					var item = _itemConverter(_boundCollection[i]);
					_itemsCollection.InsertSafe(i, item);
				}
				break;
			case NotifyEvent.Remove:
				_itemsCollection.RemoveRangeSafe(e.StartIndex, e.ModifiedItems);
				break;
			case NotifyEvent.Set:
				for(int i = e.StartIndex; i <= e.EndIndex; ++i)
				{
					var item = _itemConverter(_boundCollection[i]);
					_itemsCollection[i] = item;
				}
				break;
			case NotifyEvent.Clear:
				_itemsCollection.ClearSafe();
				break;
		}
	}

	public void Dispose()
	{
		_boundCollection.Changed -= OnChanged;
		_itemsCollection.Clear();
	}
}
