#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
			Verify.Argument.IsNotNull(font, nameof(font));

			_font = font.Font;
		}

		public Font Font
		{
			get => _font;
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
					return measureEventArgs.MeasureText(DataContext.Name);
				case 1:
					return _font != null
						? measureEventArgs.MeasureText(_font.Name, _font)
						: Size.Empty;
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.SubItemId)
			{
				case 0:
					paintEventArgs.PaintText(DataContext.Name);
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
