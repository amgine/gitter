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

namespace gitter.Framework.Controls;

using System.Drawing;

using Resources = gitter.Framework.Properties.Resources;

sealed class LogListBoxTimestampColumn : DateColumn
{
	/// <summary>Initializes a new instance of the <see cref="LogListBoxTimestampColumn"/> class.</summary>
	public LogListBoxTimestampColumn()
		: base((int)LogListBoxColumnId.Timestamp, Resources.StrTimestamp, visible: true)
	{
		Width      = 158;
		DateFormat = DateFormat.ISO8601;
	}

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		if(measureEventArgs.Item is LogEventListItem item)
		{
			return OnMeasureSubItem(measureEventArgs, item.DataContext.Timestamp);
		}
		return base.OnMeasureSubItem(measureEventArgs);
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		if(paintEventArgs.Item is LogEventListItem item)
		{
			OnPaintSubItem(paintEventArgs, item.DataContext.Timestamp);
		}
	}

	/// <inheritdoc/>
	public override string IdentificationString => "Timestamp";
}
