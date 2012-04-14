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
		private readonly ReferencesView _view;

		private readonly ToolStripButton _btnRefresh;
		private readonly ToolStripButton _btnCreateBranch;
		private readonly ToolStripButton _btnCreateTag;

		public ReferencesToolbar(ReferencesView view)
		{
			if(view == null) throw new ArgumentNullException("view");
			_view = view;

			Items.Add(_btnRefresh = new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"],
				(sender, e) =>
				{
					_view.RefreshContent();
				})
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});
			Items.Add(new ToolStripSeparator());
			Items.Add(_btnCreateBranch = new ToolStripButton(Resources.StrCreateBranch, CachedResources.Bitmaps["ImgBranchAdd"],
				(sender, e) => _view.Gui.StartCreateBranchDialog()));
			Items.Add(_btnCreateTag = new ToolStripButton(Resources.StrCreateTag, CachedResources.Bitmaps["ImgTagAdd"],
				(sender, e) => _view.Gui.StartCreateTagDialog()));
		}
	}
}
