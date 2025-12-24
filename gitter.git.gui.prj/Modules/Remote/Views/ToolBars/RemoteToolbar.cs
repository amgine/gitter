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

namespace gitter.Git.Gui.Views;

using System;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Git.Gui.Dialogs;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
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
		Verify.Argument.IsNotNull(remoteView);

		_remoteView = remoteView;

		var dpiBindings = new DpiBindings(this);
		var factory     = new GuiItemFactory(dpiBindings);

		Items.Add(_btnRefresh = factory.CreateRefreshContentButton(remoteView));
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

		dpiBindings.BindImage(_btnFetch, Icons.Fetch);
		dpiBindings.BindImage(_btnPull,  Icons.Pull);
		dpiBindings.BindImage(_btnPush,  Icons.Push);
		dpiBindings.BindImage(_btnPrune, Icons.Prune);
	}

	private void OnFetchClick(object? sender, EventArgs e)
	{
		var remote = _remoteView.Remote;
		if(remote is { IsDeleted: false })
		{
			GuiCommands.Fetch(_remoteView, remote);
		}
	}

	private void OnPullClick(object? sender, EventArgs e)
	{
		var remote = _remoteView.Remote;
		if(remote is { IsDeleted: false })
		{
			GuiCommands.Pull(_remoteView, remote);
		}
	}

	private void OnPushClick(object? sender, EventArgs e)
	{
		var remote = _remoteView.Remote;
		if(remote is { IsDeleted: false })
		{
			using var dialog = new PushDialog(remote.Repository);
			dialog.Remote.Value = remote;
			dialog.Run(_remoteView);
		}
	}

	private void OnPruneClick(object? sender, EventArgs e)
	{
		var remote = _remoteView.Remote;
		if(remote is { IsDeleted: false })
		{
			GuiCommands.Prune(_remoteView, remote);
		}
	}
}
