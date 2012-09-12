namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public class ViewListItem : CustomListBoxItem<IViewFactory>
	{
		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="ViewListItem"/> class.</summary>
		/// <param name="viewFactory">View factory.</param>
		public ViewListItem(IViewFactory viewFactory)
			: base(viewFactory)
		{
			Verify.Argument.IsNotNull(viewFactory, "viewFactory");
		}

		#endregion

		/// <summary>Measures part of this item.</summary>
		/// <param name="measureEventArgs">Measure event args.</param>
		/// <returns>Subitem size.</returns>
		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch(measureEventArgs.SubItemId)
			{
				case 0:
					return measureEventArgs.MeasureImageAndText(DataContext.Image, DataContext.Name);
				default:
					return Size.Empty;
			}
		}

		/// <summary>Paints part of this item.</summary>
		/// <param name="paintEventArgs">Paint event args.</param>
		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.SubItemId)
			{
				case 0:
					paintEventArgs.PaintImageAndText(DataContext.Image, DataContext.Name);
					break;
			}
		}
	}
}
