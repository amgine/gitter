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

namespace gitter.Framework.Services;

using System;

/// <summary>Tracking helper.</summary>
public class TrackingService
{
	#region Data

	private int _index = -1;

	#endregion

	#region Events

	/// <summary>Tracking changed.</summary>
	public event EventHandler<TrackingEventArgs>? Changed;

	#endregion

	#region .ctor

	/// <summary>Create <see cref="TrackingService"/>.</summary>
	public TrackingService()
	{
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
	public int Index => _index;

	/// <summary>Something is hovered.</summary>
	public bool IsTracked => _index != -1;

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
				Changed?.Invoke(this, args);
			}
			if(index != -1)
			{
				_index = index;
				var args = new TrackingEventArgs(true, _index);
				Changed?.Invoke(this, args);
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
			Changed?.Invoke(this, args);
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

	private T? _item;
	private int _index;
	private int _partId;

	#endregion

	#region Events

	/// <summary>Tracking changed.</summary>
	public event EventHandler<TrackingEventArgs<T>>? Changed;

	#endregion

	#region .ctor

	/// <summary>Create <see cref="TrackingService{T}"/>.</summary>
	public TrackingService()
	{
		_index = -1;
	}

	/// <summary>Create <see cref="TrackingService{T}"/>.</summary>
	public TrackingService(Action<TrackingEventArgs<T>> onChanged)
		: this()
	{
		Changed += (sender, e) => onChanged(e);
	}

	/// <summary>Create <see cref="TrackingService{T}"/>.</summary>
	public TrackingService(EventHandler<TrackingEventArgs<T>> onChanged)
		: this()
	{
		Changed += onChanged;
	}

	#endregion

	#region Properties

	/// <summary>Index of currently tracked item.</summary>
	public int Index =>  _index;

	/// <summary>Currently tracked item.</summary>
	public T? Item => _item;

	/// <summary>Tracked item part id.</summary>
	public int PartId => _partId;

	/// <summary>Something is tracked.</summary>
	public bool IsTracked => _index != -1;

	#endregion

	#region Methods

	/// <summary>Track element.</summary>
	/// <param name="index">Tracked element index.</param>
	/// <param name="element">Tracked element.</param>
	public void Track(int index, T? element)
	{
		Track(index, element, 0);
	}

	/// <summary>Track element.</summary>
	/// <param name="index">Tracked element index.</param>
	/// <param name="element">Tracked element.</param>
	/// <param name="partId">Tracked part id.</param>
	public void Track(int index, T? element, int partId)
	{
		if(_index != index || _partId != partId)
		{
			if(_index != -1)
			{
				var args = new TrackingEventArgs<T>(false, _index, _item);
				_index = -1;
				_partId = 0;
				_item = default;
				Changed?.Invoke(this, args);
			}
			if(index != -1)
			{
				_index = index;
				_item = element;
				_partId = partId;
				var args = new TrackingEventArgs<T>(true, _index, _item);
				Changed?.Invoke(this, args);
			}
		}
	}

	/// <summary>Notify about item change.</summary>
	/// <param name="index">New tracked item index.</param>
	/// <param name="item">New tracked item.</param>
	public void Reset(int index, T? item)
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
			Changed?.Invoke(this, args);
		}
	}

	#endregion
}
