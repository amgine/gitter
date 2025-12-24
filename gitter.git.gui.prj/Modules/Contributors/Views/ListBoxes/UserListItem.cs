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
	using System.Globalization;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary><see cref="CustomListBoxItem"/> representing <see cref="User"/>.</summary>
	public sealed class UserListItem : CustomListBoxItem<User>
	{
		public static int CompareByName(UserListItem item1, UserListItem item2)
		{
			var data1 = item1.DataContext.Name;
			var data2 = item2.DataContext.Name;
			return string.Compare(data2, data1);
		}

		public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
			=> item1 is UserListItem userListItem1 && item2 is UserListItem userListItem2
				? CompareByName(userListItem1, userListItem2)
				: 0;

		public static int CompareByEmail(UserListItem item1, UserListItem item2)
		{
			var data1 = item1.DataContext.Email;
			var data2 = item2.DataContext.Email;
			return string.Compare(data2, data1);
		}

		public static int CompareByEmail(CustomListBoxItem item1, CustomListBoxItem item2)
			=> item1 is UserListItem userListItem1 && item2 is UserListItem userListItem2
				? CompareByEmail(userListItem1, userListItem2)
				: 0;

		public static int CompareByCommitCount(UserListItem item1, UserListItem item2)
		{
			var data1 = item1.DataContext.Commits;
			var data2 = item2.DataContext.Commits;
			return data1 > data2 ? 1 : (data1 == data2 ? 0 : -1);
		}

		public static int CompareByCommitCount(CustomListBoxItem item1, CustomListBoxItem item2)
			=> item1 is UserListItem userListItem1 && item2 is UserListItem userListItem2
				? CompareByCommitCount(userListItem1, userListItem2)
				: 0;

		/// <summary>Create <see cref="UserListItem"/>.</summary>
		/// <param name="user">Related <see cref="User"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="user"/> == <c>null</c>.</exception>
		public UserListItem(User user)
			: base(user)
		{
			Verify.Argument.IsNotNull(user);
		}

		private void OnDataAvatarUpdated(object? sender, EventArgs e)
		{
			InvalidateSafe();
		}

		protected override void OnListBoxAttached(CustomListBox listBox)
		{
			base.OnListBoxAttached(listBox);
			DataContext.Avatar.Updated += OnDataAvatarUpdated;
		}

		protected override void OnListBoxDetached(CustomListBox listBox)
		{
			DataContext.Avatar.Updated -= OnDataAvatarUpdated;
			base.OnListBoxDetached(listBox);
		}

		private static Image? GetIcon(Dpi dpi)
			=> Icons.User.GetImage(DpiConverter.FromDefaultTo(dpi).ConvertX(16));

		private static async void UpdateAvatarAsync(IAvatar avatar, CustomListBoxItem item, CustomListBoxColumn column)
		{
			await avatar.UpdateAsync();
			item.InvalidateSubItem(column.Id);
		}

		private void PaintAvatar(SubItemPaintEventArgs paintEventArgs)
		{
			Assert.IsNotNull(paintEventArgs);

			if(GitterApplication.IntegrationFeatures.Gravatar.IsEnabled)
			{
				var avatar = DataContext?.Avatar;
				if(avatar is not null)
				{
					var imgAvatar = avatar.Image;
					if(imgAvatar is not null)
					{
						paintEventArgs.PaintImageAndText(imgAvatar, ImagePainter.Circle, DataContext?.Name);
						return;
					}
					else
					{
						UpdateAvatarAsync(avatar, paintEventArgs.Item, paintEventArgs.Column);
					}
				}
			}
			var image = GetIcon(paintEventArgs.Dpi);
			paintEventArgs.PaintImageAndText(image, DataContext?.Name);
		}

		/// <inheritdoc/>
		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			Assert.IsNotNull(measureEventArgs);

			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
				case ColumnId.Committer:
					return measureEventArgs.MeasureImageAndText(GetIcon(measureEventArgs.Dpi), DataContext.Name);
				case ColumnId.Commits:
					return measureEventArgs.MeasureText(DataContext.Commits.ToString());
				default:
					return base.OnMeasureSubItem(measureEventArgs);
			}
		}

		/// <inheritdoc/>
		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			Assert.IsNotNull(paintEventArgs);

			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
				case ColumnId.Author:
				case ColumnId.Committer:
					PaintAvatar(paintEventArgs);
					break;
				case ColumnId.Commits:
					paintEventArgs.PaintText(DataContext.Commits.ToString(CultureInfo.InvariantCulture));
					break;
				default:
					base.OnPaintSubItem(paintEventArgs);
					break;
			}
		}

		/// <inheritdoc/>
		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			Assert.IsNotNull(requestEventArgs);

			var menu = new UserMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
