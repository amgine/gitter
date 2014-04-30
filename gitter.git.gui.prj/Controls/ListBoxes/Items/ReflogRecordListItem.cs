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
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	/// <summary><see cref="CustomListBoxItem"/>, representing <see cref="ReflogRecord"/>.</summary>
	public class ReflogRecordListItem : RevisionPointerListItemBase<ReflogRecord>
	{
		#region Static

		private static Image GetImage(string message)
		{
			if(string.IsNullOrWhiteSpace(message))
			{
				return null;
			}
			if(message.StartsWith("fetch"))
			{
				return CachedResources.Bitmaps["ImgFetch"];
			}
			if(message.StartsWith("pull"))
			{
				return CachedResources.Bitmaps["ImgPull"];
			}
			if(message.StartsWith("branch: Created "))
			{
				return CachedResources.Bitmaps["ImgBranch"];
			}
			if(message.StartsWith("Branch: renamed "))
			{
				return CachedResources.Bitmaps["ImgBranchRename"];
			}
			if(message.StartsWith("branch: Reset "))
			{
				return CachedResources.Bitmaps["ImgReset"];
			}
			if(message.StartsWith("reset:"))
			{
				return CachedResources.Bitmaps["ImgReset"];
			}
			if(message.StartsWith("update by push"))
			{
				return CachedResources.Bitmaps["ImgPush"];
			}
			if(message.StartsWith("commit"))
			{
				return CachedResources.Bitmaps["ImgCommit"];
			}
			if(message.StartsWith("merge"))
			{
				return CachedResources.Bitmaps["ImgMerge"];
			}
			if(message.StartsWith("rebase"))
			{
				return CachedResources.Bitmaps["ImgRebase"];
			}
			if(message.StartsWith("checkout:"))
			{
				return CachedResources.Bitmaps["ImgCheckout"];
			}
			if(message.StartsWith("cherry-pick"))
			{
				return CachedResources.Bitmaps["ImgCherryPick"];
			}
			if(message.StartsWith("revert"))
			{
				return CachedResources.Bitmaps["ImgRevert"];
			}
			if(message.EndsWith(": updating HEAD"))
			{
				return CachedResources.Bitmaps["ImgReset"];
			}
			if(message.StartsWith("clone:"))
			{
				return CachedResources.Bitmaps["ImgClone"];
			}
			return null;
		}

		#endregion

		#region Constants

		private const int PointerTagHitOffset = 1;

		#endregion

		#region Data

		private readonly List<PointerBounds> _drawnPointers;
		private Image _image;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="RevisionListItem"/>.</summary>
		/// <param name="reflogRecord">Associated revision.</param>
		public ReflogRecordListItem(ReflogRecord reflogRecord)
			: base(reflogRecord)
		{
			var revision = reflogRecord.Revision;
			_drawnPointers = new List<PointerBounds>();
			UpdateImage();
		}

		#endregion

		#region Properties

		public Image Image
		{
			get { return _image; }
			private set { _image = value; }
		}

		#endregion

		#region Methods

		private void UpdateImage()
		{
			Image = GetImage(DataContext.Message);
		}

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

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.Deleted += OnDeleted;
			DataContext.MessageChanged += OnMessageChanged;
		}

		protected override void OnListBoxDetached()
		{
			DataContext.Deleted -= OnDeleted;
			DataContext.MessageChanged -= OnMessageChanged;
			base.OnListBoxDetached();
		}

		protected override int OnHitTest(int x, int y)
		{
			for(int i = 0; i < _drawnPointers.Count; ++i)
			{
				var rc = _drawnPointers[i].Bounds;
				if(rc.X <= x && rc.Right > x)
				{
					return PointerTagHitOffset + i;
				}
			}
			return base.OnHitTest(x, y);
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Hash:
					return HashColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.HashString);
				case ColumnId.TreeHash:
					return TreeHashColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.TreeHashString);
				case ColumnId.Name:
				case ColumnId.Message:
					return measureEventArgs.MeasureImageAndText(Image, DataContext.Message);
				case ColumnId.Subject:
					return SubjectColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision, null);
				case ColumnId.Date:
				case ColumnId.CommitDate:
					return CommitDateColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.CommitDate);
				case ColumnId.Committer:
					return CommitterColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.Committer);
				case ColumnId.CommitterEmail:
					return CommitterEmailColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.Committer.Email);
				case ColumnId.AuthorDate:
					return AuthorDateColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.AuthorDate);
				case ColumnId.User:
				case ColumnId.Author:
					return AuthorColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.Author);
				case ColumnId.AuthorEmail:
					return AuthorEmailColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.Author.Email);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Hash:
					HashColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision.HashString);
					break;
				case ColumnId.TreeHash:
					TreeHashColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision.TreeHashString);
					break;
				case ColumnId.Name:
				case ColumnId.Message:
					paintEventArgs.PaintImageAndText(Image, DataContext.Message);
					break;
				case ColumnId.Subject:
					SubjectColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision, null, _drawnPointers, paintEventArgs.HoveredPart - PointerTagHitOffset);
					break;
				case ColumnId.Date:
				case ColumnId.CommitDate:
					CommitDateColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision.CommitDate);
					break;
				case ColumnId.Committer:
					CommitterColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision.Committer);
					break;
				case ColumnId.CommitterEmail:
					CommitterEmailColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision.Committer.Email);
					break;
				case ColumnId.AuthorDate:
					AuthorDateColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision.AuthorDate);
					break;
				case ColumnId.User:
				case ColumnId.Author:
					AuthorColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision.Author);
					break;
				case ColumnId.AuthorEmail:
					AuthorEmailColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision.Author.Email);
					break;
			}
		}

		/// <summary>Gets the context menu.</summary>
		/// <param name="requestEventArgs">Request parameters.</param>
		/// <returns>Context menu for specified location.</returns>
		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			ContextMenuStrip menu = null;
			switch((ColumnId)requestEventArgs.SubItemId)
			{
				case ColumnId.Subject:
					menu = PointerBounds.GetContextMenu(_drawnPointers, requestEventArgs.X, requestEventArgs.Y);
					break;
			}
			if(menu == null)
			{
				menu = new ReflogRecordMenu(DataContext);
			}
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		#endregion
	}
}
