﻿#region Copyright Notice
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

#nullable enable

/// <summary>Dock markers of <see cref="ViewHost"/>.</summary>
sealed class ViewHostDockMarkers : DockMarkers<ViewHostDockMarker>
{
	/// <summary>Initializes a new instance of the <see cref="ViewHostDockMarkers"/> class.</summary>
	/// <param name="dockHost"><see cref="ViewHost"/> which is the source of dock markers.</param>
	public ViewHostDockMarkers(ViewHost dockHost)
	{
		Verify.Argument.IsNotNull(dockHost);

		ViewHost = dockHost;
	}

	/// <summary><see cref="ViewHost"/> which is the source of dock markers.</summary>
	/// <value>Source of dock markers.</value>
	public ViewHost ViewHost { get; }

	/// <summary>Creates the markers.</summary>
	/// <param name="dockClient">The dock client.</param>
	/// <returns>Created markers.</returns>
	protected override ViewHostDockMarker[]? CreateMarkers(ViewHost dockClient)
		=> ViewHost.IsDocumentWell || !dockClient.IsDocumentWell
			? new[] { new ViewHostDockMarker(ViewHost, dockClient) }
			: default;
}
