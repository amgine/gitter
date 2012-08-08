namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	public sealed class UntrackedFileContextMenuRequestedEventArgs : EventArgs
	{
		private readonly TreeFile _file;
		private ContextMenuStrip _contextMenu;

		public UntrackedFileContextMenuRequestedEventArgs(TreeFile file)
		{
			_file = file;
		}

		public TreeFile File
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
