namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	public sealed class RemoteReferenceListItem : CustomListBoxItem<IRemoteReference>
	{
		private static readonly Bitmap ImgBranch = CachedResources.Bitmaps["ImgBranch"];
		private static readonly Bitmap ImgTag = CachedResources.Bitmaps["ImgTag"];
		private static readonly Bitmap ImgTagAnnotated = CachedResources.Bitmaps["ImgTagAnnotated"];

		public RemoteReferenceListItem(IRemoteReference reference)
			: base(reference)
		{
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			Data.Deleted += OnDeleted;
		}

		protected override void OnListBoxDetached()
		{
			Data.Deleted -= OnDeleted;
			base.OnListBoxDetached();
		}

		private void OnDeleted(object sender, EventArgs e)
		{
			RemoveSafe();
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					Bitmap image;
					switch(Data.ReferenceType)
					{
						case ReferenceType.LocalBranch:
							image = ImgBranch;
							break;
						case ReferenceType.Tag:
							image = ImgTag;
							break;
						default:
							image = null;
							break;
					}
					return measureEventArgs.MeasureImageAndText(image, Data.Name);
				case ColumnId.Hash:
					return HashColumn.OnMeasureSubItem(measureEventArgs, Data.Hash);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					Bitmap image;
					switch(Data.ReferenceType)
					{
						case ReferenceType.LocalBranch:
							image = ImgBranch;
							break;
						case ReferenceType.Tag:
							var tag = Data as RemoteRepositoryTag;
							if(tag != null && tag.TagType == TagType.Annotated)
								image = ImgTagAnnotated;
							else
								image = ImgTag;
							break;
						default:
							image = null;
							break;
					}
					paintEventArgs.PaintImageAndText(image, Data.Name);
					break;
				case ColumnId.Hash:
					var rhc = paintEventArgs.Column as HashColumn;
					bool abbreviate;
					int abbrevLength = HashColumn.DefaultAbbrevLength;
					if(rhc != null)
					{
						abbreviate = rhc.Abbreviate;
					}
					else
					{
						abbreviate = HashColumn.DefaultAbbreviate;
					}
					paintEventArgs.PaintText(abbreviate?Data.Hash.Substring(0, abbrevLength):Data.Hash, HashColumn.Font);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			ContextMenuStrip menu = null;
			var branch = Data as RemoteRepositoryBranch;
			if(branch != null)
			{
				menu = new RemoteBranchMenu(branch);
			}
			else
			{
				var tag = Data as RemoteRepositoryTag;
				if(tag != null)
				{
					menu = new RemoteTagMenu(tag);
				}
			}
			if(menu != null)
			{
				Utility.MarkDropDownForAutoDispose(menu);
			}
			return menu;
		}
	}
}
