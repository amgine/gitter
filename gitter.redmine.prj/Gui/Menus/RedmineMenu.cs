namespace gitter.Redmine.Gui
{
	using System;
	using System.Globalization;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Redmine.Properties.Resources;

	[ToolboxItem(false)]
	sealed class RedmineMenu : ContextMenuStrip
	{
		private readonly IWorkingEnvironment _workingEnvironment;
		private readonly RedmineGuiProvider _guiProvider;

		public RedmineMenu(IWorkingEnvironment environment, RedmineGuiProvider guiProvider)
		{
			if(environment == null) throw new ArgumentNullException("environment");
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");

			_workingEnvironment = environment;
			_guiProvider = guiProvider;

			Items.Add(new ToolStripMenuItem("Setup...", null,
				(s, e) =>
				{
					using(var dlg = new ProviderSetupControl(_guiProvider.Repository))
					{
						dlg.Run(GitterApplication.MainForm);
					}
				}));
		}
	}
}
