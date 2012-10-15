namespace gitter.Git.Gui
{
	using System;
	using System.Drawing;

	sealed class ContentInvalidatedEventArgs : EventArgs
	{
		private readonly Rectangle _bounds;

		public ContentInvalidatedEventArgs(Rectangle bounds)
		{
			_bounds = bounds;
		}

		public Rectangle Bounds
		{
			get { return _bounds; }
		}
	}
}
