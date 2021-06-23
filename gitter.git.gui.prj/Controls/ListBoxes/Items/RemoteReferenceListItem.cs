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
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	public class RemoteReferenceListItem : CustomListBoxItem<IRemoteReference>
	{
		public RemoteReferenceListItem(IRemoteReference reference)
			: base(reference)
		{
		}

		private void OnDeleted(object sender, EventArgs e)
		{
			RemoveSafe();
		}

		/// <inheritdoc/>
		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.Deleted += OnDeleted;
		}

		/// <inheritdoc/>
		protected override void OnListBoxDetached()
		{
			DataContext.Deleted -= OnDeleted;
			base.OnListBoxDetached();
		}

		private Bitmap GetIcon(Dpi dpi)
		{
			var name = DataContext switch
			{
				RemoteRepositoryBranch => @"branch",
				RemoteRepositoryTag tag when tag.TagType == TagType.Annotated => @"atag",
				RemoteRepositoryTag => @"tag",
				_ => null,
			};
			if(name is null) return default;
			return CachedResources.ScaledBitmaps[name, DpiConverter.FromDefaultTo(dpi).ConvertX(16)];
		}

		/// <inheritdoc/>
		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			Assert.IsNotNull(measureEventArgs);

			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					return measureEventArgs.MeasureImageAndText(GetIcon(measureEventArgs.Dpi), DataContext.Name);
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
					paintEventArgs.PaintImageAndText(GetIcon(paintEventArgs.Dpi), DataContext.Name);
					break;
				default:
					base.OnPaintSubItem(paintEventArgs);
					break;
			}
		}

		/// <inheritdoc/>
		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			Assert.IsNotNull(requestEventArgs);

			ContextMenuStrip menu = DataContext switch
			{
				RemoteRepositoryBranch branch => new RemoteBranchMenu(branch),
				RemoteRepositoryTag tag       => new RemoteTagMenu(tag),
				_ => default,
			};
			if(menu is not null)
			{
				Utility.MarkDropDownForAutoDispose(menu);
			}
			return menu;
		}
	}
}
