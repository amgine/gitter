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

namespace gitter.Framework.Controls;

using System;
using System.Drawing;

public ref struct Gdi
{
	private Graphics _graphics;
	private IntPtr   _handle;

	public Gdi(Graphics graphics)
	{
		_graphics = graphics;
		_handle   = graphics.GetHdc();
	}

	public void Dispose()
	{
		if(_graphics is not null)
		{
			_graphics.ReleaseHdc(_handle);
			_graphics = null;
			_handle   = IntPtr.Zero;
		}
	}

	public IntPtr Handle => _handle;

	public static implicit operator IntPtr(Gdi gdi) => gdi._handle;

	public void Rectangle(Color color, Rectangle bounds)
	{
		static Native.StockObject GetStockPen(Color color)
		{
			if(color == Color.White) return Native.StockObject.WHITE_PEN;
			if(color == Color.Black) return Native.StockObject.BLACK_PEN;
			return Native.StockObject.NULL_PEN;
		}

		var stockPen = GetStockPen(color);
		if(stockPen != Native.StockObject.NULL_PEN)
		{
			SelectObject(Native.Gdi32.GetStockObject(stockPen));
			SelectObject(Native.Gdi32.GetStockObject(Native.StockObject.NULL_BRUSH));
			Rectangle(bounds);
		}
		else
		{
			var pen = CreatePen(color);
			try
			{
				SelectObject(pen);
				SelectObject(Native.Gdi32.GetStockObject(Native.StockObject.NULL_BRUSH));
				Rectangle(bounds);
			}
			finally
			{
				DeleteObject(pen);
			}
		}
	}

	public void Fill(Color color, Rectangle bounds)
	{
		static Native.StockObject GetStockBrush(Color color)
		{
			if(color == Color.White) return Native.StockObject.WHITE_BRUSH;
			if(color == Color.Black) return Native.StockObject.BLACK_BRUSH;
			return Native.StockObject.NULL_BRUSH;
		}

		var stockBrush = GetStockBrush(color);
		if(stockBrush != Native.StockObject.NULL_BRUSH)
		{
			SelectObject(Native.Gdi32.GetStockObject(stockBrush));
			SelectObject(Native.Gdi32.GetStockObject(Native.StockObject.NULL_PEN));
			Fill(bounds);
		}
		else
		{
			var brush = CreateSolidBrush(color);
			try
			{
				SelectObject(brush);
				SelectObject(Native.Gdi32.GetStockObject(Native.StockObject.NULL_PEN));
				Fill(bounds);
			}
			finally
			{
				DeleteObject(brush);
			}
		}
	}

	public void Fill(Rectangle bounds)
	{
		_ = Native.Gdi32.Rectangle(_handle, bounds.X, bounds.Y, bounds.Right + 1, bounds.Bottom + 1);
	}

	public void Rectangle(Rectangle bounds)
	{
		_ = Native.Gdi32.Rectangle(_handle, bounds.X, bounds.Y, bounds.Right, bounds.Bottom);
	}

	public IntPtr SelectObject(IntPtr handle)
		=> Native.Gdi32.SelectObject(_handle, handle);

	public static void DeleteObject(IntPtr handle)
	{
		_ = Native.Gdi32.DeleteObject(handle);
	}

	public static IntPtr CreateSolidBrush(Color color)
	{
		return Native.Gdi32.CreateSolidBrush(Native.Macro.RGB(color.R, color.G, color.B));
	}

	public static IntPtr CreatePen(Color color, int width = 1)
	{
		return Native.Gdi32.CreatePen(Native.PenStyle.PS_SOLID, width, Native.Macro.RGB(color.R, color.G, color.B));
	}

	public unsafe void Line(Color color, Native.POINT* points, int count, int width = 1)
	{
		var pen = Native.Gdi32.CreatePen(Native.PenStyle.PS_SOLID, width, Native.Macro.RGB(color.R, color.G, color.B));
		SelectObject(pen);
		try
		{
			_ = Native.Gdi32.Polyline(_handle, points, 2);
		}
		finally
		{
			DeleteObject(pen);
		}
	}

	public unsafe void Line(Color color, int x1, int y1, int x2, int y2, int width = 1)
	{
		var points = stackalloc Native.POINT[2]
		{
			new Native.POINT(x1, y1),
			new Native.POINT(x2, y2),
		};
		Line(color, points, 2, width);
	}
}
