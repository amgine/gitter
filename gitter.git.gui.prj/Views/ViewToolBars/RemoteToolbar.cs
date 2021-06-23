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

		private readonly ToolStripButton _btnRefresh;
		private readonly ToolStripButton _btnFetch;
		private readonly ToolStripButton _btnPull;
		private readonly ToolStripButton _btnPush;
		private readonly ToolStripButton _btnPrune;

		public RemoteToolbar(RemoteView remoteView)
		{
			Verify.Argument.IsNotNull(remoteView, nameof(remoteView));

			_remoteView = remoteView;

			Items.Add(_btnRefresh = new ToolStripButton(Resources.StrRefresh, default,
				(_, _) => _remoteView.RefreshContent())
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});
			Items.Add(new ToolStripSeparator());
			Items.Add(_btnFetch = new ToolStripButton(Resources.StrFetch, default, OnFetchClick)
				{
					ToolTipText = Resources.TipFetch,
				});
			Items.Add(_btnPull = new ToolStripButton(Resources.StrPull, default, OnPullClick)
				{
					ToolTipText = Resources.TipPull,
				});
			Items.Add(_btnPush = new ToolStripButton(Resources.StrPush, default, OnPushClick)
				{
					ToolTipText = Resources.TipPush,
				});
			Items.Add(new ToolStripSeparator());
			Items.Add(_btnPrune = new ToolStripButton(Resources.StrPrune, default, OnPruneClick));

			UpdateIcons(DeviceDpi);
		}

		private void UpdateIcons(int dpi)
		{
			var iconSize = dpi * 16 / 96;

			_btnRefresh.Image = CachedResources.ScaledBitmaps[@"refresh", iconSize];
			_btnFetch.Image   = CachedResources.ScaledBitmaps[@"fetch",   iconSize];
			_btnPull.Image    = CachedResources.ScaledBitmaps[@"pull",    iconSize];
			_btnPush.Image    = CachedResources.ScaledBitmaps[@"push",    iconSize];
			_btnPrune.Image   = CachedResources.ScaledBitmaps[@"prune",   iconSize];
		}

		protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
		{
			base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
			UpdateIcons(deviceDpiNew);
		}

		private void OnFetchClick(object sender, EventArgs e)
		{
			var remote = _remoteView.Remote;
			if(remote is { IsDeleted: false })
			{
				GuiCommands.Fetch(_remoteView, remote);
			}
		}

		private void OnPullClick(object sender, EventArgs e)
		{
			var remote = _remoteView.Remote;
			if(remote is { IsDeleted: false })
			{
				GuiCommands.Pull(_remoteView, remote);
			}
		}

		private void OnPushClick(object sender, EventArgs e)
		{
			var remote = _remoteView.Remote;
			if(remote is { IsDeleted: false })
			{
				using var dlg = new PushDialog(_remoteView.Repository);
				dlg.Remote.Value = remote;
				dlg.Run(_remoteView);
			}
		}

		private void OnPruneClick(object sender, EventArgs e)
		{
			var remote = _remoteView.Remote;
			if(remote is { IsDeleted: false })
			{
				GuiCommands.Prune(_remoteView, remote);
			}
		}
	}
}
