#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Controls;

using System;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework.Services;

using Resources = gitter.Framework.Properties.Resources;

[DesignerCategory("")]
public class HyperlinkContextMenu : ContextMenuStrip
{
	public static void AddItemsTo(DpiBindings dpiBindings, ToolStripItemCollection items, string url)
	{
		Verify.Argument.IsNotNull(items);

		items.Add(GetOpenInBrowserItem<ToolStripMenuItem>(url));
		items.Add(GetCopyToClipboardItem<ToolStripMenuItem>(dpiBindings, Resources.StrCopyLinkLocation, url));
	}

	public HyperlinkContextMenu(Hyperlink hyperlink)
	{
		Verify.Argument.IsNotNull(hyperlink);

		var dpiBindings = new DpiBindings(this);
		AddItemsTo(dpiBindings, Items, hyperlink.Url);
	}

	public HyperlinkContextMenu(string url)
	{
		Verify.Argument.IsNotNull(url);

		var dpiBindings = new DpiBindings(this);
		AddItemsTo(dpiBindings, Items, url);
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

	private static T GetCopyToClipboardItem<T>(DpiBindings dpiBindings, string name, string text, bool enableToolTip = false)
		where T : ToolStripItem, new()
	{
		var item = new T()
		{
			Text  = name,
			Tag   = text,
		};
		if(enableToolTip && name != text) item.ToolTipText = text;
		dpiBindings.BindImage(item, CommonIcons.ClipboardCopy);
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
