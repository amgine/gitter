namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>A <see cref="CustomListBoxItem"/> representing <see cref="Tag"/> object.</summary>
	public sealed class TagListItem : ReferenceListItemBase<Tag>
	{
		private static readonly Bitmap ImgTag = CachedResources.Bitmaps["ImgTag"];
		private static readonly Bitmap ImgTagAnnotated = CachedResources.Bitmaps["ImgTagAnnotated"];

		#region .ctor

		/// <summary>Create <see cref="TagListItem"/>.</summary>
		/// <param name="tag">Related <see cref="Tag"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="tag"/> == <c>null</c>.</exception>
		public TagListItem(Tag tag)
			: base(tag)
		{
			if(tag == null) throw new ArgumentNullException("tag");
		}

		#endregion

		protected override Image Image
		{
			get { return (DataContext.TagType == TagType.Annotated) ? ImgTagAnnotated : ImgTag; }
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var mnu = new TagMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(mnu);
			return mnu;
		}
	}
}
