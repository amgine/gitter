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

namespace gitter.Framework;

using System;
using System.Collections.Generic;

using gitter.Framework.Options;

public sealed class PropertyPageRepository : IPropertyPageProvider, IPropertyPageRepository
{
	private readonly Dictionary<Guid, IPropertyPageFactory> _propertyPages = new();

	public void RegisterPropertyPageFactory(IPropertyPageFactory description)
	{
		Verify.Argument.IsNotNull(description);

		_propertyPages.Add(description.Guid, description);
	}

	public IReadOnlyList<PropertyPageItem> GetListBoxItems()
	{
		var list = new List<PropertyPageItem>(_propertyPages.Count);
		var dic  = new Dictionary<Guid, PropertyPageItem>(_propertyPages.Count);
		foreach(var kvp in _propertyPages)
		{
			var item = new PropertyPageItem(kvp.Value);
			dic.Add(kvp.Key, item);
			if(kvp.Value.GroupGuid != PropertyPageFactory.RootGroupGuid)
			{
				list.Add(item);
			}
		}
		foreach(var item in list)
		{
			if(dic.TryGetValue(item.DataContext.GroupGuid, out var parent))
			{
				parent.Items.Add(item);
				parent.IsExpanded = true;
				dic.Remove(item.DataContext.Guid);
			}
		}
		list.Clear();
		foreach(var kvp in dic)
		{
			list.Add(kvp.Value);
		}
		return list;
	}
}
