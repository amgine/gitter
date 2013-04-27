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
	using System.Windows.Forms;

	sealed class GridDockMarkers : DockMarkers<GridDockMarker>
	{
		private readonly ViewDockGrid _grid;

		public GridDockMarkers(ViewDockGrid grid)
		{
			_grid = grid;
		}

		public ViewDockGrid Grid
		{
			get { return _grid; }
		}

		/// <summary>Creates the markers.</summary>
		/// <param name="dockClient">The dock client.</param>
		/// <returns>Created dock markers.</returns>
		protected override GridDockMarker[] CreateMarkers(ViewHost dockClient)
		{
			if(dockClient.IsDocumentWell || (dockClient.ViewsCount == 1 && dockClient.GetView(0).IsDocument))
			{
				return null;
			}
			else
			{
				return new GridDockMarker[]
				{
					new GridDockMarker(_grid, dockClient, AnchorStyles.Left),
					new GridDockMarker(_grid, dockClient, AnchorStyles.Top),
					new GridDockMarker(_grid, dockClient, AnchorStyles.Right),
					new GridDockMarker(_grid, dockClient, AnchorStyles.Bottom),
				};
			}
		}
	}
}
