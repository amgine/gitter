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
internal sealed class RemotesToolbar : ToolStrip
{
	private readonly RemotesView _remotesView;

	private readonly ToolStripButton _btnRefresh;
	private readonly ToolStripButton _btnAddRemote;

	public RemotesToolbar(RemotesView remotesView)
	{
		Verify.Argument.IsNotNull(remotesView);

		_remotesView = remotesView;

		var dpiBindings = new DpiBindings(this);
		var factory     = new GuiItemFactory(dpiBindings);

		Items.Add(_btnRefresh = factory.CreateRefreshContentButton(remotesView));
		Items.Add(new ToolStripSeparator());
		Items.Add(_btnAddRemote = new ToolStripButton(Resources.StrAddRemote, default,
			(_, _) =>
			{
				if(_remotesView.Repository is null) return;

				using var dialog = new AddRemoteDialog(_remotesView.Repository);
				dialog.Run(_remotesView);
			}));

		dpiBindings.BindImage(_btnAddRemote, Icons.RemoteAdd);
	}
}
