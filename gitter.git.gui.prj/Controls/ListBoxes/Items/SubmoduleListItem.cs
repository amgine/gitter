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

	public class SubmoduleListItem : CustomListBoxItem<Submodule>
	{
		#region Static

		private static Bitmap ImgSubmodule = CachedResources.Bitmaps["ImgSubmodule"];

		#endregion

		#region Comparers

		public static int CompareByName(SubmoduleListItem item1, SubmoduleListItem item2)
		{
			var data1 = item1.DataContext.Name;
			var data2 = item2.DataContext.Name;
			return string.Compare(data1, data2);
		}

		public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as SubmoduleListItem;
			if(i1 == null) return 0;
			var i2 = item2 as SubmoduleListItem;
			if(i2 == null) return 0;
			try
			{
				return CompareByName(i1, i2);
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

		public static int CompareByPath(SubmoduleListItem item1, SubmoduleListItem item2)
		{
			var data1 = item1.DataContext.Path;
			var data2 = item2.DataContext.Path;
			return string.Compare(data1, data2);
		}

		public static int CompareByPath(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as SubmoduleListItem;
			if(i1 == null) return 0;
			var i2 = item2 as SubmoduleListItem;
			if(i2 == null) return 0;
			try
			{
				return CompareByPath(i1, i2);
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

		public static int CompareByUrl(SubmoduleListItem item1, SubmoduleListItem item2)
		{
			var data1 = item1.DataContext.Url;
			var data2 = item2.DataContext.Url;
			return string.Compare(data1, data2);
		}

		public static int CompareByUrl(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as SubmoduleListItem;
			if(i1 == null) return 0;
			var i2 = item2 as SubmoduleListItem;
			if(i2 == null) return 0;
			try
			{
				return CompareByUrl(i1, i2);
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

		public SubmoduleListItem(Submodule submodule)
			: base(submodule)
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
			DataContext.Deleted += OnDeleted;
			base.OnListBoxAttached();
		}

		protected override void OnListBoxDetached()
		{
			DataContext.Deleted -= OnDeleted;
			base.OnListBoxDetached();
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					return measureEventArgs.MeasureImageAndText(ImgSubmodule, DataContext.Name);
				case ColumnId.Path:
					return measureEventArgs.MeasureText(DataContext.Path);
				case ColumnId.Url:
					return measureEventArgs.MeasureText(DataContext.Url);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					paintEventArgs.PaintImageAndText(ImgSubmodule, DataContext.Name);
					break;
				case ColumnId.Path:
					paintEventArgs.PaintText(DataContext.Path);
					break;
				case ColumnId.Url:
					paintEventArgs.PaintText(DataContext.Url);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new SubmoduleMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		#endregion
	}
}
