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

namespace gitter.TeamCity.Gui;

using System;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.TeamCity.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
sealed class TeamCityMenu : gitter.Framework.Controls.HyperlinkContextMenu
{
	private readonly IWorkingEnvironment _workingEnvironment;
	private readonly TeamCityGuiProvider _guiProvider;

	static string FormatUrl(TeamCityGuiProvider guiProvider)
	{
		var uri = guiProvider.ServiceContext.ServiceUri;
		var id  = guiProvider.ServiceContext.DefaultProjectId;
		return uri.EndsWith('/')
			? uri +  "project/" + id
			: uri + "/project/" + id;
	}

	public TeamCityMenu(IWorkingEnvironment environment, TeamCityGuiProvider guiProvider)
		: base(FormatUrl(guiProvider))
	{
		Verify.Argument.IsNotNull(environment);
		Verify.Argument.IsNotNull(guiProvider);

		Renderer = GitterApplication.Style.ToolStripRenderer;

		_workingEnvironment = environment;
		_guiProvider = guiProvider;

		Items.Add(new ToolStripSeparator());
		Items.Add(new ToolStripMenuItem("Setup...", null, OnSetupClick));
	}

	private void OnSetupClick(object? sender, EventArgs e)
	{
		using var dialog = new ProviderSetupDialog(_guiProvider.Repository, _guiProvider.Servers);
		dialog.Run(GitterApplication.MainForm);
	}
}
