namespace gitter.Framework.Options
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	using gitter.Framework.Controls;

	public sealed class FontListItem : CustomListBoxItem<SelectableFont>
	{
		private Font _font;

		public FontListItem(SelectableFont font)
			: base(font)
		{
			if(font == null) throw new ArgumentNullException("font");

			_font = font.Font;
		}

		public Font Font
		{
			get { return _font; }
			set
			{
				if(_font != value)
				{
					_font = value;
					InvalidateSubItem(1);
				}
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch(measureEventArgs.SubItemId)
			{
				case 0:
					return measureEventArgs.MeasureText(Data.Name);
				case 1:
					if(_font != null)
					{
						return measureEventArgs.MeasureText(_font.Name, _font);
					}
					else
					{
						return Size.Empty;
					}
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.SubItemId)
			{
				case 0:
					paintEventArgs.PaintText(Data.Name);
					break;
				case 1:
					if(_font != null)
					{
						paintEventArgs.PaintText(_font.Name, _font);
					}
					break;
			}
		}
	}
}
