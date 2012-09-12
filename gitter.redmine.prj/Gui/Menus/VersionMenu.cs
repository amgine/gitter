namespace gitter.Redmine.Gui
{
	using System;
	using System.Globalization;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Redmine.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class VersionMenu : ContextMenuStrip
	{
		private readonly ProjectVersion _version;

		public VersionMenu(ProjectVersion version)
		{
			Verify.Argument.IsNotNull(version, "version");

			_version = version;

			Items.Add(GuiItemFactory.GetUpdateRedmineObjectItem<ToolStripMenuItem>(_version));

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrId, _version.Id.ToString(CultureInfo.InvariantCulture)));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, _version.Name));
			if(!string.IsNullOrWhiteSpace(_version.Description))
			{
				item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrDescription, _version.Description));
			}

			Items.Add(item);
		}

		public ProjectVersion Version
		{
			get { return _version; }
		}
	}
}
