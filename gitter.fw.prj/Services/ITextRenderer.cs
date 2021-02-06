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

namespace gitter.Framework.Services
{
	using System.Drawing;

	public interface ITextRenderer
	{
		StringFormat LeftAlign { get; }

		StringFormat RightAlign { get; }

		StringFormat CenterAlign { get; }

		void DrawText(Graphics graphics, string text, Font font, Brush brush, Rectangle layoutRectangle, StringFormat format);

		void DrawText(Graphics graphics, string text, Font font, Brush brush, Point point, StringFormat format);

		void DrawText(Graphics graphics, string text, Font font, Brush brush, int x, int y, StringFormat format);

		void DrawText(Graphics graphics, string text, Font font, Brush brush, Rectangle layoutRectangle);

		void DrawText(Graphics graphics, string text, Font font, Brush brush, Point point);

		void DrawText(Graphics graphics, string text, Font font, Brush brush, int x, int y);

		void DrawText(Graphics graphics, string text, Font font, Color color, Rectangle layoutRectangle, StringFormat format);

		void DrawText(Graphics graphics, string text, Font font, Color color, Point point, StringFormat format);

		void DrawText(Graphics graphics, string text, Font font, Color color, int x, int y, StringFormat format);

		void DrawText(Graphics graphics, string text, Font font, Color color, Rectangle layoutRectangle);

		void DrawText(Graphics graphics, string text, Font font, Color color, Point point);

		void DrawText(Graphics graphics, string text, Font font, Color color, int x, int y);

		Size MeasureText(Graphics graphics, string text, Font font, Size layoutArea, StringFormat format);

		Size MeasureText(Graphics graphics, string text, Font font, int width, StringFormat format);

		Size MeasureText(Graphics graphics, string text, Font font, Size layoutArea);

		Size MeasureText(Graphics graphics, string text, Font font, int width);

		float GetFontHeight(Font font);

		float GetFontHeight(Graphics graphics, Font font);
	}
}
