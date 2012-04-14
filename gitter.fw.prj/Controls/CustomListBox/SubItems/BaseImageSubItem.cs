namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Base class for image subitems.</summary>
	public abstract class BaseImageSubItem : CustomListBoxSubItem
	{
		/// <summary>Create <see cref="BaseImageSubItem"/>.</summary>
		/// <param name="id">Subitem id.</param>
		public BaseImageSubItem(int id)
			: base(id)
		{
		}

		/// <summary>Subitem image.</summary>
		public abstract Image Image { get; set; }

		/// <summary>Subitem overlay image.</summary>
		public virtual Image OverlayImage { get { return null; } set { } }

		/// <summary>Paint event handler.</summary>
		/// <param name="paintEventArgs">Paint event args.</param>
		protected override void OnPaint(SubItemPaintEventArgs paintEventArgs)
		{
			paintEventArgs.PaintImage(Image, OverlayImage);
		}

		/// <summary>Measure event handler.</summary>
		/// <param name="measureEventArgs">Measure event args.</param>
		/// <returns>Subitem content size.</returns>
		protected override Size OnMeasure(SubItemMeasureEventArgs measureEventArgs)
		{
			return measureEventArgs.MeasureImage(Image);
		}
	}
}
