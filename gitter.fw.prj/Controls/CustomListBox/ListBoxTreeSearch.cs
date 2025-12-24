#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

/// <summary>Implements search for <see cref="CustomListBox"/> displaying a tree.</summary>
/// <typeparam name="T">Search options type.</typeparam>
public abstract class ListBoxTreeSearch<T> : SearchBase, ISearch<T>
	where T : SearchOptions
{
	protected ListBoxTreeSearch(CustomListBox listBox)
	{
		Verify.Argument.IsNotNull(listBox);

		ListBox = listBox;
	}

	protected CustomListBox ListBox { get; }

	protected abstract bool TestItem(CustomListBoxItem item, T search);

	private static CustomListBoxItemsCollection? GetParentItems(CustomListBoxItem item)
		=> item.Parent?.Items ?? item.ListBox?.Items;

	private static CustomListBoxItem? First(CustomListBoxItemsCollection items)
	{
		return items.Count != 0 ? items[0] : null;
	}

	private static CustomListBoxItem? Last(CustomListBoxItemsCollection items)
	{
		if(items.Count == 0) return null;
		var item = items[items.Count - 1];
		while(item.Items.Count > 0)
		{
			item = item.Items[item.Items.Count - 1];
		}
		return item;
	}

	private static CustomListBoxItem? Next(CustomListBoxItem? item)
	{
		if(item is null) return default;
		if(item.Items.Count > 0)
		{
			return item.Items[0];
		}
		do
		{
			var items = GetParentItems(item);
			if(items is not null)
			{
				var index = items.IndexOf(item);
				if(index < items.Count - 1)
				{
					return items[index + 1];
				}
			}
			item = item.Parent;
		}
		while(item is not null);
		return default;
	}

	private static CustomListBoxItem? Prev(CustomListBoxItem? item)
	{
		if(item is null) return default;
		var items = GetParentItems(item);
		if(items is not null)
		{
			var index = items.IndexOf(item);
			if(index > 0)
			{
				item = items[index - 1];
				return Last(item.Items) ?? item;
			}
		}
		return item.Parent;
	}

	private bool Search(CustomListBoxItem? start, T search, int direction)
	{
		if(search.Text is not { Length: not 0 }) return true;

		start ??= direction switch
		{
			 1 => First(ListBox.Items),
			-1 => Last (ListBox.Items),
			_  => throw new ArgumentException("Invalid direction", nameof(direction)),
		};

		if(start is null) return false;

		var current = start;
		do
		{
			if(TestItem(current, search))
			{
				current.FocusAndSelect();
				return true;
			}

			current = direction switch
			{
				 1 => Next(current) ?? First(ListBox.Items),
				-1 => Prev(current) ?? Last (ListBox.Items),
				_  => throw new ArgumentException("Invalid direction", nameof(direction)),
			};
		}
		while(current is not null && current != start);
		return false;
	}

	public bool First(T search)
	{
		Verify.Argument.IsNotNull(search);

		return Search(null, search, 1);
	}

	public bool Current(T search)
	{
		Verify.Argument.IsNotNull(search);

		if(search.Text is not { Length: not 0 }) return true;
		if(ListBox.SelectedItems.Count == 0)
		{
			return Search(null, search, 1);
		}
		return Search(ListBox.SelectedItems[0], search, 1);
	}

	public bool Next(T search)
	{
		Verify.Argument.IsNotNull(search);

		if(search.Text is not { Length: not 0 }) return true;
		if(ListBox.SelectedItems.Count == 0)
		{
			return Search(null, search, 1);
		}
		return Search(Next(ListBox.SelectedItems[0]) ?? First(ListBox.Items), search, 1);
	}

	public bool Previous(T search)
	{
		Verify.Argument.IsNotNull(search);

		if(search.Text is not { Length: not 0 }) return true;
		if(ListBox.SelectedItems.Count == 0)
		{
			return Search(null, search, 1);
		}
		return Search(Prev(ListBox.SelectedItems[0]) ?? Last(ListBox.Items), search, -1);
	}
}
