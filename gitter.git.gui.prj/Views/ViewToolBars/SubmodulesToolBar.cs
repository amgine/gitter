namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Toolbar for <see cref="SubmodulesView"/>.</summary>
	[ToolboxItem(false)]
	internal sealed class SubmodulesToolbar : ToolStrip
	{
		#region Data

		private readonly SubmodulesView _submodulesView;
		private readonly ToolStripButton _btnAddSubmodule;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="SubmodulesToolbar"/> class.</summary>
		/// <param name="submodulesView">Host view.</param>
		public SubmodulesToolbar(SubmodulesView submodulesView)
		{
			Verify.Argument.IsNotNull(submodulesView, "submodulesView");

			_submodulesView = submodulesView;

			Items.Add(new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"],
				(sender, e) =>
				{
					_submodulesView.RefreshContent();
				})
				{
					DisplayStyle = ToolStripItemDisplayStyle.Image,
				});
			Items.Add(new ToolStripSeparator());
			Items.Add(_btnAddSubmodule = new ToolStripButton(Resources.StrAddSubmodule, CachedResources.Bitmaps["ImgSubmoduleAdd"],
				(sender, e) =>
				{
					using(var dlg = new AddSubmoduleDialog(_submodulesView.Repository))
					{
						dlg.Run(_submodulesView);
					}
				})
				{
					DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
				});
		}
	}
}
