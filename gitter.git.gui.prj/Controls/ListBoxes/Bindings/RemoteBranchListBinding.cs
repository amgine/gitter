namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Controls;

	public sealed class RemoteBranchListBinding : BaseListBinding<RemoteBranch, RemoteBranchEventArgs>
	{
		public RemoteBranchListBinding(CustomListBoxItemsCollection itemHost, Repository repository)
			: base(itemHost, repository.Refs.Remotes)
		{
		}

		public RemoteBranchListBinding(CustomListBoxItemsCollection itemHost, RefsRemotesCollection branches)
			: base(itemHost, branches)
		{
		}

		protected override Comparison<CustomListBoxItem> GetComparison()
		{
			return RemoteBranchListItem.CompareByName;
		}

		protected override CustomListBoxItem<RemoteBranch> RepresentObject(RemoteBranch obj)
		{
			return new RemoteBranchListItem(obj);
		}
	}
}
