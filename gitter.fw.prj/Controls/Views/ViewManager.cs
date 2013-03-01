namespace gitter.Framework.Controls
{
	using System;

	public static class ViewManager
	{
		#region Data

		private static ViewRenderer _msvs2010StyleRender;
		private static ViewRenderer _msvs2012DarkStyleRender;
		private static ViewRenderer _viewRenderer;

		#endregion

		#region Events

		public static event EventHandler RendererChanged;

		private static void OnRendererChanged()
		{
			var handler = RendererChanged;
			if(handler != null) handler(null, EventArgs.Empty);
		}

		#endregion

		#region .ctor

		static ViewManager()
		{
		}

		#endregion

		#region Properties

		public static ViewRenderer Renderer
		{
			get
			{
				if(_viewRenderer == null)
				{
					_viewRenderer = MSVS2010StyleRenderer;
				}
				return _viewRenderer;
			}
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				if(_viewRenderer != value)
				{
					_viewRenderer = value;
					OnRendererChanged();
				}
			}
		}

		public static ViewRenderer MSVS2010StyleRenderer
		{
			get
			{
				if(_msvs2010StyleRender == null)
				{
					_msvs2010StyleRender = new MSVS2010StyleViewRenderer();
				}
				return _msvs2010StyleRender;
			}
		}

		public static ViewRenderer MSVS2012DarkStyleRenderer
		{
			get
			{
				if(_msvs2012DarkStyleRender == null)
				{
					_msvs2012DarkStyleRender = new MSVS2012StyleViewRenderer(MSVS2012StyleViewRenderer.DarkColors);
				}
				return _msvs2012DarkStyleRender;
			}
		}

		#endregion
	}
}
