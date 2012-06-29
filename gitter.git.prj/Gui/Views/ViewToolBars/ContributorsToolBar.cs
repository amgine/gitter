namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class ContributorsToolBar : ToolStrip
	{
		#region Data

		private readonly ContributorsView _view;
		private ToolStripButton _btnRefresh;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="ContributorsToolBar"/> class.</summary>
		/// <param name="view">Host contributors view.</param>
		public ContributorsToolBar(ContributorsView view)
		{
			_view = view;

			Items.AddRange(
				new ToolStripItem[]
				{
					// left-aligned
					_btnRefresh = new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"], OnRefreshButtonClick)
						{
							DisplayStyle = ToolStripItemDisplayStyle.Image,
						},
				});
		}

		private void OnRefreshButtonClick(object sender, EventArgs e)
		{
			_view.RefreshContent();
		}

		public ToolStripButton RefreshButton
		{
			get { return _btnRefresh; }
		}
	}
}
