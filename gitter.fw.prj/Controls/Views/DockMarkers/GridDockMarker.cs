namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	sealed class GridDockMarker : DockMarker
	{
		private static readonly Point[] Border = new Point[]
		{
			new Point(0, 0),
			new Point(39, 0),
			new Point(39, 39),
			new Point(0, 39),
		};

		private static Rectangle GetPositionBounds(ViewDockGrid grid, AnchorStyles side)
		{
			var bounds = grid.RectangleToScreen(grid.ClientRectangle);
			int cx, cy;
			switch(side)
			{
				case AnchorStyles.Left:
					cx = bounds.X + 20 + ViewConstants.Spacing;
					cy = bounds.Y + bounds.Height / 2;
					break;
				case AnchorStyles.Top:
					cx = bounds.X + bounds.Width / 2;
					cy = bounds.Y + 20 + ViewConstants.Spacing;
					break;
				case AnchorStyles.Right:
					cx = bounds.X + bounds.Width - 20 - ViewConstants.Spacing;
					cy = bounds.Y + bounds.Height / 2;
					break;
				case AnchorStyles.Bottom:
					cx = bounds.X + bounds.Width / 2;
					cy = bounds.Y + bounds.Height - 20 - ViewConstants.Spacing;
					break;
				default:
					throw new ArgumentException("side");
			}
			return new Rectangle(cx - 20, cy - 20, 40, 40);
		}

		private static DockMarkerButton GetButton(AnchorStyles side)
		{
			var bounds = new Rectangle(4, 4, 32, 32);
			switch(side)
			{
				case AnchorStyles.Left:
					return new DockMarkerButton(bounds, DockResult.Left);
				case AnchorStyles.Top:
					return new DockMarkerButton(bounds, DockResult.Top);
				case AnchorStyles.Right:
					return new DockMarkerButton(bounds, DockResult.Right);
				case AnchorStyles.Bottom:
					return new DockMarkerButton(bounds, DockResult.Bottom);
				default:
					throw new ArgumentException("side");
			}
		}

		private readonly ViewDockGrid _grid;
		private readonly AnchorStyles _side;

		public GridDockMarker(ViewDockGrid grid, ViewHost viewHost, AnchorStyles side)
			: base(grid, viewHost, new[] { GetButton(side) }, Border, GetPositionBounds(grid, side))
		{
			_grid = grid;
			_side = side;
		}

		public ViewDockGrid Grid
		{
			get { return _grid; }
		}

		public AnchorStyles Side
		{
			get { return _side; }
		}
	}
}
