namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework.Controls;

	public sealed class PatchSourceListItem : CustomListBoxItem<IPatchSource>
	{
		private static readonly Bitmap ImgPatch = CachedResources.Bitmaps["ImgPatch"];

		public PatchSourceListItem(IPatchSource patchSource)
			: base(patchSource)
		{
			if(patchSource == null) throw new ArgumentNullException("patchSource");
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					paintEventArgs.PaintImageAndText(ImgPatch, Data.DisplayName);
					break;
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					return measureEventArgs.MeasureImageAndText(ImgPatch, Data.DisplayName);
				default:
					return Size.Empty;
			}
		}
	}
}
