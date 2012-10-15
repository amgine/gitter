namespace gitter.Git.Gui
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	sealed class ContentContextMenuEventArgs : EventArgs
	{
		private readonly ContextMenuStrip _contextMenu;
		private readonly Point _position;

		public ContentContextMenuEventArgs(ContextMenuStrip contextMenu, Point position)
		{
			_contextMenu = contextMenu;
			_position = position;
		}

		public ContextMenuStrip ContextMenu
		{
			get { return _contextMenu; }
		}

		public Point Position
		{
			get { return _position; }
		}
	}
}
