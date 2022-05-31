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
using System.Windows.Forms;

sealed class DockPanelDockMarkers : DockMarkers<DockPanelDockMarker>
{
	public DockPanelDockMarkers(DockPanel dockPanel)
	{
		Assert.IsNotNull(dockPanel);

		DockPanel = dockPanel;
	}

	public DockPanel DockPanel { get; }

	/// <summary>Creates the markers.</summary>
	/// <param name="dockClient">The dock client.</param>
	/// <returns>Created dock markers.</returns>
	protected override DockPanelDockMarker[]? CreateMarkers(ViewHost dockClient)
	{
		if(dockClient.IsDocumentWell) return default;
		return new[]
		{
			new DockPanelDockMarker(DockPanel, dockClient, AnchorStyles.Left),
			new DockPanelDockMarker(DockPanel, dockClient, AnchorStyles.Top),
			new DockPanelDockMarker(DockPanel, dockClient, AnchorStyles.Right),
			new DockPanelDockMarker(DockPanel, dockClient, AnchorStyles.Bottom),
		};
	}
}
