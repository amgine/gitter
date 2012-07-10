namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	public abstract class BaseListBinding<TObject, TEventArgs> : IDisposable
		where TObject : GitLifeTimeNamedObject
		where TEventArgs : ObjectEventArgs<TObject>
	{
		#region Data

		private readonly GitObjectsCollection<TObject, TEventArgs> _collection;
		private readonly CustomListBoxItemsCollection _itemHost;

		#endregion

		#region Events

		public event EventHandler<BoundItemActivatedEventArgs<TObject>> ItemActivated;

		private void InvokeItemActivated(CustomListBoxItem item, TObject obj)
		{
			var handler = ItemActivated;
			if(handler != null) handler(this, new BoundItemActivatedEventArgs<TObject>(item, obj));
		}

		#endregion

		#region .ctor

		protected BaseListBinding(CustomListBoxItemsCollection itemHost, GitObjectsCollection<TObject, TEventArgs> collection)
		{
			if(itemHost == null) throw new ArgumentNullException("itemHost");
			if(collection == null) throw new ArgumentNullException("collection");

			_itemHost = itemHost;
			_collection = collection;

			_itemHost.Comparison = GetComparison();
			_itemHost.SortOrder = GetSortOrder();

			lock(collection.SyncRoot)
			{
				foreach(var obj in collection)
				{
					var item = RepresentObject(obj);
					item.Activated += OnItemActivated;
					_itemHost.Add(item);
				}
				collection.ObjectAdded += OnObjectAdded;
			}
		}

		#endregion

		#region Virtual

		protected virtual SortOrder GetSortOrder()
		{
			return SortOrder.Ascending;
		}

		#endregion

		#region Abstract

		protected abstract CustomListBoxItem<TObject> RepresentObject(TObject obj);

		protected abstract Comparison<CustomListBoxItem> GetComparison();

		#endregion

		#region Properties

		public GitObjectsCollection<TObject, TEventArgs> Source
		{
			get { return _collection; }
		}

		public CustomListBoxItemsCollection Target
		{
			get { return _itemHost; }
		}

		#endregion

		#region Event Handlers

		private void OnObjectAdded(object sender, TEventArgs e)
		{
			var item = RepresentObject(e.Object);
			item.Activated += OnItemActivated;
			_itemHost.AddSafe(item);
		}

		private void OnItemActivated(object sender, EventArgs e)
		{
			var item = (CustomListBoxItem<TObject>)sender;
			InvokeItemActivated(item, item.DataContext);
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			_collection.ObjectAdded -= OnObjectAdded;
			_itemHost.Clear();
		}

		#endregion
	}
}
