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

	/// <summary>Context menu for staged changes item.</summary>
	[ToolboxItem(false)]
	public sealed class StagedChangesMenu : ContextMenuStrip
	{
		public StagedChangesMenu(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));

			Repository = repository;

			Items.Add(GuiItemFactory.GetCommitItem<ToolStripMenuItem>(repository));
			Items.Add(GuiItemFactory.GetStashSaveItem<ToolStripMenuItem>(repository));

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetUnstageAllItem<ToolStripMenuItem>(repository));
			Items.Add(GuiItemFactory.GetResetItem<ToolStripMenuItem>(repository, ResetMode.Mixed | ResetMode.Hard));
		}

		public Repository Repository { get; }
	}
}
