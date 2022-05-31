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

namespace gitter.Framework;

using System;
using System.Drawing;

public sealed class MSVS2012LightColors : IGitterStyleColors
{
	public static readonly Color WORK_AREA = Color.FromArgb(239, 239, 242);
	public static readonly Color WINDOW = Color.FromArgb(255, 255, 255);
	public static readonly Color SCROLLBAR_SPACING = Color.FromArgb(232, 232, 236);
	public static readonly Color SEPARATOR = Color.FromArgb(54, 54, 57);
	public static readonly Color WINDOW_TEXT = Color.FromArgb(0, 0, 0);
	public static readonly Color GRAY_TEXT = Color.FromArgb(113, 113, 113);
	public static readonly Color ALTERNATE = Color.FromArgb(30, 30, 31);
	public static readonly Color HYPERLINK_TEXT = Color.FromArgb(0, 151, 251);
	public static readonly Color HYPERLINK_TEXT_HOT_TRACK = Color.FromArgb(66, 170, 224);
	public static readonly Color HIGHLIGHT = Color.FromArgb(51, 153, 255);
	public static readonly Color HIDDEN_HIGHLIGHT = Color.FromArgb(204, 206, 219);
	public static readonly Color HOT_TRACK = Color.FromArgb(18, 18, 18);

	public Color WorkArea                    => WORK_AREA;
	public Color Window                      => WINDOW;
	public Color ScrollBarSpacing            => SCROLLBAR_SPACING;
	public Color Separator                   => SEPARATOR;
	public Color Alternate                   => ALTERNATE;
	public Color WindowText                  => WINDOW_TEXT;
	public Color GrayText                    => GRAY_TEXT;
	public Color HyperlinkText               => HYPERLINK_TEXT;
	public Color HyperlinkTextHotTrack       => HYPERLINK_TEXT_HOT_TRACK;
	public Color FileHeaderColor1            => Color.FromArgb(245, 245, 245);
	public Color FileHeaderColor2            => Color.FromArgb(245, 245, 245);
	public Color FilePanelBorder             => Color.Gray;
	public Color LineContextForeground       => Color.FromArgb(0, 0, 0);
	public Color LineContextBackground       => Color.FromArgb(255, 255, 255);
	public Color LineAddedForeground         => Color.FromArgb(0, 100, 0);
	public Color LineAddedBackground         => Color.FromArgb(221, 255, 233);
	public Color LineRemovedForeground       => Color.FromArgb(200, 0, 0);
	public Color LineRemovedBackground       => Color.FromArgb(255, 238, 238);
	public Color LineNumberForeground        => Color.Gray;
	public Color LineNumberBackground        => Color.FromArgb(247, 247, 247);
	public Color LineNumberBackgroundHover   => LineNumberBackground.Darker(0.1f);
	public Color LineHeaderForeground        => Color.Gray;
	public Color LineHeaderBackground        => Color.FromArgb(247, 247, 247);
	public Color LineSelectedBackground      => Color.FromArgb(173, 214, 255);
	public Color LineSelectedBackgroundHover => LineSelectedBackground.Darker(0.1f);
	public Color LineBackgroundHover         => LineSelectedBackground.Lighter(0.3f);
}
