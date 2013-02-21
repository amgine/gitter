namespace gitter.Framework.Controls
{
	using System;

	public static class CustomListBoxManager
	{
		#region Data

		private static CustomListBoxRenderer _renderer;
		private static CustomListBoxRenderer _win7Renderer;
		private static CustomListBoxRenderer _msvs2012DarkRenderer;

		#endregion

		#region .ctor

		static CustomListBoxManager()
		{
		}

		#endregion

		#region Properties

		public static CustomListBoxRenderer Renderer
		{
			get
			{
				if(_renderer == null)
				{
					return Win7Renderer;
				}
				return _renderer;
			}
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				_renderer = value;
			}
		}

		public static CustomListBoxRenderer Win7Renderer
		{
			get
			{
				if(_win7Renderer == null)
				{
					_win7Renderer = new Win7CustomListBoxRenderer();
				}
				return _win7Renderer;
			}
		}

		public static CustomListBoxRenderer MSVS2012DarkRenderer
		{
			get
			{
				if(_msvs2012DarkRenderer == null)
				{
					_msvs2012DarkRenderer = new MSVS2012CustomListBoxRenderer(MSVS2012CustomListBoxRenderer.DarkColors);
				}
				return _msvs2012DarkRenderer;
			}
		}

		#endregion
	}
}
