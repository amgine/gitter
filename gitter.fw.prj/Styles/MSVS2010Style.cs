namespace gitter.Framework
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	sealed class MSVS2010Style : IGitterStyle
	{
		private ToolStripRenderer _toolStriprenderer;

		public MSVS2010Style()
		{
		}

		public string Name
		{
			get { return "MSVS2010Style"; }
		}

		public string DisplayName
		{
			get { return "Microsoft Visual Studio 2010"; }
		}

		public ToolStripRenderer ToolStripRenderer
		{
			get
			{
				if(_toolStriprenderer == null)
				{
					_toolStriprenderer = new MSVS2010StyleToolStripRenderer();
				}
				return _toolStriprenderer;
			}
		}

		public ViewRenderer ViewRenderer
		{
			get
			{
				return ViewManager.MSVS2010StyleRender;
			}
		}
	}
}
