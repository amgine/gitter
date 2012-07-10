namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Controls;

	public sealed class BranchListBinding : BaseListBinding<Branch, BranchEventArgs>
	{
		public BranchListBinding(CustomListBoxItemsCollection itemHost, Repository repository)
			: base(itemHost, repository.Refs.Heads)
		{
		}

		public BranchListBinding(CustomListBoxItemsCollection itemHost, RefsHeadsCollection branches)
			: base(itemHost, branches)
		{
		}

		protected override Comparison<CustomListBoxItem> GetComparison()
		{
			return BranchListItem.CompareByName;
		}

		protected override CustomListBoxItem<Branch> RepresentObject(Branch obj)
		{
			return new BranchListItem(obj);
		}
	}
}
