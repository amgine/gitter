namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>SubItem paint event args.</summary>
	public class SubItemPaintEventArgs : ItemPaintEventArgs
	{
		#region Data

		private readonly int _columnIndex;
		private readonly CustomListBoxColumn _column;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="ItemPaintEventArgs"/>.</summary>
		/// <param name="graphics">Graphics surface to draw the item on.</param>
		/// <param name="clipRectangle">Clipping rectangle.</param>
		/// <param name="bounds">Rectangle that represents the bounds of the item that is being drawn.</param>
		/// <param name="index">Index value of the item that is being drawn.</param>
		/// <param name="state">State of the item being drawn.</param>
		/// <param name="hoveredPart">Hovered part of the item.</param>
		/// <param name="hostControlFocused">Host control is focused.</param>
		public SubItemPaintEventArgs(
			Graphics graphics, Rectangle clipRectangle, Rectangle bounds, int index,
			ItemState state, int hoveredPart, bool hostControlFocused,
			int columnIndex, CustomListBoxColumn column)
			: base(graphics, clipRectangle, bounds, index, state, hoveredPart, hostControlFocused)
		{
			_columnIndex = columnIndex;
			_column = column;
		}

		#endregion

		#region Properties

		/// <summary>Subitem column index.</summary>
		public int ColumnIndex
		{
			get { return _columnIndex; }
		}

		/// <summary>Host listbox.</summary>
		public CustomListBox ListBox
		{
			get { return _column.ListBox; }
		}

		/// <summary>Subitem column.</summary>
		public CustomListBoxColumn Column
		{
			get { return _column; }
		}

		/// <summary>Column id.</summary>
		public int SubItemId
		{
			get { return _column.Id; }
		}

		/// <summary>Font for painting subitem.</summary>
		public Font Font
		{
			get { return _column.ContentFont; }
		}

		/// <summary>Text brush to use.</summary>
		public Brush Brush
		{
			get { return _column.ContentBrush; }
		}

		/// <summary>Text horizontal alignment.</summary>
		public StringAlignment Alignment
		{
			get { return _column.ContentAlignment; }
		}

		#endregion

		#region Methods

		/// <summary>Get <see cref="StringFormat"/> for <see cref="StringAlignment"/>.</summary>
		/// <param name="alignment">Text horizontal alignment</param>
		/// <returns></returns>
		protected static StringFormat GetFormat(StringAlignment alignment)
		{
			switch(alignment)
			{
				case StringAlignment.Near:
					return GitterApplication.TextRenderer.LeftAlign;
				case StringAlignment.Far:
					return GitterApplication.TextRenderer.RightAlign;
				case StringAlignment.Center:
					return GitterApplication.TextRenderer.CenterAlign;
				default:
					return GitterApplication.TextRenderer.LeftAlign;
			}
		}

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
			var h1 = GitterApplication.TextRenderer.GetFontHeight(Graphics, _column.ContentFont);
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
			if(font == null) throw new ArgumentNullException("font");
			if(brush == null) throw new ArgumentNullException("brush");
			if(stringFormat == null) throw new ArgumentNullException("stringFormat");

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
			if(font == null) throw new ArgumentNullException("font");
			if(brush == null) throw new ArgumentNullException("brush");

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
			if(font == null) throw new ArgumentNullException("font");
			if(brush == null) throw new ArgumentNullException("brush");

			PaintTextCore(text, font, brush, GetFormat(_column.ContentAlignment));
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
			if(brush == null) throw new ArgumentNullException("brush");
			if(stringFormat == null) throw new ArgumentNullException("stringFormat");

			PaintTextCore(text, _column.ContentFont, brush, stringFormat);
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="brush"><see cref="Brush"/> to use.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="brush"/> == <c>null</c>.</exception>
		public void PaintText(string text, Brush brush, StringAlignment stringAlignment)
		{
			if(brush == null) throw new ArgumentNullException("brush");

			PaintTextCore(text, _column.ContentFont, brush, GetFormat(stringAlignment));
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="brush"><see cref="Brush"/> to use.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="brush"/> == <c>null</c>.</exception>
		public void PaintText(string text, Brush brush)
		{
			if(brush == null) throw new ArgumentNullException("brush");

			PaintTextCore(text, _column.ContentFont, brush, GetFormat(_column.ContentAlignment));
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
			if(font == null) throw new ArgumentNullException("font");
			if(stringFormat == null) throw new ArgumentNullException("stringFormat");

			PaintTextCore(text, font, _column.ContentBrush, stringFormat);
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="font"/> == <c>null</c>.</exception>
		public void PaintText(string text, Font font)
		{
			if(font == null) throw new ArgumentNullException("font");

			PaintTextCore(text, font, _column.ContentBrush, GetFormat(_column.ContentAlignment));
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use.</param>
		public void PaintText(string text, StringFormat stringFormat)
		{
			if(stringFormat == null) throw new ArgumentNullException("stringFormat");

			PaintTextCore(text, _column.ContentFont, _column.ContentBrush, stringFormat);
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use.</param>
		public void PaintText(string text, StringAlignment stringAlignment)
		{
			PaintTextCore(text, _column.ContentFont, _column.ContentBrush, GetFormat(stringAlignment));
		}

		/// <summary>Paint text content.</summary>
		/// <param name="text">Text to paint.</param>
		public void PaintText(string text)
		{
			PaintTextCore(text, _column.ContentFont, _column.ContentBrush, GetFormat(_column.ContentAlignment));
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
			if(font == null) throw new ArgumentNullException("font");
			if(brush == null) throw new ArgumentNullException("brush");
			if(stringFormat == null) throw new ArgumentNullException("stringFormat");

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
			if(font == null) throw new ArgumentNullException("font");
			if(brush == null) throw new ArgumentNullException("brush");

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
			if(font == null) throw new ArgumentNullException("font");
			if(stringFormat == null) throw new ArgumentNullException("stringFormat");

			PaintImageAndTextCore(image, text, font, _column.ContentBrush, stringFormat);
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use for text horizontal alignment.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="font"/> == <c>null</c>.</exception>
		public void PaintImageAndText(Image image, string text, Font font, StringAlignment stringAlignment)
		{
			if(font == null) throw new ArgumentNullException("font");

			PaintImageAndTextCore(image, text, font, _column.ContentBrush, GetFormat(stringAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="font"/> == <c>null</c>.</exception>
		public void PaintImageAndText(Image image, string text, Font font)
		{
			if(font == null) throw new ArgumentNullException("font");

			PaintImageAndTextCore(image, text, font, _column.ContentBrush, GetFormat(_column.ContentAlignment));
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
			if(brush == null) throw new ArgumentNullException("brush");
			if(stringFormat == null) throw new ArgumentNullException("stringFormat");

			PaintImageAndTextCore(image, text, _column.ContentFont, brush, stringFormat);
		}

		public void PaintImageAndText(Image image, string text, Brush brush, StringAlignment stringAlignment)
		{
			if(brush == null) throw new ArgumentNullException("brush");

			PaintImageAndTextCore(image, text, _column.ContentFont, brush, GetFormat(stringAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="brush"><see cref="Brush"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="brush"/> == <c>null</c>.</exception>
		public void PaintImageAndText(Image image, string text, Brush brush)
		{
			if(brush == null) throw new ArgumentNullException("brush");

			PaintImageAndTextCore(image, text, _column.ContentFont, brush, GetFormat(_column.ContentAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="stringFormat"/> == <c>null</c>.</exception>
		public void PaintImageAndText(Image image, string text, StringFormat stringFormat)
		{
			if(stringFormat == null) throw new ArgumentNullException("stringFormat");

			PaintImageAndTextCore(image, text, _column.ContentFont, _column.ContentBrush, stringFormat);
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use for text horizontal alignment.</param>
		public void PaintImageAndText(Image image, string text, StringAlignment stringAlignment)
		{
			PaintImageAndTextCore(image, text, _column.ContentFont, _column.ContentBrush, GetFormat(stringAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="text">Text to paint.</param>
		public void PaintImageAndText(Image image, string text)
		{
			PaintImageAndTextCore(image, text, _column.ContentFont, _column.ContentBrush, GetFormat(_column.ContentAlignment));
		}

		#endregion

		#region PaintImageOverlayAndText()

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
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
			if(font == null) throw new ArgumentNullException("font");
			if(brush == null) throw new ArgumentNullException("brush");
			if(stringFormat == null) throw new ArgumentNullException("stringFormat");

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
			if(font == null) throw new ArgumentNullException("font");
			if(brush == null) throw new ArgumentNullException("brush");

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
			if(brush == null) throw new ArgumentNullException("brush");
			if(stringFormat == null) throw new ArgumentNullException("stringFormat");

			PaintImageOverlayAndTextCore(image, overlay, text, _column.ContentFont, brush, stringFormat);
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
			if(brush == null) throw new ArgumentNullException("brush");

			PaintImageOverlayAndTextCore(image, overlay, text, _column.ContentFont, brush, GetFormat(stringAlignment));
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
			if(font == null) throw new ArgumentNullException("font");
			if(stringFormat == null) throw new ArgumentNullException("stringFormat");

			PaintImageOverlayAndTextCore(image, overlay, text, font, _column.ContentBrush, stringFormat);
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
			if(font == null) throw new ArgumentNullException("font");

			PaintImageOverlayAndTextCore(image, overlay, text, font, _column.ContentBrush, GetFormat(stringAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="stringFormat"><see cref="StringFormat"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="stringFormat"/> == null.</para>
		/// </exception>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text, StringFormat stringFormat)
		{
			if(stringFormat == null) throw new ArgumentNullException("stringFormat");

			PaintImageOverlayAndTextCore(image, overlay, text, _column.ContentFont, _column.ContentBrush, stringFormat);
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="stringAlignment"><see cref="StringAlignment"/> to use for text horizontal alignment.</param>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text, StringAlignment stringAlignment)
		{
			PaintImageOverlayAndTextCore(image, overlay, text, _column.ContentFont, _column.ContentBrush, GetFormat(stringAlignment));
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
			if(font == null) throw new ArgumentNullException("font");
			if(brush == null) throw new ArgumentNullException("brush");

			PaintImageOverlayAndTextCore(image, overlay, text, font, brush, GetFormat(_column.ContentAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		/// <param name="font"><see cref="Font"/> to use for text painting.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="font"/> == <c>null</c>.</exception>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text, Font font)
		{
			if(font == null) throw new ArgumentNullException("font");

			PaintImageOverlayAndTextCore(image, overlay, text, font, _column.ContentBrush, GetFormat(_column.ContentAlignment));
		}

		/// <summary>Paint <paramref name="image"/> and <paramref name="text"/> content.</summary>
		/// <param name="image">Image to paint.</param>
		/// <param name="overlay">Image overlay to paint.</param>
		/// <param name="text">Text to paint.</param>
		public void PaintImageOverlayAndText(Image image, Image overlay, string text)
		{
			PaintImageOverlayAndTextCore(image, overlay, text, _column.ContentFont, _column.ContentBrush, GetFormat(_column.ContentAlignment));
		}

		#endregion

		#endregion
	}
}
