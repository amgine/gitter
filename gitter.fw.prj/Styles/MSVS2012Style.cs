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

namespace gitter.Framework;

using System;
using System.Drawing;

using gitter.Framework.Controls;

abstract class MSVS2012Style
{
	protected sealed class BackgroundWithBorder : IBackgroundStyle
	{
		private readonly Color _backgroundColor;
		private readonly Color _borderColor;

		public BackgroundWithBorder(Color backgroundColor, Color borderColor)
		{
			_backgroundColor = backgroundColor;
			_borderColor     = borderColor;
		}

		public void Draw(Graphics graphics, Dpi dpi, Rectangle rect)
		{
			graphics.GdiFill(_backgroundColor, rect);
			using(var pen = new Pen(_borderColor))
			{
				rect.Width  -= 1;
				rect.Height -= 1;
				graphics.DrawRectangle(pen, rect);
			}
		}
	}

	protected sealed class SolidBackground : IBackgroundStyle
	{
		private readonly Color _backgroundColor;

		public SolidBackground(Color backgroundColor)
		{
			_backgroundColor = backgroundColor;
		}

		public void Draw(Graphics graphics, Dpi dpi, Rectangle rect)
		{
			graphics.GdiFill(_backgroundColor, rect);
		}
	}
}
