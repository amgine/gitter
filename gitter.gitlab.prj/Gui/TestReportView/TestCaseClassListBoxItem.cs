﻿#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Gui;

using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Git.Gui;

sealed class TestCaseClassListBoxItem : CustomListBoxItem<string>
{
	public TestCaseClassListBoxItem(string className) : base(className)
	{
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		switch(paintEventArgs.Column.Id)
		{
			case 0:
				paintEventArgs.PaintImageAndText(CommonIcons.Folder.GetImage(16 * paintEventArgs.Dpi.X / 96), DataContext);
				break;
			default:
				base.OnPaintSubItem(paintEventArgs);
				break;
		}
	}

	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNotNull(requestEventArgs);



		return base.GetContextMenu(requestEventArgs);
	}
}
