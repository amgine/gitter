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
using System.Drawing;
using System.Windows.Forms;

sealed class DockPanelDockMarker : DockMarker
{
	private static readonly IDpiBoundValue<Point[]> Border = new ScalablePoints(
	[
		new( 0,  0),
		new(39,  0),
		new(39, 39),
		new( 0, 39),
	]);

	private static Rectangle GetPositionBounds(DockPanel dockPanel, AnchorStyles side)
	{
		var dpi  = Dpi.FromControl(dockPanel);
		var conv = DpiConverter.FromDefaultTo(dpi);

		int d = conv.ConvertX(20);

		var bounds = dockPanel.RectangleToScreen(dockPanel.ClientRectangle);
		int cx, cy;
		switch(side)
		{
			case AnchorStyles.Left:
				cx = bounds.X + d + ViewConstants.Spacing.GetValue(dpi).Width;
				cy = bounds.Y + bounds.Height / 2;
				break;
			case AnchorStyles.Top:
				cx = bounds.X + bounds.Width / 2;
				cy = bounds.Y + d + ViewConstants.Spacing.GetValue(dpi).Height;
				break;
			case AnchorStyles.Right:
				cx = bounds.X + bounds.Width - d - ViewConstants.Spacing.GetValue(dpi).Width;
				cy = bounds.Y + bounds.Height / 2;
				break;
			case AnchorStyles.Bottom:
				cx = bounds.X + bounds.Width / 2;
				cy = bounds.Y + bounds.Height - d - ViewConstants.Spacing.GetValue(dpi).Height;
				break;
			default:
				throw new ArgumentException(
					$"Unknown AnchorStyles value: {side}.", nameof(side));
		}
		return new Rectangle(cx - d, cy - d, d * 2, d * 2);
	}

	private static DockMarkerButton GetButton(AnchorStyles side, Dpi dpi)
	{
		var bounds = DpiConverter.FromDefaultTo(dpi).Convert(new Rectangle(4, 4, 32, 32));
		return side switch
		{
			AnchorStyles.Left   => new DockMarkerButton(bounds, DockResult.Left),
			AnchorStyles.Top    => new DockMarkerButton(bounds, DockResult.Top),
			AnchorStyles.Right  => new DockMarkerButton(bounds, DockResult.Right),
			AnchorStyles.Bottom => new DockMarkerButton(bounds, DockResult.Bottom),
			_ => throw new ArgumentException($"Unknown AnchorStyles value: {side}", nameof(side)),
		};
	}

	public DockPanelDockMarker(DockPanel dockPanel, ViewHost viewHost, AnchorStyles side)
		: base(dockPanel, viewHost, new[] { GetButton(side, Dpi.FromControl(dockPanel)) }, Border, GetPositionBounds(dockPanel, side))
	{
		DockPanel = dockPanel;
		Side = side;
	}

	public DockPanel DockPanel { get; }

	public AnchorStyles Side { get; }
}
