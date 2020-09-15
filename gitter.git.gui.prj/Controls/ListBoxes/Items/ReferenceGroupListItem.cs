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

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Item used to group reference-representing items together.</summary>
	public class ReferenceGroupListItem : CustomListBoxItem<ReferenceType>
	{
		#region Static

		private static readonly Image ImgRefsHeads		= CachedResources.Bitmaps["ImgRefsHeads"];
		private static readonly Image ImgRefsRemotes	= CachedResources.Bitmaps["ImgRefsRemotes"];
		private static readonly Image ImgRefsTags		= CachedResources.Bitmaps["ImgRefsTags"];

		#endregion

		#region .ctor

		/// <summary>Create <see cref="ReferenceGroupListItem"/>.</summary>
		/// <param name="repository">Related repository.</param>
		/// <param name="referenceTypes">Reference types to group.</param>
		public ReferenceGroupListItem(Repository repository, ReferenceType referenceTypes)
			: base(referenceTypes)
		{
			Repository = repository;
		}

		#endregion

		#region Properties

		public Repository Repository { get; }

		#endregion

		#region Overrides

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					switch(DataContext)
					{
						case ReferenceType.LocalBranch:
							return measureEventArgs.MeasureImageAndText(ImgRefsHeads, Resources.StrHeads);
						case ReferenceType.RemoteBranch:
							return measureEventArgs.MeasureImageAndText(ImgRefsRemotes, Resources.StrRemotes);
						case ReferenceType.Tag:
							return measureEventArgs.MeasureImageAndText(ImgRefsTags, Resources.StrTags);
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
				switch(DataContext)
				{
					case ReferenceType.LocalBranch:
						paintEventArgs.PaintImageAndText(ImgRefsHeads, Resources.StrHeads);
						break;
					case ReferenceType.RemoteBranch:
						paintEventArgs.PaintImageAndText(ImgRefsRemotes, Resources.StrRemotes);
						break;
					case ReferenceType.Tag:
						paintEventArgs.PaintImageAndText(ImgRefsTags, Resources.StrTags);
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
			if(Repository != null)
			{
				menu = new ContextMenuStrip();
				menu.Items.Add(GuiItemFactory.GetRefreshReferencesItem<ToolStripMenuItem>(Repository, DataContext, Resources.StrRefresh));
				switch(DataContext)
				{
					case ReferenceType.LocalBranch:
						menu.Items.Add(GuiItemFactory.GetCreateBranchItem<ToolStripMenuItem>(Repository));
						break;
					case ReferenceType.RemoteBranch:
						menu.Items.Add(GuiItemFactory.GetAddRemoteItem<ToolStripMenuItem>(Repository));
						break;
					case ReferenceType.Tag:
						menu.Items.Add(GuiItemFactory.GetCreateTagItem<ToolStripMenuItem>(Repository));
						break;
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
