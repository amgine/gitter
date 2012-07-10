namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	public sealed class UserListBinding : BaseListBinding<User, UserEventArgs>
	{
		public UserListBinding(CustomListBoxItemsCollection itemHost, Repository repository)
			: base(itemHost, repository.Users)
		{
		}

		public UserListBinding(CustomListBoxItemsCollection itemHost, UsersCollection users)
			: base(itemHost, users)
		{
		}

		protected override SortOrder GetSortOrder()
		{
			return SortOrder.Descending;
		}

		protected override Comparison<CustomListBoxItem> GetComparison()
		{
			return UserListItem.CompareByName;
		}

		protected override CustomListBoxItem<User> RepresentObject(User obj)
		{
			return new UserListItem(obj);
		}
	}
}
