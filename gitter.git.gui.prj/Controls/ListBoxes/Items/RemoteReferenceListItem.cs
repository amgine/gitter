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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	public class RemoteReferenceListItem : CustomListBoxItem<IRemoteReference>
	{
		#region Static

		private static readonly Bitmap ImgBranch		= CachedResources.Bitmaps["ImgBranch"];
		private static readonly Bitmap ImgTag			= CachedResources.Bitmaps["ImgTag"];
		private static readonly Bitmap ImgTagAnnotated	= CachedResources.Bitmaps["ImgTagAnnotated"];

		#endregion

		#region .ctor

		public RemoteReferenceListItem(IRemoteReference reference)
			: base(reference)
		{
		}

		#endregion

		#region Event Handlers

		private void OnDeleted(object sender, EventArgs e)
		{
			RemoveSafe();
		}

		#endregion

		#region Overrides

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.Deleted += OnDeleted;
		}

		protected override void OnListBoxDetached()
		{
			DataContext.Deleted -= OnDeleted;
			base.OnListBoxDetached();
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					Bitmap image;
					switch(DataContext.ReferenceType)
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
					return measureEventArgs.MeasureImageAndText(image, DataContext.Name);
				case ColumnId.Hash:
					return HashColumn.OnMeasureSubItem(measureEventArgs, DataContext.Hash.ToString());
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
					switch(DataContext.ReferenceType)
					{
						case ReferenceType.LocalBranch:
							image = ImgBranch;
							break;
						case ReferenceType.Tag:
							var tag = DataContext as RemoteRepositoryTag;
							if(tag != null && tag.TagType == TagType.Annotated)
							{
								image = ImgTagAnnotated;
							}
							else
								image = ImgTag;
							break;
						default:
							image = null;
							break;
					}
					paintEventArgs.PaintImageAndText(image, DataContext.Name);
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
					paintEventArgs.PaintText(abbreviate
						? DataContext.Hash.ToString(abbrevLength)
						: DataContext.Hash.ToString(),
						HashColumn.Font);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			ContextMenuStrip menu = null;
			var branch = DataContext as RemoteRepositoryBranch;
			if(branch != null)
			{
				menu = new RemoteBranchMenu(branch);
			}
			else
			{
				var tag = DataContext as RemoteRepositoryTag;
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

		#endregion
	}
}
