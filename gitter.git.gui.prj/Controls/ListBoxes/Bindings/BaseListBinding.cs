#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	public abstract class BaseListBinding<TObject, TEventArgs> : IDisposable
		where TObject : GitNamedObjectWithLifetime
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
			Verify.Argument.IsNotNull(itemHost, "itemHost");
			Verify.Argument.IsNotNull(collection, "collection");

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
