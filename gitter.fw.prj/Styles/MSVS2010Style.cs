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

	sealed class MSVS2010Style : IGitterStyle
	{
		private readonly IGitterStyleColors _colors;
		private readonly IItemBackgroundStyles _itemBackgroundStyles;
		private ToolStripRenderer _toolStriprenderer;

		private sealed class SystemItemBackgroundStyles : IItemBackgroundStyles
		{
			public IBackgroundStyle Focused
			{
				get { return BackgroundStyle.Focused; }
			}

			public IBackgroundStyle SelectedFocused
			{
				get { return BackgroundStyle.SelectedFocused; }
			}

			public IBackgroundStyle Selected
			{
				get { return BackgroundStyle.Selected; }
			}

			public IBackgroundStyle SelectedNoFocus
			{
				get { return BackgroundStyle.SelectedNoFocus; }
			}

			public IBackgroundStyle Hovered
			{
				get { return BackgroundStyle.Hovered; }
			}

			public IBackgroundStyle HoveredFocused
			{
				get { return BackgroundStyle.HoveredFocused; }
			}
		}

		public MSVS2010Style()
		{
			_colors = new SystemStyleColors();
			_itemBackgroundStyles = new SystemItemBackgroundStyles();
		}

		public string Name
		{
			get { return "MSVS2010Style"; }
		}

		public string DisplayName
		{
			get { return "Microsoft Visual Studio 2010"; }
		}

		public GitterStyleType Type
		{
			get { return GitterStyleType.LightBackground; }
		}

		public IGitterStyleColors Colors
		{
			get { return _colors; }
		}

		public IItemBackgroundStyles ItemBackgroundStyles
		{
			get { return _itemBackgroundStyles; }
		}

		public IScrollBarWidget CreateScrollBar(Orientation orientation)
		{
			return new SystemScrollBarAdapter(orientation);
		}

		public ICheckBoxWidget CreateCheckBox()
		{
			return new SystemCheckBoxAdapter();
		}

		public IButtonWidget CreateButton()
		{
			return new SystemButtonAdapter();
		}

		public CustomListBoxRenderer ListBoxRenderer
		{
			get { return CustomListBoxManager.Win7Renderer; }
		}

		public ProcessOverlayRenderer OverlayRenderer
		{
			get { return ProcessOverlayRenderer.Default; }
		}

		public ToolStripRenderer ToolStripRenderer
		{
			get
			{
				if(_toolStriprenderer == null)
				{
					_toolStriprenderer = new MSVS2010StyleToolStripRenderer();
				}
				return _toolStriprenderer;
			}
		}

		public ViewRenderer ViewRenderer
		{
			get { return ViewManager.MSVS2010StyleRenderer; }
		}
	}
}
