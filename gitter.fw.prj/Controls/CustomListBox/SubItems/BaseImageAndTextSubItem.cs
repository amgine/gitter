namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Base class for subitems with image and text.</summary>
	public abstract class BaseImageAndTextSubItem : BaseTextSubItem
	{
		/// <summary>Create <see cref="BaseImageAndTextSubItem"/>.</summary>
		/// <param name="id">Subitem id.</param>
		protected BaseImageAndTextSubItem(int id)
			: base(id)
		{
		}

		/// <summary>Image.</summary>
		public abstract Image Image { get; set; }

		/// <summary>Overlay image.</summary>
		public virtual Image OverlayImage { get { return null; } set { } }

		/// <summary>Paint event handler.</summary>
		/// <param name="paintEventArgs">Paint event args.</param>
		protected override void OnPaint(SubItemPaintEventArgs paintEventArgs)
		{
			paintEventArgs.PaintImageOverlayAndText(Image, OverlayImage, Text,
				Font ?? paintEventArgs.Font,
				TextBrush ?? paintEventArgs.Brush,
				TextAlignment ?? paintEventArgs.Alignment);
		}

		/// <summary>Measure event handler.</summary>
		/// <param name="measureEventArgs">Measure event args.</param>
		/// <returns>Subitem content size.</returns>
		protected override Size OnMeasure(SubItemMeasureEventArgs measureEventArgs)
		{
			return measureEventArgs.MeasureImageAndText(Image, Text);
		}
	}
}
