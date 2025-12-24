#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

public ref struct Gdi(Graphics graphics)
{
	public ref struct GdiSelectObjectScope(IntPtr hdc, IntPtr handle)
	{
		public IntPtr OldObjectHandle { readonly get; private set; } = handle;

		public void Dispose()
		{
			if(OldObjectHandle != IntPtr.Zero)
			{
				_ = Native.Gdi32.SelectObject(hdc, OldObjectHandle);
				OldObjectHandle = IntPtr.Zero;
			}
		}
	}

	private IntPtr _handle = graphics.GetHdc();

	public void Dispose()
	{
		if(_handle != IntPtr.Zero)
		{
			graphics.ReleaseHdc(_handle);
			_handle = IntPtr.Zero;
		}
	}

	public readonly IntPtr Handle => _handle;

	public static implicit operator IntPtr(Gdi gdi) => gdi._handle;

	public readonly void DrawBorder(Rectangle bounds, Color borderColor, Color backColor, int borderThickness)
	{
		static void ApplyInnerBorder(Rectangle bounds, int borderThickness, out int left, out int top, out int right, out int bottom)
		{
			int num = borderThickness >> 1;
			int num2 = num - ((borderThickness + 1) & 1);
			left   = bounds.X + num;
			top    = bounds.Y + num;
			right  = bounds.X + bounds.Width  - num2;
			bottom = bounds.Y + bounds.Height - num2;
		}

		if(borderThickness < 1)
		{
			Fill(backColor, bounds);
			return;
		}

		IntPtr brush = default;
		IntPtr pen   = default;
		try
		{
			brush = CreateSolidBrush(backColor);
			pen   = CreatePen(borderColor, borderThickness);
			ApplyInnerBorder(bounds, borderThickness, out var left, out var top, out var right, out var bottom);
			using(SelectObjectScoped(brush))
			using(SelectObjectScoped(pen))
			{
				Rectangle(left, top, right, bottom);
			}
		}
		finally
		{
			if(brush != IntPtr.Zero) DeleteObject(brush);
			if(pen   != IntPtr.Zero) DeleteObject(pen);
		}
	}

	public readonly void Rectangle(Color color, Rectangle bounds)
	{
		static GdiStockObject GetStockPen(Color color)
		{
			if(color == Color.White) return GdiStockObject.WhitePen;
			if(color == Color.Black) return GdiStockObject.BlackPen;
			return GdiStockObject.NullPen;
		}

		var stockPen = GetStockPen(color);
		if(stockPen != GdiStockObject.NullPen)
		{
			using(SelectObjectScoped(stockPen))
			using(SelectObjectScoped(GdiStockObject.NullBrush))
			{
				Rectangle(bounds);
			}
		}
		else
		{
			var pen = CreatePen(color);
			try
			{
				using(SelectObjectScoped(pen))
				using(SelectObjectScoped(GdiStockObject.NullBrush))
				{
					Rectangle(bounds);
				}
			}
			finally
			{
				DeleteObject(pen);
			}
		}
	}

	public readonly void Fill(Color color, Rectangle bounds)
	{
		static GdiStockObject GetStockBrush(Color color)
		{
			if(color == Color.White) return GdiStockObject.WhiteBrush;
			if(color == Color.Black) return GdiStockObject.BlackBrush;
			return GdiStockObject.NullBrush;
		}

		var stockBrush = GetStockBrush(color);
		if(stockBrush != GdiStockObject.NullBrush)
		{
			using(SelectObjectScoped(stockBrush))
			using(SelectObjectScoped(GdiStockObject.NullPen))
			{
				Fill(bounds);
			}
		}
		else
		{
			var brush = CreateSolidBrush(color);
			try
			{
				using(SelectObjectScoped(brush))
				using(SelectObjectScoped(GdiStockObject.NullPen))
				{
					Fill(bounds);
				}
			}
			finally
			{
				DeleteObject(brush);
			}
		}
	}

	public readonly void Rectangle(int left, int top, int right, int bottom)
		=> Native.Gdi32.Rectangle(_handle, left, top, right, bottom);

	public readonly void Fill(Rectangle bounds)
		=> Rectangle(bounds.X, bounds.Y, bounds.Right + 1, bounds.Bottom + 1);

	public readonly void Rectangle(Rectangle bounds)
		=> Rectangle(bounds.X, bounds.Y, bounds.Right, bounds.Bottom);

	public readonly IntPtr SelectObject(IntPtr handle)
		=> Native.Gdi32.SelectObject(_handle, handle);

	public readonly IntPtr SelectObject(GdiStockObject stockObject)
		=> Native.Gdi32.SelectObject(_handle, GetStockObject(stockObject));

	public readonly GdiSelectObjectScope SelectObjectScoped(IntPtr handle)
		=> new(_handle, Native.Gdi32.SelectObject(_handle, handle));

	public readonly GdiSelectObjectScope SelectObjectScoped(GdiStockObject stockObject)
		=> new(_handle, Native.Gdi32.SelectObject(_handle, GetStockObject(stockObject)));

	public static void DeleteObject(IntPtr handle)
		=> Native.Gdi32.DeleteObject(handle);

	public static IntPtr GetStockObject(GdiStockObject stockObject)
		=> Native.Gdi32.GetStockObject((Native.StockObject)stockObject);

	public static IntPtr CreateSolidBrush(Color color)
	{
		return Native.Gdi32.CreateSolidBrush(Native.Macro.RGB(color.R, color.G, color.B));
	}

	public static IntPtr CreatePen(Color color, int width = 1)
	{
		return Native.Gdi32.CreatePen(Native.PenStyle.PS_SOLID, width, Native.Macro.RGB(color.R, color.G, color.B));
	}

	public unsafe readonly void Line(Color color, Native.POINT* points, int count, int width = 1)
	{
		var pen = CreatePen(color, width);
		try
		{
			using(SelectObjectScoped(pen))
			{
				_ = Native.Gdi32.Polyline(_handle, points, count);
			}
		}
		finally
		{
			DeleteObject(pen);
		}
	}

	public unsafe readonly void Line(Color color, int x1, int y1, int x2, int y2, int width = 1)
	{
		var points = stackalloc Native.POINT[2]
		{
			new Native.POINT(x1, y1),
			new Native.POINT(x2, y2),
		};
		Line(color, points, 2, width);
	}
}
