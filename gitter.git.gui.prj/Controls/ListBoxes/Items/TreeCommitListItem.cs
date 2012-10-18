namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public class TreeCommitListItem : TreeItemListItem<TreeCommit>
	{
		private bool _showFullPath;

		public TreeCommitListItem(TreeCommit commit, bool showFullPath)
			: base(commit, showFullPath)
		{
			_showFullPath = showFullPath;
		}

		/// <summary>Called when item is attached to listbox.</summary>
		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.StagedStatusChanged += OnStagedStatusChanged;
			DataContext.StatusChanged += OnStatusChanged; 
		}

		/// <summary>Called when item is detached from listbox.</summary>
		protected override void OnListBoxDetached()
		{
			DataContext.StagedStatusChanged -= OnStagedStatusChanged;
			DataContext.StatusChanged -= OnStatusChanged;
			base.OnListBoxDetached();
		}

		protected virtual void OnStatusChanged(object sender, EventArgs e)
		{
			InvalidateSubItemSafe((int)ColumnId.Name);
		}

		protected virtual void OnStagedStatusChanged(object sender, EventArgs e)
		{
			InvalidateSubItemSafe((int)ColumnId.Name);
		}

		//protected override Icon GetIcon()
		//{
		//    if(Data.Status == FileStatus.Removed)
		//    {
		//        return Utility.ExtractAssociatedFileIcon16ByExt(Data.FullPath);
		//    }
		//    else
		//    {
		//        return Utility.ExtractAssociatedFileIcon16(Data.FullPath);
		//    }
		//}

		protected override FileSize? GetSize()
		{
			return default(FileSize?);
		}

		protected override Bitmap GetBitmapIcon()
		{
			return CachedResources.Bitmaps["ImgSubmodule"];
		}

		protected override string GetItemType()
		{
			return string.Empty;
		}
	}
}
