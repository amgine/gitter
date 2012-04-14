namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Toolbar for <see cref="StashView"/>.</summary>
	[ToolboxItem(false)]
	internal sealed class StashToolbar : ToolStrip
	{
		#region Data

		private readonly StashView _stashTool;

		private readonly ToolStripButton _save;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="StashToolbar"/> class.</summary>
		/// <param name="stashTool">Host tool.</param>
		public StashToolbar(StashView stashTool)
		{
			if(stashTool == null) throw new ArgumentNullException("stashTool");
			_stashTool = stashTool;

			Items.Add(new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"],
				(sender, e) =>
				{
					_stashTool.RefreshContent();
				})
				{
					DisplayStyle = ToolStripItemDisplayStyle.Image,
				});
			Items.Add(new ToolStripSeparator());
			Items.Add(_save = new ToolStripButton(Resources.StrSave, CachedResources.Bitmaps["ImgStashSave"],
				(sender, e) =>
				{
					_stashTool.Gui.StartStashSaveDialog();
				})
			{
				ToolTipText = Resources.TipStashSave,
			});
			//Items.Add(_clear = new ToolStripButton(Resources.StrClear, CachedResources.Bitmaps["ImgStashClear"],
			//    (sender, e) =>
			//    {
			//        _stashTool.Gui.StartStashSaveDialog();
			//    })
			//{
			//});
		}
	}
}
