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
	using System.Collections.Generic;
	using System.Text;
	using System.IO;

	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public sealed class SubmoduleMenu : ContextMenuStrip
	{
		private readonly Submodule _submodule;

		public SubmoduleMenu(Submodule submodule)
		{
			Verify.Argument.IsValidGitObject(submodule, "submodule");

			_submodule = submodule;

			Items.Add(GuiItemFactory.GetOpenAppItem<ToolStripMenuItem>(
				Resources.StrOpenWithGitter, null, Application.ExecutablePath, _submodule.FullPath.SurroundWithDoubleQuotes()));
			Items.Add(GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(
				Resources.StrOpenInWindowsExplorer, null, _submodule.FullPath));
			Items.Add(GuiItemFactory.GetOpenCmdAtItem<ToolStripMenuItem>(
				Resources.StrOpenCommandLine, null, _submodule.FullPath));

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetUpdateSubmoduleItem<ToolStripMenuItem>(submodule));

			Items.Add(new ToolStripSeparator()); // copy to clipboard section

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, _submodule.Name));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrPath, _submodule.Path));
			item.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrUrl, _submodule.Url));

			Items.Add(item);
		}

		public Submodule Submodule
		{
			get { return _submodule; }
		}
	}
}
