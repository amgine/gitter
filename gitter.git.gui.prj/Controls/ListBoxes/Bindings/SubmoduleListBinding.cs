namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Controls;

	public sealed class SubmoduleListBinding : BaseListBinding<Submodule, SubmoduleEventArgs>
	{
		public SubmoduleListBinding(CustomListBoxItemsCollection itemHost, Repository repository)
			: base(itemHost, repository.Submodules)
		{
		}

		public SubmoduleListBinding(CustomListBoxItemsCollection itemHost, SubmodulesCollection submodules)
			: base(itemHost, submodules)
		{
		}

		protected override Comparison<CustomListBoxItem> GetComparison()
		{
			return SubmoduleListItem.CompareByName;
		}

		protected override CustomListBoxItem<Submodule> RepresentObject(Submodule obj)
		{
			return new SubmoduleListItem(obj);
		}
	}
}
