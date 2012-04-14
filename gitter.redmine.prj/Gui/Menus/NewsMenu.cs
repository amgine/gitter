namespace gitter.Redmine.Gui
{
	using System;
	using System.Globalization;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Redmine.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class NewsMenu : ContextMenuStrip
	{
		private readonly News _news;

		public NewsMenu(News news)
		{
			if(news == null) throw new ArgumentNullException("news");
			_news = news;

			//Items.Add(GuiItemFactory.GetUpdateRedmineObjectItem<ToolStripMenuItem>(_news));

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrId, _news.Id.ToString(CultureInfo.InvariantCulture)));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrTitle, _news.Title));
			if(!string.IsNullOrWhiteSpace(_news.Summary))
			{
				item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSummary, _news.Summary));
			}

			Items.Add(item);
		}

		public News News
		{
			get { return _news; }
		}
	}
}
