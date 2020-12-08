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

	/// <summary>SubItem paint event args.</summary>
	public class SubItemPaintEventArgs : ItemPaintEventArgs
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
		/// <param name="item">Item to paint.</param>
		public SubItemPaintEventArgs(
			Graphics graphics, Rectangle clipRectangle, Rectangle bounds, int index,
			ItemState state, int hoveredPart, bool hostControlFocused,
			CustomListBoxItem item, int columnIndex, CustomListBoxColumn column)
			: base(graphics, clipRectangle, bounds, index, state, hoveredPart, hostControlFocused)
		{
			Item        = item;
			ColumnIndex = columnIndex;
			Column      = column;
		}

		#endregion

		#region Properties

		public CustomListBoxItem Item { get; }

		/// <summary>Subitem column index.</summary>
		public int ColumnIndex { get; }

		/// <summary>Host listbox.</summary>
		public CustomListBox ListBox => Column.ListBox;

		/// <summary>Subitem column.</summary>
		public CustomListBoxColumn Column { get; }

		/// <summary>Column id.</summary>
		public int SubItemId => Column.Id;

		/// <summary>Font for painting subitem.</summary>
		public Font Font => Column.ContentFont;

		/// <summary>Text brush to use.</summary>
		public Brush Brush => Column.ContentBrush;

		/// <summary>Text horizontal alignment.</summary>
		public StringAlignment Alignment => Column.ContentAlignment;

		#endregion

		#region Methods

		/// <summary>Get <see cref="StringFormat"/> for <see cref="StringAlignment"/>.</summary>
		/// <param name="alignment">Text horizontal alignment</param>
		/// <returns></returns>
		protected static StringFormat GetFormat(StringAlignment alignment)
			=> alignment switch
			{
				StringAlignment.Near   => GitterApplication.TextRenderer.LeftAlign,
				StringAlignment.Far    => GitterApplication.TextRenderer.RightAlign,
				StringAlignment.Center => GitterApplication.TextRenderer.CenterAlign,
				_ => GitterApplication.TextRenderer.LeftAlign,
			};

		/// <summary>Prepare rectangle <paramref name="rect"/> for painting text by applying text content offsets.</summary>
		/// <param name="rect">Rectangle to prepare.</param>
		public void PrepareTextRectangle(ref Rectangle rect)
		{
			var h = GitterApplication.TextRenderer.GetFontHeight(Graphics, Font);
			var d = (int)((rect.Height - h) / 2.0f);
			rect.Y += d;
			rect.Height -= d;
		}

		/// <summary>Prepare rectangle <paramref name="rect"/> for painting text by applying text content offsets.</summary>
		/// <param name="font">Text font.</param>
		/// <param name="rect">Rectangle to prepare.</param>
		public void PrepareTextRectangle(Font font, ref Rectangle rect)
		{
			var h1 = GitterApplication.TextRenderer.GetFontHeight(Graphics, Column.ContentFont);
			var h = GitterApplication.TextRenderer.GetFontHeight(Graphics, font);
			var d = (int)((rect.Height - h1) / 2.0f + h1 - h);
			rect.Y += d;
			rect.Height -= d;
		}

		#region PaintImage()

		/// <summary>Paint image content.</summary>
		/// <param name="image"><see cref="Image"/> to paint.</param>
		public void PaintImage(Image image)
		{
			if(image != null)
			{
				var rect = Bounds;
				rect.X += ListBoxConstants.ContentSpacing;
				var graphics = Graphics;
				int w = image.Width;
				int h = image.Height;
				if(w + ListBoxConstants.SpaceBeforeImage > rect.Width)
					w = rect.Width - ListBoxConstants.SpaceBeforeImage;
				graphics.DrawImage(image,
					new Rectangle(rect.X + ListBoxConstants.SpaceBeforeImage, rect.Y + (rect.Height - h) / 2, w, h),
					new Rectangle(0, 0, w, h), GraphicsUnit.Pixel);
			}
		}

		/// <summary>Paint image with overlay content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Overlay to paint.</param>
		public void PaintImage(Image image, Image overlay)
		{
			if(image != null)
			{
				var rect = Bounds;
				rect.X += ListBoxConstants.ContentSpacing;
				var graphics = Graphics;
				int w = image.Width;
				int h = image.Height;
				if(w + ListBoxConstants.SpaceBeforeImage > rect.Width)
					w = rect.Width - ListBoxConstants.SpaceBeforeImage;
				var dest = new Rectangle(rect.X + ListBoxConstants.SpaceBeforeImage, rect.Y + (rect.Height - h) / 2, w, h);
				var src = new Rectangle(0, 0, w, h);
				graphics.DrawImage(image, dest, src, GraphicsUnit.Pixel);
				if(overlay != null)
					graphics.DrawImage(overlay, dest, src, GraphicsUnit.Pixel);
			}
		}

		#endregion

		#region PaintText()

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="font">Font to use.</param>
		/// <param name="brush">Brush to use.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use.</param>
		private void PaintTextCore(string text, Font font, Brush brush, StringFormat stringFormat)
		{
			var rect = Bounds;
			var graphics = Graphics;
			PrepareContentRectangle(ref rect);
			PrepareTextRectangle(font, ref rect);
			GitterApplication.TextRenderer.DrawText(
				graphics, text, font, brush, rect, stringFormat);
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use.</param>
		/// <param name="brush"><see cref="Brush"/> to use.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="font"/> == null or</para>
		///		<para><paramref name="brush"/> == null or</para>
		///		<para><paramref name="stringFormat"/> == null.</para>
		/// </exception>
		public void PaintText(string text, Font font, Brush brush, StringFormat stringFormat)
		{
			Verify.Argument.IsNotNull(font, nameof(font));
			Verify.Argument.IsNotNull(brush, nameof(brush));
			Verify.Argument.IsNotNull(stringFormat, nameof(stringFormat));

			PaintTextCore(text, font, brush, stringFormat);
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use.</param>
		/// <param name="brush"><see cref="Brush"/> to use.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="font"/> == null or</para>
		///		<para><paramref name="brush"/> == null.</para>
		/// </exception>
		public void PaintText(string text, Font font, Brush brush, StringAlignment stringAlignment)
		{
			Verify.Argument.IsNotNull(font, nameof(font));
			Verify.Argument.IsNotNull(brush, nameof(brush));

			PaintTextCore(text, font, brush, GetFormat(stringAlignment));
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use.</param>
		/// <param name="brush"><see cref="Brush"/> to use.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="font"/> == null or</para>
		///		<para><paramref name="brush"/> == null.</para>
		/// </exception>
		public void PaintText(string text, Font font, Brush brush)
		{
			Verify.Argument.IsNotNull(font, nameof(font));
			Verify.Argument.IsNotNull(brush, nameof(brush));

			PaintTextCore(text, font, brush, GetFormat(Column.ContentAlignment));
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="brush"><see cref="Brush"/> to use.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="brush"/> == null or</para>
		///		<para><paramref name="stringFormat"/> == null.</para>
		/// </exception>
		public void PaintText(string text, Brush brush, StringFormat stringFormat)
		{
			Verify.Argument.IsNotNull(brush, nameof(brush));
			Verify.Argument.IsNotNull(stringFormat, nameof(stringFormat));

			PaintTextCore(text, Column.ContentFont, brush, stringFormat);
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="brush"><see cref="Brush"/> to use.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="brush"/> == <c>null</c>.</exception>
		public void PaintText(string text, Brush brush, StringAlignment stringAlignment)
		{
			Verify.Argument.IsNotNull(brush, nameof(brush));

			PaintTextCore(text, Column.ContentFont, brush, GetFormat(stringAlignment));
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="brush"><see cref="Brush"/> to use.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="brush"/> == <c>null</c>.</exception>
		public void PaintText(string text, Brush brush)
		{
			Verify.Argument.IsNotNull(brush, nameof(brush));

			PaintTextCore(text, Column.ContentFont, brush, GetFormat(Column.ContentAlignment));
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="font">Font to use.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="font"/> == null or</para>
		///		<para><paramref name="stringFormat"/> == null.</para>
		/// </exception>
		public void PaintText(string text, Font font, StringFormat stringFormat)
		{
			Verify.Argument.IsNotNull(font, nameof(font));
			Verify.Argument.IsNotNull(stringFormat, nameof(stringFormat));

			PaintTextCore(text, font, Column.ContentBrush, stringFormat);
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="font"/> == <c>null</c>.</exception>
		public void PaintText(string text, Font font)
		{
			Verify.Argument.IsNotNull(font, nameof(font));

			PaintTextCore(text, font, Column.ContentBrush, GetFormat(Column.ContentAlignment));
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use.</param>
		public void PaintText(string text, StringFormat stringFormat)
		{
			Verify.Argument.IsNotNull(stringFormat, nameof(stringFormat));

			PaintTextCore(text, Column.ContentFont, Column.ContentBrush, stringFormat);
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use.</param>
		public void PaintText(string text, StringAlignment stringAlignment)
		{
			PaintTextCore(text, Column.ContentFont, Column.ContentBrush, GetFormat(stringAlignment));
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		public void PaintText(string text)
		{
			PaintTextCore(text, Column.ContentFont, Column.ContentBrush, GetFormat(Column.ContentAlignment));
		}

		#endregion

		#region PaintImageAndText()

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font">Font to use for text painting.</param>
		/// <param name="brush">Brush to use for text painting.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use for text painting.</param>
		private void PaintImageAndTextCore(Image image, string text, Font font, Brush brush, StringFormat stringFormat)
		{
			var rect = Bounds;
			var graphics = Graphics;
			PrepareContentRectangle(ref rect);
			if(image != null)
			{
				int w = image.Width;
				int h = image.Height;
				if(w + ListBoxConstants.SpaceBeforeImage > rect.Width)
					w = rect.Width - ListBoxConstants.SpaceBeforeImage;
				graphics.DrawImage(image,
					new Rectangle(rect.X + ListBoxConstants.SpaceBeforeImage, rect.Y + (rect.Height - h) / 2, w, h),
					new Rectangle(0, 0, w, h), GraphicsUnit.Pixel);
				rect.X     += w + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage;
				rect.Width -= w + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage;
			}
			else
			{
				rect.X     += ListBoxConstants.DefaultImageWidth + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage;
				rect.Width -= ListBoxConstants.DefaultImageWidth + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage;
			}
			if(rect.Width > 0)
			{
				PrepareTextRectangle(font, ref rect);
				GitterApplication.TextRenderer.DrawText(
					graphics, text, font, brush, rect, stringFormat);
			}
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <param name="brush"><see cref="Brush"/> to use for text painting.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="font"/> == null or</para>
		///		<para><paramref name="brush"/> == null or</para>
		///		<para><paramref name="stringFormat"/> == null.</para>
		/// </exception>
		public void PaintImageAndText(Image image, string text, Font font, Brush brush, StringFormat stringFormat)
		{
			Verify.Argument.IsNotNull(font, nameof(font));
			Verify.Argument.IsNotNull(brush, nameof(brush));
			Verify.Argument.IsNotNull(stringFormat, nameof(stringFormat));

			PaintImageAndTextCore(image, text, font, brush, stringFormat);
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <param name="brush"><see cref="Brush"/> to use for text painting.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use for text horizontal alignment.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="font"/> == null or</para>
		///		<para><paramref name="brush"/> == null.</para>
		/// </exception>
		public void PaintImageAndText(Image image, string text, Font font, Brush brush, StringAlignment stringAlignment)
		{
			Verify.Argument.IsNotNull(font, nameof(font));
			Verify.Argument.IsNotNull(brush, nameof(brush));

			PaintImageAndTextCore(image, text, font, brush, GetFormat(stringAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="font"/> == null or</para>
		///		<para><paramref name="stringFormat"/> == null.</para>
		/// </exception>
		public void PaintImageAndText(Image image, string text, Font font, StringFormat stringFormat)
		{
			Verify.Argument.IsNotNull(font, nameof(font));
			Verify.Argument.IsNotNull(stringFormat, nameof(stringFormat));

			PaintImageAndTextCore(image, text, font, Column.ContentBrush, stringFormat);
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use for text horizontal alignment.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="font"/> == <c>null</c>.</exception>
		public void PaintImageAndText(Image image, string text, Font font, StringAlignment stringAlignment)
		{
			Verify.Argument.IsNotNull(font, nameof(font));

			PaintImageAndTextCore(image, text, font, Column.ContentBrush, GetFormat(stringAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="font"/> == <c>null</c>.</exception>
		public void PaintImageAndText(Image image, string text, Font font)
		{
			Verify.Argument.IsNotNull(font, nameof(font));

			PaintImageAndTextCore(image, text, font, Column.ContentBrush, GetFormat(Column.ContentAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="brush"><see cref="Brush"/> to use for text painting.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="brush"/> == null or</para>
		///		<para><paramref name="stringFormat"/> == null.</para>
		/// </exception>
		public void PaintImageAndText(Image image, string text, Brush brush, StringFormat stringFormat)
		{
			Verify.Argument.IsNotNull(brush, nameof(brush));
			Verify.Argument.IsNotNull(stringFormat, nameof(stringFormat));

			PaintImageAndTextCore(image, text, Column.ContentFont, brush, stringFormat);
		}

		public void PaintImageAndText(Image image, string text, Brush brush, StringAlignment stringAlignment)
		{
			Verify.Argument.IsNotNull(brush, nameof(brush));

			PaintImageAndTextCore(image, text, Column.ContentFont, brush, GetFormat(stringAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="brush"><see cref="Brush"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="brush"/> == <c>null</c>.</exception>
		public void PaintImageAndText(Image image, string text, Brush brush)
		{
			Verify.Argument.IsNotNull(brush, nameof(brush));

			PaintImageAndTextCore(image, text, Column.ContentFont, brush, GetFormat(Column.ContentAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="stringFormat"/> == <c>null</c>.</exception>
		public void PaintImageAndText(Image image, string text, StringFormat stringFormat)
		{
			Verify.Argument.IsNotNull(stringFormat, nameof(stringFormat));

			PaintImageAndTextCore(image, text, Column.ContentFont, Column.ContentBrush, stringFormat);
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use for text horizontal alignment.</param>
		public void PaintImageAndText(Image image, string text, StringAlignment stringAlignment)
		{
			PaintImageAndTextCore(image, text, Column.ContentFont, Column.ContentBrush, GetFormat(stringAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		public void PaintImageAndText(Image image, string text)
		{
			PaintImageAndTextCore(image, text, Column.ContentFont, Column.ContentBrush, GetFormat(Column.ContentAlignment));
		}

		#endregion

		#region PaintImageOverlayAndText()

		/// <summary>Paint <paramref name="icon"/> and <paramref name="text"/> content.</summary>
		/// <param name="icon">Image to paint.</param>
		/// <param name="overlay">Image overlay.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <param name="brush"><see cref="Brush"/> to use for text painting.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use for text painting.</param>
		private void PaintImageOverlayAndTextCore(Image icon, Image overlay, string text, Font font, Brush brush, StringFormat stringFormat)
		{
			var rect = Bounds;
			var graphics = Graphics;
			PrepareContentRectangle(ref rect);
			if(icon != null)
			{
				int w = icon.Width;
				int h = icon.Height;
				if(w + ListBoxConstants.SpaceBeforeImage > rect.Width)
					w = rect.Width - ListBoxConstants.SpaceBeforeImage;
				var destRect = new Rectangle(rect.X + ListBoxConstants.SpaceBeforeImage, rect.Y + (rect.Height - w) / 2, w, h);
				var srcRect = new Rectangle(0, 0, w, h);
				graphics.DrawImage(icon, destRect, srcRect, GraphicsUnit.Pixel);
				if(overlay != null)
					graphics.DrawImage(overlay, destRect, srcRect, GraphicsUnit.Pixel);
				rect.X += w + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage;
				rect.Width -= w + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage;
			}
			else
			{
				rect.X += ListBoxConstants.DefaultImageWidth + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage;
				rect.Width -= ListBoxConstants.DefaultImageWidth + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage;
			}
			if(rect.Width > 0)
			{
				PrepareTextRectangle(font, ref rect);
				GitterApplication.TextRenderer.DrawText(
					graphics, text, font, brush, rect, stringFormat);
			}
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <param name="brush"><see cref="Brush"/> to use for text painting.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="font"/> == null or</para>
		///		<para><paramref name="brush"/> == null or</para>
		///		<para><paramref name="stringFormat"/> == null.</para>
		/// </exception>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text, Font font, Brush brush, StringFormat stringFormat)
		{
			Verify.Argument.IsNotNull(font, nameof(font));
			Verify.Argument.IsNotNull(brush, nameof(brush));
			Verify.Argument.IsNotNull(stringFormat, nameof(stringFormat));

			PaintImageOverlayAndTextCore(image, overlay, text, font, brush, stringFormat);
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <param name="brush"><see cref="Brush"/> to use for text painting.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use for text horizontal alignment.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="font"/> == null or</para>
		///		<para><paramref name="brush"/> == null.</para>
		/// </exception>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text, Font font, Brush brush, StringAlignment stringAlignment)
		{
			Verify.Argument.IsNotNull(font, nameof(font));
			Verify.Argument.IsNotNull(brush, nameof(brush));

			PaintImageOverlayAndTextCore(image, overlay, text, font, brush, GetFormat(stringAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="brush"><see cref="Brush"/> to use for text painting.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="brush"/> == null or</para>
		///		<para><paramref name="stringFormat"/> == null.</para>
		/// </exception>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text, Brush brush, StringFormat stringFormat)
		{
			Verify.Argument.IsNotNull(brush, nameof(brush));
			Verify.Argument.IsNotNull(stringFormat, nameof(stringFormat));

			PaintImageOverlayAndTextCore(image, overlay, text, Column.ContentFont, brush, stringFormat);
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="brush"><see cref="Brush"/> to use for text painting.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use for text horizontal alignment.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="brush"/> == <c>null</c>.</exception>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text, Brush brush, StringAlignment stringAlignment)
		{
			Verify.Argument.IsNotNull(brush, nameof(brush));

			PaintImageOverlayAndTextCore(image, overlay, text, Column.ContentFont, brush, GetFormat(stringAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="font"/> == null or</para>
		///		<para><paramref name="stringFormat"/> == null.</para>
		/// </exception>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text, Font font, StringFormat stringFormat)
		{
			Verify.Argument.IsNotNull(font, nameof(font));
			Verify.Argument.IsNotNull(stringFormat, nameof(stringFormat));

			PaintImageOverlayAndTextCore(image, overlay, text, font, Column.ContentBrush, stringFormat);
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use for text horizontal alignment.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="font"/> == <c>null</c>.</exception>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text, Font font, StringAlignment stringAlignment)
		{
			Verify.Argument.IsNotNull(font, nameof(font));

			PaintImageOverlayAndTextCore(image, overlay, text, font, Column.ContentBrush, GetFormat(stringAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="stringFormat"/> == <c>null</c>.</exception>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text, StringFormat stringFormat)
		{
			Verify.Argument.IsNotNull(stringFormat, nameof(stringFormat));

			PaintImageOverlayAndTextCore(image, overlay, text, Column.ContentFont, Column.ContentBrush, stringFormat);
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use for text horizontal alignment.</param>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text, StringAlignment stringAlignment)
		{
			PaintImageOverlayAndTextCore(image, overlay, text, Column.ContentFont, Column.ContentBrush, GetFormat(stringAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <param name="brush"><see cref="Brush"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///		<para><paramref name="font"/> == null or</para>
		///		<para><paramref name="brush"/> == null.</para>
		/// </exception>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text, Font font, Brush brush)
		{
			Verify.Argument.IsNotNull(font, nameof(font));
			Verify.Argument.IsNotNull(brush, nameof(brush));

			PaintImageOverlayAndTextCore(image, overlay, text, font, brush, GetFormat(Column.ContentAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="font"/> == <c>null</c>.</exception>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text, Font font)
		{
			Verify.Argument.IsNotNull(font, nameof(font));

			PaintImageOverlayAndTextCore(image, overlay, text, font, Column.ContentBrush, GetFormat(Column.ContentAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text)
		{
			PaintImageOverlayAndTextCore(image, overlay, text, Column.ContentFont, Column.ContentBrush, GetFormat(Column.ContentAlignment));
		}

		#endregion

		#endregion
	}
}
