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

namespace gitter.Git.Gui.Controls;

using System;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

/// <summary>A <see cref="CustomListBoxItem"/> representing <see cref="StashedState"/> object.</summary>
public class StashedStateListItem : RevisionPointerListItemBase<StashedState>
{
	#region Comparers

	public static int CompareByIndex(StashedStateListItem item1, StashedStateListItem item2)
	{
		var data1 = item1.DataContext.Index;
		var data2 = item2.DataContext.Index;
		return (data1>data2)?1:((data1==data2)?0:-1);
	}

	public static int CompareByIndex(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		try
		{
			return item1 is StashedStateListItem i1 && item2 is StashedStateListItem i2
				? CompareByIndex(i1, i2)
				: 0;
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			return 0;
		}
	}

	#endregion

	/// <summary>Create <see cref="StashedStateListItem"/>.</summary>
	/// <param name="stashedState">Associated <see cref="StashedState"/>.</param>
	public StashedStateListItem(StashedState stashedState)
		: base(stashedState)
	{
	}

	private void OnDeleted(object? sender, EventArgs e)
	{
		RemoveSafe();
	}

	/// <inheritdoc/>
	protected override void OnListBoxAttached(CustomListBox listBox)
	{
		base.OnListBoxAttached(listBox);
		DataContext.Deleted += OnDeleted;
	}

	/// <inheritdoc/>
	protected override void OnListBoxDetached(CustomListBox listBox)
	{
		DataContext.Deleted -= OnDeleted;
		base.OnListBoxDetached(listBox);
	}

	/// <inheritdoc/>
	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNull(requestEventArgs);

		var menu = new StashedStateMenu(DataContext);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}
}
