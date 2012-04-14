namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class BranchDragDropMenu : ContextMenuStrip
	{
		private readonly BranchBase _branch;

		public BranchDragDropMenu(Branch branch)
		{
			if(branch == null) throw new ArgumentNullException("branch");

			_branch = branch;

			Items.Add(new ToolStripMenuItem("TEST"));
		}
	}
}
