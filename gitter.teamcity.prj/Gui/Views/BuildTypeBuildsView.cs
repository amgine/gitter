#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.TeamCity.Gui.Views;

using gitter.Framework;
using gitter.Framework.Controls;

using Resources = gitter.TeamCity.Properties.Resources;

sealed class BuildTypeBuildsView : TeamCityViewBase
{
	private BuildType? _buildType;
	private readonly CustomListBox _lstBuilds;

	public BuildTypeBuildsView(IWorkingEnvironment environment)
		: base(Guids.BuildTypeBuildsViewGuid, environment)
	{
		_lstBuilds = new CustomListBox();
		_lstBuilds.BorderStyle = System.Windows.Forms.BorderStyle.None;
		_lstBuilds.Columns.AddRange(
			[
				new CustomListBoxColumn((int)ColumnId.Id, Resources.StrId) { Width = 50 },
				new CustomListBoxColumn((int)ColumnId.Number, Resources.StrNumber) { Width = 150 },
				new CustomListBoxColumn((int)ColumnId.BranchName, Resources.StrBranchName) { Width = 150 },
				new DateColumn((int)ColumnId.StartDate, Resources.StrStartDate, true) { Width = 150 },
				new CustomListBoxColumn((int)ColumnId.Status, Resources.StrStatus) { Width = 100 },
			]);
		_lstBuilds.Bounds = ClientRectangle;
		_lstBuilds.Anchor = System.Windows.Forms.AnchorStyles.Left |
							System.Windows.Forms.AnchorStyles.Top |
							System.Windows.Forms.AnchorStyles.Right |
							System.Windows.Forms.AnchorStyles.Bottom;
		_lstBuilds.Parent = this;
	}

	public override IImageProvider ImageProvider { get; } = new ScaledImageProvider(CachedResources.ScaledBitmaps, @"builds");

	protected override void AttachViewModel(object viewModel)
	{
		base.AttachViewModel(viewModel);

		if(viewModel is BuildTypeBuildsViewModel vm)
		{
			_buildType = vm.BuildType;
			if(_buildType is not null)
			{
				Text = _buildType.Name;
				if(ServiceContext is not null)
				{
					RefreshContent();
				}
			}
		}
	}

	protected override void DetachViewModel(object viewModel)
	{
		base.DetachViewModel(viewModel);

		if(viewModel is BuildTypeBuildsViewModel)
		{
			_buildType = null;
			Text = string.Empty;
		}
	}

	protected override void OnContextAttached(TeamCityServiceContext context)
	{
		base.OnContextAttached(context);
		RefreshContent();
	}

	protected override void OnContextDetached(TeamCityServiceContext context)
	{
		base.OnContextDetached(context);
		RefreshContent();
	}

	public override async void RefreshContent()
	{
		base.RefreshContent();

		if(ServiceContext is not null && _buildType is not null)
		{
			_lstBuilds.BeginUpdate();
			_lstBuilds.Items.Clear();
			await _buildType.Builds.RefreshAsync();
			lock(_buildType.Builds.SyncRoot)
			{
				foreach(var build in _buildType.Builds)
				{
					_lstBuilds.Items.Add(new BuildListItem(build));
				}
			}
			_lstBuilds.EndUpdate();
		}
		else
		{
			_lstBuilds.Items.Clear();
		}
	}
}
