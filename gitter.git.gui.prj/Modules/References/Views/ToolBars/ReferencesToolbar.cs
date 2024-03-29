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

namespace gitter.Git.Gui.Views;

using System;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
internal sealed class ReferencesToolbar : ToolStrip
{
	private readonly ReferencesView  _referencesView;
	private readonly ToolStripButton _btnCreateBranch;
	private readonly ToolStripButton _btnCreateTag;

	public ReferencesToolbar(ReferencesView referencesView)
	{
		Verify.Argument.IsNotNull(referencesView);

		var dpiBindings = new DpiBindings(this);
		var factory     = new GuiItemFactory(dpiBindings);

		_referencesView = referencesView;
		Items.Add(factory.CreateRefreshContentButton(referencesView));

		Items.Add(new ToolStripSeparator());

		Items.Add(_btnCreateBranch =
			new ToolStripButton(
				Resources.StrCreateBranch,
				null,
				OnCreateBranchButtonClick));

		Items.Add(_btnCreateTag =
			new ToolStripButton(
				Resources.StrCreateTag,
				null,
				OnCreateTagButtonClick));

		dpiBindings.BindImage(_btnCreateBranch, Icons.BranchAdd);
		dpiBindings.BindImage(_btnCreateTag,    Icons.TagAdd);
	}

	private void OnCreateBranchButtonClick(object sender, EventArgs e)
		=> _referencesView.Gui.StartCreateBranchDialog();

	private void OnCreateTagButtonClick(object sender, EventArgs e)
		=> _referencesView.Gui.StartCreateTagDialog();
}
