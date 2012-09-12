namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Toolbar for <see cref="StashView"/>.</summary>
	[ToolboxItem(false)]
	internal sealed class StashToolbar : ToolStrip
	{
		#region Data

		private readonly StashView _stashView;

		private readonly ToolStripButton _saveButton;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="StashToolbar"/> class.</summary>
		/// <param name="stashView">Stash view.</param>
		public StashToolbar(StashView stashView)
		{
			Verify.Argument.IsNotNull(stashView, "stashView");

			_stashView = stashView;
			Items.Add(
				new ToolStripButton(
					Resources.StrRefresh,
					CachedResources.Bitmaps["ImgRefresh"],
					OnRefreshButtonClick)
					{
						DisplayStyle = ToolStripItemDisplayStyle.Image,
					});

			Items.Add(new ToolStripSeparator());

			Items.Add(_saveButton =
				new ToolStripButton(
					Resources.StrSave,
					CachedResources.Bitmaps["ImgStashSave"],
					OnStashSaveButtonClick)
					{
						ToolTipText = Resources.TipStashSave,
					});
		}

		private void OnRefreshButtonClick(object sender, EventArgs e)
		{
			_stashView.RefreshContent();
		}

		private void OnStashSaveButtonClick(object sender, EventArgs e)
		{
			_stashView.Gui.StartStashSaveDialog();
		}
	}
}
