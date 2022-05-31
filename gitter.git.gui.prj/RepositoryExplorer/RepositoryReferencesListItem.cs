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

namespace gitter.Git.Gui;

using System;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Views;

using Resources = gitter.Git.Gui.Properties.Resources;

sealed class RepositoryReferencesListItem : RepositoryExplorerItemBase
{
	private readonly IWorkingEnvironment _environment;
	private ReferenceTreeBinding _refsBinding;

	public RepositoryReferencesListItem(IWorkingEnvironment environment)
		: base(Icons.Branch, Resources.StrReferences)
	{
		Verify.Argument.IsNotNull(environment);

		_environment = environment;
	}

	/// <inheritdoc/>
	protected override void OnActivate()
	{
		base.OnActivate();
		_environment.ViewDockService.ShowView(Guids.ReferencesViewGuid);
	}

	/// <inheritdoc/>
	public override void OnDoubleClick(int x, int y) { }

	/// <inheritdoc/>
	protected override void AttachToRepository()
	{
		_refsBinding = new ReferenceTreeBinding(Items, Repository, true, true, null,
			ReferenceType.LocalBranch | ReferenceType.RemoteBranch | ReferenceType.Tag);
		_refsBinding.ReferenceItemActivated += OnReferenceItemActivated;
	}

	/// <inheritdoc/>
	protected override void DetachFromRepository()
	{
		_refsBinding.ReferenceItemActivated -= OnReferenceItemActivated;
		_refsBinding.Dispose();
		_refsBinding = null;
		Collapse();
	}

	private void OnReferenceItemActivated(object sender, RevisionPointerEventArgs e)
	{
		Assert.IsNotNull(e);

		var rev = e.Object;
		var view = (HistoryView)_environment.ViewDockService.ShowView(Guids.HistoryViewGuid, false);
		view.SelectRevision(rev);
	}

	/// <inheritdoc/>
	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNotNull(requestEventArgs);

		if(Repository is null) return default;

		var menu = new ReferencesMenu(Repository);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}
}
