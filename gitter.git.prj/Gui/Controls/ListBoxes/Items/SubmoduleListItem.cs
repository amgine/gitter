namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	public sealed class SubmoduleListItem : CustomListBoxItem<Submodule>
	{
		private static Bitmap ImgIcon = CachedResources.Bitmaps["ImgSubmodule"];

		#region Comparers

		public static int CompareByName(SubmoduleListItem item1, SubmoduleListItem item2)
		{
			var data1 = item1.Data.Name;
			var data2 = item2.Data.Name;
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
			catch
			{
				return 0;
			}
		}

		public static int CompareByPath(SubmoduleListItem item1, SubmoduleListItem item2)
		{
			var data1 = item1.Data.Path;
			var data2 = item2.Data.Path;
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
			catch
			{
				return 0;
			}
		}

		public static int CompareByUrl(SubmoduleListItem item1, SubmoduleListItem item2)
		{
			var data1 = item1.Data.Url;
			var data2 = item2.Data.Url;
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
			catch
			{
				return 0;
			}
		}

		#endregion

		public SubmoduleListItem(Submodule submodule)
			: base(submodule)
		{
		}

		protected override void OnListBoxAttached()
		{
			Data.Deleted += OnDeleted;
			base.OnListBoxAttached();
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

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					return measureEventArgs.MeasureImageAndText(ImgIcon, Data.Name);
				case ColumnId.Path:
					return measureEventArgs.MeasureText(Data.Path);
				case ColumnId.Url:
					return measureEventArgs.MeasureText(Data.Url);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					paintEventArgs.PaintImageAndText(ImgIcon, Data.Name);
					break;
				case ColumnId.Path:
					paintEventArgs.PaintText(Data.Path);
					break;
				case ColumnId.Url:
					paintEventArgs.PaintText(Data.Url);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new SubmoduleMenu(Data);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
