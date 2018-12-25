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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class RemotesMenu : ContextMenuStrip
	{
		public RemotesMenu(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));

			Repository = repository;

			Items.Add(GuiItemFactory.GetShowRemotesViewItem<ToolStripMenuItem>());
			Items.Add(GuiItemFactory.GetRefreshRemotesItem<ToolStripMenuItem>(repository));
			Items.Add(new ToolStripSeparator());
			Items.Add(GuiItemFactory.GetFetchItem<ToolStripMenuItem>(repository));
			Items.Add(GuiItemFactory.GetPullItem<ToolStripMenuItem>(repository));
			Items.Add(new ToolStripSeparator());
			Items.Add(GuiItemFactory.GetAddRemoteItem<ToolStripMenuItem>(repository));
		}

		public Repository Repository { get; }
	}
}
