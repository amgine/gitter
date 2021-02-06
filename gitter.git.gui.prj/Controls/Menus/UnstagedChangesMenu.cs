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

	/// <summary>Context menu for unstaged changes item.</summary>
	[ToolboxItem(false)]
	public sealed class UnstagedChangesMenu : ContextMenuStrip
	{
		public UnstagedChangesMenu(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));

			Repository = repository;

			Items.Add(GuiItemFactory.GetStashSaveKeepIndexItem<ToolStripMenuItem>(repository));

			if(repository.Status.UnmergedCount != 0)
			{
				Items.Add(new ToolStripSeparator());

				Items.Add(GuiItemFactory.GetResolveConflictsItem<ToolStripMenuItem>(repository));
			}
			
			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetStageAllItem<ToolStripMenuItem>(repository, Resources.StrStageAll));
			Items.Add(GuiItemFactory.GetUpdateItem<ToolStripMenuItem>(repository, Resources.StrUpdate));
			Items.Add(GuiItemFactory.GetManualStageItem<ToolStripMenuItem>(repository, Resources.StrManualStage.AddEllipsis()));
			
			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetCleanItem<ToolStripMenuItem>(repository));
			Items.Add(GuiItemFactory.GetResetItem<ToolStripMenuItem>(repository, ResetMode.Mixed | ResetMode.Hard));
		}

		public Repository Repository { get; }
	}
}
