namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class ConfigParameterMenu : ContextMenuStrip
	{
		private readonly ConfigParameterListItem _listItem;
		private readonly ConfigParameter _parameter;

		public ConfigParameterMenu(ConfigParameterListItem listItem)
		{
			Verify.Argument.IsNotNull(listItem, "listItem");
			Verify.Argument.IsValidGitObject(listItem.DataContext, "parameter");

			_listItem = listItem;
			_parameter = listItem.DataContext;

			Items.Add(new ToolStripMenuItem(Resources.StrEditValue, CachedResources.Bitmaps["ImgConfigEdit"], (s, e) => listItem.StartValueEditor()));
			Items.Add(GuiItemFactory.GetUnsetParameterItem<ToolStripMenuItem>(_parameter));
			Items.Add(new ToolStripSeparator());
			Items.Add(new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
				GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, _parameter.Name),
				GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrValue, _parameter.Value)));
		}

		public ConfigParameterMenu(ConfigParameter parameter)
		{
			Verify.Argument.IsValidGitObject(parameter, "parameter");

			_parameter = parameter;
			Items.Add(GuiItemFactory.GetUnsetParameterItem<ToolStripMenuItem>(parameter));
			Items.Add(new ToolStripSeparator());
			Items.Add(new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
				GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, _parameter.Name),
				GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrValue, _parameter.Value)));
		}

		public ConfigParameter ConfigParameter
		{
			get { return _parameter; }
		}
	}
}
