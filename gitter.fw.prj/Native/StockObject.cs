#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Native;

/// <summary>Stock objects.</summary>
enum StockObject : int
{
	/// <summary>White brush.</summary>
	WHITE_BRUSH = 0,
	/// <summary>Light gray brush.</summary>
	LTGRAY_BRUSH = 1,
	/// <summary>Gray brush.</summary>
	GRAY_BRUSH = 2,
	/// <summary>Dark gray brush.</summary>
	DKGRAY_BRUSH = 3,
	/// <summary>Black brush.</summary>
	BLACK_BRUSH = 4,
	/// <summary>Null brush (equivalent to HOLLOW_BRUSH).</summary>
	NULL_BRUSH = 5,
	/// <summary>Hollow brush (equivalent to NULL_BRUSH).</summary>
	HOLLOW_BRUSH = NULL_BRUSH,
	/// <summary>White pen.</summary>
	WHITE_PEN = 6,
	/// <summary>Black pen.</summary>
	BLACK_PEN = 7,
	/// <summary>Null pen. The null pen draws nothing.</summary>
	NULL_PEN = 8,
	/// <summary>Original equipment manufacturer (OEM) dependent fixed-pitch (monospace) font.</summary>
	OEM_FIXED_FONT = 10,
	/// <summary>Windows fixed-pitch (monospace) system font.</summary>
	ANSI_FIXED_FONT = 11,
	/// <summary>Windows variable-pitch (proportional space) system font.</summary>
	ANSI_VAR_FONT = 12,
	/// <summary>
	/// System font. By default, the system uses the system font to draw menus, dialog box controls, and text.
	/// It is not recommended that you use DEFAULT_GUI_FONT or SYSTEM_FONT to obtain the font used by dialogs and windows.
	/// The default system font is Tahoma.
	/// </summary>
	SYSTEM_FONT = 13,
	/// <summary> Device-dependent font.</summary>
	DEVICE_DEFAULT_FONT = 14,
	/// <summary>Default palette. This palette consists of the static colors in the system palette.</summary>
	DEFAULT_PALETTE = 15,
	/// <summary></summary>
	SYSTEM_FIXED_FONT = 16,
	/// <summary>
	/// Default font for user interface objects such as menus and dialog boxes.
	/// It is not recommended that you use DEFAULT_GUI_FONT or SYSTEM_FONT to obtain the font used by dialogs and windows.
	/// The default font is Tahoma.
	/// </summary>
	DEFAULT_GUI_FONT = 17,
	/// <summary>
	/// Solid color brush. The default color is white.
	/// The color can be changed by using the SetDCBrushColor function.
	/// For more information, see the Remarks section.
	/// </summary>
	DC_BRUSH = 18,
	/// <summary>
	/// Solid pen color. The default color is white.
	/// The color can be changed by using the SetDCPenColor function.
	/// For more information, see the Remarks section.
	/// </summary>
	DC_PEN = 19,
}
