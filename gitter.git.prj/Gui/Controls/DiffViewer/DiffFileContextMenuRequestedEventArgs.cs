namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	public sealed class DiffFileContextMenuRequestedEventArgs : EventArgs
	{
		private readonly DiffFile _file;
		private ContextMenuStrip _contextMenu;

		public DiffFileContextMenuRequestedEventArgs(DiffFile file)
		{
			_file = file;
		}

		public DiffFile File
		{
			get { return _file; }
		}

		public ContextMenuStrip ContextMenu
		{
			get { return _contextMenu; }
			set { _contextMenu = value; }
		}
	}
}
