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
			: base(view, Utility.InvertAnchor(side.Side))
		{
			Verify.Argument.IsNotNull(side, "side");
			Verify.Argument.IsNotNull(viewHost, "viewHost");

			_side = side;
			_viewHost = viewHost;
		}

		#endregion

		#region Properties

		private ViewRenderer Renderer
		{
			get { return ViewManager.Renderer; }
		}

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
			return Renderer.MeasureViewDockSideTabLength(this, graphics);
		}

		internal override void OnPaint(Graphics graphics, Rectangle bounds)
		{
			if(bounds.Width > 0 && bounds.Height > 0)
			{
				Renderer.RenderViewDockSideTabBackground(this, graphics, bounds);
				Renderer.RenderViewDockSideTabContent(this, graphics, bounds);
			}
		}
	}
}
