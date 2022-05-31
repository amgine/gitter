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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

/// <summary><see cref="CustomListBoxItem"/>, representing <see cref="ReflogRecord"/>.</summary>
public class ReflogRecordListItem : RevisionPointerListItemBase<ReflogRecord>
{
	#region Static

	private static IImageProvider GetImage(string message)
	{
		if(string.IsNullOrWhiteSpace(message))
		{
			return null;
		}
		if(message.StartsWith("fetch"))
		{
			return Icons.Fetch;
		}
		if(message.StartsWith("pull"))
		{
			return Icons.Pull;
		}
		if(message.StartsWith("branch: Created "))
		{
			return Icons.Branch;
		}
		if(message.StartsWith("Branch: renamed "))
		{
			return Icons.BranchRename;
		}
		if(message.StartsWith("branch: Reset "))
		{
			return Icons.Reset;
		}
		if(message.StartsWith("reset:"))
		{
			return Icons.Reset;
		}
		if(message.StartsWith("update by push"))
		{
			return Icons.Push;
		}
		if(message.StartsWith("commit"))
		{
			return Icons.Commit;
		}
		if(message.StartsWith("merge"))
		{
			return Icons.Merge;
		}
		if(message.StartsWith("rebase"))
		{
			return Icons.Rebase;
		}
		if(message.StartsWith("checkout:"))
		{
			return Icons.Checkout;
		}
		if(message.StartsWith("cherry-pick"))
		{
			return Icons.CherryPick;
		}
		if(message.StartsWith("revert"))
		{
			return Icons.Revert;
		}
		if(message.EndsWith(": updating HEAD"))
		{
			return Icons.Reset;
		}
		if(message.StartsWith("clone:"))
		{
			return CommonIcons.Clone;
		}
		return null;
	}

	#endregion

	#region Data

	private readonly List<PointerBounds> _drawnPointers = new();

	#endregion

	#region .ctor

	/// <summary>Create <see cref="RevisionListItem"/>.</summary>
	/// <param name="reflogRecord">Associated revision.</param>
	public ReflogRecordListItem(ReflogRecord reflogRecord)
		: base(reflogRecord)
	{
		UpdateImage();
	}

	#endregion

	#region Properties

	public IImageProvider ImageProvider { get; private set; }

	public List<PointerBounds> DrawnPointers => _drawnPointers;

	#endregion

	#region Methods

	private void UpdateImage() => ImageProvider = GetImage(DataContext.Message);

	private void OnDeleted(object sender, EventArgs e)
	{
		RemoveSafe();
	}

	private void OnMessageChanged(object sender, EventArgs e)
	{
		UpdateImage();
		InvalidateSubItemSafe((int)ColumnId.Message);
	}

	#endregion

	#region Overrides

	/// <inheritdoc/>
	protected override void OnListBoxAttached()
	{
		base.OnListBoxAttached();
		DataContext.Deleted        += OnDeleted;
		DataContext.MessageChanged += OnMessageChanged;
	}

	/// <inheritdoc/>
	protected override void OnListBoxDetached()
	{
		DataContext.Deleted        -= OnDeleted;
		DataContext.MessageChanged -= OnMessageChanged;
		base.OnListBoxDetached();
	}

	/// <inheritdoc/>
	protected override int OnHitTest(int x, int y)
	{
		for(int i = 0; i < _drawnPointers.Count; ++i)
		{
			var rc = _drawnPointers[i].Bounds;
			if(rc.X <= x && rc.Right > x)
			{
				return SubjectColumn.PointerTagHitOffset + i;
			}
		}
		return base.OnHitTest(x, y);
	}

	/// <inheritdoc/>
	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNotNull(requestEventArgs);

		var menu = default(ContextMenuStrip);
		switch((ColumnId)requestEventArgs.SubItemId)
		{
			case ColumnId.Subject:
				var x = requestEventArgs.X - requestEventArgs.ItemBounds.X;
				var y = requestEventArgs.Y - requestEventArgs.ItemBounds.Y;
				menu = PointerBounds.GetContextMenu(_drawnPointers, x, y);
				break;
		}
		menu ??= new ReflogRecordMenu(DataContext);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}

	#endregion
}
