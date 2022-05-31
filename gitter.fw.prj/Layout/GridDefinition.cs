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

/// <summary>Структурный элемент таблицы (строка или столбец).</summary>
public abstract class GridDefinition
{
	private ISizeSpec _sizeSpec;

	/// <summary>Спецификация размера элемента изменилась.</summary>
	public event EventHandler? SizeSpecChanged;

	/// <summary>Вызывается при изменении спецификации размеров элемента.</summary>
	/// <param name="e">Аргументы события.</param>
	protected virtual void OnSizeSpecChanged(EventArgs e)
		=> SizeSpecChanged?.Invoke(this, e);

	private protected GridDefinition(Grid grid, ISizeSpec sizeSpec)
	{
		Verify.Argument.IsNotNull(sizeSpec);

		Grid      = grid;
		_sizeSpec = sizeSpec;
	}

	/// <summary>Возвращает сетку, которой принадлежит данный элемент.</summary>
	public Grid Grid { get; }

	/// <summary>
	/// Возвращает и устанавливает размер элемента в значащем для него
	/// измерении (ширина для столбца или высота для строки).
	/// </summary>
	public ISizeSpec SizeSpec
	{
		get => _sizeSpec;
		set
		{
			Verify.Argument.IsNotNull(value);

			if(_sizeSpec != value)
			{
				_sizeSpec = value;
				OnSizeSpecChanged(EventArgs.Empty);
			}
		}
	}
}
