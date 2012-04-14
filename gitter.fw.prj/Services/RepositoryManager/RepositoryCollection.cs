namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	public sealed class RepositoryCollection : Collection<RepositoryLink>
	{
		public event EventHandler<RepositoryLinkEventArgs> Added;
		public event EventHandler<RepositoryLinkEventArgs> Deleted;
		public event EventHandler Cleared;

		protected override void InsertItem(int index, RepositoryLink item)
		{
			base.InsertItem(index, item);

			var handler = Added;
			if(handler != null) handler(this, new RepositoryLinkEventArgs(item));
		}

		protected override void RemoveItem(int index)
		{
			var item = Items[index];
			base.RemoveItem(index);
			item.InvokeDeleted();
			var handler = Deleted;
			if(handler != null) handler(this, new RepositoryLinkEventArgs(item));
		}

		protected override void ClearItems()
		{
			base.ClearItems();

			var handler = Cleared;
			if(handler != null) handler(this, EventArgs.Empty);
		}
	}
}
