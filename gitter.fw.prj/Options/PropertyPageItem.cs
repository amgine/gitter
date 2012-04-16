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
			if(description == null) throw new ArgumentNullException("description");
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch(measureEventArgs.SubItemId)
			{
				case 0:
					if(Data.Icon != null)
						return measureEventArgs.MeasureImageAndText(Data.Icon, Data.Name);
					else
						return measureEventArgs.MeasureText(Data.Name);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.Column.Id)
			{
				case 0:
					if(Data.Icon != null)
						paintEventArgs.PaintImageAndText(Data.Icon, Data.Name);
					else
						paintEventArgs.PaintText(Data.Name);
					break;
			}
		}
	}
}
