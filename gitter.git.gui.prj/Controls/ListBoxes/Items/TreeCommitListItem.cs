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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;

	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public class TreeCommitListItem : TreeItemListItem<TreeCommit>
	{
		public TreeCommitListItem(TreeCommit commit, bool showFullPath)
			: base(commit, showFullPath)
		{
		}

		/// <summary>Called when item is attached to listbox.</summary>
		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.StagedStatusChanged += OnStagedStatusChanged;
			DataContext.StatusChanged       += OnStatusChanged; 
		}

		/// <summary>Called when item is detached from listbox.</summary>
		protected override void OnListBoxDetached()
		{
			DataContext.StagedStatusChanged -= OnStagedStatusChanged;
			DataContext.StatusChanged       -= OnStatusChanged;
			base.OnListBoxDetached();
		}

		protected virtual void OnStatusChanged(object sender, EventArgs e)
			=> InvalidateSubItemSafe((int)ColumnId.Name);

		protected virtual void OnStagedStatusChanged(object sender, EventArgs e)
			=> InvalidateSubItemSafe((int)ColumnId.Name);

		protected override FileSize? GetSize() => default;

		protected override Bitmap GetBitmapIcon(Dpi dpi)
			=> CachedResources.ScaledBitmaps[@"submodule", DpiConverter.FromDefaultTo(dpi).ConvertX(16)];

		protected override string GetItemType()
			=> string.Empty;
	}
}
