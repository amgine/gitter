namespace gitter.Framework.Controls
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework.Services;

	using Resources = gitter.Framework.Properties.Resources;

	public class HyperlinkContextMenu : ContextMenuStrip
	{
		public HyperlinkContextMenu(Hyperlink hyperlink)
		{
			Verify.Argument.IsNotNull(hyperlink, nameof(hyperlink));

			Items.Add(GetOpenInBrowserItem<ToolStripMenuItem>(hyperlink.Url));
			Items.Add(GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyLinkLocation, hyperlink.Url));
		}

		public HyperlinkContextMenu(string url)
		{
			Verify.Argument.IsNotNull(url, nameof(url));

			Items.Add(GetOpenInBrowserItem<ToolStripMenuItem>(url));
			Items.Add(GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyLinkLocation, url));
		}

		private static T GetOpenInBrowserItem<T>(string url, bool enableToolTip = false)
			where T : ToolStripItem, new()
		{
			var item = new T()
			{
				Text  = Resources.StrOpenInBrowser,
				Tag   = url,
			};
			if(enableToolTip) item.ToolTipText = url;
			item.Click += OnOpenInBrowserClick;
			return item;
		}

		private static T GetCopyToClipboardItem<T>(string name, string text, bool enableToolTip = false)
			where T : ToolStripItem, new()
		{
			var item = new T()
			{
				Text  = name,
				Image = CachedResources.Bitmaps["ImgCopyToClipboard"],
				Tag   = text,
			};
			if(enableToolTip && name != text) item.ToolTipText = text;
			item.Click += OnCopyToClipboardClick;
			return item;
		}

		private static void OnCopyToClipboardClick(object sender, EventArgs e)
		{
			var text = (string)((ToolStripItem)sender).Tag;
			ClipboardEx.SetTextSafe(text);
		}

		private static void OnOpenInBrowserClick(object sender, EventArgs e)
		{
			var url = (string)((ToolStripItem)sender).Tag;
			Utility.OpenUrl(url);
		}
	}
}
