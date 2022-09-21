#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

public readonly record struct ViewSplitSlotSize(
	ViewSplitSlotSizeType Type,
	Dpi   Dpi,
	int   Size,
	float Fraction)
{
	public static ViewSplitSlotSize Absolute(int value) => new(ViewSplitSlotSizeType.Absolute, Dpi.Default, value, 0);

	public static ViewSplitSlotSize Absolute(Dpi dpi, int value) => new(ViewSplitSlotSizeType.Absolute, dpi, value, 0);

	public static ViewSplitSlotSize Relative(float value) => new(ViewSplitSlotSizeType.Relative, Dpi.Default, 0, value);

	public static ViewSplitSlotSize Leftover() => new(ViewSplitSlotSizeType.Leftover, Dpi.Default, 0, 0);
}
