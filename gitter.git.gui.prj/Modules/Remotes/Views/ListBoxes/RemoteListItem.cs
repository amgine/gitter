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

/// <summary><see cref="gitter.Framework.Controls.CustomListBoxItem"/> representing <see cref="gitter.Git.Remote"/>.</summary>
public class RemoteListItem : CustomListBoxItem<Remote>
{
	public static int CompareByName(RemoteListItem item1, RemoteListItem item2)
	{
		var data1 = item1.DataContext.Name;
		var data2 = item2.DataContext.Name;
		return string.Compare(data1, data2);
	}

	public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		try
		{
			return item1 is RemoteListItem i1 && item2 is RemoteListItem i2
				? CompareByName(i1, i2)
				: 0;
		}
		catch(Exception exc) when(!exc.IsCritical())
		{
			return 0;
		}
	}

	public static int CompareByFetchUrl(RemoteListItem item1, RemoteListItem item2)
	{
		var data1 = item1.DataContext.FetchUrl;
		var data2 = item2.DataContext.FetchUrl;
		return string.Compare(data1, data2);
	}

	public static int CompareByFetchUrl(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		try
		{
			return item1 is RemoteListItem i1 && item2 is RemoteListItem i2
				? CompareByFetchUrl(i1, i2)
				: 0;
		}
		catch(Exception exc) when(!exc.IsCritical())
		{
			return 0;
		}
	}

	public static int CompareByPushUrl(RemoteListItem item1, RemoteListItem item2)
	{
		var data1 = item1.DataContext.PushUrl;
		var data2 = item2.DataContext.PushUrl;
		return string.Compare(data1, data2);
	}

	public static int CompareByPushUrl(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		try
		{
			return item1 is RemoteListItem i1 && item2 is RemoteListItem i2
				? CompareByPushUrl(i1, i2)
				: 0;
		}
		catch(Exception exc) when(!exc.IsCritical())
		{
			return 0;
		}
	}

	/// <summary>Create <see cref="RemoteListItem"/>.</summary>
	/// <param name="remote">Related <see cref="Remote"/>.</param>
	/// <exception cref="ArgumentNullException"><paramref name="remote"/> == <c>null</c>.</exception>
	public RemoteListItem(Remote remote)
		: base(remote)
	{
		Verify.Argument.IsNotNull(remote);
	}

	private void OnRemoteDeleted(object sender, EventArgs e)
	{
		RemoveSafe();
	}

	private void OnRenamed(object sender, EventArgs e)
	{
		if(EnsureSortOrderSafe())
		{
			InvalidateSubItemSafe((int)ColumnId.Name);
		}
	}

	/// <inheritdoc/>
	protected override void OnListBoxAttached()
	{
		DataContext.Deleted += OnRemoteDeleted;
		DataContext.Renamed += OnRenamed;
		base.OnListBoxAttached();
	}

	/// <inheritdoc/>
	protected override void OnListBoxDetached()
	{
		DataContext.Deleted -= OnRemoteDeleted;
		DataContext.Renamed -= OnRenamed;
		base.OnListBoxDetached();
	}

	private static Image GetImage(Dpi dpi) => Icons.Remote.GetImage(16 * dpi.X / 96);

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		switch((ColumnId)measureEventArgs.SubItemId)
		{
			case ColumnId.Name:
				return measureEventArgs.MeasureImageAndText(GetImage(measureEventArgs.Dpi), DataContext.Name);
			default:
				return base.OnMeasureSubItem(measureEventArgs);
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

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

	/// <inheritdoc/>
	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		var mnu = new RemoteMenu(DataContext);
		Utility.MarkDropDownForAutoDispose(mnu);
		return mnu;
	}
}
