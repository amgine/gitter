namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	public class BlameViewer : FlowLayoutControl
	{
		public BlameViewer()
		{
		}

		protected override void OnFreeSpaceMouseDown(int x, int y, MouseButtons button)
		{
			foreach(var p in Panels)
			{
				var filePanel = p as BlameFilePanel;
				if(filePanel != null) filePanel.DropSelection();
			}
			base.OnFreeSpaceMouseDown(x, y, button);
		}
	}
}
