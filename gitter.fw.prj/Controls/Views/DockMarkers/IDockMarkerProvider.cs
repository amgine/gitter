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
		void Hide();

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
