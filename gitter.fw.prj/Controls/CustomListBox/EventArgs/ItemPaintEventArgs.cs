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

namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Item paint event args.</summary>
	public class ItemPaintEventArgs : EventArgs
	{
		#region .ctor

		/// <summary>Create <see cref="ItemPaintEventArgs"/>.</summary>
		/// <param name="graphics">Graphics surface to draw the item on.</param>
		/// <param name="clipRectangle">Clipping rectangle.</param>
		/// <param name="bounds">Rectangle that represents the bounds of the item that is being drawn.</param>
		/// <param name="index">Index value of the item that is being drawn.</param>
		/// <param name="state">State of the item being drawn.</param>
		/// <param name="hoveredPart">Hovered part of the item.</param>
		/// <param name="hostControlFocused">Host control is focused.</param>
		public ItemPaintEventArgs(
			Graphics graphics, Rectangle clipRectangle, Rectangle bounds, int index,
			ItemState state, int hoveredPart, bool hostControlFocused)
		{
			Graphics             = graphics;
			ClipRectangle        = clipRectangle;
			Bounds               = bounds;
			Index                = index;
			State                = state;
			HoveredPart          = hoveredPart;
			IsHostControlFocused = hostControlFocused;
		}

		#endregion

		#region Properties

		/// <summary>Gets the graphics surface to draw the item on.</summary>
		public Graphics Graphics { get; }

		/// <summary>Clipping rectangle.</summary>
		public Rectangle ClipRectangle { get; }

		/// <summary>Gets the rectangle that represents the bounds of the item that is being drawn.</summary>
		public Rectangle Bounds { get; }

		/// <summary>Gets the index value of the item that is being drawn.</summary>
		public int Index { get; }

		/// <summary>Gets the state of the item being drawn.</summary>
		public ItemState State { get; }

		/// <summary>Hovered part of the item.</summary>
		public int HoveredPart { get; }

		/// <summary>Host control is focused.</summary>
		public bool IsHostControlFocused { get; }

		#endregion

		#region Methods

		/// <summary>Prepare rectangle <paramref name="rect"/> for painting by applying content offsets.</summary>
		/// <param name="rect">Rectangle to prepare.</param>
		public void PrepareContentRectangle(ref Rectangle rect)
		{
			var conv    = new DpiConverter(Graphics);
			var spacing = conv.ConvertX(ListBoxConstants.ContentSpacing);

			rect.X     += spacing;
			rect.Width -= spacing * 2;
		}

		/// <summary>Prepare rectangle <paramref name="rect"/> for painting text by applying text offsets.</summary>
		/// <param name="listBoxFont">Font of hosing <see cref="CustomListBox"/>.</param>
		/// <param name="itemFont">Text font.</param>
		/// <param name="rect">Rectangle to prepare.</param>
		public void PrepareTextRectangle(Font listBoxFont, Font itemFont, ref Rectangle rect)
		{
			if(listBoxFont == itemFont)
			{
				var h = GitterApplication.TextRenderer.GetFontHeight(Graphics, listBoxFont);
				var d = (int)((rect.Height - h) / 2.0f);
				rect.Y += d;
				rect.Height -= d;
			}
			else
			{
				var h1 = GitterApplication.TextRenderer.GetFontHeight(Graphics, listBoxFont);
				var h  = GitterApplication.TextRenderer.GetFontHeight(Graphics, itemFont);
				var d  = (int)((rect.Height - h1) / 2.0f + h1 - h);
				rect.Y      += d;
				rect.Height -= d;
			}
		}

		#endregion
	}
}
