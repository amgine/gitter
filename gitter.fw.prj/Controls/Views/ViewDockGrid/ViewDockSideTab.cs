namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	sealed class ViewDockSideTab : ViewTabBase
	{
		#region Data

		private readonly ViewDockSide _side;
		private readonly ViewHost _viewHost;

		#endregion

		#region .ctor

		public ViewDockSideTab(ViewDockSide side, ViewHost viewHost, ViewBase view)
			: base(side, view, InvertAnchor(side.Side))
		{
			if(viewHost == null)
				throw new ArgumentNullException("toolHost");

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
