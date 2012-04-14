namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class ConfigToolBar : ToolStrip
	{
		private readonly ConfigView _configTool;

		public ConfigToolBar(ConfigView configTool)
		{
			if(configTool == null) throw new ArgumentNullException("configTool");
			_configTool = configTool;

			Items.Add(new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"],
				(sender, e) =>
				{
					_configTool.RefreshContent();
				})
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});

			Items.Add(new ToolStripSeparator());

			Items.Add(new ToolStripButton(Resources.StrAddParameter, CachedResources.Bitmaps["ImgConfigAdd"],
				(sender, e) =>
				{
					using(var dlg = new AddParameterDialog(_configTool.Repository))
					{
						dlg.Run(_configTool);
					}
				}));
		}
	}
}
