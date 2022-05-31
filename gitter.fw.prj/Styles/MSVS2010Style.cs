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
using System.Windows.Forms;

using gitter.Framework.Controls;

sealed class MSVS2010Style : IGitterStyle
{
	private readonly IGitterStyleColors _colors;
	private readonly IItemBackgroundStyles _itemBackgroundStyles;
	private ToolStripRenderer _toolStriprenderer;

	private sealed class SystemItemBackgroundStyles : IItemBackgroundStyles
	{
		public IBackgroundStyle Focused => BackgroundStyle.Focused;

		public IBackgroundStyle SelectedFocused => BackgroundStyle.SelectedFocused;

		public IBackgroundStyle Selected => BackgroundStyle.Selected;

		public IBackgroundStyle SelectedNoFocus => BackgroundStyle.SelectedNoFocus;

		public IBackgroundStyle Hovered => BackgroundStyle.Hovered;

		public IBackgroundStyle HoveredFocused => BackgroundStyle.HoveredFocused;
	}

	public MSVS2010Style()
	{
		_colors = new SystemStyleColors();
		_itemBackgroundStyles = new SystemItemBackgroundStyles();
	}

	public string Name => "MSVS2010Style";

	public string DisplayName => "Microsoft Visual Studio 2010";

	public GitterStyleType Type => GitterStyleType.LightBackground;

	public IGitterStyleColors Colors => _colors;

	public IItemBackgroundStyles ItemBackgroundStyles
		=> _itemBackgroundStyles;

	public IScrollBarWidget CreateScrollBar(Orientation orientation)
		=> new SystemScrollBarAdapter(orientation);

	public ICheckBoxWidget CreateCheckBox()
		=> new SystemCheckBoxAdapter();

	public IButtonWidget CreateButton()
		=> new SystemButtonAdapter();

	public CustomListBoxRenderer ListBoxRenderer
		=> CustomListBoxManager.Win7Renderer;

	public ProcessOverlayRenderer OverlayRenderer
		=> ProcessOverlayRenderer.Default;

	public ToolStripRenderer ToolStripRenderer
		=> _toolStriprenderer ??= new MSVS2010StyleToolStripRenderer();

	public ViewRenderer ViewRenderer => ViewManager.MSVS2010StyleRenderer;
}
