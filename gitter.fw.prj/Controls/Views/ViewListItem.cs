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
			Verify.Argument.IsNotNull(viewFactory, nameof(viewFactory));
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
