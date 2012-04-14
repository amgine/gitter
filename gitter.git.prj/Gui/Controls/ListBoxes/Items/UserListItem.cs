namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary><see cref="CustomListBoxItem"/> representing <see cref="User"/>.</summary>
	public sealed class UserListItem : CustomListBoxItem<User>
	{
		private static readonly Bitmap ImgIcon = CachedResources.Bitmaps["ImgUser"];
		private Bitmap _imgAvatar;

		#region Comparers

		public static int CompareByName(UserListItem item1, UserListItem item2)
		{
			var data1 = item1.Data.Name;
			var data2 = item2.Data.Name;
			return string.Compare(data1, data2);
		}

		public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			try
			{
				return CompareByName((UserListItem)item1, (UserListItem)item2);
			}
			catch
			{
				return 0;
			}
		}

		public static int CompareByEmail(UserListItem item1, UserListItem item2)
		{
			var data1 = item1.Data.Email;
			var data2 = item2.Data.Email;
			return string.Compare(data1, data2);
		}

		public static int CompareByEmail(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			try
			{
				return CompareByEmail((UserListItem)item1, (UserListItem)item2);
			}
			catch
			{
				return 0;
			}
		}

		public static int CompareByCommitCount(UserListItem item1, UserListItem item2)
		{
			var data1 = item1.Data.Commits;
			var data2 = item2.Data.Commits;
			return data1 > data2 ? 1 : (data1 == data2 ? 0 : -1);
		}

		public static int CompareByCommitCount(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			try
			{
				return CompareByCommitCount((UserListItem)item1, (UserListItem)item2);
			}
			catch
			{
				return 0;
			}
		}

		#endregion

		#region .ctor

		/// <summary>Create <see cref="UserListItem"/>.</summary>
		/// <param name="committer">Related <see cref="User"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="committer"/> == <c>null</c>.</exception>
		public UserListItem(User committer)
			: base(committer)
		{
			if(committer == null) throw new ArgumentNullException("committer");
		}

		#endregion

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			Data.Avatar.Updated += OnDataAvatarUpdated;
		}

		protected override void OnListBoxDetached()
		{
			Data.Avatar.Updated -= OnDataAvatarUpdated;
			base.OnListBoxDetached();
		}

		private void OnDataAvatarUpdated(object sender, EventArgs e)
		{
			InvalidateSafe();
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
				case ColumnId.Committer:
					return measureEventArgs.MeasureImageAndText(ImgIcon, Data.Name);
				case ColumnId.Email:
				case ColumnId.CommitterEmail:
					return EmailColumn.OnMeasureSubItem(measureEventArgs, Data.Email);
				case ColumnId.Commits:
					return measureEventArgs.MeasureText(Data.Commits.ToString());
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
				case ColumnId.Committer:
					Bitmap image;
					if(RepositoryProvider.Integration.Gravatar.Enabled)
					{
						if(_imgAvatar != null)
						{
							image = _imgAvatar;
						}
						else
						{
							var avatar = Data.Avatar;
							var imgAvatar = avatar.Image;
							if(imgAvatar == null)
							{
								avatar.BeginUpdate();
								image = ImgIcon;
							}
							else
							{
								_imgAvatar = image = new Bitmap(imgAvatar, new Size(16, 16));
							}
						}
					}
					else
					{
						image = ImgIcon;
					}
					paintEventArgs.PaintImageAndText(image, Data.Name);
					break;
				case ColumnId.Email:
				case ColumnId.CommitterEmail:
					paintEventArgs.PaintText(Data.Email);
					break;
				case ColumnId.Commits:
					paintEventArgs.PaintText(Data.Commits.ToString(System.Globalization.CultureInfo.InvariantCulture));
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new UserMenu(Data);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
