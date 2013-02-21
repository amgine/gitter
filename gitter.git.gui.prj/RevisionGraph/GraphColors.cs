namespace gitter.Git.Gui
{
	using System;
	using System.Drawing;

	internal static class GraphColors
	{
		public static readonly int TotalColors;
		public static readonly Pen[] PensForLightBackground;
		public static readonly Pen[] PensForDarkBackground;

		public static readonly Brush DotBrushForLightBackground;
		public static readonly Brush DotBrushForDarkBackground;

		public static readonly Pen TagBorderPenForLightBackground;
		public static readonly Pen TagBorderPenForDarkBackground;

		public static readonly Pen CirclePenForLightBackground;
		public static readonly Pen CirclePenForDarkBackground;

		static GraphColors()
		{
			PensForLightBackground = new Pen[]
				{
					Pens.Black,
					Pens.Red,
					Pens.Green,
					Pens.Blue,
					Pens.Fuchsia,

					Pens.Cyan,
					Pens.Gray,
					Pens.Gold,
					Pens.HotPink,
					Pens.Magenta,

					Pens.Orange,
					Pens.Olive,
					Pens.Navy,
					Pens.DarkViolet,
					Pens.Aquamarine,
				};
			PensForDarkBackground = new Pen[]
				{
					Pens.White,
					Pens.Red,
					Pens.Green,
					Pens.Blue,
					Pens.Fuchsia,

					Pens.Cyan,
					Pens.Gray,
					Pens.Gold,
					Pens.HotPink,
					Pens.Magenta,

					Pens.Orange,
					Pens.Olive,
					Pens.Navy,
					Pens.DarkViolet,
					Pens.Aquamarine,
				};
			TotalColors = Math.Min(PensForLightBackground.Length, PensForDarkBackground.Length);

			DotBrushForLightBackground = Brushes.Black;
			DotBrushForDarkBackground = Brushes.White;

			TagBorderPenForLightBackground = Pens.Black;
			TagBorderPenForDarkBackground = Pens.White;

			CirclePenForLightBackground = new Pen(Color.Black, 1.5f);
			CirclePenForDarkBackground = new Pen(Color.White, 1.5f);
		}
	}
}
