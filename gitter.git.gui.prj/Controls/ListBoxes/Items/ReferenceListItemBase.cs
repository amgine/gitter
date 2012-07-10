namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	using gitter.Framework.Controls;

	public static class BaseReferenceListItem
	{
		public static int UniversalComparer(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			if(item1.GetType() == item2.GetType())
			{
				var branchItem = item1 as BranchListItem;
				if(branchItem != null)
					return BranchListItem.CompareByName(branchItem, (BranchListItem)item2);
				var remoeBranchItem = item1 as RemoteBranchListItem;
				if(remoeBranchItem != null)
					return RemoteBranchListItem.CompareByName(branchItem, (RemoteBranchListItem)item2);
				var remoteItem = item1 as RemoteListItem;
				if(remoteItem != null)
					return RemoteListItem.CompareByName(remoteItem, (RemoteListItem)item2);
				var tagItem = item1 as TagListItem;
				if(tagItem != null)
					return TagListItem.CompareByName(tagItem, (TagListItem)item2);
				return 0;
			}
			else
			{
				var branchItem = item1 as BranchListItem;
				if(branchItem != null)
					return -1;
				var remoteBranchItem = item1 as RemoteBranchListItem;
				if(remoteBranchItem != null)
				{
					var branchItem2 = item2 as BranchListItem;
					if(branchItem2 != null)
						return 1;
					else
						return -1;
				}
				var remoteListItem = item1 as RemoteListItem;
				if(remoteListItem != null)
				{
					var tagItem2 = item2 as TagListItem;
					if(tagItem2 == null)
						return -1;
					else
						return 1;
				}
				return 1;
			}
		}
	}

	public abstract class ReferenceListItemBase<T> : RevisionPointerListItemBase<T>
		where T : Reference
	{
		#region Comparers

		public static int CompareByName(ReferenceListItemBase<T> item1, ReferenceListItemBase<T> item2)
		{
			var data1 = item1.DataContext;
			var data2 = item2.DataContext;
			return string.Compare(data1.Name, data2.Name);
		}

		public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as ReferenceListItemBase<T>;
			if(i1 == null) return 0;
			var i2 = item2 as ReferenceListItemBase<T>;
			if(i2 == null) return 0;
			try
			{
				return CompareByName(i1, i2);
			}
			catch
			{
				return 0;
			}
		}

		public static int CompareByPosition(ReferenceListItemBase<T> item1, ReferenceListItemBase<T> item2)
		{
			var data1 = item1.DataContext;
			var data2 = item2.DataContext;
			return string.Compare(data1.Revision.Name, data2.Revision.Name);
		}

		public static int CompareByPosition(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as ReferenceListItemBase<T>;
			if(i1 == null) return 0;
			var i2 = item2 as ReferenceListItemBase<T>;
			if(i2 == null) return 0;
			try
			{
				return CompareByPosition(i1, i2);
			}
			catch
			{
				return 0;
			}
		}

		#endregion

		protected ReferenceListItemBase(T data)
			: base(data)
		{
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.Deleted += OnReferenceDeleted;
			DataContext.PositionChanged += OnPositionChanged;
		}

		protected override void OnListBoxDetached()
		{
			DataContext.Deleted -= OnReferenceDeleted;
			DataContext.PositionChanged -= OnPositionChanged;
			base.OnListBoxDetached();
		}

		private void OnPositionChanged(object sender, RevisionChangedEventArgs e)
		{
			InvalidateSubItemSafe((int)ColumnId.Hash);
		}

		private void OnReferenceDeleted(object sender, EventArgs e)
		{
			RemoveSafe();
		}

		protected abstract Image Image { get; }

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					return measureEventArgs.MeasureImageAndText(Image, DataContext.Name);
				case ColumnId.Hash:
					return HashColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.Name);
				case ColumnId.TreeHash:
					return TreeHashColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.TreeHash);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					paintEventArgs.PaintImageAndText(Image, DataContext.Name);
					break;
				case ColumnId.Hash:
					HashColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision.Name);
					break;
				case ColumnId.TreeHash:
					TreeHashColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision.TreeHash);
					break;
			}
		}
	}
}
