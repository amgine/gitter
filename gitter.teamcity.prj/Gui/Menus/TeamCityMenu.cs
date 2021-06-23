#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
			Verify.Argument.IsNotNull(environment, nameof(environment));
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_workingEnvironment = environment;
			_guiProvider = guiProvider;

			Items.Add(new ToolStripMenuItem("Setup...", null, OnSetupClick));
		}

		private void OnSetupClick(object sender, EventArgs e)
		{
			using var dlg = new ProviderSetupControl(_guiProvider.Repository);
			dlg.Run(GitterApplication.MainForm);
		}
	}
}
