namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>A <see cref="CustomListBoxItem"/> representing <see cref="StashedState"/> object.</summary>
	public class StashedStateListItem : RevisionPointerListItemBase<StashedState>
	{
		#region Comparers

		public static int CompareByIndex(StashedStateListItem item1, StashedStateListItem item2)
		{
			var data1 = item1.Data.Index;
			var data2 = item2.Data.Index;
			return (data1>data2)?1:((data1==data2)?0:-1);
		}

		public static int CompareByIndex(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as StashedStateListItem;
			if(i1 == null) return 0;
			var i2 = item2 as StashedStateListItem;
			if(i2 == null) return 0;
			try
			{
				return CompareByIndex(i1, i2);
			}
			catch
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

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			Data.Deleted += OnDeleted;
		}

		protected override void OnListBoxDetached()
		{
			Data.Deleted -= OnDeleted;
			base.OnListBoxDetached();
		}

		private void OnDeleted(object sender, EventArgs e)
		{
			RemoveSafe();
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var mnu = new StashedStateMenu(Data);
			Utility.MarkDropDownForAutoDispose(mnu);
			return mnu;
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Hash:
					return HashColumn.OnMeasureSubItem(measureEventArgs, Data.Name);
				case ColumnId.Name:
				case ColumnId.Subject:
					return SubjectColumn.OnMeasureSubItem(measureEventArgs, Data.Revision.Subject);
				case ColumnId.Date:
				case ColumnId.CommitDate:
					return CommitDateColumn.OnMeasureSubItem(measureEventArgs, Data.Revision.CommitDate);
				case ColumnId.Committer:
					return CommitterColumn.OnMeasureSubItem(measureEventArgs, Data.Revision.Committer);
				case ColumnId.CommitterEmail:
					return CommitterEmailColumn.OnMeasureSubItem(measureEventArgs, Data.Revision.Committer.Email);
				case ColumnId.AuthorDate:
					return AuthorDateColumn.OnMeasureSubItem(measureEventArgs, Data.Revision.AuthorDate);
				case ColumnId.User:
				case ColumnId.Author:
					return AuthorColumn.OnMeasureSubItem(measureEventArgs, Data.Revision.Author);
				case ColumnId.AuthorEmail:
					return AuthorEmailColumn.OnMeasureSubItem(measureEventArgs, Data.Revision.Author.Email);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Hash:
					HashColumn.OnPaintSubItem(paintEventArgs, Data.Name);
					break;
				case ColumnId.Name:
				case ColumnId.Subject:
					SubjectColumn.OnPaintSubItem(paintEventArgs, Data.Revision, null, null, -1);
					break;
				case ColumnId.Date:
				case ColumnId.CommitDate:
					CommitDateColumn.OnPaintSubItem(paintEventArgs, Data.Revision.CommitDate);
					break;
				case ColumnId.Committer:
					CommitterColumn.OnPaintSubItem(paintEventArgs, Data.Revision.Committer);
					break;
				case ColumnId.CommitterEmail:
					CommitterEmailColumn.OnPaintSubItem(paintEventArgs, Data.Revision.Committer.Email);
					break;
				case ColumnId.AuthorDate:
					AuthorDateColumn.OnPaintSubItem(paintEventArgs, Data.Revision.AuthorDate);
					break;
				case ColumnId.User:
				case ColumnId.Author:
					AuthorColumn.OnPaintSubItem(paintEventArgs, Data.Revision.Author);
					break;
				case ColumnId.AuthorEmail:
					AuthorEmailColumn.OnPaintSubItem(paintEventArgs, Data.Revision.Author.Email);
					break;
			}
		}
	}
}
