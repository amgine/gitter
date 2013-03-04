namespace gitter.Framework
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	sealed class MSVS2012LightStyle : MSVS2012Style, IGitterStyle
	{
		private readonly IGitterStyleColors _colors;
		private readonly IItemBackgroundStyles _itemBackgroundStyles;
		private ToolStripRenderer _toolStriprenderer;

		private sealed class MSVS2012LightItemBackgroundStyles : IItemBackgroundStyles
		{
			#region Data

			private readonly IBackgroundStyle _focused;
			private readonly IBackgroundStyle _selectedFocused;
			private readonly IBackgroundStyle _selected;
			private readonly IBackgroundStyle _selectedNoFocus;
			private readonly IBackgroundStyle _hovered;
			private readonly IBackgroundStyle _hoveredFocused;

			#endregion

			#region .ctor

			public MSVS2012LightItemBackgroundStyles()
			{
				_focused			= new BackgroundWithBorder(MSVS2012LightColors.WINDOW, MSVS2012LightColors.HIGHLIGHT);
				_selectedFocused	= new SolidBackground(MSVS2012LightColors.HIGHLIGHT);
				_selected			= _selectedFocused;
				_selectedNoFocus	= new SolidBackground(MSVS2012LightColors.HIDDEN_HIGHLIGHT);
				_hovered			= new SolidBackground(MSVS2012LightColors.HOT_TRACK);
				_hoveredFocused		= new BackgroundWithBorder(MSVS2012LightColors.HOT_TRACK, MSVS2012LightColors.HIGHLIGHT);
			}

			#endregion

			#region Properties

			public IBackgroundStyle Focused
			{
				get { return _focused; }
			}

			public IBackgroundStyle SelectedFocused
			{
				get { return _selectedFocused; }
			}

			public IBackgroundStyle Selected
			{
				get { return _selected; }
			}

			public IBackgroundStyle SelectedNoFocus
			{
				get { return _selectedNoFocus; }
			}

			public IBackgroundStyle Hovered
			{
				get { return _hovered; }
			}

			public IBackgroundStyle HoveredFocused
			{
				get { return _hoveredFocused; }
			}

			#endregion
		}

		public MSVS2012LightStyle()
		{
			_colors = new MSVS2012LightColors();
			_itemBackgroundStyles = new MSVS2012LightItemBackgroundStyles();
		}

		public string Name
		{
			get { return "MSVS2012LightStyle"; }
		}

		public string DisplayName
		{
			get { return "Microsoft Visual Studio 2012 Light"; }
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
			return new CustomScrollBarAdapter(orientation, CustomScrollBarRenderer.MSVS2012Light);
		}

		public ICheckBoxWidget CreateCheckBox()
		{
			return new CustomCheckBoxAdapter(CustomCheckBoxRenderer.MSVS2012Dark);
		}

		public CustomListBoxRenderer ListBoxRenderer
		{
			get { return CustomListBoxManager.MSVS2012LightRenderer; }
		}

		public ProcessOverlayRenderer OverlayRenderer
		{
			get { return ProcessOverlayRenderer.MSVS2012Dark; }
		}

		public ToolStripRenderer ToolStripRenderer
		{
			get
			{
				if(_toolStriprenderer == null)
				{
					_toolStriprenderer = new MSVS2012StyleToolStripRenderer(MSVS2012StyleToolStripRenderer.LightColors);
				}
				return _toolStriprenderer;
			}
		}

		public ViewRenderer ViewRenderer
		{
			get { return ViewManager.MSVS2012LightStyleRenderer; }
		}
	}
}
