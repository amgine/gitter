namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Controls;

	public sealed class TagListBinding : BaseListBinding<Tag, TagEventArgs>
	{
		public TagListBinding(CustomListBoxItemsCollection itemHost, Repository repository)
			: base(itemHost, repository.Refs.Tags)
		{
		}

		public TagListBinding(CustomListBoxItemsCollection itemHost, RefsTagsCollection tags)
			: base(itemHost, tags)
		{
		}

		protected override Comparison<CustomListBoxItem> GetComparison()
		{
			return TagListItem.CompareByName;
		}

		protected override CustomListBoxItem<Tag> RepresentObject(Tag obj)
		{
			return new TagListItem(obj);
		}
	}
}
