namespace gitter.Git.Gui.Controls
{
	using gitter.Framework.Controls;

	public abstract class RevisionPointerListItemBase<T> : CustomListBoxItem<T>, IRevisionPointerListItem
		where T : IRevisionPointer
	{
		#region .ctor

		protected RevisionPointerListItemBase(T data)
			: base(data)
		{
		}

		#endregion

		#region IRevisionPointerListItem Members

		IRevisionPointer IRevisionPointerListItem.RevisionPointer
		{
			get { return DataContext; }
		}

		#endregion
	}
}
