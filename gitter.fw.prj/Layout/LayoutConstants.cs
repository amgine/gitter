#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Layout;

using System.Windows.Forms;

using gitter.Framework;

public static class LayoutConstants
{
	public static ISizeSpec ToolbarHeight           { get; } = SizeSpec.Absolute(25);
	public static ISizeSpec LabelRowHeight          { get; } = SizeSpec.Absolute(18);
	public static ISizeSpec LabelRowSpacing         { get; } = SizeSpec.Absolute(2);
	public static ISizeSpec RowSpacing              { get; } = SizeSpec.Absolute(4);
	public static ISizeSpec ColumnSpacing           { get; } = SizeSpec.Absolute(8);
	public static ISizeSpec TextInputRowHeight      { get; } = SizeSpec.Absolute(28);
	public static ISizeSpec GroupSeparatorRowHeight { get; } = SizeSpec.Absolute(30);
	public static ISizeSpec CheckBoxRowHeight       { get; } = SizeSpec.Absolute(20);
	public static ISizeSpec RadioButtonRowHeight    { get; } = SizeSpec.Absolute(20);
	public static ISizeSpec ButtonRowHeight         { get; } = SizeSpec.Absolute(23);
	public static ISizeSpec CommandLinkHeight       { get; } = SizeSpec.Absolute(66);

	public static IDpiBoundValue<Padding> NoMargin           { get; } = DpiBoundValue.Constant(Padding.Empty);
	public static IDpiBoundValue<Padding> TextBoxMargin      { get; } = DpiBoundValue.Padding(new(0, 2, 0, 2));
	public static IDpiBoundValue<Padding> TextBoxLabelMargin { get; } = DpiBoundValue.Padding(new(0, 2, 0, 2));
	public static IDpiBoundValue<Padding> GroupPadding       { get; } = DpiBoundValue.Padding(new(12, 0, 0, 0));
}
