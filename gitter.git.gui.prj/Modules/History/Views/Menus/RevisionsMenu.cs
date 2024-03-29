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

namespace gitter.Git.Gui.Controls;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;

[ToolboxItem(false)]
[DesignerCategory("")]
public sealed class RevisionsMenu : ContextMenuStrip
{
	/// <summary>Create <see cref="RevisionsMenu"/>.</summary>
	/// <param name="revisions">List of related revisions.</param>
	public RevisionsMenu(IList<Revision> revisions)
	{
		Verify.Argument.IsNotNull(revisions);

		Revisions = revisions;

		var dpiBindings = new DpiBindings(this);
		var factory     = new GuiItemFactory(dpiBindings);

		//Items.Add(GuiItemFactory.GetCherryPickItem<ToolStripMenuItem>(revisions));
		//Items.Add(GuiItemFactory.GetRevertItem<ToolStripMenuItem>(revisions));

		if(revisions.Count == 2)
		{
			Items.Add(factory.GetCompareWithItem<ToolStripMenuItem>(revisions[0], revisions[1]));
		}
	}

	public IList<Revision> Revisions { get; }
}
