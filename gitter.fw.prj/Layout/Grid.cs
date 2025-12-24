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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public sealed class Grid : IContent
{
	sealed class ContentLayout(Grid grid, GridContent content)
	{
		private readonly GridContent _content = content;
		private Rectangle _bounds;
		private Dpi _dpi;

		public Grid Grid { get; } = grid;

		private int GetX()
		{
			int x = 0;
			for(int i = 0; i < _content.Column; ++i)
			{
				x += Grid._columns[i].Width;
			}
			return x;
		}

		private int GetY()
		{
			int y = 0;
			for(int i = 0; i < _content.Row; ++i)
			{
				y += Grid._rows[i].Height;
			}
			return y;
		}

		private int GetWidth()
		{
			int w = 0;
			for(int i = _content.Column; i < _content.Column + _content.ColumnSpan; ++i)
			{
				w += Grid._columns[i].Width;
			}
			return w;
		}

		private int GetHeight()
		{
			int h = 0;
			for(int i = _content.Row; i < _content.Row + _content.RowSpan; ++i)
			{
				h += Grid._rows[i].Height;
			}
			return h;
		}

		public void UpdateBounds(Point origin, Dpi dpi)
		{
			var x = origin.X + GetX();
			var y = origin.Y + GetY();
			var w = GetWidth();
			var h = GetHeight();

			var bounds = new Rectangle(x, y, w, h);
			if(bounds != _bounds || dpi != _dpi)
			{
				_bounds = bounds;
				_dpi    = dpi;
				_content.Content.UpdateBounds(bounds, dpi);
			}
		}
	}

	private readonly GridRow[]       _rows;
	private readonly GridColumn[]    _columns;
	private readonly ContentLayout[] _content;
	private IDpiBoundValue<Padding>? _padding;
	private Rectangle _bounds;
	private Dpi _dpi;

	/// <summary>Создание <see cref="Grid"/>.</summary>
	/// <param name="columns">Размеры колонок.</param>
	/// <param name="rows">Размеры строк.</param>
	/// <param name="padding">Отступы от границ до содержимого.</param>
	/// <param name="content">Контент.</param>
	public Grid(ISizeSpec[]? columns = default, ISizeSpec[]? rows = default, IDpiBoundValue<Padding>? padding = default, GridContent[]? content = default)
	{
		_rows = rows is { Length: > 0 }
			? Array.ConvertAll(rows, s => new GridRow(this, s))
			: [new GridRow(this, SizeSpec.Everything())];
		_columns = columns is { Length: > 0 }
			? Array.ConvertAll(columns, s => new GridColumn(this, s))
			: [new GridColumn(this, SizeSpec.Everything())];
		_content = content is { Length: > 0 }
			? Array.ConvertAll(content, c => new ContentLayout(this, c))
			: Preallocated<ContentLayout>.EmptyArray;
		_padding = padding;

		foreach(var row in _rows)
		{
			row.SizeSpecChanged += OnRowSizeSpecChanged;
		}
		foreach(var column in _columns)
		{
			column.SizeSpecChanged += OnColumnSizeSpecChanged;
		}
	}

	private void OnRowSizeSpecChanged(object? sender, EventArgs e)
	{
		if(_dpi != default)
		{
			UpdateRows();
			UpdateContent(_dpi);
		}
	}

	private void OnColumnSizeSpecChanged(object? sender, EventArgs e)
	{
		if(_dpi != default)
		{
			UpdateColumns();
			UpdateContent(_dpi);
		}
	}

	/// <summary>Возвращает список строк таблицы.</summary>
	/// <value>Список строк таблицы.</value>
	public IReadOnlyList<GridRow> Rows => _rows;

	/// <summary>Возвращает список колонок таблицы.</summary>
	/// <value>Список колонок таблицы.</value>
	public IReadOnlyList<GridColumn> Columns => _columns;

	/// <summary>Возвращает и устанавливает отступ от границ таблицы до контента.</summary>
	/// <value>Отступ от границ таблицы до контента.</value>
	public IDpiBoundValue<Padding>? Padding
	{
		get => _padding;
		set
		{
			if(_padding != value)
			{
				_padding = value;
				if(_dpi != default)
				{
					UpdateBounds(_bounds, _dpi);
				}
			}
		}
	}

	/// <inheritdoc/>
	public void UpdateBounds(Rectangle bounds, Dpi dpi)
	{
		if(Padding is not null)
		{
			var padding = Padding.GetValue(dpi);
			bounds = bounds.WithPadding(padding);
		}

		bool dpiChanged    = _dpi != dpi;
		bool widthChanged  = _bounds.Width  != bounds.Width;
		bool heightChanged = _bounds.Height != bounds.Height;
		bool xChanged      = _bounds.X != bounds.X;
		bool yChanged      = _bounds.Y != bounds.Y;

		if(!dpiChanged && !widthChanged && !heightChanged)
		{
			if(!xChanged && !yChanged) return;

			_bounds = bounds;
			UpdateContent(dpi);
			return;
		}

		_bounds = bounds;
		_dpi    = dpi;

		if(dpiChanged || widthChanged)
		{
			UpdateColumns();
		}
		if(dpiChanged || heightChanged)
		{
			UpdateRows();
		}
		UpdateContent(dpi);
	}

	private void UpdateColumns()
	{
		int w = _bounds.Width;
		var remaining = w;
		foreach(var column in _columns)
		{
			if(column.SizeSpec.Priority == 1)
			{
				if(w > 0)
				{
					var size = column.SizeSpec.GetSize(remaining, _dpi);
					if(size > w) size = w;
					column.Width = size;
					w -= size;
				}
				else
				{
					column.Width = 0;
				}
			}
		}
		remaining = w;
		foreach(var column in _columns)
		{
			if(column.SizeSpec.Priority == 0)
			{
				if(w > 0)
				{
					var size = column.SizeSpec.GetSize(remaining, _dpi);
					if(size > w) size = w;
					column.Width = size;
					w -= size;
				}
				else
				{
					column.Width = 0;
				}
			}
		}
	}

	private void UpdateRows()
	{
		int h = _bounds.Height;
		var remaining = h;
		foreach(var row in _rows)
		{
			if(row.SizeSpec.Priority == 1)
			{
				if(h > 0)
				{
					var size = row.SizeSpec.GetSize(remaining, _dpi);
					if(size > h) size = h;
					row.Height = size;
					h -= size;
				}
				else
				{
					row.Height = 0;
				}
			}
		}
		remaining = h;
		foreach(var row in _rows)
		{
			if(row.SizeSpec.Priority == 0)
			{
				if(h > 0)
				{
					var size = row.SizeSpec.GetSize(remaining, _dpi);
					if(size > h) size = h;
					row.Height = size;
					h -= size;
				}
				else
				{
					row.Height = 0;
				}
			}
		}
	}

	private void UpdateContent(Dpi dpi)
	{
		foreach(var content in _content)
		{
			content.UpdateBounds(_bounds.Location, dpi);
		}
	}
}
