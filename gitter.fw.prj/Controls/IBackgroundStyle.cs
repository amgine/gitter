namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Item background style.</summary>
	public interface IBackgroundStyle
	{
		/// <summary>Draw item background.</summary>
		/// <param name="graphics"><see cref="Graphics"/> surface to draw on.</param>
		/// <param name="rect">Item rectangle.</param>
		void Draw(Graphics graphics, Rectangle rect);
	}
}
