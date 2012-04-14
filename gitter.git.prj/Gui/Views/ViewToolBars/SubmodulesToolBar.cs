namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Toolbar for <see cref="SubmodulesView"/>.</summary>
	[ToolboxItem(false)]
	internal sealed class SubmodulesToolbar : ToolStrip
	{
		#region Data

		private readonly SubmodulesView _tool;
		private readonly ToolStripButton _btnAddSubmodule;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="SubmodulesToolbar"/> class.</summary>
		/// <param name="tool">Host tool.</param>
		public SubmodulesToolbar(SubmodulesView tool)
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
			Items.Add(_btnAddSubmodule = new ToolStripButton(Resources.StrAddSubmodule, CachedResources.Bitmaps["ImgSubmoduleAdd"],
				(sender, e) =>
				{
					using(var dlg = new AddSubmoduleDialog(_tool.Repository))
					{
						dlg.Run(_tool);
					}
				})
				{
					DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
				});
		}
	}
}
