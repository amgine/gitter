﻿#region Copyright Notice
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

namespace gitter.Git.Gui;

using System;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Views;

using Resources = gitter.Git.Gui.Properties.Resources;

sealed class RepositoryRemotesListItem : RepositoryExplorerItemBase
{
	private RemoteListBinding _binding;

	public RepositoryRemotesListItem(IWorkingEnvironment environment)
		: base(Icons.Remotes, Resources.StrRemotes)
	{
		Verify.Argument.IsNotNull(environment);

		WorkingEnvironment = environment;
	}

	private IWorkingEnvironment WorkingEnvironment { get; }

	/// <inheritdoc/>
	protected override void OnActivate()
	{
		base.OnActivate();
		WorkingEnvironment.ViewDockService.ShowView(Guids.RemotesViewGuid);
	}

	private void OnRemoteItemActivated(object sender, BoundItemActivatedEventArgs<Remote> e)
	{
		Assert.IsNotNull(e);

		WorkingEnvironment.ViewDockService.ShowView(Guids.RemoteViewGuid, new RemoteViewModel(e.Object));
	}

	/// <inheritdoc/>
	public override void OnDoubleClick(int x, int y) { }

	/// <inheritdoc/>
	protected override void DetachFromRepository()
	{
		_binding.ItemActivated -= OnRemoteItemActivated;
		_binding.Dispose();
		_binding = null;
	}

	/// <inheritdoc/>
	protected override void AttachToRepository()
	{
		_binding = new RemoteListBinding(Items, Repository);
		_binding.ItemActivated += OnRemoteItemActivated;
	}

	/// <inheritdoc/>
	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNotNull(requestEventArgs);

		if(Repository is null) return default;

		var menu = new RemotesMenu(Repository);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}
}
