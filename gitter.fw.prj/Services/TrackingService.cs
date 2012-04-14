namespace gitter.Framework.Services
{
	using System;

	/// <summary>Tracking helper.</summary>
	public class TrackingService
	{
		#region Data

		private int _index;

		#endregion

		#region Events

		/// <summary>Tracking changed.</summary>
		public event EventHandler<TrackingEventArgs> Changed;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="TrackingService"/>.</summary>
		public TrackingService()
		{
			_index = -1;
		}

		/// <summary>Create <see cref="TrackingService"/>.</summary>
		public TrackingService(Action<TrackingEventArgs> onChanged)
			: this()
		{
			Changed += (sender, e) => onChanged(e);
		}

		/// <summary>Create <see cref="TrackingService"/>.</summary>
		public TrackingService(EventHandler<TrackingEventArgs> onChanged)
			: this()
		{
			Changed += onChanged;
		}

		#endregion

		#region Properties

		/// <summary>Index of currently hovered item.</summary>
		public int Index
		{
			get { return _index; }
		}

		/// <summary>Something is hovered.</summary>
		public bool IsTracked
		{
			get { return _index != -1; }
		}

		#endregion

		#region Methods

		/// <summary>Track element.</summary>
		/// <param name="index">Tracked element index.</param>
		public void Track(int index)
		{
			if(_index != index)
			{
				if(_index != -1)
				{
					var args = new TrackingEventArgs(false, _index);
					_index = -1;
					Changed.Raise(this, args);
				}
				if(index != -1)
				{
					_index = index;
					var args = new TrackingEventArgs(true, _index);
					Changed.Raise(this, args);
				}
			}
		}

		/// <summary>Stop tracking.</summary>
		public void Drop()
		{
			if(_index != -1)
			{
				var args = new TrackingEventArgs(false, _index);
				_index = -1;
				Changed.Raise(this, args);
			}
		}

		/// <summary>Notify about item index change.</summary>
		/// <param name="index">New tracked item index.</param>
		public void ResetIndex(int index)
		{
			_index = index;
		}

		#endregion
	}

	/// <summary>Tracking helper.</summary>
	/// <typeparam name="T">Type of tracked element.</typeparam>
	public class TrackingService<T>
	{
		#region Data

		private T _item;
		private int _index;
		private int _partId;

		#endregion

		#region Events

		/// <summary>Tracking changed.</summary>
		public event EventHandler<TrackingEventArgs<T>> Changed;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="TrackingService&lt;T&gt;"/>.</summary>
		public TrackingService()
		{
			_index = -1;
		}

		/// <summary>Create <see cref="TrackingService&lt;T&gt;"/>.</summary>
		public TrackingService(Action<TrackingEventArgs<T>> onChanged)
			: this()
		{
			Changed += (sender, e) => onChanged(e);
		}

		/// <summary>Create <see cref="TrackingService&lt;T&gt;"/>.</summary>
		public TrackingService(EventHandler<TrackingEventArgs<T>> onChanged)
			: this()
		{
			Changed += onChanged;
		}

		#endregion

		#region Properties

		/// <summary>Index of currently tracked item.</summary>
		public int Index
		{
			get { return _index; }
		}

		/// <summary>Currently tracked item.</summary>
		public T Item
		{
			get { return _item; }
		}

		/// <summary>Tracked item part id.</summary>
		public int PartId
		{
			get { return _partId; }
		}

		/// <summary>Something is tracked.</summary>
		public bool IsTracked
		{
			get { return _index != -1; }
		}

		#endregion

		#region Methods

		/// <summary>Track element.</summary>
		/// <param name="index">Tracked element index.</param>
		/// <param name="element">Tracked element.</param>
		public void Track(int index, T element)
		{
			Track(index, element, 0);
		}

		/// <summary>Track element.</summary>
		/// <param name="index">Tracked element index.</param>
		/// <param name="element">Tracked element.</param>
		/// <param name="partId">Tracked part id.</param>
		public void Track(int index, T element, int partId)
		{
			if(_index != index || _partId != partId)
			{
				if(_index != -1)
				{
					var args = new TrackingEventArgs<T>(false, _index, _item);
					_index = -1;
					_partId = 0;
					_item = default(T);
					Changed.Raise(this, args);
				}
				if(index != -1)
				{
					_index = index;
					_item = element;
					_partId = partId;
					var args = new TrackingEventArgs<T>(true, _index, _item);
					Changed.Raise(this, args);
				}
			}
		}

		/// <summary>Notify about item change.</summary>
		/// <param name="index">New tracked item index.</param>
		/// <param name="item">New tracked item.</param>
		public void Reset(int index, T item)
		{
			_index = index;
			_item = item;
		}

		/// <summary>Notify about item index change.</summary>
		/// <param name="index">New tracked item index.</param>
		public void ResetIndex(int index)
		{
			_index = index;
		}

		/// <summary>Notify about item change.</summary>
		/// <param name="item">New tracked item.</param>
		public void ResetItem(T item)
		{
			_item = item;
		}

		/// <summary>Stop tracking.</summary>
		public void Drop()
		{
			if(_index != -1)
			{
				var args = new TrackingEventArgs<T>(false, _index, _item);
				_index = -1;
				_partId = 0;
				_item = default(T);
				Changed.Raise(this, args);
			}
		}

		#endregion
	}
}
