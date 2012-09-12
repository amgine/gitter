namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class BranchDragDropMenu : ContextMenuStrip
	{
		private readonly BranchBase _branch;

		public BranchDragDropMenu(Branch branch)
		{
			Verify.Argument.IsValidGitObject(branch, "branch");

			_branch = branch;

			Items.Add(new ToolStripMenuItem("TEST"));
		}
	}
}
