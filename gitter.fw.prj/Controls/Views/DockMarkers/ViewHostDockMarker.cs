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
using System.Drawing.Drawing2D;

sealed class ViewHostDockMarker : DockMarker
{
	private static readonly IDpiBoundValue<Point[]> SmallCrossPolygon = new ScalablePoints(new[]
	{
		new Point(36, 25), new Point(36, 0),
		new Point(75, 0), new Point(75, 25),

		new Point(86, 36), new Point(111, 36),
		new Point(111, 75), new Point(86, 75),

		new Point(75, 86), new Point(75, 111),
		new Point(36, 111), new Point(36, 86),

		new Point(25, 75), new Point(0, 75),
		new Point(0, 36), new Point(25, 36),
	});

	private static readonly IDpiBoundValue<Point[]> RegionSmallCrossPolygon = new ScalablePoints(new[]
	{
		new Point(36, 25), new Point(36, 0),
		new Point(76, 0), new Point(76, 25),

		new Point(86, 36), new Point(112, 36),
		new Point(112, 76), new Point(86, 76),

		new Point(76, 86), new Point(76, 112),
		new Point(36, 112), new Point(36, 86),

		new Point(26, 76), new Point(0, 76),
		new Point(0, 36), new Point(25, 36),
	});

	private static readonly IDpiBoundValue<Point[]> LargeCrossPolygon = new ScalablePoints(new[]
	{
		new Point(72, 62), new Point(72, 0),
		new Point(111, 0), new Point(111, 62),

		new Point(121, 72), new Point(183, 72),
		new Point(183, 111), new Point(122, 111),

		new Point(111, 122), new Point(111, 183),
		new Point(72, 183), new Point(72, 121),

		new Point(62, 111), new Point(0, 111),
		new Point(0, 72), new Point(62, 72),
	});

	private static readonly IDpiBoundValue<Point[]> RegionLargeCrossPolygon = new ScalablePoints(new[]
	{
		new Point(72, 62), new Point(72, 0),
		new Point(112, 0), new Point(112, 62),

		new Point(122, 72), new Point(184, 72),
		new Point(184, 112), new Point(122, 112),

		new Point(112, 122), new Point(112, 184),
		new Point(72, 184), new Point(72, 122),

		new Point(63, 112), new Point(0, 112),
		new Point(0, 72), new Point(62, 72),
	});

	public ViewHostDockMarker(ViewHost dockHost, ViewHost dockClient)
		: base(dockHost, dockClient, GetButtons(dockHost, dockClient), GetBorder(dockHost, dockClient), GetBounds(dockHost, dockClient))
	{
		Host = dockHost;

		Region = GetRegion(dockHost, dockClient);
		dockHost.Resize += OnHostBoundsChanged;
		dockHost.LocationChanged += OnHostBoundsChanged;
	}

	private static DockMarkerButton[] GetButtons(ViewHost dockHost, ViewHost dockClient)
	{
		var dpi  = Dpi.FromControl(dockHost);
		var conv = DpiConverter.FromDefaultTo(dpi);
		if(dockHost.IsDocumentWell)
		{
			if(dockClient.IsDocumentWell)
			{
				return new[]
				{
					new DockMarkerButton(conv.Convert(new Rectangle(40,  4, 32, 32)), DockResult.DocumentTop),
					new DockMarkerButton(conv.Convert(new Rectangle( 4, 40, 32, 32)), DockResult.DocumentLeft),
					new DockMarkerButton(conv.Convert(new Rectangle(40, 40, 32, 32)), DockResult.Fill),
					new DockMarkerButton(conv.Convert(new Rectangle(76, 40, 32, 32)), DockResult.DocumentRight),
					new DockMarkerButton(conv.Convert(new Rectangle(40, 76, 32, 32)), DockResult.DocumentBottom),
				};
			}
			else
			{
				return new[]
				{
					new DockMarkerButton(conv.Convert(new Rectangle( 76,   4, 32, 32)), DockResult.Top),
					new DockMarkerButton(conv.Convert(new Rectangle( 76,  40, 32, 32)), DockResult.DocumentTop),
					new DockMarkerButton(conv.Convert(new Rectangle(  4,  76, 32, 32)), DockResult.Left),
					new DockMarkerButton(conv.Convert(new Rectangle( 40,  76, 32, 32)), DockResult.DocumentLeft),
					new DockMarkerButton(conv.Convert(new Rectangle( 76,  76, 32, 32)), DockResult.Fill),
					new DockMarkerButton(conv.Convert(new Rectangle(112,  76, 32, 32)), DockResult.DocumentRight),
					new DockMarkerButton(conv.Convert(new Rectangle(148,  76, 32, 32)), DockResult.Right),
					new DockMarkerButton(conv.Convert(new Rectangle( 76, 112, 32, 32)), DockResult.DocumentBottom),
					new DockMarkerButton(conv.Convert(new Rectangle( 76, 148, 32, 32)), DockResult.Bottom),
				};
			}
		}
		else
		{
			return new[]
			{
				new DockMarkerButton(conv.Convert(new Rectangle(40,  4, 32, 32)), DockResult.Top),
				new DockMarkerButton(conv.Convert(new Rectangle( 4, 40, 32, 32)), DockResult.Left),
				new DockMarkerButton(conv.Convert(new Rectangle(40, 40, 32, 32)), DockResult.Fill),
				new DockMarkerButton(conv.Convert(new Rectangle(76, 40, 32, 32)), DockResult.Right),
				new DockMarkerButton(conv.Convert(new Rectangle(40, 76, 32, 32)), DockResult.Bottom),
			};
		}
	}

