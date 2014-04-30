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

namespace gitter
{
	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	using Resources = gitter.Properties.Resources;

	internal sealed class RecentRepositoryListItem : CustomListBoxItem<RepositoryLink>
	{
		private static readonly Bitmap ImgRepositorySmall = CachedResources.Bitmaps["ImgRepository"];
		private static readonly StringFormat PathStringFormat;

		static RecentRepositoryListItem()
		{
			PathStringFormat = new StringFormat(GitterApplication.TextRenderer.LeftAlign);
			PathStringFormat.Trimming = StringTrimming.EllipsisPath;
			PathStringFormat.FormatFlags |= StringFormatFlags.NoClip;
		}

		public RecentRepositoryListItem(RepositoryLink repository)
			: base(repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch(measureEventArgs.SubItemId)
			{
				case 0:
					return measureEventArgs.MeasureImageAndText(ImgRepositorySmall, DataContext.Path);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.SubItemId)
			{
				case 0:
					paintEventArgs.PaintImageAndText(ImgRepositorySmall, DataContext.Path, paintEventArgs.Brush, PathStringFormat);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new RecentRepositoryMenu(this);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
