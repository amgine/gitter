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

namespace gitter.Git.Gui.Controls;

using System;
using System.Windows.Forms;

using gitter.Framework.Controls;

public abstract class BaseListBinding<TObject, TEventArgs> : IDisposable
	where TObject    : GitNamedObjectWithLifetime
	where TEventArgs : ObjectEventArgs<TObject>
{
	#region Events

	public event EventHandler<BoundItemActivatedEventArgs<TObject>>? ItemActivated;

	private void InvokeItemActivated(CustomListBoxItem item, TObject obj)
		=> ItemActivated?.Invoke(this, new BoundItemActivatedEventArgs<TObject>(item, obj));

	#endregion

	#region .ctor

	protected BaseListBinding(CustomListBoxItemsCollection itemHost, GitObjectsCollection<TObject, TEventArgs> collection)
	{
		Verify.Argument.IsNotNull(itemHost);
		Verify.Argument.IsNotNull(collection);

		Target = itemHost;
		Source = collection;

		Target.Comparison = GetComparison();
		Target.SortOrder = GetSortOrder();

		lock(collection.SyncRoot)
		{
			foreach(var obj in collection)
			{
				var item = RepresentObject(obj);
				item.Activated += OnItemActivated;
				Target.Add(item);
			}
			collection.ObjectAdded += OnObjectAdded;
		}
	}

	#endregion

	#region Virtual

	protected virtual SortOrder GetSortOrder() => SortOrder.Ascending;

	#endregion

	#region Abstract

	protected abstract CustomListBoxItem<TObject> RepresentObject(TObject obj);

	protected abstract Comparison<CustomListBoxItem> GetComparison();

	#endregion

	#region Properties

	public GitObjectsCollection<TObject, TEventArgs> Source { get; }

	public CustomListBoxItemsCollection Target { get; }

	#endregion

	#region Event Handlers

	private void OnObjectAdded(object? sender, TEventArgs e)
	{
		var item = RepresentObject(e.Object);
		item.Activated += OnItemActivated;
		Target.AddSafe(item);
	}

	private void OnItemActivated(object? sender, EventArgs e)
	{
		var item = (CustomListBoxItem<TObject>)sender!;
		InvokeItemActivated(item, item.DataContext);
	}

	#endregion

	#region IDisposable

	public void Dispose()
	{
		Source.ObjectAdded -= OnObjectAdded;
		Target.Clear();
	}

	#endregion
}

public abstract class BaseListBinding<TKey, TObject, TEventArgs> : IDisposable
	where TKey       : notnull
	where TObject    : GitNamedObjectWithLifetime
	where TEventArgs : ObjectEventArgs<TObject>
{
	#region Events

	public event EventHandler<BoundItemActivatedEventArgs<TObject>>? ItemActivated;

	private void InvokeItemActivated(CustomListBoxItem item, TObject obj)
		=> ItemActivated?.Invoke(this, new BoundItemActivatedEventArgs<TObject>(item, obj));

	#endregion

	#region .ctor

	protected BaseListBinding(CustomListBoxItemsCollection itemHost, GitObjectsCollection<TKey, TObject, TEventArgs> collection)
	{
		Verify.Argument.IsNotNull(itemHost);
		Verify.Argument.IsNotNull(collection);

		Target = itemHost;
		Source = collection;

		Target.Comparison = GetComparison();
		Target.SortOrder = GetSortOrder();

		lock(collection.SyncRoot)
		{
			foreach(var obj in collection)
			{
				var item = RepresentObject(obj);
				item.Activated += OnItemActivated;
				Target.Add(item);
			}
			collection.ObjectAdded += OnObjectAdded;
		}
	}

	#endregion

	#region Virtual

	protected virtual SortOrder GetSortOrder() => SortOrder.Ascending;

	#endregion

	#region Abstract

	protected abstract CustomListBoxItem<TObject> RepresentObject(TObject obj);

	protected abstract Comparison<CustomListBoxItem> GetComparison();

	#endregion

	#region Properties

	public GitObjectsCollection<TKey, TObject, TEventArgs> Source { get; }

	public CustomListBoxItemsCollection Target { get; }

	#endregion

	#region Event Handlers

	private void OnObjectAdded(object? sender, TEventArgs e)
	{
		var item = RepresentObject(e.Object);
		item.Activated += OnItemActivated;
		Target.AddSafe(item);
	}

	private void OnItemActivated(object? sender, EventArgs e)
	{
		var item = (CustomListBoxItem<TObject>)sender!;
		InvokeItemActivated(item, item.DataContext);
	}

	#endregion

	#region IDisposable

	public void Dispose()
	{
		Source.ObjectAdded -= OnObjectAdded;
		Target.Clear();
	}

	#endregion
}
