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

using gitter.Framework;

public abstract class TimestampPainterBase<T> : ISubItemPainter
{
#if NETCOREAPP

	const int CharBufferSize = 64;

#endif

	private static string GetText(DateColumn column, DateTimeOffset timestamp)
	{
		Assert.IsNotNull(column);

		var format         = column.DateFormat;
		var convertTolocal = column.ConvertToLocal;
		var showUTCOffset  = column.ShowUTCOffset;
		if(convertTolocal) timestamp = timestamp.ToLocalTime();
		return Utility.FormatDate(timestamp, format, showUTCOffset);
	}

#if NETCOREAPP

	private static bool TryGetText(DateColumn column, Span<char> text, out int charsWritten, DateTimeOffset timestamp)
	{
		Assert.IsNotNull(column);

		var format         = column.DateFormat;
		var convertToLocal = column.ConvertToLocal;
		var showUTCOffset  = column.ShowUTCOffset;
		if(convertToLocal) timestamp = timestamp.ToLocalTime();
		return Utility.TryFormatDate(timestamp, text, out charsWritten, format, showUTCOffset);
	}

#endif

	protected abstract bool TryGetTimestamp(T item, out DateTimeOffset timestamp);

	protected virtual bool TryGetTextBrush(SubItemPaintEventArgs paintEventArgs, out Brush textBrush, out bool disposeBrush)
	{
		textBrush    = default;
		disposeBrush = false;
		return false;
	}

	/// <inheritdoc/>
	public bool TryMeasure(SubItemMeasureEventArgs measureEventArgs, out Size size)
	{
		Verify.Argument.IsNotNull(measureEventArgs);

		if( measureEventArgs.Column is not DateColumn column ||
			measureEventArgs.Item   is not T item)
		{
			size = Size.Empty;
			return false;
		}

		if(!TryGetTimestamp(item, out var timestamp))
		{
			size = Size.Empty;
			return false;
		}

#if NETCOREAPP
		Span<char> buffer = stackalloc char[CharBufferSize];
		if(TryGetText(column, buffer, out var charsWritten, timestamp))
		{
			buffer = buffer.Slice(0, charsWritten);
			size = measureEventArgs.MeasureText(buffer);
			return true;
		}
#endif
		size = measureEventArgs.MeasureText(GetText(column, timestamp));
		return true;
	}

	/// <inheritdoc/>
	public bool TryPaint(SubItemPaintEventArgs paintEventArgs)
	{
		Verify.Argument.IsNotNull(paintEventArgs);

		if(paintEventArgs.Column is not DateColumn column) return false;
		if(paintEventArgs.Item   is not T          item)   return false;

		if(!TryGetTimestamp(item, out var timestamp)) return false;

		if(!TryGetTextBrush(paintEventArgs, out var brush, out var disposeBrush))
		{
			brush = column.ContentBrush;
		}
		try
		{
#if NETCOREAPP
			Span<char> buffer = stackalloc char[CharBufferSize];
			if(TryGetText(column, buffer, out var charsWritten, timestamp))
			{
				buffer = buffer.Slice(0, charsWritten);
				paintEventArgs.PaintText(buffer);
				return true;
			}
#endif
			paintEventArgs.PaintText(GetText(column, timestamp), brush);
			return true;
		}
		finally
		{
			if(disposeBrush) brush.Dispose();
		}
	}
}
