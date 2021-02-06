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

namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Object which can dock <see cref="ViewHost"/>.</summary>
	public interface IDockHost
	{
		/// <summary>Provides dock helper markers to determine dock position (<see cref="DockResult"/>).</summary>
		IDockMarkerProvider DockMarkers { get; }

		/// <summary>Determines if <see cref="ViewHost"/> can be docked into this <see cref="IDockHost"/>.</summary>
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
