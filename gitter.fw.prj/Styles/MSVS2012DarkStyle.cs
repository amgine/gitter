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
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	sealed class MSVS2012DarkStyle : MSVS2012Style, IGitterStyle
	{
		private ToolStripRenderer _toolStriprenderer;

		private sealed class MSVS2012DarkItemBackgroundStyles : IItemBackgroundStyles
		{
			public IBackgroundStyle Focused         { get; } = new BackgroundWithBorder(MSVS2012DarkColors.WINDOW, MSVS2012DarkColors.HIGHLIGHT);
			public IBackgroundStyle SelectedFocused { get; } = new SolidBackground(MSVS2012DarkColors.HIGHLIGHT);
			public IBackgroundStyle Selected        { get; } = new SolidBackground(MSVS2012DarkColors.HIGHLIGHT);
			public IBackgroundStyle SelectedNoFocus { get; } = new SolidBackground(MSVS2012DarkColors.HIDDEN_HIGHLIGHT);
			public IBackgroundStyle Hovered         { get; } = new SolidBackground(MSVS2012DarkColors.HOT_TRACK);
			public IBackgroundStyle HoveredFocused  { get; } = new BackgroundWithBorder(MSVS2012DarkColors.HOT_TRACK, MSVS2012DarkColors.HIGHLIGHT);
		}

		public string Name => "MSVS2012DarkStyle";

		public string DisplayName => "Microsoft Visual Studio 2012 Dark";

		public GitterStyleType Type => GitterStyleType.DarkBackground;

		public IGitterStyleColors Colors { get; } = new MSVS2012DarkColors();

		public IItemBackgroundStyles ItemBackgroundStyles { get; } = new MSVS2012DarkItemBackgroundStyles();

		public IScrollBarWidget CreateScrollBar(Orientation orientation)
			=> new CustomScrollBarAdapter(orientation, CustomScrollBarRenderer.MSVS2012Dark);

		public ICheckBoxWidget CreateCheckBox()
			=> new CustomCheckBoxAdapter(CustomCheckBoxRenderer.MSVS2012Dark);

		public IButtonWidget CreateButton()
			=> new CustomButtonAdapter(CustomButtonRenderer.MSVS2012Dark);

		public CustomListBoxRenderer ListBoxRenderer
			=> CustomListBoxManager.MSVS2012DarkRenderer;

		public ProcessOverlayRenderer OverlayRenderer
			=> ProcessOverlayRenderer.MSVS2012Dark;

		public ToolStripRenderer ToolStripRenderer
			=> _toolStriprenderer ??= new MSVS2012StyleToolStripRenderer(MSVS2012StyleToolStripRenderer.DarkColors);

		public ViewRenderer ViewRenderer
			=> ViewManager.MSVS2012DarkStyleRenderer;
	}
}
