namespace gitter.Framework.Services
{
	using System.Drawing;

	public interface ITextRenderer
	{
		void DrawText(Graphics graphics, string text, Font font, Brush brush, Rectangle layoutRectangle, StringFormat format);

		void DrawText(Graphics graphics, string text, Font font, Brush brush, Point point, StringFormat format);

		void DrawText(Graphics graphics, string text, Font font, Brush brush, int x, int y, StringFormat format);

		void DrawText(Graphics graphics, string text, Font font, Brush brush, Rectangle layoutRectangle);

		void DrawText(Graphics graphics, string text, Font font, Brush brush, Point point);

		void DrawText(Graphics graphics, string text, Font font, Brush brush, int x, int y);

		Size MeasureText(Graphics graphics, string text, Font font, Size layoutArea, StringFormat format);

		Size MeasureText(Graphics graphics, string text, Font font, int width, StringFormat format);

		Size MeasureText(Graphics graphics, string text, Font font, Size layoutArea);

		Size MeasureText(Graphics graphics, string text, Font font, int width);

		float GetFontHeight(Font font);

		float GetFontHeight(Graphics graphics, Font font);
	}
}
