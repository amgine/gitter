namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Drawing;

	public abstract class CustomButtonRenderer
	{
		private static CustomButtonRenderer _msvs2012Dark;

		public static CustomButtonRenderer MSVS2012Dark
		{
			get
			{
				if(_msvs2012Dark == null)
				{
					_msvs2012Dark = new MSVS2012ButtonRenderer(MSVS2012ButtonRenderer.DarkColors);
				}
				return _msvs2012Dark;
			}
		}

		public static CustomButtonRenderer Default
		{
			get { return MSVS2012Dark; }
		}

		public abstract void Render(Graphics graphics, Rectangle clipRectangle, CustomButton button);
	}
}
