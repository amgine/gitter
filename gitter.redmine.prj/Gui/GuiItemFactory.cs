namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Redmine.Properties.Resources;

	static class GuiItemFactory
	{
		public static T GetCopyToClipboardItem<T>(string name, Func<string> text)
			where T : ToolStripItem, new()
		{
			var item = new T()
			{
				Text = name,
				Image = CachedResources.Bitmaps["ImgCopyToClipboard"],
				Tag = text,
			};
			item.Click += OnCopyToClipboardClick;
			return item;
		}

		public static T GetCopyToClipboardItem<T>(string name, string text)
			where T : ToolStripItem, new()
		{
			return GetCopyToClipboardItem<T>(name, text, true);
		}

		public static T GetCopyToClipboardItem<T>(string name, string text, bool enableToolTip)
			where T : ToolStripItem, new()
		{
			var item = new T()
			{
				Text = name,
				Image = CachedResources.Bitmaps["ImgCopyToClipboard"],
				Tag = text,
			};
			if(enableToolTip && name != text) item.ToolTipText = text;
			item.Click += OnCopyToClipboardClick;
			return item;
		}

		public static T GetUpdateRedmineObjectItem<T>(RedmineObject obj)
			where T : ToolStripItem, new()
		{
			Verify.Argument.IsNotNull(obj, "obj");

			var item = new T()
			{
				Text = Resources.StrRefresh,
				Image = CachedResources.Bitmaps["ImgRefresh"],
				Tag = obj,
			};
			item.Click += OnUpdateRedmineObjectClick;
			return item;
		}

		private static void OnCopyToClipboardClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var text = item.Tag as string;
			if(text == null)
			{
				text = ((Func<string>)item.Tag)();
			}
			ClipboardEx.SetTextSafe(text);
		}

		private static void OnUpdateRedmineObjectClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var obj = (RedmineObject)item.Tag;
			try
			{
				obj.Update();
			}
			catch
			{
			}
		}
	}
}
