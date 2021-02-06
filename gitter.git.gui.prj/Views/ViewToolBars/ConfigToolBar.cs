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

namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class ConfigToolBar : ToolStrip
	{
		private readonly ConfigView _configView;

		public ConfigToolBar(ConfigView configView)
		{
			Verify.Argument.IsNotNull(configView, nameof(configView));

			_configView = configView;
			Items.Add(
				new ToolStripButton(
					Resources.StrRefresh,
					CachedResources.Bitmaps["ImgRefresh"],
					OnRefreshButtonClick)
					{
						DisplayStyle = ToolStripItemDisplayStyle.Image,
					});

			Items.Add(new ToolStripSeparator());

			Items.Add(
				new ToolStripButton(
					Resources.StrAddParameter,
					CachedResources.Bitmaps["ImgConfigAdd"],
					OnAddParameterButtonClick));
		}

		private void OnRefreshButtonClick(object sender, EventArgs e)
		{
			_configView.RefreshContent();
		}

		private void OnAddParameterButtonClick(object sender, EventArgs e)
		{
			using var dlg = new AddParameterDialog(_configView.WorkingEnvironment, _configView.Repository);
			dlg.Run(_configView);
		}
	}
}
