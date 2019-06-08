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
					throw new ArgumentException(
						"Unknown AnchorStyles value: {0}".UseAsFormat(side),
						"side");
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
					throw new ArgumentException(
						"Unknown AnchorStyles value: {0}".UseAsFormat(side),
						"side");
			}
		}

		public GridDockMarker(ViewDockGrid grid, ViewHost viewHost, AnchorStyles side)
			: base(grid, viewHost, new[] { GetButton(side) }, Border, GetPositionBounds(grid, side))
		{
			Grid = grid;
			Side = side;
		}

		public ViewDockGrid Grid { get; }

		public AnchorStyles Side { get; }
	}
}
