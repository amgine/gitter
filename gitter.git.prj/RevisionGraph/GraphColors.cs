namespace gitter.Git.Gui
{
	using System;
	using System.Drawing;

	internal static class GraphColors
	{
		public static readonly int TotalColors;
		public static readonly Color[] Colors;
		public static readonly Pen[] Pens;

		static GraphColors()
		{
			Colors = new Color[]
				{
					Color.Black,
					Color.Red,
					Color.Green,
					Color.Blue,
					Color.Fuchsia,

					Color.Cyan,
					Color.Gray,
					Color.Gold,
					Color.HotPink,
					Color.Magenta,

					Color.Orange,
					Color.Olive,
					Color.Navy,
					Color.DarkViolet,
					Color.Aquamarine,
				};
			TotalColors = Colors.Length;
			Pens = new Pen[TotalColors];
			for(int i = 0; i < TotalColors; ++i)
			{
				Pens[i] = new Pen(Colors[i], 1.0f);
			}
		}
	}
}
