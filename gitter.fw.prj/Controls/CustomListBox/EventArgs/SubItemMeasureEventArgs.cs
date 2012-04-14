namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public class SubItemMeasureEventArgs : EventArgs
	{
		#region Data

		private readonly Graphics _graphics;
		private readonly int _columnIndex;
		private readonly CustomListBoxColumn _column;

		#endregion

		#region .ctor

		public SubItemMeasureEventArgs(Graphics graphics, int columnIndex, CustomListBoxColumn column)
		{
			_graphics = graphics;
			_columnIndex = columnIndex;
			_column = column;
		}

		#endregion

		#region Properties

		public Graphics Gaphics
		{
			get { return _graphics; }
		}

		public int SubItemId
		{
			get { return _column.Id; }
		}

		public CustomListBox ListBox
		{
			get { return _column.ListBox; }
		}

		public CustomListBoxColumn Column
		{
			get { return _column; }
		}

		public Font Font
		{
			get { return _column.ContentFont; }
		}

		public int ColumnIndex
		{
			get { return _columnIndex; }
		}

		#endregion

		#region Methods

		public Size MeasureImage(Image image)
		{
			int w = 2 * ListBoxConstants.ContentSpacing + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage;
			int h = 0;
			if(image != null)
			{
				w += image.Width;
				h += image.Height;
			}
			else
			{
				w += ListBoxConstants.DefaultImageWidth;
				h += ListBoxConstants.DefaultImageHeight;
			}
			return new Size(w, h);
		}

		public Size MeasureIcon(Icon icon)
		{
			int w = 2 * ListBoxConstants.ContentSpacing + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage;
			int h = 0;
			if(icon != null)
			{
				w += icon.Width;
				h += icon.Height;
			}
			else
			{
				w += ListBoxConstants.DefaultImageWidth;
				h += ListBoxConstants.DefaultImageHeight;
			}
			return new Size(w, h);
		}

		#region MeasureText()

		private Size MeasureTextCore(string text, Font font)
		{
			var s = GitterApplication.TextRenderer.MeasureText(
				_graphics, text, font, int.MaxValue);

			return new Size((int)(s.Width + 1 + 2 * ListBoxConstants.ContentSpacing), s.Height);
		}

		public Size MeasureText(string text, Font font)
		{
			if(font == null) throw new ArgumentNullException("font");

			return MeasureTextCore(text, font);
		}

		public Size MeasureText(string text)
		{
			return MeasureTextCore(text, _column.ContentFont);
		}

		#endregion

		#region MeasureImageAndText()

		private Size MeasureImageAndTextCore(Image image, string text, Font font)
		{
			var s = GitterApplication.TextRenderer.MeasureText(
				_graphics, text, font, int.MaxValue, Utility.DefaultStringFormat);
			var iconW = (image != null) ? (image.Width) : (ListBoxConstants.DefaultImageWidth);
			return new Size(s.Width + 1 + 2 * ListBoxConstants.ContentSpacing + (iconW + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage), s.Height);
		}

		public Size MeasureImageAndText(Image image, string text, Font font)
		{
			if(font == null) throw new ArgumentNullException("font");

			return MeasureImageAndTextCore(image, text, font);
		}

		public Size MeasureImageAndText(Image image, string text)
		{
			return MeasureImageAndTextCore(image, text, _column.ContentFont);
		}

		#endregion

		#region MeasureIconAndText()

		private Size MeasureIconAndTextCore(Icon icon, string text, Font font)
		{
			var s = GitterApplication.TextRenderer.MeasureText(
				_graphics, text, font, int.MaxValue, Utility.DefaultStringFormat);
			var iconW = (icon != null) ? (icon.Width) : (ListBoxConstants.DefaultImageWidth);
			return new Size(s.Width + 1 + 2 * ListBoxConstants.ContentSpacing + (iconW + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage), s.Height);
		}

		public Size MeasureIconAndText(Icon icon, string text, Font font)
		{
			if(font == null) throw new ArgumentNullException("font");

			return MeasureIconAndTextCore(icon, text, font);
		}

		public Size MeasureIconAndText(Icon icon, string text)
		{
			return MeasureIconAndTextCore(icon, text, _column.ContentFont);
		}

		#endregion

		#endregion
	}
}
