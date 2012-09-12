namespace gitter.Framework.Options
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	using gitter.Framework.Controls;

	public sealed class PropertyPageItem : CustomListBoxItem<PropertyPageFactory>
	{
		public PropertyPageItem(PropertyPageFactory description)
			: base(description)
		{
			Verify.Argument.IsNotNull(description, "description");
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch(measureEventArgs.SubItemId)
			{
				case 0:
					if(DataContext.Icon != null)
						return measureEventArgs.MeasureImageAndText(DataContext.Icon, DataContext.Name);
					else
						return measureEventArgs.MeasureText(DataContext.Name);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.Column.Id)
			{
				case 0:
					if(DataContext.Icon != null)
						paintEventArgs.PaintImageAndText(DataContext.Icon, DataContext.Name);
					else
						paintEventArgs.PaintText(DataContext.Name);
					break;
			}
		}
	}
}
