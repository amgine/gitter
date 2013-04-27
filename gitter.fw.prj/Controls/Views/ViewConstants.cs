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
	using System.Drawing;
	using System.Windows.Forms;

	static class ViewConstants
	{
		public const AnchorStyles AnchorDefault =
			AnchorStyles.Left | AnchorStyles.Top;

		public const AnchorStyles AnchorAll =
			AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

		public const AnchorStyles AnchorDockLeft =
			AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;

		public const AnchorStyles AnchorDockTop =
			AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

		public const AnchorStyles AnchorDockRight =
			AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

		public const AnchorStyles AnchorDockBottom =
			AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

		public const int Spacing = 6;

		public const int BeforeTabContent = 3;
		public const int AfterTabContent = 6;
		public const int ImageSpacing = 3;

		public const int MinimumHostWidth = 100;
		public const int MinimumHostHeight = 100;

		public const int ViewFloatDragMargin = 6;

		public const int SideDockPanelBorderSize = 3;

		public const int TabHeaderButtonSpacing = 2;

		public const double OpacityHover = 1.0;
		public const double OpacityNormal = 0.70;

		public const double SplitterOpacity = 0.70;

		public const double DockPositionMarkerOpacity = 0.50;

		public const int PopupWidth = 300;
	}
}
