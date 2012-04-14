namespace gitter.Redmine.Gui
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Redmine.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class NewsToolbar : ToolStrip
	{
		#region Data

		private readonly NewsView _view;
		private ToolStripButton _btnRefresh;

		#endregion

		public NewsToolbar(NewsView view)
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
