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

namespace gitter.Framework.Options;

using System;
using System.Drawing;

public sealed class SelectableColor : IDisposable
{
	private Color _color;

	public event EventHandler? Changed;

	public SelectableColor(string id, string name, Color color, string categoryId)
	{
		Id = id;
		Name = name;
		_color = color;
		Brush = new SolidBrush(color);
		Pen = new Pen(color);
		CategoryId = categoryId;
	}

	public SelectableColor(string id, string name, Color color)
	{
		Id = id;
		Name = name;
		_color = color;
		Brush = new SolidBrush(color);
		Pen = new Pen(color);
	}

	~SelectableColor() => Dispose(disposing: false);

	public string Id { get; }

	public string Name { get; }

	public string? CategoryId { get; }

	public Color Color
	{
		get => _color;
		set
		{
			if(_color != value)
			{
				_color = value;
				Brush?.Dispose();
				Pen?.Dispose();
				Brush = new SolidBrush(value);
				Pen = new Pen(value);
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public Brush Brush { get; private set; }

	public Pen Pen { get; private set; }

	public static implicit operator Color(SelectableColor color) => color._color;

	public static implicit operator Brush(SelectableColor color) => color.Brush;

	public static implicit operator Pen(SelectableColor color) => color.Pen;

	public override string ToString() => Name;

	private void Dispose(bool disposing)
	{
		Brush?.Dispose();
		Pen?.Dispose();
		if(disposing)
		{
			Brush = null!;
			Pen = null!;
		}
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(disposing: true);
	}
}
