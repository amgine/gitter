namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public abstract class ViewRenderer
	{
		#region Properties

		public abstract Color BackgroundColor { get; }

		public abstract Color DockMarkerBackgroundColor { get; }

		public abstract Color DockMarkerBorderColor { get; }

		public abstract int TabHeight { get; }

		public abstract int TabFooterHeight { get; }

		public abstract int HeaderHeight { get; }

		public abstract int FooterHeight { get; }

		public abstract int ViewButtonSize { get; }

		public abstract int SideTabSpacing { get; }

		public abstract int SideTabHeight { get; }

		#endregion

		#region Methods

		public abstract int MeasureViewDockSideTabLength(ViewDockSideTab tab, Graphics graphics);

		public abstract void RenderViewDockSideTabBackground(ViewDockSideTab tab, Graphics graphics, Rectangle bounds);

		public abstract void RenderViewDockSideTabContent(ViewDockSideTab tab, Graphics graphics, Rectangle bounds);

		public abstract int MeasureViewHostTabLength(ViewHostTab tab, Graphics graphics);

		public abstract void RenderViewHostTabBackground(ViewHostTab tab, Graphics graphics, Rectangle bounds);

		public abstract void RenderViewHostTabContent(ViewHostTab tab, Graphics graphics, Rectangle bounds);

		public abstract void RenderViewHostTabsBackground(ViewHostTabs tabs, PaintEventArgs e);

		public abstract void RenderViewButton(ViewButton viewButton, Graphics graphics, Rectangle bounds, bool focus, bool hover, bool pressed);

		public abstract void RenderViewHostFooter(ViewHostFooter footer, PaintEventArgs e);

		public abstract void RenderViewHostHeader(ViewHostHeader header, PaintEventArgs e);

		public abstract void RenderViewDockSide(ViewDockSide side, PaintEventArgs e);

		public abstract void RenderDockMarkerButton(DockMarkerButton button, Graphics graphics, bool hover);

		public abstract void RenderPopupNotificationHeader(PopupNotificationHeader header, PaintEventArgs e);

		#endregion
	}
}
