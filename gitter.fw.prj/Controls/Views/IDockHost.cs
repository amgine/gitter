namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Object which can dock <see cref="ViewHost"/>.</summary>
	public interface IDockHost
	{
		/// <summary>Provides dock helper markers to determine dock position (<see cref="DockResult"/>).</summary>
		IDockMarkerProvider DockMarkers { get; }

		/// <summary>Determines if <see cref="ViewHost"/> cn be docked into this <see cref="IDockHost"/>.</summary>
		/// <param name="host"><see cref="ViewHost"/> to dock.</param>
		/// <param name="dockResult">Position for docking.</param>
		/// <returns>true if docking is possible.</returns>
		bool CanDock(ViewHost host, DockResult dockResult);

		/// <summary>Docks <paramref name="host"/> into this <see cref="IDockHost"/>.</summary>
		/// <param name="host"><see cref="ViewHost"/> to dock.</param>
		/// <param name="dockResult">Position for docking.</param>
		void PerformDock(ViewHost host, DockResult dockResult);

		/// <summary>Get bounding rectangle for docked tool.</summary>
		/// <param name="host">Tested <see cref="ViewHost"/>.</param>
		/// <param name="dockResult">Position for docking.</param>
		/// <returns>Bounding rectangle for docked tool.</returns>
		Rectangle GetDockBounds(ViewHost host, DockResult dockResult);
	}
}
