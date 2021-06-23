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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Controls;

	public class PatchSourceListItem : CustomListBoxItem<IPatchSource>
	{
		public PatchSourceListItem(IPatchSource patchSource)
			: base(patchSource)
		{
			Verify.Argument.IsNotNull(patchSource, nameof(patchSource));
		}

		private static Bitmap GetIcon(Dpi dpi)
			=> CachedResources.ScaledBitmaps[@"patch", DpiConverter.FromDefaultTo(dpi).ConvertX(16)];

		/// <inheritdoc/>
		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			Assert.IsNotNull(measureEventArgs);

			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					return measureEventArgs.MeasureImageAndText(GetIcon(measureEventArgs.Dpi), DataContext.DisplayName);
				default:
					return Size.Empty;
			}
		}

		/// <inheritdoc/>
		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			Assert.IsNotNull(paintEventArgs);

			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					paintEventArgs.PaintImageAndText(GetIcon(paintEventArgs.Dpi), DataContext.DisplayName);
					break;
			}
		}
	}
}
