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

namespace gitter
{
	using System;
	using System.Windows.Forms;

	using Resources = gitter.Properties.Resources;

	sealed class RecentRepositoryMenu : ContextMenuStrip
	{
		private readonly RecentRepositoryListItem _repository;

		public RecentRepositoryMenu(RecentRepositoryListItem repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			Items.Add(new ToolStripMenuItem(Resources.StrOpen, null, (s, e) => _repository.Activate()));
			Items.Add(GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenInWindowsExplorer, null, _repository.DataContext.Path));
			Items.Add(GuiItemFactory.GetOpenCmdAtItem<ToolStripMenuItem>(Resources.StrOpenCommandLine, null, _repository.DataContext.Path));

			var actions = GuiItemFactory.GetRepositoryActions<ToolStripMenuItem>(repository.DataContext.Path);
			if(actions.Count != 0)
			{
				Items.Add(new ToolStripSeparator());
				foreach(var item in actions)
				{
					Items.Add(item);
				}
			}

			Items.Add(new ToolStripSeparator());
			Items.Add(GuiItemFactory.GetRemoveRecentRepositoryItem<ToolStripMenuItem>(_repository.DataContext));
		}

		public RecentRepositoryListItem Repository
		{
			get { return _repository; }
		}
	}
}
