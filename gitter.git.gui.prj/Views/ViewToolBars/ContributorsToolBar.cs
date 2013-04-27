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

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class ContributorsToolBar : ToolStrip
	{
		#region Data

		private readonly ContributorsView _view;
		private ToolStripButton _btnRefresh;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="ContributorsToolBar"/> class.</summary>
		/// <param name="view">Host contributors view.</param>
		public ContributorsToolBar(ContributorsView view)
		{
			_view = view;

			Items.AddRange(
				new ToolStripItem[]
				{
					// left-aligned
					_btnRefresh = new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"], OnRefreshButtonClick)
						{
							DisplayStyle = ToolStripItemDisplayStyle.Image,
						},
				});
		}

		private void OnRefreshButtonClick(object sender, EventArgs e)
		{
			_view.RefreshContent();
		}

		public ToolStripButton RefreshButton
		{
			get { return _btnRefresh; }
		}
	}
}
