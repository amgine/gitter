namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Item used to group reference-representing items together.</summary>
	public sealed class ReferenceGroupListItem : CustomListBoxItem<ReferenceType>
	{
		private readonly Repository _repository;

		/// <summary>Create <see cref="ReferenceGroupListItem"/>.</summary>
		/// <param name="repository">Related repository.</param>
		/// <param name="referenceTypes">Reference types to group.</param>
		public ReferenceGroupListItem(Repository repository, ReferenceType referenceTypes)
			: base(referenceTypes)
		{
			_repository = repository;
		}

		public Repository Repository
		{
			get { return _repository; }
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					switch(Data)
					{
						case ReferenceType.LocalBranch:
							return measureEventArgs.MeasureImageAndText(
								CachedResources.Bitmaps["ImgRefsHeads"], Resources.StrHeads);
						case ReferenceType.RemoteBranch:
							return measureEventArgs.MeasureImageAndText(
								CachedResources.Bitmaps["ImgRefsRemotes"], Resources.StrRemotes);
						case ReferenceType.Tag:
							return measureEventArgs.MeasureImageAndText(
								CachedResources.Bitmaps["ImgRefsTags"], Resources.StrTags);
						default:
							return Size.Empty;
					}
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			if(paintEventArgs.SubItemId == (int)ColumnId.Name)
			{
				switch(Data)
				{
					case ReferenceType.LocalBranch:
						paintEventArgs.PaintImageAndText(
							CachedResources.Bitmaps["ImgRefsHeads"], Resources.StrHeads);
						break;
					case ReferenceType.RemoteBranch:
						paintEventArgs.PaintImageAndText(
							CachedResources.Bitmaps["ImgRefsRemotes"], Resources.StrRemotes);
						break;
					case ReferenceType.Tag:
						paintEventArgs.PaintImageAndText(
							CachedResources.Bitmaps["ImgRefsTags"], Resources.StrTags);
						break;
				}
			}
		}

		/// <summary>Gets the context menu.</summary>
		/// <param name="requestEventArgs">Request parameters.</param>
		/// <returns>Context menu for specified location.</returns>
		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			ContextMenuStrip menu = null;
			if(_repository != null)
			{
				menu = new ContextMenuStrip();
				menu.Items.Add(GuiItemFactory.GetRefreshReferencesItem<ToolStripMenuItem>(_repository, Data, Resources.StrRefresh));
				switch(Data)
				{
					case ReferenceType.LocalBranch:
						menu.Items.Add(GuiItemFactory.GetCreateBranchItem<ToolStripMenuItem>(_repository));
						break;
					case ReferenceType.RemoteBranch:
						menu.Items.Add(GuiItemFactory.GetAddRemoteItem<ToolStripMenuItem>(_repository));
						break;
					case ReferenceType.Tag:
						menu.Items.Add(GuiItemFactory.GetCreateTagItem<ToolStripMenuItem>(_repository));
						break;
				}
			}
			if(menu != null) Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
