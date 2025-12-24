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

using System.Diagnostics.CodeAnalysis;
using System.Drawing;

using gitter.Framework.Controls;

abstract class EmailPainterBase<T> : ISubItemPainter
{
	protected virtual bool TryGetTextBrush(SubItemPaintEventArgs paintEventArgs,
		[MaybeNullWhen(returnValue: false)] out Brush textBrush,
		[MaybeNullWhen(returnValue: false)] out bool disposeBrush)
	{
		textBrush    = default;
		disposeBrush = false;
		return false;
	}

	protected abstract string? GetEmail(T item);

	public bool TryMeasure(SubItemMeasureEventArgs measureEventArgs, out Size size)
	{
		Verify.Argument.IsNotNull(measureEventArgs);

		if(measureEventArgs.Item is not T item)
		{
			size = Size.Empty;
			return false;
		}

		var email = GetEmail(item);
		size = measureEventArgs.MeasureText(email);
		return true;
	}

	public bool TryPaint(SubItemPaintEventArgs paintEventArgs)
	{
		Verify.Argument.IsNotNull(paintEventArgs);

		if(paintEventArgs.Item is not T item) return false;

		if(GetEmail(item) is { Length: not 0 } email)
		{
			if(!TryGetTextBrush(paintEventArgs, out var brush, out var disposeBrush))
			{
				brush = paintEventArgs.Brush;
			}
			try
			{
				paintEventArgs.PaintText(email, brush);
			}
			finally
			{
				if(disposeBrush) brush.Dispose();
			}
		}
		return true;
	}
}
