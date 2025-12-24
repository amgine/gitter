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

namespace gitter.Git;

using System;

using gitter.Framework.Controls;

using gitter.Git.AccessLayer;
using gitter.Git.Gui.Controls;

sealed class AccessorProviderPicker : CustomObjectPicker<CustomListBox, GitAccessorProviderItem, IGitAccessorProvider>
{
	/// <summary>Initializes a new instance of the <see cref="AccessorProviderPicker"/> class.</summary>
	public AccessorProviderPicker()
	{
		DropDownControl.Columns.Clear();
		DropDownControl.Columns.Add(new NameColumn { SizeMode = ColumnSizeMode.Fill });
	}

	protected override IGitAccessorProvider GetValue(GitAccessorProviderItem item)
		=> item.DataContext;

	public void LoadData(IGitRepositoryProvider provider)
	{
		Verify.Argument.IsNotNull(provider);

		var selected = default(GitAccessorProviderItem);
		DropDownControl.BeginUpdate();
		try
		{
			DropDownControl.Items.Clear();
			foreach(var accessorProvider in provider.GitAccessorProviders)
			{
				var item = new GitAccessorProviderItem(accessorProvider);
				DropDownControl.Items.Add(item);
				if(accessorProvider == provider.ActiveGitAccessorProvider)
				{
					selected = item;
				}
			}
		}
		finally
		{
			DropDownControl.EndUpdate();
		}
		SelectedItem = selected;
	}
}
