#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Controls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

#nullable enable

/// <summary>Manages collection of docking markers.</summary>
/// <typeparam name="T">Type of dock marker.</typeparam>
abstract class DockMarkers<T> : IDockMarkerProvider, IDisposable
	where T : DockMarker
{
	private T[]? _markers;

	/// <summary>Initializes a new instance of the <see cref="DockMarkers{T}"/> class.</summary>
	protected DockMarkers()
	{
	}

	/// <summary>Creates docking markers.</summary>
	/// <param name="dockClient">Docking client.</param>
	/// <returns>Array of created markers.</returns>
	protected abstract T[]? CreateMarkers(ViewHost dockClient);

	public T this[int index]
	{
		get
		{
			if(_markers is null || index < 0 || index >= _markers.Length) throw new IndexOutOfRangeException();

			return _markers[index];
		}
	}

	/// <summary>Gets the count of dock markers.</summary>
	/// <value>Dock marker count.</value>
	public int Count => _markers is not null ? _markers.Length : 0;

	/// <summary>Gets a value indicating whether markers are visible.</summary>
	/// <value><c>true</c> if markers are visible; otherwise, <c>false</c>.</value>
	public bool MarkersVisible => _markers is not null;

	/// <summary>Shows markers to assist docking process.</summary>
	/// <param name="dockClient">View host which is being docked.</param>
	public void Show(ViewHost dockClient)
	{
		Verify.Argument.IsNotNull(dockClient);
		Verify.State.IsFalse(MarkersVisible);

		_markers = CreateMarkers(dockClient);
		if(_markers is not null)
		{
			foreach(var marker in _markers)
			{
				marker.Show();
			}
		}
	}

	/// <summary>Updates hover status of dock markers.</summary>
	/// <returns>true if mouse is hovering docking button.</returns>
	public bool UpdateHover() => UpdateHover(Control.MousePosition);

	/// <summary>
	/// Updates hover status of dock markers.
	/// </summary>
	/// <param name="position">Mouse position.</param>
	/// <returns>
	/// true if mouse is hovering docking button.
	/// </returns>
	public bool UpdateHover(Point position)
	{
		bool result = false;
		if(_markers is not null)
		{
			foreach(var marker in _markers)
			{
				if(marker.UpdateHover(position))
				{
					result = true;
				}
			}
		}
		return result;
	}

	/// <summary>
	/// Notifies that mouse no longer hovers any docking markers associated with this instance.
	/// </summary>
	public void Unhover()
	{
		if(_markers is not null)
		{
			foreach(var marker in _markers)
			{
				marker.Unhover();
			}
		}
	}

	/// <summary>Hides and disposes all dock markers.</summary>
	public void Hide()
	{
		if(_markers is not null)
		{
			foreach(var marker in _markers)
			{
				marker.Dispose();
			}
			_markers = null;
		}
	}

	/// <summary>
	/// Checks docking position at current mouse position.
	/// </summary>
	/// <returns>Position for docking client control.</returns>
	public DockResult HitTest() => HitTest(Control.MousePosition);

	/// <summary>
	/// Checks docking position at specified <paramref name="position"/>.
	/// </summary>
	/// <param name="position">Mouse position.</param>
	/// <returns>Position for docking client control.</returns>
	public DockResult HitTest(Point position)
	{
		if(_markers is not null)
		{
			foreach(var marker in _markers)
			{
				var test = marker.HitTest(position);
				if(test != DockResult.None) return test;
			}
		}
		return DockResult.None;
	}

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>
	/// An <see cref="T:System.Collections.IEnumerator&lt;DockMarker&gt;"/> object that can be used to iterate through the collection.
	/// </returns>
	public IEnumerator<DockMarker> GetEnumerator()
		=> _markers is not null
			? ((IEnumerable<DockMarker>)_markers).GetEnumerator()
			: System.Linq.Enumerable.Empty<DockMarker>().GetEnumerator();

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>
	/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
	/// </returns>
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		=> GetEnumerator();

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	/// </summary>
	public void Dispose() => Hide();
}
