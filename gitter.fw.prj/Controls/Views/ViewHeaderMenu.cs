namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	sealed class ViewHeaderMenu : ContextMenuStrip
	{
		private readonly ViewHost _host;
		private readonly ViewBase _view;

		public ViewHeaderMenu(ViewHost viewHost, ViewBase view)
		{
			if(view == null) throw new ArgumentNullException("view");

			_host = viewHost;
			_view = view;

			Items.Add(new ToolStripMenuItem(Resources.StrClose, null, (s, e) => _view.Close()));
		}
	}
}
