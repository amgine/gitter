namespace gitter.Framework.Controls
{
	using System.Drawing;
	using System.Windows.Forms;

	static class ViewConstants
	{
		public const AnchorStyles AnchorDefault =
			AnchorStyles.Left | AnchorStyles.Top;

		public const AnchorStyles AnchorAll =
			AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

		public const AnchorStyles AnchorDockLeft =
			AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;

		public const AnchorStyles AnchorDockTop =
			AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

		public const AnchorStyles AnchorDockRight =
			AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

		public const AnchorStyles AnchorDockBottom =
			AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

		public const int Spacing = 6;

		public const int TabHeight = 20;
		public const int TabFooterHeight = 4;
		public const int HeaderHeight = 19;
		public const int FooterHeight = 4;
		public const int ViewButtonSize = 15;

		public const int SideTabHeight = 21;
		public const int SideTabSpacing = 1;

		public const int BeforeTabContent = 3;
		public const int AfterTabContent = 6;
		public const int ImageSpacing = 3;

		public const int MinimumHostWidth = 100;
		public const int MinimumHostHeight = 100;

		public const int ViewFloatDragMargin = 6;

		public const int FloatTitleHeight = 16;
		public const int FloatBorderSize = 4;
		public const int FloatCornderRadius = 3;

		public const int SideDockPanelBorderSize = 3;

		public const int TabHeaderButtonSpacing = 2;

		public const double OpacityHover = 1.0;
		public const double OpacityNormal = 0.70;

		public const double SplitterOpacity = 0.70;

		public const double DockPositionMarkerOpacity = 0.50;

		public const int PopupWidth = 300;
	}
}
