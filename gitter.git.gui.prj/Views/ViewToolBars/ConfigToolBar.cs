namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class ConfigToolBar : ToolStrip
	{
		private readonly ConfigView _configView;

		public ConfigToolBar(ConfigView configView)
		{
			if(configView == null) throw new ArgumentNullException("configView");

			_configView = configView;

			Items.Add(
				new ToolStripButton(
					Resources.StrRefresh,
					CachedResources.Bitmaps["ImgRefresh"],
					OnRefreshButtonClick)
					{
						DisplayStyle = ToolStripItemDisplayStyle.Image,
					});

			Items.Add(new ToolStripSeparator());

			Items.Add(
				new ToolStripButton(
					Resources.StrAddParameter,
					CachedResources.Bitmaps["ImgConfigAdd"],
					OnAddParameterButtonClick));
		}

		private void OnRefreshButtonClick(object sender, EventArgs e)
		{
			_configView.RefreshContent();
		}

		private void OnAddParameterButtonClick(object sender, EventArgs e)
		{
			using(var dlg = new AddParameterDialog(_configView.WorkingEnvironment, _configView.Repository))
			{
				dlg.Run(_configView);
			}
		}
	}
}
