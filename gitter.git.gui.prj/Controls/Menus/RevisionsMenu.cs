namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class RevisionsMenu : ContextMenuStrip
	{
		private readonly IList<Revision> _revisions;

		/// <summary>Create <see cref="RevisionsMenu"/>.</summary>
		/// <param name="revisions">List of related revisions.</param>
		public RevisionsMenu(IList<Revision> revisions)
		{
			Verify.Argument.IsNotNull(revisions, "revisions");

			_revisions = revisions;

			//Items.Add(GuiItemFactory.GetCherryPickItem<ToolStripMenuItem>(revisions));
			//Items.Add(GuiItemFactory.GetRevertItem<ToolStripMenuItem>(revisions));

			if(revisions.Count == 2)
			{
				Items.Add(GuiItemFactory.GetCompareWithItem<ToolStripMenuItem>(revisions[0], revisions[1]));
			}
		}

		public IList<Revision> Revisions
		{
			get { return _revisions; }
		}
	}
}
