namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class RemotesToolbar : ToolStrip
	{
		private readonly RemotesView _remotesView;

		private readonly ToolStripButton _btnAddRemote;

		public RemotesToolbar(RemotesView remotesView)
		{
			Verify.Argument.IsNotNull(remotesView, "remotesView");

			_remotesView = remotesView;

			Items.Add(new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"],
				(sender, e) =>
				{
					_remotesView.RefreshContent();
				})
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});
			Items.Add(new ToolStripSeparator());
			Items.Add(_btnAddRemote = new ToolStripButton(Resources.StrAddRemote, CachedResources.Bitmaps["ImgRemoteAdd"],
				(sender, e) =>
				{
					using(var dlg = new AddRemoteDialog(_remotesView.Repository))
					{
						dlg.Run(_remotesView);
					}
				}));
		}
	}
}
