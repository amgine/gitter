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
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class RemotesToolbar : ToolStrip
	{
		static class Icons
		{
			const int Size = 16;

			public static readonly IDpiBoundValue<Bitmap> Refresh   = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"refresh",    Size);
			public static readonly IDpiBoundValue<Bitmap> RemoteAdd = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"remote.add", Size);
		}

		private readonly RemotesView _remotesView;

		private readonly ToolStripButton _btnRefresh;
		private readonly ToolStripButton _btnAddRemote;
		private readonly DpiBindings _bindings;

		public RemotesToolbar(RemotesView remotesView)
		{
			Verify.Argument.IsNotNull(remotesView, nameof(remotesView));

			_remotesView = remotesView;

			Items.Add(_btnRefresh = new ToolStripButton(Resources.StrRefresh, default,
				(_, _) => _remotesView.RefreshContent())
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});
			Items.Add(new ToolStripSeparator());
			Items.Add(_btnAddRemote = new ToolStripButton(Resources.StrAddRemote, default,
				(_, _) =>
				{
					using var dlg = new AddRemoteDialog(_remotesView.Repository);
					dlg.Run(_remotesView);
				}));

			_bindings = new DpiBindings(this);
			_bindings.BindImage(_btnRefresh,   Icons.Refresh);
			_bindings.BindImage(_btnAddRemote, Icons.RemoteAdd);
		}
	}
}
