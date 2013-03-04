namespace gitter.Framework
{
	using System;
	using System.Drawing;

	using gitter.Framework.Controls;

	abstract class MSVS2012Style
	{
		protected sealed class BackgroundWithBorder : IBackgroundStyle
		{
			#region Data

			private readonly Color _backgroundColor;
			private readonly Color _borderColor;

			#endregion

			#region .ctor

			public BackgroundWithBorder(Color backgroundColor, Color borderColor)
			{
				_backgroundColor = backgroundColor;
				_borderColor = borderColor;
			}

			#endregion

			#region Methods

			public void Draw(Graphics graphics, Rectangle rect)
			{
				using(var brush = new SolidBrush(_backgroundColor))
				{
					graphics.FillRectangle(brush, rect);
				}
				using(var pen = new Pen(_borderColor))
				{
					rect.Width -= 1;
					rect.Height -= 1;
					graphics.DrawRectangle(pen, rect);
				}
			}

			#endregion
		}

		protected sealed class SolidBackground : IBackgroundStyle
		{
			#region Data

			private readonly Color _backgroundColor;

			#endregion

			#region .ctor

			public SolidBackground(Color backgroundColor)
			{
				_backgroundColor = backgroundColor;
			}

			#endregion

			#region Methods

			public void Draw(Graphics graphics, Rectangle rect)
			{
				using(var brush = new SolidBrush(_backgroundColor))
				{
					graphics.FillRectangle(brush, rect);
				}
			}

			#endregion
		}
	}
}
