namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using gitter.Framework.Controls;

	public abstract class RevisionPointerListItemBase<T> : CustomListBoxItem<T>, IRevisionPointerListItem
		where T : IRevisionPointer
	{
		protected RevisionPointerListItemBase(T data)
			: base(data)
		{
		}

		#region IRevisionPointerListItem Members

		IRevisionPointer IRevisionPointerListItem.RevisionPointer
		{
			get { return DataContext; }
		}

		#endregion
	}
}
