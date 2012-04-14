namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	using Resources = gitter.Framework.Properties.Resources;

	sealed class ViewButton
	{
		#region Static Data

		private static readonly Bitmap ImgMenu = Resources.ImgToolHostMenu;
		private static readonly Bitmap ImgNormalize = Resources.ImgNormalize;
		private static readonly Bitmap ImgMaximize = Resources.ImgMaximize;
		private static readonly Bitmap ImgPin = Resources.ImgToolHostPin;
		private static readonly Bitmap ImgClose = Resources.ImgToolHostClose;
		private static readonly Bitmap ImgScrollLeft = Resources.ImgTabScrollLeft;
		private static readonly Bitmap ImgScrollRight = Resources.ImgTabScrollRight;
		private static readonly Bitmap ImgTabMenu = Resources.ImgTabMenu;
		private static readonly Bitmap ImgTabMenuExt = Resources.ImgTabMenuExtends;

		#endregion

		#region Data

		private int _offset;
		private ViewButtonType _type;
		private Image _image;

		#endregion

		public ViewButton(int offset, ViewButtonType type)
		{
			_offset = offset;
			_type = type;
			switch(_type)
			{
				case ViewButtonType.Menu:
					_image = ImgMenu;
					break;
				case ViewButtonType.Pin:
					_image = ImgPin;
					break;
				case ViewButtonType.Unpin:
					_image = ImgPin;
					break;
				case ViewButtonType.Normalize:
					_image = ImgNormalize;
					break;
				case ViewButtonType.Maximize:
					_image = ImgMaximize;
					break;
				case ViewButtonType.Close:
					_image = ImgClose;
					break;
				case ViewButtonType.ScrollTabsLeft:
					_image = ImgScrollLeft;
					break;
				case ViewButtonType.ScrollTabsRight:
					_image = ImgScrollRight;
					break;
				case ViewButtonType.TabsMenu:
					_image = ImgTabMenu;
					break;
				case ViewButtonType.TabsScrollMenu:
					_image = ImgTabMenuExt;
					break;
				default:
					_image = null;
					break;
			}
		}

		#region Properties

		public int Offset
		{
			get { return _offset; }
		}

		public ViewButtonType Type
		{
			get { return _type; }
		}

		public Image Image
		{
			get { return _image; }
		}

		#endregion

		public void OnPaint(Graphics graphics, Rectangle rect, bool focus, bool hover, bool pressed)
		{
			if(hover || pressed)
			{
				var rc = rect;
				rc.Width -= 1;
				rc.Height -= 1;
				Color border;
				Color background;
				if(pressed)
				{
					border = Color.FromArgb(229, 195, 101);
					background = Color.FromArgb(255, 232, 166);
				}
				else
				{
					border = Color.FromArgb(229, 195, 101);
					background = Color.FromArgb(255, 252, 244);
				}
				using(var brush = new SolidBrush(background))
				{
					graphics.FillRectangle(brush, rc);
				}
				using(var pen = new Pen(border))
				{
					graphics.DrawRectangle(pen, rc);
				}
			}
			if(_image != null)
				graphics.DrawImage(_image, rect);
		}

		public override string ToString()
		{
			return _type.ToString();
		}
	}
}
