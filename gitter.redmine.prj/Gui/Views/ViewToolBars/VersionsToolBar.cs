namespace gitter.Redmine.Gui
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Redmine.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class VersionsToolbar : ToolStrip
	{
		#region Data

		private readonly VersionsView _view;
		private ToolStripButton _btnRefresh;

		#endregion

		public VersionsToolbar(VersionsView view)
		{
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
		}

		public ToolStripButton RefreshButton
		{
			get { return _btnRefresh; }
		}
	}
}
