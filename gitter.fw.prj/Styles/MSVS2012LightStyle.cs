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
using System.Windows.Forms;

using gitter.Framework.Controls;

sealed class MSVS2012LightStyle : MSVS2012Style, IGitterStyle
{
	private readonly IGitterStyleColors _colors;
	private readonly IItemBackgroundStyles _itemBackgroundStyles;
	private ToolStripRenderer? _toolStriprenderer;

	private sealed class MSVS2012LightItemBackgroundStyles : IItemBackgroundStyles
	{
		public MSVS2012LightItemBackgroundStyles()
		{
			Focused         = new BackgroundWithBorder(MSVS2012LightColors.WINDOW, MSVS2012LightColors.HIGHLIGHT);
			SelectedFocused = new SolidBackground(MSVS2012LightColors.HIGHLIGHT);
			Selected        = SelectedFocused;
			SelectedNoFocus = new SolidBackground(MSVS2012LightColors.HIDDEN_HIGHLIGHT);
			Hovered         = new SolidBackground(MSVS2012LightColors.HOT_TRACK);
			HoveredFocused  = new BackgroundWithBorder(MSVS2012LightColors.HOT_TRACK, MSVS2012LightColors.HIGHLIGHT);
		}

		public IBackgroundStyle Focused         { get; }
		public IBackgroundStyle SelectedFocused { get; }
		public IBackgroundStyle Selected        { get; }
		public IBackgroundStyle SelectedNoFocus { get; }
		public IBackgroundStyle Hovered         { get; }
		public IBackgroundStyle HoveredFocused  { get; }
	}

	public MSVS2012LightStyle()
	{
		_colors = new MSVS2012LightColors();
		_itemBackgroundStyles = new MSVS2012LightItemBackgroundStyles();
	}

	public string Name => "MSVS2012LightStyle";

	public string DisplayName => "Microsoft Visual Studio 2012 Light";

	public GitterStyleType Type => GitterStyleType.LightBackground;

	public IGitterStyleColors Colors => _colors;

	public IItemBackgroundStyles ItemBackgroundStyles => _itemBackgroundStyles;

	public IScrollBarWidget CreateScrollBar(Orientation orientation)
		=> new CustomScrollBarAdapter(orientation, CustomScrollBarRenderer.MSVS2012Light);

	public IFactory<ICheckBoxWidget> CheckBoxFactory
		=> Controls.CheckBoxFactory.MSVS2012Dark;

	public IFactory<IRadioButtonWidget> RadioButtonFactory
		=> Controls.RadioButtonFactory.MSVS2012Dark;

	public IFactory<IButtonWidget> ButtonFactory
		=> Controls.ButtonFactory.MSVS2012Dark;

	public IFactory<IProgressBarWidget> ProgressBarFactory
		=> Controls.ProgressBarFactory.System;

	public CustomListBoxRenderer ListBoxRenderer
		=> CustomListBoxManager.MSVS2012LightRenderer;

	public ProcessOverlayRenderer OverlayRenderer
		=> ProcessOverlayRenderer.MSVS2012Dark;

	public ToolStripRenderer ToolStripRenderer
		=> _toolStriprenderer ??= new MSVS2012StyleToolStripRenderer(MSVS2012StyleToolStripRenderer.ColorTable.Light);

	public ViewRenderer ViewRenderer => ViewManager.MSVS2012LightStyleRenderer;
}
