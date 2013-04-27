#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
