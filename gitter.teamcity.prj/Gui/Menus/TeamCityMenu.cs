namespace gitter.TeamCity.Gui
{
	using System;
	using System.Globalization;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.TeamCity.Properties.Resources;

	[ToolboxItem(false)]
	sealed class TeamCityMenu : ContextMenuStrip
	{
		private readonly IWorkingEnvironment _workingEnvironment;
		private readonly TeamCityGuiProvider _guiProvider;

		public TeamCityMenu(IWorkingEnvironment environment, TeamCityGuiProvider guiProvider)
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
