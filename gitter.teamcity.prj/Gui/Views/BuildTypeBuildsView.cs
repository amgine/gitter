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

namespace gitter.TeamCity.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.TeamCity.Properties.Resources;

	sealed class BuildTypeBuildsView : TeamCityViewBase
	{
		private BuildType _buildType;
		private CustomListBox _lstBuilds;

		public BuildTypeBuildsView(IWorkingEnvironment environment)
			: base(Guids.BuildTypeBuildsViewGuid, environment)
		{
			_lstBuilds = new CustomListBox();
			_lstBuilds.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_lstBuilds.Columns.AddRange(new CustomListBoxColumn[]
				{
					new CustomListBoxColumn((int)ColumnId.Id, Resources.StrId) { Width = 50 },
					new CustomListBoxColumn((int)ColumnId.Number, Resources.StrNumber) { Width = 150 },
					new DateColumn((int)ColumnId.StartDate, Resources.StrStartDate, true) { Width = 150 },
					new CustomListBoxColumn((int)ColumnId.Status, Resources.StrStatus) { Width = 100 },
				});
			_lstBuilds.Bounds = this.ClientRectangle;
			_lstBuilds.Anchor = System.Windows.Forms.AnchorStyles.Left |
								System.Windows.Forms.AnchorStyles.Top |
								System.Windows.Forms.AnchorStyles.Right |
								System.Windows.Forms.AnchorStyles.Bottom;
			_lstBuilds.Parent = this;
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgBuildType"]; }
		}

		protected override void AttachViewModel(object viewModel)
		{
			base.AttachViewModel(viewModel);

			var vm = viewModel as BuildTypeBuildsViewModel;
			if(vm != null)
			{
				_buildType = vm.BuildType;
				if(_buildType != null)
				{
					Text = _buildType.Name;
					if(ServiceContext != null)
					{
						RefreshContent();
					}
				}
			}
		}

		protected override void DetachViewModel(object viewModel)
		{
			base.DetachViewModel(viewModel);

			var vm = viewModel as BuildTypeBuildsViewModel;
			if(vm != null)
			{
				_buildType = null;
				Text = string.Empty;
			}
		}

		protected override void OnContextAttached()
		{
			base.OnContextAttached();
			RefreshContent();
		}

		protected override void OnContextDetached()
		{
			base.OnContextDetached();
			RefreshContent();
		}

		public override void RefreshContent()
		{
			base.RefreshContent();

			if(ServiceContext != null && _buildType != null)
			{
				_lstBuilds.BeginUpdate();
				_lstBuilds.Items.Clear();
				_buildType.Builds.Refresh();
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
}
