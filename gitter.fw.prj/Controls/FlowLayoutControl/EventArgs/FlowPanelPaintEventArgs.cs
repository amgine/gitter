namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	public class FlowPanelPaintEventArgs : EventArgs
	{
		#region Data

		private readonly Graphics _graphics;
		private readonly Rectangle _bounds;
		private readonly Rectangle _clipRectangle;

		#endregion

		#region .ctor

		public FlowPanelPaintEventArgs(Graphics graphics, Rectangle clipRectangle, Rectangle bounds)
		{
			_graphics = graphics;
			_clipRectangle = clipRectangle;
			_bounds = bounds;
		}

		#endregion

		#region Properties

		public Graphics Graphics
		{
			get { return _graphics; }
		}

		public Rectangle Bounds
		{
			get { return _bounds; }
		}

		public Rectangle ClipRectangle
		{
			get { return _clipRectangle; }
		}

		#endregion
	}
}
