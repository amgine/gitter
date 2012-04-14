namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	public class FlowPanelMeasureEventArgs : EventArgs
	{
		#region Data

		private readonly Graphics _graphics;
		private readonly int _width;

		#endregion

		#region .ctor

		public FlowPanelMeasureEventArgs(Graphics graphics, int width)
		{
			_graphics = graphics;
			_width = width;
		}

		#endregion

		#region Properties

		public Graphics Graphics
		{
			get { return _graphics; }
		}

		public int Width
		{
			get { return _width; }
		}

		#endregion
	}
}
