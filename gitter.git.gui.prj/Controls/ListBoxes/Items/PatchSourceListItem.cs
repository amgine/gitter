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

	using gitter.Framework.Controls;

	public class PatchSourceListItem : CustomListBoxItem<IPatchSource>
	{
		#region Static

		private static readonly Bitmap ImgPatch = CachedResources.Bitmaps["ImgPatch"];

		#endregion

		#region .ctor

		public PatchSourceListItem(IPatchSource patchSource)
			: base(patchSource)
		{
			Verify.Argument.IsNotNull(patchSource, nameof(patchSource));
		}

		#endregion

		#region Overrides

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					paintEventArgs.PaintImageAndText(ImgPatch, DataContext.DisplayName);
					break;
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					return measureEventArgs.MeasureImageAndText(ImgPatch, DataContext.DisplayName);
				default:
					return Size.Empty;
			}
		}

		#endregion
	}
}
