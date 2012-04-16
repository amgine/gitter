namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class ReferencesToolbar : ToolStrip
	{
		#region Data

		private readonly ReferencesView _referencesView;

		private readonly ToolStripButton _btnRefresh;
		private readonly ToolStripButton _btnCreateBranch;
		private readonly ToolStripButton _btnCreateTag;

		#endregion

		public ReferencesToolbar(ReferencesView referencesView)
		{
			if(referencesView == null) throw new ArgumentNullException("referencesView");
			_referencesView = referencesView;

			Items.Add(_btnRefresh =
				new ToolStripButton(
					Resources.StrRefresh,
					CachedResources.Bitmaps["ImgRefresh"],
					OnRefreshButtonClick)
					{
						DisplayStyle = ToolStripItemDisplayStyle.Image,
					});

			Items.Add(new ToolStripSeparator());

			Items.Add(_btnCreateBranch =
				new ToolStripButton(
					Resources.StrCreateBranch,
					CachedResources.Bitmaps["ImgBranchAdd"],
					OnCreateBranchButtonClick));

			Items.Add(_btnCreateTag =
				new ToolStripButton(
					Resources.StrCreateTag,
					CachedResources.Bitmaps["ImgTagAdd"],
					OnCreateTagButtonClick));
		}

		private void OnRefreshButtonClick(object sender, EventArgs e)
		{
			_referencesView.RefreshContent();
		}

		private void OnCreateBranchButtonClick(object sender, EventArgs e)
		{
			_referencesView.Gui.StartCreateBranchDialog();
		}

		private void OnCreateTagButtonClick(object sender, EventArgs e)
		{
			_referencesView.Gui.StartCreateTagDialog();
		}
	}
}
