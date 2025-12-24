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
using System.Linq;

using gitter.Framework;
using gitter.Framework.Configuration;
using gitter.Framework.Controls;

using gitter.Git;
using gitter.Git.Gui.Controls;

using gitter.TeamCity.Gui.Views;

sealed class TeamCityGuiProvider(IRepository repository, NotifyCollection<ServerInfo> servers, TeamCityServiceContext context) : IGuiProvider
{
	private RepositoryExplorer? _repositoryExplorer;

	public IRepository Repository { get; } = repository;

	public NotifyCollection<ServerInfo> Servers { get; } = servers;

	public TeamCityServiceContext ServiceContext { get; } = context;

	public void AttachToEnvironment(IWorkingEnvironment environment)
	{
		Verify.Argument.IsNotNull(environment);

		_repositoryExplorer = new RepositoryExplorer(environment, this);
		environment.ProvideRepositoryExplorerItem(_repositoryExplorer.RootItem);
		DiffHeaderPanelsProvider.CreatingPanels += OnDiffHeaderPanelsProviderCreatingPanels;
	}

	public void DetachFromEnvironment(IWorkingEnvironment environment)
	{
		Verify.Argument.IsNotNull(environment);

		var views = environment.ViewDockService.FindViews(Guids.BuildTypeBuildsViewGuid).ToList();
		foreach(var view in views) view.Close();
		if(_repositoryExplorer is not null)
		{
			environment.RemoveRepositoryExplorerItem(_repositoryExplorer.RootItem);
			_repositoryExplorer = null;
		}
		DiffHeaderPanelsProvider.CreatingPanels -= OnDiffHeaderPanelsProviderCreatingPanels;
	}

	private void OnDiffHeaderPanelsProviderCreatingPanels(object? sender, CreatingPanelsEventArgs e)
	{
		Assert.IsNotNull(e);

		switch(e.DiffSource)
		{
			case IRevisionDiffSource revisionSource:
				{
					var revision = revisionSource.Revision.Dereference();
					if(revision is not null)
					{
						e.Panels.Add(new TeamCityRevisionPanel(ServiceContext, revision) { });
						e.Panels.Add(new FlowPanelSeparator { SeparatorStyle = FlowPanelSeparatorStyle.Line });
					}
				}
				break;
		}
	}

	public void SaveTo(Section section)
	{
	}

	public void LoadFrom(Section section)
	{
	}
}
