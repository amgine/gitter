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
using System.Drawing;

/// <summary>Custom list box composite item.</summary>
/// <typeparam name="TData">The type of the data.</typeparam>
public class CustomListBoxRow<TData> : CustomListBoxItem<TData>, IEnumerable<CustomListBoxSubItem>
{
	#region Data

	private readonly Dictionary<int, CustomListBoxSubItem> _subItems;

	#endregion

	#region .ctor

	/// <summary>Initializes a new instance of the <see cref="CustomListBoxRow{TData}"/> class.</summary>
	/// <param name="data">Data.</param>
	/// <param name="subItems">Sub items.</param>
	public CustomListBoxRow(TData data, IEnumerable<CustomListBoxSubItem> subItems)
		: base(data)
	{
		Verify.Argument.IsNotNull(subItems);
		Verify.Argument.HasNoNullItems(subItems);

		_subItems = new Dictionary<int, CustomListBoxSubItem>();
		foreach(var subItem in subItems)
		{
			_subItems.Add(subItem.Id, subItem);
			subItem.Item = this;
		}
	}

	/// <summary>Initializes a new instance of the <see cref="CustomListBoxRow{TData}"/> class.</summary>
	/// <param name="data">Row data.</param>
	/// <param name="subItems">Sub items.</param>
	public CustomListBoxRow(TData data, params CustomListBoxSubItem[] subItems)
		: base(data)
	{
		Verify.Argument.IsNotNull(subItems);
		Verify.Argument.HasNoNullItems(subItems);

		_subItems = new Dictionary<int, CustomListBoxSubItem>(subItems.Length);
		for(int i = 0; i < subItems.Length; ++i)
		{
			var subItem = subItems[i];
			_subItems.Add(subItems[i].Id, subItem);
			subItem.Item = this;
		}
	}

	/// <summary>Initializes a new instance of the <see cref="CustomListBoxRow{TData}"/> class.</summary>
	/// <param name="data">Row data.</param>
	public CustomListBoxRow(TData data)
		: base(data)
	{
		_subItems = [];
	}

	#endregion

	#region Properties

	/// <summary>Gets the <see cref="CustomListBoxSubItem"/> with the specified id.</summary>
	/// <param name="id">Subitem id to get.</param>
	public CustomListBoxSubItem this[int id] => _subItems[id];

	/// <summary>Collection of subitems.</summary>
	public IEnumerable<CustomListBoxSubItem> SubItems => _subItems.Values;

	/// <summary>Gets the subitem count.</summary>
	/// <value>The sub item count.</value>
	public int SubItemCount => _subItems.Count;

	#endregion

	#region Methods

	/// <summary>Adds the subitem.</summary>
	/// <param name="subItem"><see cref="CustomListBoxSubItem"/> to add.</param>
	public void AddSubItem(CustomListBoxSubItem subItem)
	{
		Verify.Argument.IsNotNull(subItem);

		_subItems.Add(subItem.Id, subItem);
		subItem.Item = this;
	}

	/// <summary>Removes the subitem.</summary>
	/// <param name="subItem"><see cref="CustomListBoxSubItem"/> to remove.</param>
	public void RemoveSubItem(CustomListBoxSubItem subItem)
	{
		Verify.Argument.IsNotNull(subItem);

		_subItems.Remove(subItem.Id);
		subItem.Item = null;
	}

	/// <summary>Removes the subitem.</summary>
	/// <param name="id">Removed subitem's id.</param>
	public void RemoveSubItem(int id)
	{
		if(_subItems.TryGetValue(id, out var subItem))
		{
			subItem.Item = null;
			_subItems.Remove(id);
		}
	}

	/// <summary>Paint subitem..</summary>
	/// <param name="paintEventArgs">Paint event args.</param>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		if(_subItems.TryGetValue(paintEventArgs.SubItemId, out var subItem))
		{
			subItem.Paint(paintEventArgs);
		}
	}

	/// <summary>Measure subitem.</summary>
	/// <param name="measureEventArgs">Measure event args.</param>
	/// <returns>Size of the subitem.</returns>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		return _subItems.TryGetValue(measureEventArgs.SubItemId, out var subItem)
			? subItem.Measure(measureEventArgs)
			: Size.Empty;
	}

	#endregion

	#region IEnumerable<CustomListBoxSubItem> Members

	public IEnumerator<CustomListBoxSubItem> GetEnumerator()
		=> _subItems.Values.GetEnumerator();

	#endregion

	#region IEnumerable Members

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		=> _subItems.Values.GetEnumerator();

	#endregion
}
