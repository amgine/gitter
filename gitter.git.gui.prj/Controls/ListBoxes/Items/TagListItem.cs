namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	/// <summary>A <see cref="CustomListBoxItem"/> representing <see cref="Tag"/> object.</summary>
	public class TagListItem : ReferenceListItemBase<Tag>
	{
		#region Static

		private static readonly Bitmap ImgTag = CachedResources.Bitmaps["ImgTag"];
		private static readonly Bitmap ImgTagAnnotated = CachedResources.Bitmaps["ImgTagAnnotated"];

		#endregion

		#region .ctor

		/// <summary>Create <see cref="TagListItem"/>.</summary>
		/// <param name="tag">Related <see cref="Tag"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="tag"/> == <c>null</c>.</exception>
		public TagListItem(Tag tag)
			: base(tag)
		{
			Verify.Argument.IsNotNull(tag, "tag");
		}

		#endregion

		#region Overrides

		protected override Image Image
		{
			get { return (DataContext.TagType == TagType.Annotated) ? ImgTagAnnotated : ImgTag; }
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new TagMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		#endregion
	}
}
