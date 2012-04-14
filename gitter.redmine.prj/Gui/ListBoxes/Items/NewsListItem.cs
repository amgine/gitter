namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class NewsListItem : CustomListBoxItem<News>
	{
		#region Comparers

		public static int CompareById(NewsListItem item1, NewsListItem item2)
		{
			var data1 = item1.Data.Id;
			var data2 = item2.Data.Id;
			return data1.CompareTo(data2);
		}

		public static int CompareById(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as NewsListItem;
			if(i1 == null) return 0;
			var i2 = item2 as NewsListItem;
			if(i2 == null) return 0;
			return CompareById(i1, i2);
		}

		public static int CompareByCreatedOn(NewsListItem item1, NewsListItem item2)
		{
			var data1 = item1.Data.CreatedOn;
			var data2 = item2.Data.CreatedOn;
			return data1.CompareTo(data2);
		}

		public static int CompareByCreatedOn(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as NewsListItem;
			if(i1 == null) return 0;
			var i2 = item2 as NewsListItem;
			if(i2 == null) return 0;
			return CompareByCreatedOn(i1, i2);
		}

		public static int CompareByAuthor(NewsListItem item1, NewsListItem item2)
		{
			var data1 = item1.Data.Author;
			var data2 = item2.Data.Author;
			if(data1 == data2) return 0;
			if(data1 == null) return 1;
			else if(data2 == null) return -1;
			return string.Compare(data1.Name, data2.Name);
		}

		public static int CompareByAuthor(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as NewsListItem;
			if(i1 == null) return 0;
			var i2 = item2 as NewsListItem;
			if(i2 == null) return 0;
			return CompareByAuthor(i1, i2);
		}

		public static int CompareByTitle(NewsListItem item1, NewsListItem item2)
		{
			var data1 = item1.Data.Title;
			var data2 = item2.Data.Title;
			return string.Compare(data1, data2);
		}

		public static int CompareByTitle(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as NewsListItem;
			if(i1 == null) return 0;
			var i2 = item2 as NewsListItem;
			if(i2 == null) return 0;
			return CompareByTitle(i1, i2);
		}

		#endregion

		public NewsListItem(News news)
			: base(news)
		{
			if(news == null) throw new ArgumentNullException("news");
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			Data.PropertyChanged += OnNewsPropertyChanged;
		}

		protected override void OnListBoxDetached()
		{
			Data.PropertyChanged -= OnNewsPropertyChanged;
			base.OnListBoxDetached();
		}

		private void OnNewsPropertyChanged(object sender, RedmineObjectPropertyChangedEventArgs e)
		{
			if(e.Property == News.IdProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Id);
			}
			else if(e.Property == News.TitleProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Title);
			}
			else if(e.Property == News.DescriptionProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Description);
			}
			else if(e.Property == News.CreatedOnProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.CreatedOn);
			}
			else if(e.Property == News.SummaryProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Summary);
			}
			else if(e.Property == News.AuthorProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Author);
			}
			else if(e.Property == News.ProjectProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Project);
			}
		}

		private static Size MeasureOptionalContent(NamedRedmineObject data, SubItemMeasureEventArgs measureEventArgs)
		{
			string text;
			if(data == null)
			{
				text = Resources.StrsUnassigned.SurroundWith('<', '>');
			}
			else
			{
				text = data.Name;
			}
			return measureEventArgs.MeasureText(text);
		}

		private static void PaintOptionalContent(NamedRedmineObject data, SubItemPaintEventArgs paintEventArgs)
		{
			string text;
			Brush brush;
			if(data == null)
			{
				text = Resources.StrsUnassigned.SurroundWith('<', '>');
				brush = SystemBrushes.GrayText;
			}
			else
			{
				text = data.Name;
				brush = paintEventArgs.Brush;
			}
			paintEventArgs.PaintText(text, brush);
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Id:
					return measureEventArgs.MeasureText(Data.Id.ToString());
				case ColumnId.Name:
				case ColumnId.Title:
					return measureEventArgs.MeasureText(Data.Title);
				case ColumnId.Summary:
					return measureEventArgs.MeasureText(Data.Summary);
				case ColumnId.Project:
					return MeasureOptionalContent(Data.Project, measureEventArgs);
				case ColumnId.Author:
					return MeasureOptionalContent(Data.Author, measureEventArgs);
				case ColumnId.CreatedOn:
					return measureEventArgs.MeasureText(Data.CreatedOn.ToString());
			}
			return base.MeasureSubItem(measureEventArgs);
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Id:
					paintEventArgs.PaintText(Data.Id.ToString());
					break;
				case ColumnId.Name:
				case ColumnId.Title:
					paintEventArgs.PaintText(Data.Title);
					break;
				case ColumnId.Summary:
					paintEventArgs.PaintText(Data.Summary);
					break;
				case ColumnId.Author:
					PaintOptionalContent(Data.Author, paintEventArgs);
					break;
				case ColumnId.Project:
					PaintOptionalContent(Data.Project, paintEventArgs);
					break;
				case ColumnId.CreatedOn:
					paintEventArgs.PaintText(Data.CreatedOn.ToString());
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new NewsMenu(Data);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
