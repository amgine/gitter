namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class RemotesToolbar : ToolStrip
	{
		private readonly RemotesView _tool;

		private readonly ToolStripButton _btnAddRemote;

		public RemotesToolbar(RemotesView tool)
		{
			if(tool == null) throw new ArgumentNullException("tool");
			_tool = tool;

			Items.Add(new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"],
				(sender, e) =>
				{
					_tool.RefreshContent();
				})
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});
			Items.Add(new ToolStripSeparator());
			Items.Add(_btnAddRemote = new ToolStripButton(Resources.StrAddRemote, CachedResources.Bitmaps["ImgRemoteAdd"],
				(sender, e) =>
				{
					using(var dlg = new AddRemoteDialog(_tool.Repository))
					{
						dlg.Run(_tool);
					}
				}));
		}
	}
}
