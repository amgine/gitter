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

	internal sealed class RepositoryListItem : CustomListBoxItem<RepositoryLink>
	{
		private static readonly Bitmap ImgRepositorySmall = CachedResources.Bitmaps["ImgRepository"];
		private static readonly Bitmap ImgRepositoryLarge = CachedResources.Bitmaps["ImgRepositoryLarge"];

		private static readonly StringFormat PathStringFormat;

		static RepositoryListItem()
		{
			PathStringFormat = new StringFormat(Utility.DefaultStringFormatLeftAlign);
			PathStringFormat.Trimming = StringTrimming.EllipsisPath;
			PathStringFormat.FormatFlags |= StringFormatFlags.NoClip;
		}

		public RepositoryListItem(RepositoryLink rlink)
			: base(rlink)
		{
			if(rlink == null) throw new ArgumentNullException("rlink");
		}

		private string Name
		{
			get
			{
				if(string.IsNullOrEmpty(Data.Description))
				{
					if(Data.Path.EndsWithOneOf(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
					{
						return Path.GetFileName(Data.Path.Substring(0, Data.Path.Length - 1));
					}
					else
					{
						return Path.GetFileName(Data.Path);
					}
				}
				return Data.Description;
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch(measureEventArgs.SubItemId)
			{
				case 0:
					return measureEventArgs.MeasureImageAndText(ImgRepositorySmall, Data.Path);
				case 1:
					return measureEventArgs.MeasureImageAndText(ImgRepositoryLarge, Data.Path);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.SubItemId)
			{
				case 0:
					paintEventArgs.PaintImageAndText(ImgRepositorySmall, Data.Path, paintEventArgs.Brush, PathStringFormat);
					break;
				case 1:
					paintEventArgs.PaintImage(ImgRepositoryLarge);
					var cy = paintEventArgs.Bounds.Y + 2;
					GitterApplication.TextRenderer.DrawText(
						paintEventArgs.Graphics, Name, paintEventArgs.Font, paintEventArgs.Brush, 36, cy, Utility.DefaultStringFormatLeftAlign);
					cy += 16;
					var rc = new Rectangle(36, cy, paintEventArgs.Bounds.Width - 42, 16);
					GitterApplication.TextRenderer.DrawText(
						paintEventArgs.Graphics, Data.Path, paintEventArgs.Font, SystemBrushes.GrayText, rc, PathStringFormat);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new RepositoryMenu(this);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
