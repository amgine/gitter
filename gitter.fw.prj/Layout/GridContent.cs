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

using System;

/// <summary>Контент, размещаемый в <see cref="Grid"/>.</summary>
public sealed class GridContent
{
	/// <summary>Создание <see cref="GridContent"/>.</summary>
	/// <param name="content">Размещаемый контент.</param>
	/// <param name="column">Колонка.</param>
	/// <param name="row">Строка.</param>
	/// <param name="columnSpan">Количество занимаемых колонок.</param>
	/// <param name="rowSpan">Количество занимаемых строк.</param>
	/// <exception cref="ArgumentNullException"><paramref name="column"/> == <c>null</c>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="column"/> должен быть неотрицательным;<br/>
	/// <paramref name="row"/> должен быть неотрицательным;<br/>
	/// <paramref name="columnSpan"/> должен быть положительным;<br/>
	/// <paramref name="rowSpan"/> должен быть положительным.<br/>
	/// </exception>
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

	/// <summary>Возвращает индекс строки, в которой размещается контент.</summary>
	/// <value>Индекс строки, в которой размещается контент.</value>
	public int Row { get; }

	/// <summary>Возвращает индекс колонки, в которой размещается контент.</summary>
	/// <value>Индекс колонки, в которой размещается контент.</value>
	public int Column { get; }

	/// <summary>Возвращает количество строк занимаемых контентом.</summary>
	/// <value>Количество строк занимаемых контентом.</value>
	public int RowSpan { get; }

	/// <summary>Возвращает количество колонок занимаемых контентом.</summary>
	/// <value>Количество колонок занимаемых контентом.</value>
	public int ColumnSpan { get; }

	/// <summary>Возвращает контент, размещаемый в ячейках <see cref="Grid"/>.</summary>
	/// <value>Контент, размещаемый в ячейках <see cref="Grid"/>.</value>
	public IContent Content { get; }
}
