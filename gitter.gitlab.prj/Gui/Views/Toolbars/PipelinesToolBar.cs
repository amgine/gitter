﻿#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Gui
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.GitLab.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class PipelinesToolbar : ToolStrip
	{
		private readonly PipelinesView _view;
		private readonly ToolStripButton _btnRefresh;

		public PipelinesToolbar(PipelinesView view)
		{
			Verify.Argument.IsNotNull(view, nameof(view));

			_view = view;

			Items.Add(_btnRefresh = new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"],
				(sender, e) =>
				{
					_view.RefreshContent();
				})
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});
			Items.Add(new ToolStripSeparator());
		}

		public ToolStripButton RefreshButton => _btnRefresh;
	}
}
