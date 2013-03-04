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
			Verify.Argument.IsNotNull(patchSource, "patchSource");
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
