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

	/// <summary><see cref="gitter.Framework.Controls.CustomListBoxItem"/> representing <see cref="gitter.Git.Remote"/>.</summary>
	public class RemoteListItem : CustomListBoxItem<Remote>
	{
		#region Static

		private static readonly Bitmap ImgRemote = CachedResources.Bitmaps["ImgRemote"];

		#endregion

		#region Comparers

		public static int CompareByName(RemoteListItem item1, RemoteListItem item2)
		{
			var data1 = item1.DataContext.Name;
			var data2 = item2.DataContext.Name;
			return string.Compare(data1, data2);
		}

		public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as RemoteListItem;
			if(i1 == null) return 0;
			var i2 = item2 as RemoteListItem;
			if(i2 == null) return 0;
			try
			{
				return CompareByName(i1, i2);
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				return 0;
			}
		}

		public static int CompareByFetchUrl(RemoteListItem item1, RemoteListItem item2)
		{
			var data1 = item1.DataContext.FetchUrl;
			var data2 = item2.DataContext.FetchUrl;
			return string.Compare(data1, data2);
		}

		public static int CompareByFetchUrl(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as RemoteListItem;
			if(i1 == null) return 0;
			var i2 = item2 as RemoteListItem;
			if(i2 == null) return 0;
			try
			{
				return CompareByFetchUrl(i1, i2);
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				return 0;
			}
		}

		public static int CompareByPushUrl(RemoteListItem item1, RemoteListItem item2)
		{
			var data1 = item1.DataContext.PushUrl;
			var data2 = item2.DataContext.PushUrl;
			return string.Compare(data1, data2);
		}

		public static int CompareByPushUrl(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as RemoteListItem;
			if(i1 == null) return 0;
			var i2 = item2 as RemoteListItem;
			if(i2 == null) return 0;
			try
			{
				return CompareByPushUrl(i1, i2);
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				return 0;
			}
		}

		#endregion

		#region .ctor

		/// <summary>Create <see cref="RemoteListItem"/>.</summary>
		/// <param name="remote">Related <see cref="Remote"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="remote"/> == <c>null</c>.</exception>
		public RemoteListItem(Remote remote)
			: base(remote)
		{
			Verify.Argument.IsNotNull(remote, "remote");
		}

		#endregion

		#region Event Handlers

		private void OnRemoteDeleted(object sender, EventArgs e)
		{
			RemoveSafe();
		}

		private void OnRenamed(object sender, EventArgs e)
		{
			if(EnsureSortOrderSafe())
			{
				InvalidateSubItemSafe((int)ColumnId.Name);
			}
		}

		#endregion

		#region Overrides

		protected override void OnListBoxAttached()
		{
			DataContext.Deleted += OnRemoteDeleted;
			DataContext.Renamed += OnRenamed;
			base.OnListBoxAttached();
		}

		protected override void OnListBoxDetached()
		{
			DataContext.Deleted -= OnRemoteDeleted;
			DataContext.Renamed -= OnRenamed;
			base.OnListBoxDetached();
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					return measureEventArgs.MeasureImageAndText(ImgRemote, DataContext.Name);
				case ColumnId.Url:
				case ColumnId.FetchUrl:
					return measureEventArgs.MeasureText(DataContext.FetchUrl);
				case ColumnId.PushUrl:
					return measureEventArgs.MeasureText(DataContext.PushUrl);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					paintEventArgs.PaintImageAndText(ImgRemote, DataContext.Name);
					break;
				case ColumnId.Url:
				case ColumnId.FetchUrl:
					paintEventArgs.PaintText(DataContext.FetchUrl);
					break;
				case ColumnId.PushUrl:
					paintEventArgs.PaintText(DataContext.PushUrl);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var mnu = new RemoteMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(mnu);
			return mnu;
		}

		#endregion
	}
}
