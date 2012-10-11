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

	internal sealed class RecentRepositoryListItem : CustomListBoxItem<string>
	{
		private static readonly Bitmap ImgRepositorySmall = CachedResources.Bitmaps["ImgRepository"];
		private static readonly StringFormat PathStringFormat;

		static RecentRepositoryListItem()
		{
			PathStringFormat = new StringFormat(GitterApplication.TextRenderer.LeftAlign);
			PathStringFormat.Trimming = StringTrimming.EllipsisPath;
			PathStringFormat.FormatFlags |= StringFormatFlags.NoClip;
		}

		public RecentRepositoryListItem(string path)
			: base(path)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(path, "path");
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch(measureEventArgs.SubItemId)
			{
				case 0:
					return measureEventArgs.MeasureImageAndText(ImgRepositorySmall, DataContext);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.SubItemId)
			{
				case 0:
					paintEventArgs.PaintImageAndText(ImgRepositorySmall, DataContext, paintEventArgs.Brush, PathStringFormat);
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
