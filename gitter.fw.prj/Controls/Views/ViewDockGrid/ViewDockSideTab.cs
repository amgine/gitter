namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	public sealed class ViewDockSideTab : ViewTabBase
	{
		#region Data

		private readonly ViewDockSide _side;
		private readonly ViewHost _viewHost;

		#endregion

		#region .ctor

		public ViewDockSideTab(ViewDockSide side, ViewHost viewHost, ViewBase view)
			: base(view, InvertAnchor(side.Side))
		{
			if(side == null) throw new ArgumentNullException("side");
			if(viewHost == null) throw new ArgumentNullException("viewHost");

			_side = side;
			_viewHost = viewHost;
		}

		#endregion

		#region Properties

		public ViewDockSide Side
		{
			get { return _side; }
		}

		public ViewHost ViewHost
		{
			get { return _viewHost; }
		}

		public override bool IsActive
		{
			get { return false; }
		}

		#endregion

		protected override int Measure(Graphics graphics)
		{
			return ViewManager.Renderer.MeasureViewDockSideTabLength(this, graphics);
		}

		internal override void OnPaint(Graphics graphics, Rectangle bounds)
		{
			if(bounds.Width > 0 && bounds.Height > 0)
			{
				ViewManager.Renderer.RenderViewDockSideTabBackground(this, graphics, bounds);
				ViewManager.Renderer.RenderViewDockSideTabContent(this, graphics, bounds);
			}
		}

		private static AnchorStyles InvertAnchor(AnchorStyles anchor)
		{
			switch(anchor)
			{
				case AnchorStyles.Left:
					return AnchorStyles.Right;
				case AnchorStyles.Top:
					return AnchorStyles.Bottom;
				case AnchorStyles.Right:
					return AnchorStyles.Left;
				case AnchorStyles.Bottom:
					return AnchorStyles.Top;
				default:
					throw new ArgumentException("anchor");
			}
		}
	}
}
