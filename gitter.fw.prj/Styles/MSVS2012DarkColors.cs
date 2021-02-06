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

namespace gitter.Framework
{
	using System;
	using System.Drawing;

	public sealed class MSVS2012DarkColors : IGitterStyleColors
	{
		public static readonly Color WORK_AREA = Color.FromArgb(45, 45, 48);
		public static readonly Color WINDOW = Color.FromArgb(37, 37, 38);
		public static readonly Color SCROLLBAR_SPACING = Color.FromArgb(62, 62, 66);
		public static readonly Color SEPARATOR = Color.FromArgb(54, 54, 57);
		public static readonly Color WINDOW_TEXT = Color.FromArgb(241, 241, 241);
		public static readonly Color GRAY_TEXT = Color.FromArgb(153, 153, 153);
		public static readonly Color ALTERNATE = Color.FromArgb(30, 30, 31);
		public static readonly Color HYPERLINK_TEXT = Color.FromArgb(0, 151, 251);
		public static readonly Color HYPERLINK_TEXT_HOT_TRACK = Color.FromArgb(66, 170, 224);
		public static readonly Color HIGHLIGHT = Color.FromArgb(51, 153, 255);
		public static readonly Color HIDDEN_HIGHLIGHT = Color.FromArgb(63, 63, 70);
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
		public Color FileHeaderColor1            => WORK_AREA;
		public Color FileHeaderColor2            => WORK_AREA;
		public Color FilePanelBorder             => Color.Gray;
		public Color LineContextForeground       => WINDOW_TEXT;
		public Color LineContextBackground       => WINDOW;
		public Color LineAddedForeground         => Color.FromArgb(181, 241, 181);
		public Color LineAddedBackground         => Color.FromArgb(37, 50, 38);
		public Color LineRemovedForeground       => Color.FromArgb(255, 181, 181);
		public Color LineRemovedBackground       => Color.FromArgb(50, 37, 38);
		public Color LineNumberForeground        => GRAY_TEXT;
		public Color LineNumberBackground        => WORK_AREA;
		public Color LineNumberBackgroundHover   => WINDOW;
		public Color LineHeaderForeground        => GRAY_TEXT;
		public Color LineHeaderBackground        => WORK_AREA;
		public Color LineSelectedBackground      => LineBackgroundHover.Darker(0.3f);
		public Color LineSelectedBackgroundHover => Color.FromArgb(38, 79, 120);
		public Color LineBackgroundHover         => Color.FromArgb(24, 50, 75);
	}
}
