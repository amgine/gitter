namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;

	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Cached collection of git objects.</summary>
	/// <typeparam name="TObject">The type of the object.</typeparam>
	/// <typeparam name="TEventArgs">The type of the event args.</typeparam>
	public abstract class GitObjectsCollection<TObject, TEventArgs> : GitObject, INotifyCollectionChanged, IEnumerable<TObject>
		where TObject : GitNamedObjectWithLifetime
		where TEventArgs : ObjectEventArgs<TObject>
	{
		#region Data

		/// <summary>Object cache.</summary>
		private readonly Dictionary<string, TObject> _dictionary;

		#endregion

		#region Events

		/// <summary>Occurs when the collection changes.</summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>Occurs when object is added.</summary>
		public event EventHandler<TEventArgs> ObjectAdded;

		/// <summary>Occurs when object is removed.</summary>
		public event EventHandler<TEventArgs> ObjectRemoved;

		/// <summary>Creates the event args, associated with specified <paramref name="item"/>.</summary>
		/// <param name="item">The item.</param>
		/// <returns>Created <typeparamref name="TEventArgs"/>.</returns>
		protected abstract TEventArgs CreateEventArgs(TObject item);

		/// <summary>Invokes <see cref="ObjectAdded"/> and <see cref="CollectionChanged"/> events.</summary>
		/// <param name="item">Added item.</param>
		protected virtual void InvokeObjectAdded(TObject item)
		{
			Verify.Argument.IsNotNull(item, "item");

			var handler1 = ObjectAdded;
			if(handler1 != null)
			{
				handler1(this, CreateEventArgs(item));
			}
			var handler2 = CollectionChanged;
			if(handler2 != null)
			{
				handler2(this, new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add, item));
			}
		}

		/// <summary>Invokes <see cref="ObjectRemoved"/> and <see cref="CollectionChanged"/> events.</summary>
		/// <param name="item">Removed item.</param>
		protected virtual void InvokeObjectRemoved(TObject item)
		{
			Verify.Argument.IsNotNull(item, "item");

			item.MarkAsDeleted();
			var handler = ObjectRemoved;
			if(handler != null)
			{
				handler(this, CreateEventArgs(item));
			}
			var handler2 = CollectionChanged;
			if(handler2 != null)
			{
				handler2(this, new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Remove, item));
			}
		}

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="GitObjectsCollection&lt;TObject, TEventArgs&gt;"/> class.</summary>
		/// <param name="repository">Related repository.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		protected GitObjectsCollection(Repository repository)
			: base(repository, false)
		{
			_dictionary = new Dictionary<string, TObject>();
		}

		#endregion

		/// <summary>Determines whether this collection contains object with the specified <paramref name="name"/>.</summary>
		/// <param name="name">Object name.</param>
		/// <returns>
		/// 	<c>true</c> if this collection contains object with the specified <paramref name="name"/>; otherwise, <c>false</c>.
		/// </returns>
		protected bool ContainsObjectName(string name)
		{
			return _dictionary.ContainsKey(name);
		}

		/// <summary>Gets the internal object storage.</summary>
		/// <value>Internal object storage.</value>
		protected IDictionary<string, TObject> ObjectStorage
		{
			get { return _dictionary; }
		}

		/// <summary>Removes object from this collection.</summary>
		/// <param name="name">Object name.</param>
		protected void RemoveObject(string name)
		{
			lock(SyncRoot)
			{
				TObject item;
				Verify.Argument.IsTrue(
					_dictionary.TryGetValue(name, out item),
					"name", "Object not found.");
				_dictionary.Remove(name);
				InvokeObjectRemoved(item);
			}
		}

		/// <summary>Removes object from this collection.</summary>
		/// <param name="item">Object to remove.</param>
		protected virtual void RemoveObject(TObject item)
		{
			lock(SyncRoot)
			{
				_dictionary.Remove(item.Name);
				InvokeObjectRemoved(item);
			}
		}

		/// <summary>Adds object to this collection.</summary>
		/// <param name="item">Object to add.</param>
		protected virtual void AddObject(TObject item)
		{
			lock(SyncRoot)
			{
				_dictionary.Add(item.Name, item);
				InvokeObjectAdded(item);
			}
		}

		/// <summary>Returns non-ambiguous object name.</summary>
		/// <param name="name">Object name.</param>
		/// <returns>Non-ambiguous object name.</returns>
		protected virtual string FixInputName(string name)
		{
			return name;
		}

		/// <summary>Gets the collection of object names.</summary>
		/// <value>Collection of object names.</value>
		public ICollection<string> Names
		{
			get { return _dictionary.Keys; }
		}

		/// <summary>Gets the <see cref="TObject"/> with the specified <paramref name="name"/>.</summary>
		/// <value><see cref="TObject"/> with the specified <paramref name="name"/>.</value>
		public TObject this[string name]
		{
			get { lock(SyncRoot) return _dictionary[FixInputName(name)]; }
		}

		/// <summary>Gets object count.</summary>
		/// <value>Object count.</value>
		public int Count
		{
			get { lock(SyncRoot) return _dictionary.Count; }
		}

		/// <summary>Gets a value indicating whether this collection is empty.</summary>
		/// <value><c>true</c> if this collection is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty
		{
			get { lock(SyncRoot) return _dictionary.Count == 0; }
		}

		/// <summary>Determines whether this collection contains object with the specified <paramref name="name"/>.</summary>
		/// <param name="name">Object name.</param>
		/// <returns>
		/// 	<c>true</c> if this collection contains object with the specified <paramref name="name"/>; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool Contains(string name)
		{
			lock(SyncRoot)
			{
				return _dictionary.ContainsKey(FixInputName(name));
			}
		}

		/// <summary>Get object with the specified <paramref name="name"/>.</summary>
		/// <param name="name">Object name.</param>
		/// <returns>
		/// Object with the specified <paramref name="name"/> or
		/// <c>default(<typeparamref name="TObject"/>)</c>.
		/// </returns>
		public TObject TryGetItem(string name)
		{
			lock(SyncRoot)
			{
				TObject value;
				if(_dictionary.TryGetValue(FixInputName(name), out value))
				{
					return value;
				}
			}
			return default(TObject);
		}

		/// <summary>Get object with the specified <paramref name="name"/>.</summary>
		/// <param name="name">Object name.</param>
		/// <param name="value">Object with the specified <paramref name="name"/> or <c>default(<typeparamref name="TObject"/>)</c>.</param>
		/// <returns><c>true</c> if this collection contains object with the specified <paramref name="name"/>; otherwise, <c>false</c>.</returns>
		public bool TryGetItem(string name, out TObject value)
		{
			lock(SyncRoot) return _dictionary.TryGetValue(FixInputName(name), out value);
		}

		/// <summary>Gets the sync root object.</summary>
		/// <value>The sync root object.</value>
		public object SyncRoot
		{
			get { return _dictionary; }
		}

		#region Notify()

		/// <summary>Notifies that object was removed externally.</summary>
		/// <param name="item">Removed object.</param>
		internal void NotifyRemoved(TObject item)
		{
			Verify.Argument.IsNotNull(item, "item");

			RemoveObject(item);
		}

		/// <summary>Notifies that object was removed externally.</summary>
		/// <param name="name">Removed object name.</param>
		internal void NotifyRemoved(string name)
		{
			Verify.Argument.IsNotNull(name, "name");

			RemoveObject(name);
		}

		#endregion

		#region IEnumerable<T>

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator&lt;TObject&gt;"/> object that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<TObject> GetEnumerator()
		{
			return _dictionary.Values.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _dictionary.Values.GetEnumerator();
		}

		#endregion
	}
}
