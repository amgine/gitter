namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.IO;

	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	public sealed class SubmoduleMenu : ContextMenuStrip
	{
		private readonly Submodule _submodule;

		public SubmoduleMenu(Submodule submodule)
		{
			if(submodule == null) throw new ArgumentNullException("submodule");
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
