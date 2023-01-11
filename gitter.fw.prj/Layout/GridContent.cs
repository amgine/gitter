#region Copyright Notice
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

namespace gitter.Framework.Layout;

public sealed class GridContent
{
	public GridContent(IContent content, int column = 0, int row = 0, int columnSpan = 1, int rowSpan = 1)
	{
		Verify.Argument.IsNotNull(content);
		Verify.Argument.IsNotNegative(column);
		Verify.Argument.IsNotNegative(row);
		Verify.Argument.IsPositive(columnSpan);
		Verify.Argument.IsPositive(rowSpan);

		Row        = row;
		Column     = column;
		RowSpan    = rowSpan;
		ColumnSpan = columnSpan;
		Content    = content;
	}

	public int Row { get; }

	public int Column { get; }

	public int RowSpan { get; }

	public int ColumnSpan { get; }

	public IContent Content { get; }
}
