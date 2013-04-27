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

namespace gitter.Framework.Controls
{
	/// <summary>Common listbox constants.</summary>
	internal static class ListBoxConstants
	{
		public const int ContentSpacing = 3;

		public const int SpaceBeforeImage = 0;
		public const int SpaceAfterImage = 5;

		public const int DefaultImageWidth = 16;
		public const int DefaultImageHeight = 16;

		public const int RootMargin = 0;
		public const int LevelMargin = 10;

		public const int PlusMinusImageWidth = 16;
		public const int PlusMinusImageHeight = 16;
		public const int SpaceBeforePlusMinus = 0;
		public const int SpaceAfterPlusMinus = 0;
		public const int PlusMinusAreaWidth =
			SpaceBeforePlusMinus + PlusMinusImageWidth + SpaceAfterPlusMinus;

		public const int CheckboxImageWidth = 16;
		public const int CheckboxImageHeight = 16;
		public const int SpaceBeforeCheckbox = 1;
		public const int SpaceAfterCheckbox = 1;
		public const int CheckBoxAreaWidth =
			SpaceBeforeCheckbox + CheckboxImageWidth + SpaceAfterCheckbox;
	}
}
