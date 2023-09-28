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

using gitter.Framework;
using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>"Author Date" column.</summary>
public sealed class AuthorDateColumn : DateColumn
{
	public AuthorDateColumn()
		: base((int)ColumnId.AuthorDate, Resources.StrAuthorDate, visible: false)
	{
	}

	/// <inheritdoc/>
	public override string IdentificationString => "AuthorDate";

	private static bool TryGetAuthorTimestamp(Revision revision, out DateTimeOffset authorDate)
	{
		if(revision is not null)
		{
			authorDate = revision.AuthorDate;
			return true;
		}
		authorDate = default;
		return false;
	}

	private static bool TryGetAuthorTimestamp(CustomListBoxItem item, out DateTimeOffset authorDate)
	{
		switch(item)
		{
			case IDataContextProvider<Revision> revItem:
				return TryGetAuthorTimestamp(revItem.DataContext, out authorDate);
			case IDataContextProvider<IRevisionPointer> revPtrItem:
				return TryGetAuthorTimestamp(revPtrItem.DataContext?.Dereference(), out authorDate);
			default:
				authorDate = default;
				return false;
		}
	}

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		if(TryGetAuthorTimestamp(measureEventArgs.Item, out var authorDate))
		{
			return OnMeasureSubItem(measureEventArgs, authorDate);
		}
		return base.OnMeasureSubItem(measureEventArgs);
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		if(TryGetAuthorTimestamp(paintEventArgs.Item, out var authorDate))
		{
			OnPaintSubItem(paintEventArgs, authorDate);
		}
	}
}