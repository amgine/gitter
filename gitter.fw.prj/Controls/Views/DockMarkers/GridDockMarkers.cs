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
