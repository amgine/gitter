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

namespace gitter.Framework.Controls;

using static Native.StockObject;

public enum GdiStockObject : int
{
	/// <summary>White brush.</summary>
	WhiteBrush = WHITE_BRUSH,
	/// <summary>Light gray brush.</summary>
	LightGrayBrush = LTGRAY_BRUSH,
	/// <summary>Gray brush.</summary>
	GrayBrush = GRAY_BRUSH,
	/// <summary>Dark gray brush.</summary>
	DarkGrayBrush = DKGRAY_BRUSH,
	/// <summary>Black brush.</summary>
	BlackBrush = BLACK_BRUSH,
	/// <summary>Null brush (equivalent to HOLLOW_BRUSH).</summary>
	NullBrush = NULL_BRUSH,
	/// <summary>Hollow brush (equivalent to NULL_BRUSH).</summary>
	HollowBrush = HOLLOW_BRUSH,
	/// <summary>White pen.</summary>
	WhitePen = WHITE_PEN,
	/// <summary>Black pen.</summary>
	BlackPen = BLACK_PEN,
	/// <summary>Null pen. The null pen draws nothing.</summary>
	NullPen = NULL_PEN,
	/// <summary>Original equipment manufacturer (OEM) dependent fixed-pitch (monospace) font.</summary>
	OemFixedFont = OEM_FIXED_FONT,
	/// <summary>Windows fixed-pitch (monospace) system font.</summary>
	AnsiFixedFont = ANSI_FIXED_FONT,
	/// <summary>Windows variable-pitch (proportional space) system font.</summary>
	AnsiVariableFont = ANSI_VAR_FONT,
	/// <summary>
	/// System font. By default, the system uses the system font to draw menus, dialog box controls, and text.
	/// It is not recommended that you use DEFAULT_GUI_FONT or SYSTEM_FONT to obtain the font used by dialogs and windows.
	/// The default system font is Tahoma.
	/// </summary>
	SystemFont = SYSTEM_FONT,
	/// <summary>Device-dependent font.</summary>
	DeviceDefaultFont = DEVICE_DEFAULT_FONT,
	/// <summary>Default palette. This palette consists of the static colors in the system palette.</summary>
	DefaultPalette = DEFAULT_PALETTE,
	/// <summary></summary>
	SystemFixedFont = SYSTEM_FIXED_FONT,
	/// <summary>
	/// Default font for user interface objects such as menus and dialog boxes.
	/// It is not recommended that you use DEFAULT_GUI_FONT or SYSTEM_FONT to obtain the font used by dialogs and windows.
	/// The default font is Tahoma.
	/// </summary>
	DefaultGuiFont = DEFAULT_GUI_FONT,
	/// <summary>
	/// Solid color brush. The default color is white.
	/// The color can be changed by using the SetDCBrushColor function.
	/// For more information, see the Remarks section.
	/// </summary>
	DcBrush = DC_BRUSH,
	/// <summary>
	/// Solid pen color. The default color is white.
	/// The color can be changed by using the SetDCPenColor function.
	/// For more information, see the Remarks section.
	/// </summary>
	DcPen = DC_PEN,
}
