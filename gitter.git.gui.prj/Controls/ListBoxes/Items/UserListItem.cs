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
		#region Static

		private static readonly Bitmap ImgUser = CachedResources.Bitmaps["ImgUser"];

		#endregion

		#region Comparers

		public static int CompareByName(UserListItem item1, UserListItem item2)
		{
			var data1 = item1.DataContext.Name;
			var data2 = item2.DataContext.Name;
			return string.Compare(data1, data2);
		}

		public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var userListItem1 = item1 as UserListItem;
			var userListItem2 = item2 as UserListItem;
			if(userListItem1 != null && userListItem2 != null)
			{
				return CompareByName(userListItem1, userListItem2);
			}
			else
			{
				return 0;
			}
		}

		public static int CompareByEmail(UserListItem item1, UserListItem item2)
		{
			var data1 = item1.DataContext.Email;
			var data2 = item2.DataContext.Email;
			return string.Compare(data1, data2);
		}

		public static int CompareByEmail(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var userListItem1 = item1 as UserListItem;
			var userListItem2 = item2 as UserListItem;
			if(userListItem1 != null && userListItem2 != null)
			{
				return CompareByEmail(userListItem1, userListItem2);
			}
			else
			{
				return 0;
			}
		}

		public static int CompareByCommitCount(UserListItem item1, UserListItem item2)
		{
			var data1 = item1.DataContext.Commits;
			var data2 = item2.DataContext.Commits;
			return data1 > data2 ? 1 : (data1 == data2 ? 0 : -1);
		}

		public static int CompareByCommitCount(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var userListItem1 = item1 as UserListItem;
			var userListItem2 = item2 as UserListItem;
			if(userListItem1 != null && userListItem2 != null)
			{
				return CompareByCommitCount(userListItem1, userListItem2);
			}
			else
			{
				return 0;
			}
		}

		#endregion

		#region Data

		private Bitmap _imgAvatar;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="UserListItem"/>.</summary>
		/// <param name="user">Related <see cref="User"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="user"/> == <c>null</c>.</exception>
		public UserListItem(User user)
			: base(user)
		{
			Verify.Argument.IsNotNull(user, "user");
		}

		#endregion

		#region Event Handlers

		private void OnDataAvatarUpdated(object sender, EventArgs e)
		{
			InvalidateSafe();
		}

		#endregion

		#region Overrides

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.Avatar.Updated += OnDataAvatarUpdated;
		}

		protected override void OnListBoxDetached()
		{
			DataContext.Avatar.Updated -= OnDataAvatarUpdated;
			base.OnListBoxDetached();
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
				case ColumnId.Committer:
					return measureEventArgs.MeasureImageAndText(ImgUser, DataContext.Name);
				case ColumnId.Email:
				case ColumnId.CommitterEmail:
					return EmailColumn.OnMeasureSubItem(measureEventArgs, DataContext.Email);
				case ColumnId.Commits:
					return measureEventArgs.MeasureText(DataContext.Commits.ToString());
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
				case ColumnId.Author:
				case ColumnId.Committer:
					Bitmap image;
					if(GitterApplication.IntegrationFeatures.Gravatar.IsEnabled)
					{
						if(_imgAvatar != null)
						{
							image = _imgAvatar;
						}
						else
						{
							var avatar = DataContext.Avatar;
							var imgAvatar = avatar.Image;
							if(imgAvatar == null)
							{
								avatar.BeginUpdate();
								image = ImgUser;
							}
							else
							{
								_imgAvatar = image = new Bitmap(imgAvatar, new Size(16, 16));
							}
						}
					}
					else
					{
						image = ImgUser;
					}
					paintEventArgs.PaintImageAndText(image, DataContext.Name);
					break;
				case ColumnId.Email:
				case ColumnId.AuthorEmail:
				case ColumnId.CommitterEmail:
					paintEventArgs.PaintText(DataContext.Email);
					break;
				case ColumnId.Commits:
					paintEventArgs.PaintText(DataContext.Commits.ToString(CultureInfo.InvariantCulture));
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new UserMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		#endregion
	}
}
