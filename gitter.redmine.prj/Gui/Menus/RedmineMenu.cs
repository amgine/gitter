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
			Verify.Argument.IsNotNull(environment, "environment");
			Verify.Argument.IsNotNull(guiProvider, "guiProvider");

			_workingEnvironment = environment;
			_guiProvider = guiProvider;

			Items.Add(new ToolStripMenuItem("Setup...", null, OnSetupClick));
		}

		private void OnSetupClick(object sender, EventArgs e)
		{
			using(var dlg = new ProviderSetupControl(_guiProvider.Repository))
			{
				dlg.Run(GitterApplication.MainForm);
			}
		}
	}
}
