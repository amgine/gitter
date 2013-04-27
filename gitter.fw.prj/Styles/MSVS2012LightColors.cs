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

	public sealed class MSVS2012LightColors : IGitterStyleColors
	{
		#region Static

		[CLSCompliant(false)]
		public static readonly Color WORK_AREA = Color.FromArgb(239, 239, 242);
		[CLSCompliant(false)]
		public static readonly Color WINDOW = Color.FromArgb(255, 255, 255);
		[CLSCompliant(false)]
		public static readonly Color SCROLLBAR_SPACING = Color.FromArgb(232, 232, 236);
		[CLSCompliant(false)]
		public static readonly Color SEPARATOR = Color.FromArgb(54, 54, 57);
		[CLSCompliant(false)]
		public static readonly Color WINDOW_TEXT = Color.FromArgb(0, 0, 0);
		[CLSCompliant(false)]
		public static readonly Color GRAY_TEXT = Color.FromArgb(113, 113, 113);
		[CLSCompliant(false)]
		public static readonly Color ALTERNATE = Color.FromArgb(30, 30, 31);
		[CLSCompliant(false)]
		public static readonly Color HYPERLINK_TEXT = Color.FromArgb(0, 151, 251);
		[CLSCompliant(false)]
		public static readonly Color HYPERLINK_TEXT_HOT_TRACK = Color.FromArgb(66, 170, 224);
		[CLSCompliant(false)]
		public static readonly Color HIGHLIGHT = Color.FromArgb(51, 153, 255);
		[CLSCompliant(false)]
		public static readonly Color HIDDEN_HIGHLIGHT = Color.FromArgb(204, 206, 219);
		[CLSCompliant(false)]
		public static readonly Color HOT_TRACK = Color.FromArgb(18, 18, 18);

		#endregion

		#region IGitterStyleColors

		public Color WorkArea
		{
			get { return WORK_AREA; }
		}

		public Color Window
		{
			get { return WINDOW; }
		}

		public Color ScrollBarSpacing
		{
			get { return SCROLLBAR_SPACING; }
		}

		public Color Separator
		{
			get { return SEPARATOR; }
		}

		public Color Alternate
		{
			get { return ALTERNATE; }
		}

		public Color WindowText
		{
			get { return WINDOW_TEXT; }
		}

		public Color GrayText
		{
			get { return GRAY_TEXT; }
		}

		public Color HyperlinkText
		{
			get { return HYPERLINK_TEXT; }
		}

		public Color HyperlinkTextHotTrack
		{
			get { return HYPERLINK_TEXT_HOT_TRACK; }
		}

		public Color FileHeaderColor1				{ get { return Color.FromArgb(245, 245, 245); } }
		public Color FileHeaderColor2				{ get { return Color.FromArgb(245, 245, 245); } }
		public Color FilePanelBorder				{ get { return Color.Gray; } }
		public Color LineContextForeground			{ get { return Color.FromArgb(0, 0, 0); } }
		public Color LineContextBackground			{ get { return Color.FromArgb(255, 255, 255); } }
		public Color LineAddedForeground			{ get { return Color.FromArgb(0, 100, 0); } }
		public Color LineAddedBackground			{ get { return Color.FromArgb(221, 255, 233); } }
		public Color LineRemovedForeground			{ get { return Color.FromArgb(200, 0, 0); } }
		public Color LineRemovedBackground			{ get { return Color.FromArgb(255, 238, 238); } }
		public Color LineNumberForeground			{ get { return Color.Gray; } }
		public Color LineNumberBackground			{ get { return Color.FromArgb(247, 247, 247); } }
		public Color LineNumberBackgroundHover		{ get { return LineNumberBackground.Darker(0.1f); } }
		public Color LineHeaderForeground			{ get { return Color.Gray; } }
		public Color LineHeaderBackground			{ get { return Color.FromArgb(247, 247, 247); } }
		public Color LineSelectedBackground			{ get { return Color.FromArgb(173, 214, 255); } }
		public Color LineSelectedBackgroundHover	{ get { return LineSelectedBackground.Darker(0.1f); } }
		public Color LineBackgroundHover			{ get { return LineSelectedBackground.Lighter(0.3f); } }

		#endregion
	}
}
