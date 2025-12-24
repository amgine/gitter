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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Services;

using Resources = gitter.TeamCity.Properties.Resources;

sealed class RepositoryExplorerRootListItem : RepositoryExplorerItemBase
{
	static readonly LoggingService Log = new("TeamCity");

	private Project? _project;

	public RepositoryExplorerRootListItem(IWorkingEnvironment env, TeamCityGuiProvider guiProvider)
		: base(env, guiProvider, Icons.TeamCity, Resources.StrTeamCity)
	{
		Expand();
	}

	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		var menu = new TeamCityMenu(WorkingEnvironment, GuiProvider);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}

	private void AddBuildTypes()
	{
		Items.Clear();
		if(_project is null) return;
		lock(_project.BuildTypes.SyncRoot)
		{
			foreach(var buildType in _project.BuildTypes)
			{
				var item = new BuildTypeListItem(buildType);
				Items.Add(item);
				item.Activated += OnBuildTypeItemActivated;
			}
		}
	}

	private void OnBuildTypeItemActivated(object? sender, EventArgs e)
	{
		if(sender is not BuildTypeListItem item) return;

		var buildType = item.DataContext;

		var view = WorkingEnvironment.ViewDockService.ShowView(
			Views.Guids.BuildTypeBuildsViewGuid,
			new Views.BuildTypeBuildsViewModel(buildType),
			true) as TeamCityViewBase;
		if(view is not null)
		{
			view.ServiceContext = ServiceContext;
		}
	}

	protected override void OnListBoxAttached(CustomListBox listBox)
	{
		base.OnListBoxAttached(listBox);

		Items.Clear();
		_project = ServiceContext.Projects.Lookup(ServiceContext.DefaultProjectId);
		Add();
	}

	private async void Add()
	{
		if(_project is not null)
		{
			try
			{
				await _project.BuildTypes.RefreshAsync();
			}
			catch(Exception exc) when(!exc.IsCritical)
			{
				Log.Error(exc, "Failed to fetch TeamCity build types.");
			}
		}
		var listBox = ListBox;
		if(listBox is not null)
		{
			AddBuildTypes();
		}
	}

	protected override void OnListBoxDetached(CustomListBox listBox)
	{
		base.OnListBoxDetached(listBox);

		Items.Clear();
	}
}
