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
	sealed class RemoteToolbar : ToolStrip
	{
		private readonly RemoteView _remoteView;

		public RemoteToolbar(RemoteView remoteView)
		{
			Verify.Argument.IsNotNull(remoteView, "remoteView");

			_remoteView = remoteView;

			Items.Add(new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"],
				(sender, e) => _remoteView.RefreshContent())
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});
			Items.Add(new ToolStripSeparator());
			Items.Add(new ToolStripButton(Resources.StrFetch, CachedResources.Bitmaps["ImgFetch"], OnFetchClick)
				{
					ToolTipText = Resources.TipFetch,
				});
			Items.Add(new ToolStripButton(Resources.StrPull, CachedResources.Bitmaps["ImgPull"], OnPullClick)
				{
					ToolTipText = Resources.TipPull,
				});
			Items.Add(new ToolStripButton(Resources.StrPush, CachedResources.Bitmaps["ImgPush"], OnPushClick)
				{
					ToolTipText = Resources.TipPush,
				});
			Items.Add(new ToolStripSeparator());
			Items.Add(new ToolStripButton(Resources.StrPrune, CachedResources.Bitmaps["ImgClean"], OnPruneClick));
		}

		private void OnFetchClick(object sender, EventArgs e)
		{
			var remote = _remoteView.Remote;
			if(remote != null && !remote.IsDeleted)
			{
				GuiCommands.Fetch(_remoteView, remote);
			}
		}

		private void OnPullClick(object sender, EventArgs e)
		{
			var remote = _remoteView.Remote;
			if(remote != null && !remote.IsDeleted)
			{
				GuiCommands.Pull(_remoteView, remote);
			}
		}

		private void OnPushClick(object sender, EventArgs e)
		{
			var remote = _remoteView.Remote;
			if(remote != null && !remote.IsDeleted)
			{
				using(var dlg = new PushDialog(_remoteView.Repository))
				{
					dlg.Remote.Value = remote;
					dlg.Run(_remoteView);
				}
			}
		}

		private void OnPruneClick(object sender, EventArgs e)
		{
			var remote = _remoteView.Remote;
			if(remote != null && !remote.IsDeleted)
			{
				GuiCommands.Prune(_remoteView, remote);
			}
		}
	}
}
