namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	public static class ViewManager
	{
		private static readonly ViewRenderer _msvs2010StyleRender;
		private static ViewRenderer _viewRenderer;

		static ViewManager()
		{
			_msvs2010StyleRender = new MSVS2010StyleViewRenderer();
			_viewRenderer = _msvs2010StyleRender;
		}

		public static ViewRenderer Renderer
		{
			get { return _viewRenderer; }
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				_viewRenderer = value;
			}
		}

		public static ViewRenderer MSVS2010StyleRender
		{
			get { return _msvs2010StyleRender; }
		}
	}
}
