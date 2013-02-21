namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	public abstract class CustomCheckBoxRenderer
	{
		private static CustomCheckBoxRenderer _msvs2012Dark;

		public static CustomCheckBoxRenderer MSVS2012Dark
		{
			get
			{
				if(_msvs2012Dark == null)
				{
					_msvs2012Dark = new MSVS2012CheckBoxRenderer(MSVS2012CheckBoxRenderer.DarkColor);
				}
				return _msvs2012Dark;
			}
		}

		public static CustomCheckBoxRenderer Default
		{
			get { return MSVS2012Dark; }
		}

		public abstract void Render(Graphics graphics, Rectangle clipRectangle, CustomCheckBox checkBox);
	}
}
