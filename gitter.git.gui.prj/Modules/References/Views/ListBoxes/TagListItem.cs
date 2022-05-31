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
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

/// <summary>A <see cref="CustomListBoxItem"/> representing <see cref="Tag"/> object.</summary>
public class TagListItem : ReferenceListItemBase<Tag>
{
	/// <summary>Create <see cref="TagListItem"/>.</summary>
	/// <param name="tag">Related <see cref="Tag"/>.</param>
	/// <exception cref="ArgumentNullException"><paramref name="tag"/> == <c>null</c>.</exception>
	public TagListItem(Tag tag)
		: base(tag)
	{
		Verify.Argument.IsNotNull(tag);
	}

	/// <inheritdoc/>
	protected override Image GetImage(Dpi dpi)
	{
		var icon = DataContext.TagType == TagType.Annotated
			? Icons.TagAnnotated
			: Icons.Tag;
		return icon.GetImage(DpiConverter.FromDefaultTo(dpi).ConvertX(16));
	}

	/// <inheritdoc/>
	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		var menu = new TagMenu(DataContext);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}
}
