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

using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>"Email" column.</summary>
public class EmailColumn : CustomListBoxColumn
{
	protected EmailColumn(ISubItemPainter painter, int id, string name, bool visible)
		: base(id, name, visible)
	{
		Verify.Argument.IsNotNull(painter);

		Width   = 80;
		Painter = painter;
	}

	public EmailColumn(ISubItemPainter painter, string name, bool visible)
		: this(painter, (int)ColumnId.Email, name, visible)
	{
	}

	public EmailColumn(ISubItemPainter painter, bool visible = true)
		: this(painter, (int)ColumnId.Email, Resources.StrEmail, visible)
	{
	}

	private ISubItemPainter Painter { get; }

	/// <inheritdoc/>
	public override string IdentificationString => "Email";

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		if(Painter.TryMeasure(measureEventArgs, out var size))
		{
			return size;
		}
		return Size.Empty;
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		Painter.TryPaint(paintEventArgs);
	}
}
