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
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.TeamCity.Properties.Resources;

	sealed class RepositoryExplorerRootListItem : RepositoryExplorerItemBase
	{
		private Project _project;

		public RepositoryExplorerRootListItem(IWorkingEnvironment env, TeamCityGuiProvider guiProvider)
			: base(env, guiProvider, CachedResources.Bitmaps["ImgTeamCity"], Resources.StrTeamCity)
		{
			Expand();
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new TeamCityMenu(WorkingEnvironment, GuiProvider);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		private void OnProjectBuildTypesUpdated(IAsyncResult ar)
		{
			var action = (Action)ar.AsyncState;
			try
			{
				action.EndInvoke(ar);
			}
			catch
			{
				return;
			}
			var listBox = ListBox;
			if(listBox != null)
			{
				listBox.BeginInvoke(new MethodInvoker(AddBuildTypes));
			}
		}

		private void AddBuildTypes()
		{
			Items.Clear();
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

		private void OnBuildTypeItemActivated(object sender, EventArgs e)
		{
			var item = (BuildTypeListItem)sender;
			var buildType = item.DataContext;

			var view = WorkingEnvironment.ViewDockService.ShowView(
				Views.Guids.BuildTypeBuildsViewGuid,
				new Views.BuildTypeBuildsViewModel(buildType),
				true) as TeamCityViewBase;
			if(view != null)
			{
				view.ServiceContext = ServiceContext;
			}
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();

			Items.Clear();
			_project = ServiceContext.Projects.Lookup(ServiceContext.DefaultProjectId);
			var action = new Action(_project.BuildTypes.Refresh);
			action.BeginInvoke(OnProjectBuildTypesUpdated, action);
		}

		protected override void OnListBoxDetached()
		{
			base.OnListBoxDetached();

			Items.Clear();
		}
	}
}
