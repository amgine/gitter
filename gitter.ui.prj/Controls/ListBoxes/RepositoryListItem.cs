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
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	internal sealed class RepositoryListItem : CustomListBoxItem<RepositoryLink>
	{
		private static readonly StringFormat PathStringFormat;
		private static readonly StringFormat NameStringFormat;

		private bool? _exists;

		static RepositoryListItem()
		{
			PathStringFormat = new StringFormat(GitterApplication.TextRenderer.LeftAlign);
			PathStringFormat.Trimming = StringTrimming.EllipsisPath;
			PathStringFormat.FormatFlags |= StringFormatFlags.NoClip;
			PathStringFormat.LineAlignment = StringAlignment.Center;

			NameStringFormat = new StringFormat(GitterApplication.TextRenderer.LeftAlign);
			NameStringFormat.LineAlignment = StringAlignment.Center;
		}

		public RepositoryListItem(RepositoryLink rlink)
			: base(rlink)
		{
			Verify.Argument.IsNotNull(rlink, nameof(rlink));
		}

		private bool CheckExists()
		{
			try
			{
				return Directory.Exists(DataContext.Path);
			}
			catch
			{
				return false;
			}
		}

		private bool Exists
		{
			get
			{
				if(!_exists.HasValue)
				{
					_exists = CheckExists();
				}
				return _exists.Value;
			}
		}

		private string Name
		{
			get
			{
				if(string.IsNullOrEmpty(DataContext.Description))
				{
					if(DataContext.Path.EndsWithOneOf(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
					{
						return Path.GetFileName(DataContext.Path.Substring(0, DataContext.Path.Length - 1));
					}
					else
					{
						return Path.GetFileName(DataContext.Path);
					}
				}
				return DataContext.Description;
			}
		}

		private static Bitmap GetIcon(bool available, int size, Dpi dpi)
			=> CachedResources.ScaledBitmaps[
				available ? @"repository" : @"repository.unavailable",
				DpiConverter.FromDefaultTo(dpi).ConvertX(size)];

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			Assert.IsNotNull(measureEventArgs);

			switch(measureEventArgs.SubItemId)
			{
				case 0:
					return measureEventArgs.MeasureImageAndText(GetIcon(true, 16, measureEventArgs.Dpi), DataContext.Path);
				case 1:
					return measureEventArgs.MeasureImageAndText(GetIcon(true, 32, measureEventArgs.Dpi), DataContext.Path);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			Assert.IsNotNull(paintEventArgs);

			switch(paintEventArgs.SubItemId)
			{
				case 0:
					paintEventArgs.PaintImageAndText(GetIcon(Exists, 16, paintEventArgs.Dpi), DataContext.Path, paintEventArgs.Brush, PathStringFormat);
					break;
				case 1:
					paintEventArgs.PaintImage(GetIcon(Exists, 32, paintEventArgs.Dpi));

					var conv = paintEventArgs.DpiConverter;

					var dx = conv.ConvertX(36);
					var dy = conv.ConvertY(2);

					var b1 = paintEventArgs.Bounds;
					b1.Height -= dy * 2;
					b1.Y      += dy;
					b1.X      += dx;
					b1.Width  -= dx;
					b1.Height /= 2;
					var b2 = b1;
					b2.Y += b1.Height;

					GitterApplication.TextRenderer.DrawText(
						paintEventArgs.Graphics, Name, paintEventArgs.Font, paintEventArgs.Brush, b1, NameStringFormat);
					if((paintEventArgs.State & ItemState.Selected) == ItemState.Selected && GitterApplication.Style.Type == GitterStyleType.DarkBackground)
					{
						GitterApplication.TextRenderer.DrawText(
							paintEventArgs.Graphics, DataContext.Path, paintEventArgs.Font, paintEventArgs.Brush, b2, PathStringFormat);
					}
					else
					{
						GitterApplication.TextRenderer.DrawText(
							paintEventArgs.Graphics, DataContext.Path, paintEventArgs.Font, GitterApplication.Style.Colors.GrayText, b2, PathStringFormat);
					}
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			Assert.IsNotNull(requestEventArgs);

			var menu = new RepositoryMenu(this);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
