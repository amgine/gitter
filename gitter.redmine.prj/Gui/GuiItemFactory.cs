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

namespace gitter.Redmine.Gui;

using System;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Redmine.Properties.Resources;

class GuiItemFactory
{
	private readonly DpiBindings _dpiBindings;

	public GuiItemFactory(DpiBindings dpiBindings)
	{
		Verify.Argument.IsNotNull(dpiBindings);

		_dpiBindings = dpiBindings;
	}

	public T GetCopyToClipboardItem<T>(string name, Func<string> text)
		where T : ToolStripItem, new()
	{
		var item = new T()
		{
			Text = name,
			Tag = text,
		};
		_dpiBindings.BindImage(item, CommonIcons.ClipboardCopy);
		item.Click += OnCopyToClipboardClick;
		return item;
	}

	public T GetCopyToClipboardItem<T>(string name, string text)
		where T : ToolStripItem, new()
	{
		return GetCopyToClipboardItem<T>(name, text, true);
	}

	public T GetCopyToClipboardItem<T>(string name, string text, bool enableToolTip)
		where T : ToolStripItem, new()
	{
		var item = new T()
		{
			Text = name,
			Tag = text,
		};
		_dpiBindings.BindImage(item, CommonIcons.ClipboardCopy);
		if(enableToolTip && name != text) item.ToolTipText = text;
		item.Click += OnCopyToClipboardClick;
		return item;
	}

	public T GetUpdateRedmineObjectItem<T>(RedmineObject obj)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(obj);

		var item = new T()
		{
			Text = Resources.StrRefresh,
			Tag = obj,
		};
		_dpiBindings.BindImage(item, CommonIcons.Refresh);
		item.Click += OnUpdateRedmineObjectClick;
		return item;
	}

	private static void OnCopyToClipboardClick(object sender, EventArgs e)
	{
		Assert.IsNotNull(sender);

		var item = (ToolStripItem)sender;
		var text = item.Tag switch
		{
			string       str  => str,
			Func<string> func => func(),
			_ => default,
		};
		ClipboardEx.SetTextSafe(text);
	}

	private static void OnUpdateRedmineObjectClick(object sender, EventArgs e)
	{
		Assert.IsNotNull(sender);

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
