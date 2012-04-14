namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Subitem with image and text.</summary>
	public class ImageAndTextSubItem : BaseImageAndTextSubItem
	{
		#region Data

		private string _text;
		private Image _image;
		private Image _overlayImage;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="ImageAndTextSubItem"/>.</summary>
		/// <param name="id">Subitem id.</param>
		/// <param name="image">Subitem image.</param>
		/// <param name="overlayImage">Overlay image.</param>
		/// <param name="text">Subitem text.</param>
		public ImageAndTextSubItem(int id, Image image, Image overlayImage, string text)
			: base(id)
		{
			_image = image;
			_overlayImage = overlayImage;
			_text = text;
		}

		/// <summary>Create <see cref="ImageAndTextSubItem"/>.</summary>
		/// <param name="id">Subitem id.</param>
		/// <param name="image">Subitem image.</param>
		/// <param name="text">Subitem text.</param>
		public ImageAndTextSubItem(int id, Image image, string text)
			: this(id, image, null, text)
		{
		}

		/// <summary>Create <see cref="ImageAndTextSubItem"/>.</summary>
		/// <param name="id">Subitem id.</param>
		/// <param name="image">Subitem image.</param>
		public ImageAndTextSubItem(int id, Image image)
			: this(id, image, null)
		{
		}

		/// <summary>Create <see cref="ImageAndTextSubItem"/>.</summary>
		/// <param name="id">Subitem id.</param>
		/// <param name="image">Subitem image.</param>
		public ImageAndTextSubItem(int id, string text)
			: this(id, null, text)
		{
		}

		/// <summary>Create <see cref="ImageAndTextSubItem"/>.</summary>
		/// <param name="id">Subitem id.</param>
		public ImageAndTextSubItem(int id)
			: this(id, null, null)
		{
		}

		#endregion

		/// <summary>Subitem text.</summary>
		public override string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				Invalidate();
			}
		}

		/// <summary>Subitem image.</summary>
		public override Image Image
		{
			get { return _image; }
			set
			{
				if(_image != value)
				{
					_image = value;
					Invalidate();
				}
			}
		}

		/// <summary>Subitem overlay image.</summary>
		public override Image OverlayImage
		{
			get { return _overlayImage; }
			set
			{
				if(_overlayImage != value)
				{
					_overlayImage = value;
					Invalidate();
				}
			}
		}
	}
}
