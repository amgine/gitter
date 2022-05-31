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

namespace gitter.Git.Gui.Controls;

using System;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Item used to group reference-representing items together.</summary>
public class ReferenceGroupListItem : CustomListBoxItem<ReferenceType>
{
	/// <summary>Create <see cref="ReferenceGroupListItem"/>.</summary>
	/// <param name="repository">Related repository.</param>
	/// <param name="referenceTypes">Reference types to group.</param>
	public ReferenceGroupListItem(Repository repository, ReferenceType referenceTypes)
		: base(referenceTypes)
	{
		Repository = repository;
	}

	public Repository Repository { get; }

	private Image GetIcon(Dpi dpi)
	{
		var icon = DataContext switch
		{
			ReferenceType.LocalBranch  => Icons.Branches,
			ReferenceType.RemoteBranch => Icons.RemoteBranches,
			ReferenceType.Tag          => Icons.Tags,
			_ => default,
		};
		return icon?.GetImage(DpiConverter.FromDefaultTo(dpi).ConvertX(16));
	}

	private string GetText()
		=> DataContext switch
		{
			ReferenceType.LocalBranch  => Resources.StrHeads,
			ReferenceType.RemoteBranch => Resources.StrRemotes,
			ReferenceType.Tag          => Resources.StrTags,
			_ => default,
		};

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		switch((ColumnId)measureEventArgs.SubItemId)
		{
			case ColumnId.Name:
				return measureEventArgs.MeasureImageAndText(GetIcon(measureEventArgs.Dpi), GetText());
			default:
				return Size.Empty;
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		if(paintEventArgs.SubItemId == (int)ColumnId.Name)
		{
			paintEventArgs.PaintImageAndText(GetIcon(paintEventArgs.Dpi), GetText());
		}
	}

	/// <inheritdoc/>
	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNotNull(requestEventArgs);

		var menu = default(ContextMenuStrip);
		if(Repository is not null)
		{
			menu = new ContextMenuStrip();

			var dpiBindings = new DpiBindings(menu);
			var factory     = new GuiItemFactory(dpiBindings);

			menu.Items.Add(factory.GetRefreshReferencesItem<ToolStripMenuItem>(Repository, DataContext, Resources.StrRefresh));
			switch(DataContext)
			{
				case ReferenceType.LocalBranch:
					menu.Items.Add(factory.GetCreateBranchItem<ToolStripMenuItem>(Repository));
					break;
				case ReferenceType.RemoteBranch:
					menu.Items.Add(factory.GetAddRemoteItem<ToolStripMenuItem>(Repository));
					break;
				case ReferenceType.Tag:
					menu.Items.Add(factory.GetCreateTagItem<ToolStripMenuItem>(Repository));
					break;
			}
		}
		if(menu is not null)
		{
			Utility.MarkDropDownForAutoDispose(menu);
		}
		return menu;
	}
}
