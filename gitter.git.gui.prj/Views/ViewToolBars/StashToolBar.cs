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

	/// <summary>Toolbar for <see cref="StashView"/>.</summary>
	[ToolboxItem(false)]
	internal sealed class StashToolbar : ToolStrip
	{
		private readonly StashView _stashView;

		private readonly ToolStripButton _btnRefresh;

		private readonly ToolStripButton _saveButton;

		/// <summary>Initializes a new instance of the <see cref="StashToolbar"/> class.</summary>
		/// <param name="stashView">Stash view.</param>
		public StashToolbar(StashView stashView)
		{
			Verify.Argument.IsNotNull(stashView, nameof(stashView));

			_stashView = stashView;
			Items.Add(
				_btnRefresh = new ToolStripButton(
					Resources.StrRefresh,
					default,
					OnRefreshButtonClick)
					{
						DisplayStyle = ToolStripItemDisplayStyle.Image,
					});

			Items.Add(new ToolStripSeparator());

			Items.Add(_saveButton =
				new ToolStripButton(
					Resources.StrSave,
					CachedResources.Bitmaps["ImgStashSave"],
					OnStashSaveButtonClick)
					{
						ToolTipText = Resources.TipStashSave,
					});

			UpdateIcons(DeviceDpi);
		}

		private void UpdateIcons(int dpi)
		{
			var iconSize = dpi * 16 / 96;

			_btnRefresh.Image = CachedResources.ScaledBitmaps[@"refresh", iconSize];
		}

		protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
		{
			base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
			UpdateIcons(deviceDpiNew);
		}

		private void OnRefreshButtonClick(object sender, EventArgs e)
		{
			_stashView.RefreshContent();
		}

		private void OnStashSaveButtonClick(object sender, EventArgs e)
		{
			_stashView.Gui.StartStashSaveDialog();
		}
	}
}
