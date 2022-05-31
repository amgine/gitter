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

namespace gitter.Framework.Controls;

using System;
using System.Drawing;
using System.Windows.Forms;

public abstract class ViewRenderer
{
	public abstract Color AccentColor { get; }

	public abstract Color BackgroundColor { get; }

	public abstract Color DockMarkerBackgroundColor { get; }

	public abstract Color DockMarkerBorderColor { get; }

	public abstract IDpiBoundValue<int> TabHeight { get; }

	public abstract IDpiBoundValue<int> TabFooterHeight { get; }

	public abstract IDpiBoundValue<int> HeaderHeight { get; }

	public abstract IDpiBoundValue<int> FooterHeight { get; }

	public abstract IDpiBoundValue<int> ViewButtonSize { get; }

	public abstract int SideTabSpacing { get; }

	public abstract IDpiBoundValue<Size> SideTabSize { get; }

	public abstract IDpiBoundValue<int> FloatTitleHeight { get; }

	public abstract IDpiBoundValue<Size> FloatBorderSize { get; }

	public abstract IDpiBoundValue<SizeF> FloatCornerRadius { get; }


	public abstract int MeasureViewDockSideTabLength(DockPanelSideTab tab, Graphics graphics);

	public abstract void RenderViewDockSideTabBackground(DockPanelSideTab tab, Graphics graphics, Rectangle bounds);

	public abstract void RenderViewDockSideTabContent(DockPanelSideTab tab, Graphics graphics, Rectangle bounds);

	public abstract int MeasureViewHostTabLength(ViewHostTab tab, Graphics graphics);

	public abstract void RenderViewHostTabBackground(ViewHostTab tab, Graphics graphics, Rectangle bounds);

	public abstract void RenderViewHostTabContent(ViewHostTab tab, Graphics graphics, Rectangle bounds);

	public abstract void RenderViewHostTabsBackground(ViewHostTabs tabs, PaintEventArgs e);

	public abstract void RenderViewButton(ViewButton viewButton, Graphics graphics, Dpi dpi, Rectangle bounds, bool focus, bool hover, bool pressed);

	public abstract void RenderViewHostFooter(ViewHostFooter footer, PaintEventArgs e);

	public abstract void RenderViewHostHeader(ViewHostHeader header, PaintEventArgs e);

	public abstract void RenderViewDockSide(DockPanelSide side, PaintEventArgs e);

	public abstract void RenderDockMarkerButton(DockMarkerButton button, Control host, Graphics graphics, bool hover);

	public abstract void RenderPopupNotificationHeader(PopupNotificationHeader header, PaintEventArgs e);
}
