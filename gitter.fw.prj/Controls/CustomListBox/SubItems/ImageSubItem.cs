namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Subitem with image content.</summary>
	public class ImageSubItem : BaseImageSubItem
	{
		#region Data

		private Image _image;
		private Image _overlayImage;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="ImageSubItem"/>.</summary>
		/// <param name="id">Subitem id.</param>
		/// <param name="image">Subitem image.</param>
		/// <param name="overlayImage">Subitem overlay image.</param>
		public ImageSubItem(int id, Image image, Image overlayImage)
			: base(id)
		{
			_image = image;
			_overlayImage = overlayImage;
		}

		/// <summary>Create <see cref="ImageSubItem"/>.</summary>
		/// <param name="id">Subitem id.</param>
		/// <param name="image">Subitem image.</param>
		public ImageSubItem(int id, Image image)
			: this(id, image, null)
		{
		}

		/// <summary>Create <see cref="ImageSubItem"/>.</summary>
		/// <param name="id">Subitem id.</param>
		public ImageSubItem(int id)
			: this(id, null, null)
		{
		}

		#endregion

		#region Properties

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
				if(_overlayImage != null)
				{
					_overlayImage = value;
					Invalidate();
				}
			}
		}

		#endregion
	}
}
