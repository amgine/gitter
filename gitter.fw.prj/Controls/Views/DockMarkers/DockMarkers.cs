namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	/// <summary>Manages collection of docking markers.</summary>
	/// <typeparam name="T">Type of dock marker.</typeparam>
	abstract class DockMarkers<T> : IDockMarkerProvider, IDisposable
		where T : DockMarker
	{
		private T[] _markers;

		/// <summary>Initializes a new instance of the <see cref="DockMarkers&lt;T&gt;"/> class.</summary>
		protected DockMarkers()
		{
		}

		/// <summary>Creates docking markers.</summary>
		/// <param name="dockClient">Docking client.</param>
		/// <returns>Array of created markers.</returns>
		protected abstract T[] CreateMarkers(ViewHost dockClient);

		public T this[int index]
		{
			get { return _markers[index]; }
		}

		/// <summary>Gets the count of dock markers.</summary>
		/// <value>Dock marker count.</value>
		public int Count
		{
			get { return _markers != null ? _markers.Length : 0; }
		}

		/// <summary>Gets a value indicating whether markers are visible.</summary>
		/// <value><c>true</c> if markers are visible; otherwise, <c>false</c>.</value>
		public bool MarkersVisible
		{
			get { return _markers != null; }
		}

		/// <summary>Shows markers to assist docking process.</summary>
		/// <param name="dockClient">View host which is being docked.</param>
		public void Show(ViewHost dockClient)
		{
			Verify.Argument.IsNotNull(dockClient, "dockClient");
			Verify.State.IsFalse(MarkersVisible);

			_markers = CreateMarkers(dockClient);
			if(_markers != null)
			{
				for(int i = 0; i < _markers.Length; ++i)
				{
					_markers[i].Show();
				}
			}
		}

		/// <summary>Updates hover status of dock markers.</summary>
		/// <returns>true if mouse is hovering docking button.</returns>
		public bool UpdateHover()
		{
			return UpdateHover(Control.MousePosition);
		}

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
			if(MarkersVisible)
			{
				for(int i = 0; i < _markers.Length; ++i)
				{
					if(_markers[i].UpdateHover(position))
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
			if(MarkersVisible)
			{
				for(int i = 0; i < _markers.Length; ++i)
				{
					_markers[i].Unhover();
				}
			}
		}

		/// <summary>Hides and disposes all dock markers.</summary>
		public void Hide()
		{
			if(MarkersVisible)
			{
				for(int i = 0; i < _markers.Length; ++i)
				{
					_markers[i].Dispose();
				}
				_markers = null;
			}
		}

		/// <summary>
		/// Checks docking position at current mose position.
		/// </summary>
		/// <returns>Position for docking client control.</returns>
		public DockResult HitTest()
		{
			return HitTest(Control.MousePosition);
		}

		/// <summary>
		/// Checks docking position at specified <paramref name="position"/>.
		/// </summary>
		/// <param name="position">Mouse position.</param>
		/// <returns>Position for docking client control.</returns>
		public DockResult HitTest(Point position)
		{
			if(MarkersVisible)
			{
				for(int i = 0; i < _markers.Length; ++i)
				{
					var test = _markers[i].HitTest(position);
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
		{
			return ((IEnumerable<DockMarker>)_markers).GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _markers.GetEnumerator();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Hide();
		}
	}
}
