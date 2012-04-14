namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	/// <summary>Interface for interacting with dock markers.</summary>
	public interface IDockMarkerProvider : IEnumerable<DockMarker>
	{
		/// <summary>Gets a value indicating whether markers are visible.</summary>
		/// <value><c>true</c> if markers are visible; otherwise, <c>false</c>.</value>
		bool MarkersVisible { get; }

		/// <summary>Shows markers to assist docking process.</summary>
		/// <param name="dockClient">Tool host which is being docked.</param>
		void Show(ViewHost dockClient);

		/// <summary>Hides and disposes all dock markers associated with this instance.</summary>
		void Kill();

		/// <summary>Updates hover status of dock markers.</summary>
		/// <returns>true if mouse is hovering docking button.</returns>
		bool UpdateHover();

		/// <summary>Updates hover status of dock markers.</summary>
		/// <param name="position">Mouse position.</param>
		/// <returns>true if mouse is hovering docking button.</returns>
		bool UpdateHover(Point position);

		/// <summary>Notifies that mouse no longer hovers any docking markers associated with this instance.</summary>
		void Unhover();

		/// <summary>Checks docking position at current mose position.</summary>
		/// <returns>Position for docking client control.</returns>
		DockResult HitTest();

		/// <summary>Checks docking position at specified <paramref name="position"/>.</summary>
		/// <param name="position">Mouse position.</param>
		/// <returns>Position for docking client control.</returns>
		DockResult HitTest(Point position);
	}
}
