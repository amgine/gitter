namespace gitter
{
	using System.Drawing;

	using gitter.Framework;

	/// <summary>Extension methods for <see cref="System.Drawing.Graphics"/>.</summary>
	public static class GraphicsExtensions
	{
		public static void DrawRoundedRectangle(this Graphics g, Pen pen, RectangleF rect, float cornerRadius)
		{
			using(var gp = Utility.GetRoundedRectangle(rect, cornerRadius))
			{
				g.DrawPath(pen, gp);
			}
		}

		public static void FillRoundedRectangle(this Graphics g, Brush brush, RectangleF rect, float cornerRadius)
		{
			using(var gp = Utility.GetRoundedRectangle(rect, cornerRadius))
			{
				g.FillPath(brush, gp);
			}
		}

		public static void FillRoundedRectangle(this Graphics g, Brush brush, RectangleF rect, float topLeftCorner, float topRightCorner, float bottomLeftCorner, float bottomRightCorner)
		{
			using(var gp = Utility.GetRoundedRectangle(rect, topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner))
			{
				g.FillPath(brush, gp);
			}
		}

		public static void FillRoundedRectangle(this Graphics g, Brush fillBrush, Pen borderPen, RectangleF rect, float cornerRadius)
		{
			using(var gp = Utility.GetRoundedRectangle(rect, cornerRadius))
			{
				g.FillPath(fillBrush, gp);
				g.DrawPath(borderPen, gp);
			}
		}
	}
}
