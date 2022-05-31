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

namespace gitter.Framework.Controls;

using System.Drawing;

/// <summary>Common listbox constants.</summary>
internal static class ListBoxConstants
{
	public const int ContentSpacing = 3;

	public const int SpaceBeforeImage = 0;
	public const int SpaceAfterImage = 5;

	public const int DefaultImageWidth = 16;
	public const int DefaultImageHeight = 16;

	public const int RootMargin = 0;
	public static readonly IDpiBoundValue<int>  LevelMargin          = DpiBoundValue.ScaleX(10);

	public static readonly IDpiBoundValue<int>  PlusMinusImageWidth  = DpiBoundValue.ScaleX(16);
	public static readonly IDpiBoundValue<int>  PlusMinusImageHeight = DpiBoundValue.ScaleY(16);
	public static readonly IDpiBoundValue<Size> PlusMinusImageSize   = DpiBoundValue.Size(new(16, 16));
	public static readonly IDpiBoundValue<int>  SpaceBeforePlusMinus = DpiBoundValue.Constant(0);
	public static readonly IDpiBoundValue<int>  SpaceAfterPlusMinus  = DpiBoundValue.Constant(0);

	public static readonly IDpiBoundValue<int>  CheckboxImageWidth   = DpiBoundValue.ScaleX(16);
	public static readonly IDpiBoundValue<int>  CheckboxImageHeight  = DpiBoundValue.ScaleY(16);
	public static readonly IDpiBoundValue<Size> CheckboxImageSize    = DpiBoundValue.Size(new(16, 16));
	public static readonly IDpiBoundValue<int>  SpaceBeforeCheckbox  = DpiBoundValue.ScaleX(1);
	public static readonly IDpiBoundValue<int>  SpaceAfterCheckbox   = DpiBoundValue.ScaleX(1);
}
