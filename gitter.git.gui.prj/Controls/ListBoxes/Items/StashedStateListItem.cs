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

	/// <summary>A <see cref="CustomListBoxItem"/> representing <see cref="StashedState"/> object.</summary>
	public class StashedStateListItem : RevisionPointerListItemBase<StashedState>
	{
		#region Comparers

		public static int CompareByIndex(StashedStateListItem item1, StashedStateListItem item2)
		{
			var data1 = item1.DataContext.Index;
			var data2 = item2.DataContext.Index;
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
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				return 0;
			}
		}

		#endregion

		#region .ctor

		/// <summary>Create <see cref="StashedStateListItem"/>.</summary>
		/// <param name="stashedState">Associated <see cref="StashedState"/>.</param>
		public StashedStateListItem(StashedState stashedState)
			: base(stashedState)
		{
		}

		#endregion

		#region Event Handlers

		private void OnDeleted(object sender, EventArgs e)
		{
			RemoveSafe();
		}

		#endregion

		#region Overrides

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.Deleted += OnDeleted;
		}

		protected override void OnListBoxDetached()
		{
			DataContext.Deleted -= OnDeleted;
			base.OnListBoxDetached();
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new StashedStateMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Hash:
					return HashColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.HashString);
				case ColumnId.Name:
				case ColumnId.Subject:
					return SubjectColumn.OnMeasureSubItem(measureEventArgs, DataContext.Revision.Subject);
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
				case ColumnId.Name:
				case ColumnId.Subject:
					SubjectColumn.OnPaintSubItem(paintEventArgs, DataContext.Revision, null, null, -1);
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

		#endregion
	}
}
