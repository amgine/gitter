namespace gitter.Redmine.Gui
{
	using System;
	using System.Globalization;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Redmine.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class IssueMenu : ContextMenuStrip
	{
		private readonly Issue _issue;

		public IssueMenu(Issue issue)
		{
			Verify.Argument.IsNotNull(issue, "issue");

			_issue = issue;

			Items.Add(GuiItemFactory.GetUpdateRedmineObjectItem<ToolStripMenuItem>(_issue));

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrId, _issue.Id.ToString(CultureInfo.InvariantCulture)));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSubject, _issue.Subject));
			if(_issue.Category != null)
			{
				item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCategory, _issue.Category.Name));
			}

			Items.Add(item);
		}

		public Issue Issue
		{
			get { return _issue; }
		}
	}
}
