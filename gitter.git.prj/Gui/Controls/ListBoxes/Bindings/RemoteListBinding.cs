namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Controls;

	public sealed class RemoteListBinding : BaseListBinding<Remote, RemoteEventArgs>
	{
		public RemoteListBinding(CustomListBoxItemsCollection itemHost, Repository repository)
			: base(itemHost, repository.Remotes)
		{
		}

		public RemoteListBinding(CustomListBoxItemsCollection itemHost, RemotesCollection remotes)
			: base(itemHost, remotes)
		{
		}

		protected override Comparison<CustomListBoxItem> GetComparison()
		{
			return RemoteListItem.CompareByName;
		}

		protected override CustomListBoxItem<Remote> RepresentObject(Remote obj)
		{
			return new RemoteListItem(obj);
		}
	}
}
