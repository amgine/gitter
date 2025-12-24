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

using gitter.Framework;
using gitter.Framework.Controls;

public static class ReferenceListItemBase
{
	public static int UniversalComparer(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		if(item1.GetType() == item2.GetType())
		{
			return item1 switch
			{
				BranchListItem       branchItem       => BranchListItem      .CompareByName(branchItem,       (BranchListItem)item2),
				RemoteBranchListItem remoteBranchItem => RemoteBranchListItem.CompareByName(remoteBranchItem, (RemoteBranchListItem)item2),
				RemoteListItem       remoteItem       => RemoteListItem      .CompareByName(remoteItem,       (RemoteListItem)item2),
				TagListItem          tagItem          => TagListItem         .CompareByName(tagItem,          (TagListItem)item2),
				_ => 0,
			};
		}
		else
		{
			return item1 switch
			{
				BranchListItem       _ => -1,
				RemoteBranchListItem _ => item2 is BranchListItem ? 1 : -1,
				RemoteListItem       _ => item2 is TagListItem    ? 1 : -1,
				_ => 1,
			};
		}
	}
}

public abstract class ReferenceListItemBase<T>(T data) : RevisionPointerListItemBase<T>(data)
	where T : Reference
{
	public static int CompareByName(ReferenceListItemBase<T> item1, ReferenceListItemBase<T> item2)
	{
		var data1 = item1.DataContext;
		var data2 = item2.DataContext;
		return string.Compare(data1.Name, data2.Name);
	}

	public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
		=> item1 is ReferenceListItemBase<T> i1 && item2 is ReferenceListItemBase<T> i2
			? CompareByName(i1, i2)
			: 0;

	public static int CompareByPosition(ReferenceListItemBase<T> item1, ReferenceListItemBase<T> item2)
	{
		var data1 = item1.DataContext;
		var data2 = item2.DataContext;
		return string.Compare(data1.Revision?.HashString, data2.Revision?.HashString);
	}

	public static int CompareByPosition(CustomListBoxItem item1, CustomListBoxItem item2)
		=> item1 is ReferenceListItemBase<T> i1 && item2 is ReferenceListItemBase<T> i2
			? CompareByPosition(i1, i2)
			: 0;

	protected abstract Image? GetImage(Dpi dpi);

	private void OnPositionChanged(object? sender, RevisionChangedEventArgs e)
	{
		InvalidateSafe();
	}

	private void OnReferenceDeleted(object? sender, EventArgs e)
	{
		RemoveSafe();
	}

	protected override void OnListBoxAttached(CustomListBox listBox)
	{
		base.OnListBoxAttached(listBox);
		DataContext.Deleted += OnReferenceDeleted;
		DataContext.PositionChanged += OnPositionChanged;
	}

	protected override void OnListBoxDetached(CustomListBox listBox)
	{
		DataContext.Deleted -= OnReferenceDeleted;
		DataContext.PositionChanged -= OnPositionChanged;
		base.OnListBoxDetached(listBox);
	}

	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		=> (ColumnId)measureEventArgs.SubItemId switch
		{
			ColumnId.Name => measureEventArgs.MeasureImageAndText(GetImage(measureEventArgs.Dpi), DataContext.Name),
			_ => base.OnMeasureSubItem(measureEventArgs),
		};

	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		switch((ColumnId)paintEventArgs.SubItemId)
		{
			case ColumnId.Name:
				paintEventArgs.PaintImageAndText(GetImage(paintEventArgs.Dpi), DataContext.Name);
				break;
			default:
				base.OnPaintSubItem(paintEventArgs);
				break;
		}
	}
}