	private static IDpiBoundValue<Point[]> GetBorder(ViewHost dockHost, ViewHost dockClient)
	{
		if(dockHost.IsDocumentWell)
		{
			return dockClient.IsDocumentWell
				? SmallCrossPolygon
				: LargeCrossPolygon;
		}
		return SmallCrossPolygon;
	}

	private static Rectangle GetBounds(ViewHost dockHost, ViewHost dockClient)
	{
		var loc  = dockHost.PointToScreen(Point.Empty);
		var size = dockHost.Size;
		var dpi  = Dpi.FromControl(dockHost);
		var conv = DpiConverter.FromDefaultTo(dpi);
		if(dockHost.IsDocumentWell)
		{
			if(dockClient.IsDocumentWell)
			{
				var markerSize = conv.Convert(new Size(112, 112));
				return new Rectangle(
					loc.X + (size.Width  - markerSize.Width ) / 2,
					loc.Y + (size.Height - markerSize.Height) / 2,
					markerSize.Width, markerSize.Height);
			}
			else
			{
				var markerSize = conv.Convert(new Size(184, 184));
				return new Rectangle(
					loc.X + (size.Width  - markerSize.Width)  / 2,
					loc.Y + (size.Height - markerSize.Height) / 2,
					markerSize.Width, markerSize.Height);
			}
		}
		else
		{
			var markerSize = conv.Convert(new Size(112, 112));
			return new Rectangle(
				loc.X + (size.Width  - markerSize.Width)  / 2,
				loc.Y + (size.Height - markerSize.Height) / 2,
				markerSize.Width, markerSize.Height);
		}
	}

	private static IDpiBoundValue<Point[]> GetRegionPolygon(ViewHost dockHost, ViewHost dockClient)
	{
		if(dockHost.IsDocumentWell)
		{
			return dockClient.IsDocumentWell
				? RegionSmallCrossPolygon
				: RegionLargeCrossPolygon;
		}
		return RegionSmallCrossPolygon;
	}

	private static Region GetRegion(ViewHost dockHost, ViewHost dockClient)
	{
		using var gp = new GraphicsPath();
		var polygon = GetRegionPolygon(dockHost, dockClient);
		gp.AddPolygon(polygon.GetValue(Dpi.FromControl(dockHost)));
		return new Region(gp);
	}

	public ViewHost Host { get; }

	private void OnHostBoundsChanged(object? sender, EventArgs e)
	{
		var bounds = Host.RectangleToScreen(Host.Bounds);
		Location = new Point(
			bounds.X + (bounds.Width - 112) / 2,
			bounds.Y + (bounds.Height - 112) / 2);
	}

	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			Host.Resize -= OnHostBoundsChanged;
			Host.LocationChanged -= OnHostBoundsChanged;
		}
		base.Dispose(disposing);
	}
}
