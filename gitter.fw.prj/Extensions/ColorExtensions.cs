namespace gitter
{
	using System.Drawing;

	/// <summary>Extension methods for <see cref="System.Drawing.Color"/>.</summary>
	public static class ColorExtensions
	{
		public static Color Darker(this Color color, float amount)
		{
			amount = 1 - amount;
			var r = (byte)(color.R * amount);
			var g = (byte)(color.G * amount);
			var b = (byte)(color.B * amount);
			return Color.FromArgb(r, g, b);
		}

		public static Color Lighter(this Color color, float amount)
		{
			var r = (byte)(color.R + (255 - color.R) * amount);
			var g = (byte)(color.G + (255 - color.G) * amount);
			var b = (byte)(color.B + (255 - color.B) * amount);
			return Color.FromArgb(r, g, b);
		}
	}
}
