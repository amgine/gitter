namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	/// <summary><see cref="CustomListBoxItem"/>, representing <see cref="ReflogRecord"/>.</summary>
	public class ReflogRecordListItem : RevisionPointerListItemBase<ReflogRecord>
	{
		private readonly List<Tuple<Rectangle, IRevisionPointer>> _drawnPointers;

		private Image _image;

		private const int PointerTagHitOffset = 1;

		/// <summary>Create <see cref="RevisionListItem"/>.</summary>
		/// <param name="reflogRecord">Associated revision.</param>
		public ReflogRecordListItem(ReflogRecord reflogRecord)
			: base(reflogRecord)
		{
			var revision = reflogRecord.Revision;
			_drawnPointers = new List<Tuple<Rectangle, IRevisionPointer>>();
			UpdateImage();
		}

		private void UpdateImage()
		{
			if(DataContext.Message.StartsWith("fetch"))
			{
				_image = CachedResources.Bitmaps["ImgFetch"];
			}
			else if(DataContext.Message.StartsWith("pull"))
			{
				_image = CachedResources.Bitmaps["ImgPull"];
			}
			else if(DataContext.Message.StartsWith("branch: Created "))
			{
				_image = CachedResources.Bitmaps["ImgBranch"];
			}
			else if(DataContext.Message.StartsWith("branch: Reset "))
			{
				_image = CachedResources.Bitmaps["ImgReset"];
			}
			else if(DataContext.Message.StartsWith("reset:"))
			{
				_image = CachedResources.Bitmaps["ImgReset"];
			}
			else if(DataContext.Message.StartsWith("update by push"))
			{
				_image = CachedResources.Bitmaps["ImgPush"];
			}
			else if(DataContext.Message.StartsWith("commit"))
			{
				_image = CachedResources.Bitmaps["ImgCommit"];
			}
			else if(DataContext.Message.StartsWith("merge"))
			{
				_image = CachedResources.Bitmaps["ImgMerge"];
			}
			else if(DataContext.Message.StartsWith("rebase"))
			{
				_image = CachedResources.Bitmaps["ImgRebase"];
			}
			else if(DataContext.Message.StartsWith("checkout:"))
			{
				_image = CachedResources.Bitmaps["ImgCheckout"];
			}
			else if(DataContext.Message.StartsWith("cherry-pick"))
			{
				_image = CachedResources.Bitmaps["ImgCherryPick"];
			}
			else if(DataContext.Message.StartsWith("revert"))
			{
				_image = CachedResources.Bitmaps["ImgRevert"];
			}
			else if(DataContext.Message.EndsWith(": updating HEAD"))
			{
				_image = CachedResources.Bitmaps["ImgReset"];
			}
			else
			{
				_image = null;
			}
		}

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

		private void OnDeleted(object sender, EventArgs e)
		{
			RemoveSafe();
		}

		private void OnMessageChanged(object sender, EventArgs e)
		{
			UpdateImage();
			InvalidateSubItemSafe((int)ColumnId.Message);
		}

		protected override int OnHitTest(int x, int y)
		{
			for(int i = 0; i < _drawnPointers.Count; ++i)
			{
				var rc = _drawnPointers[i].Item1;
				if(rc.X <= x && rc.Right > x)
					return PointerTagHitOffset + i;
			}
			return base.OnHitTest(x, y);
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Hash:
					return HashColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.Name);
				case ColumnId.TreeHash:
					return TreeHashColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.TreeHash);
				case ColumnId.Name:
				case ColumnId.Message:
					return measureEventArgs.MeasureImageAndText(_image, DataContext.Message);
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
					HashColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision.Name);
					break;
				case ColumnId.TreeHash:
					TreeHashColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision.TreeHash);
					break;
				case ColumnId.Name:
				case ColumnId.Message:
					paintEventArgs.PaintImageAndText(_image, DataContext.Message);
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
			var menu = new ReflogRecordMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
