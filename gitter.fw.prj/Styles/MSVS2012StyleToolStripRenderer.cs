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
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	sealed class MSVS2012StyleToolStripRenderer : ToolStripRenderer
	{
		#region Color Tables

		public interface IColorTable
		{
			Color Grip { get; }
			Color ResizeGrip0 { get; }
			Color ResizeGrip1 { get; }
			Color Highlight { get; }
			Color MenuBorder { get; }
			Color VerticalSeparator0 { get; }
			Color VerticalSeparator1 { get; }
			Color Pressed { get; }
			Color CheckedBorder { get; }

			Color Text { get; }
			Color ArrowNormal { get; }
			Color ArrowHighlight { get; }

			Color ToolStripBackground { get; }
			Color MenuStripBackground { get; }
			Color StatusStripBackground { get; }
			Color DropDownBackground { get; }
			Color StatusLabelBackground { get; }
			Color ContentPanelBackground { get; }
			Color TextBoxBackground { get; }

			Color SelectedCheckboxBackground { get; }
			Color SelectedCheckboxForeground { get; }
			Color NormalCheckboxBackground { get; }
			Color NormalCheckboxForeground { get; }
		}

		private sealed class DarkColorTable : IColorTable
		{
			private static readonly Color GRIP						= Color.FromArgb(70, 70, 74);
			private static readonly Color RESIZE_GRIP0				= Color.FromArgb(0, 92, 153);
			private static readonly Color RESIZE_GRIP1				= Color.FromArgb(127, 188, 229);
			private static readonly Color HIGHLIGHT					= Color.FromArgb(62, 62, 64);
			private static readonly Color MENU_BORDER				= Color.FromArgb(51, 51, 55);
			private static readonly Color VERTICAL_SEPARATOR0		= Color.FromArgb(34, 34, 34);
			private static readonly Color VERTICAL_SEPARATOR1		= Color.FromArgb(70, 70, 74);
			private static readonly Color PRESSED					= Color.FromArgb(0, 122, 204);
			private static readonly Color CHECKED_BORDER			= Color.FromArgb(51, 153, 255);

			private static readonly Color TEXT						= MSVS2012DarkColors.WINDOW_TEXT;
			private static readonly Color ARROW_NORMAL				= Color.FromArgb(153, 153, 153);
			private static readonly Color ARROW_HIGHLIGHT			= Color.FromArgb(0, 122, 204);

			private static readonly Color TOOL_STRIP_BACKGROUND		= MSVS2012DarkColors.WORK_AREA;
			private static readonly Color MENU_STRIP_BACKGROUND		= MSVS2012DarkColors.WORK_AREA;
			private static readonly Color STATUS_STRIP_BACKGROUND	= MSVS2012DarkColors.WORK_AREA;
			private static readonly Color DROP_DOWN_BACKGROUND		= Color.FromArgb(27, 27, 28);
			private static readonly Color STATUS_LABEL_BACKGROUND	= MSVS2012DarkColors.WORK_AREA;
			private static readonly Color CONTENT_PANEL_BACKGROUND	= MSVS2012DarkColors.WORK_AREA;
			private static readonly Color TEXT_BOX_BACKGROUND		= MSVS2012DarkColors.WINDOW;

			private static readonly Color SELECTED_CHECKBOX_BACKGROUND	= Color.FromArgb(62, 62, 64);
			private static readonly Color SELECTED_CHECKBOX_FOREGROUND	= Color.FromArgb(241, 241, 241);
			private static readonly Color NORMAL_CHECKBOX_BACKGROUND	= Color.FromArgb(45, 45, 48);
			private static readonly Color NORMAL_CHECKBOX_FOREGROUND	= Color.FromArgb(153, 153, 153);

			#region IColorTable

			public Color Grip { get { return GRIP; } }
			public Color ResizeGrip0 { get { return RESIZE_GRIP0; } }
			public Color ResizeGrip1 { get { return RESIZE_GRIP1; } }
			public Color Highlight { get { return HIGHLIGHT; } }
			public Color MenuBorder { get { return MENU_BORDER; } }
			public Color VerticalSeparator0 { get { return VERTICAL_SEPARATOR0; } }
			public Color VerticalSeparator1 { get { return VERTICAL_SEPARATOR1; } }
			public Color Pressed { get { return PRESSED; } }
			public Color CheckedBorder { get { return CHECKED_BORDER; } }

			public Color Text { get { return TEXT; } }
			public Color ArrowNormal { get { return ARROW_NORMAL; } }
			public Color ArrowHighlight { get { return ARROW_HIGHLIGHT; } }

			public Color ToolStripBackground { get { return TOOL_STRIP_BACKGROUND; } }
			public Color MenuStripBackground { get { return MENU_STRIP_BACKGROUND; } }
			public Color StatusStripBackground { get { return STATUS_STRIP_BACKGROUND; } }
			public Color DropDownBackground { get { return DROP_DOWN_BACKGROUND; } }
			public Color StatusLabelBackground { get { return STATUS_STRIP_BACKGROUND; } }
			public Color ContentPanelBackground { get { return CONTENT_PANEL_BACKGROUND; } }
			public Color TextBoxBackground { get { return TEXT_BOX_BACKGROUND; } }

			public Color SelectedCheckboxBackground { get { return SELECTED_CHECKBOX_BACKGROUND; } }
			public Color SelectedCheckboxForeground { get { return SELECTED_CHECKBOX_FOREGROUND; } }
			public Color NormalCheckboxBackground { get { return NORMAL_CHECKBOX_BACKGROUND; } }
			public Color NormalCheckboxForeground { get { return NORMAL_CHECKBOX_FOREGROUND; } }

			#endregion
		}

		private sealed class LightColorTable : IColorTable
		{
			private static readonly Color GRIP						= Color.FromArgb(70, 70, 74);
			private static readonly Color RESIZE_GRIP0				= Color.FromArgb(0, 92, 153);
			private static readonly Color RESIZE_GRIP1				= Color.FromArgb(127, 188, 229);
			private static readonly Color HIGHLIGHT					= Color.FromArgb(248, 249, 250);
			private static readonly Color MENU_BORDER				= Color.FromArgb(204, 206, 219);
			private static readonly Color VERTICAL_SEPARATOR0		= Color.FromArgb(34, 34, 34);
			private static readonly Color VERTICAL_SEPARATOR1		= Color.FromArgb(70, 70, 74);
			private static readonly Color PRESSED					= Color.FromArgb(0, 122, 204);
			private static readonly Color CHECKED_BORDER			= Color.FromArgb(51, 153, 255);

			private static readonly Color TEXT						= MSVS2012LightColors.WINDOW_TEXT;
			private static readonly Color ARROW_NORMAL				= Color.FromArgb(113, 113, 113);
			private static readonly Color ARROW_HIGHLIGHT			= Color.FromArgb(0, 122, 204);

			private static readonly Color TOOL_STRIP_BACKGROUND		= MSVS2012LightColors.WORK_AREA;
			private static readonly Color MENU_STRIP_BACKGROUND		= MSVS2012LightColors.WORK_AREA;
			private static readonly Color STATUS_STRIP_BACKGROUND	= MSVS2012LightColors.WORK_AREA;
			private static readonly Color DROP_DOWN_BACKGROUND		= Color.FromArgb(231, 232, 236);
			private static readonly Color STATUS_LABEL_BACKGROUND	= MSVS2012LightColors.WORK_AREA;
			private static readonly Color CONTENT_PANEL_BACKGROUND	= MSVS2012LightColors.WORK_AREA;
			private static readonly Color TEXT_BOX_BACKGROUND		= MSVS2012LightColors.WINDOW;

			private static readonly Color SELECTED_CHECKBOX_BACKGROUND	= Color.FromArgb(62, 62, 64);
			private static readonly Color SELECTED_CHECKBOX_FOREGROUND	= Color.FromArgb(241, 241, 241);
			private static readonly Color NORMAL_CHECKBOX_BACKGROUND	= Color.FromArgb(45, 45, 48);
			private static readonly Color NORMAL_CHECKBOX_FOREGROUND	= Color.FromArgb(153, 153, 153);

			#region IColorTable

			public Color Grip { get { return GRIP; } }
			public Color ResizeGrip0 { get { return RESIZE_GRIP0; } }
			public Color ResizeGrip1 { get { return RESIZE_GRIP1; } }
			public Color Highlight { get { return HIGHLIGHT; } }
			public Color MenuBorder { get { return MENU_BORDER; } }
			public Color VerticalSeparator0 { get { return VERTICAL_SEPARATOR0; } }
			public Color VerticalSeparator1 { get { return VERTICAL_SEPARATOR1; } }
			public Color Pressed { get { return PRESSED; } }
			public Color CheckedBorder { get { return CHECKED_BORDER; } }

			public Color Text { get { return TEXT; } }
			public Color ArrowNormal { get { return ARROW_NORMAL; } }
			public Color ArrowHighlight { get { return ARROW_HIGHLIGHT; } }

			public Color ToolStripBackground { get { return TOOL_STRIP_BACKGROUND; } }
			public Color MenuStripBackground { get { return MENU_STRIP_BACKGROUND; } }
			public Color StatusStripBackground { get { return STATUS_STRIP_BACKGROUND; } }
			public Color DropDownBackground { get { return DROP_DOWN_BACKGROUND; } }
			public Color StatusLabelBackground { get { return STATUS_STRIP_BACKGROUND; } }
			public Color ContentPanelBackground { get { return CONTENT_PANEL_BACKGROUND; } }
			public Color TextBoxBackground { get { return TEXT_BOX_BACKGROUND; } }

			public Color SelectedCheckboxBackground { get { return SELECTED_CHECKBOX_BACKGROUND; } }
			public Color SelectedCheckboxForeground { get { return SELECTED_CHECKBOX_FOREGROUND; } }
			public Color NormalCheckboxBackground { get { return NORMAL_CHECKBOX_BACKGROUND; } }
			public Color NormalCheckboxForeground { get { return NORMAL_CHECKBOX_FOREGROUND; } }

			#endregion
		}

		private static IColorTable _darkColors;
		private static IColorTable _lightColors;

		public static IColorTable DarkColors
		{
			get
			{
				if(_darkColors == null)
				{
					_darkColors = new DarkColorTable();
				}
				return _darkColors;
			}
		}

		public static IColorTable LightColors
		{
			get
			{
				if(_lightColors == null)
				{
					_lightColors = new LightColorTable();
				}
				return _lightColors;
			}
		}

		#endregion

		#region Data

		private readonly IColorTable _colorTable;

		#endregion

		#region .ctor

		public MSVS2012StyleToolStripRenderer(IColorTable colorTable)
		{
			Verify.Argument.IsNotNull(colorTable, "colorTable");

			_colorTable = colorTable;
		}

		#endregion

		#region Properties

		private IColorTable ColorTable
		{
			get { return _colorTable; }
		}

		#endregion

		#region Stage 0 - Initialization

		protected override void Initialize(ToolStrip toolStrip)
		{
			if(toolStrip is ToolStripDropDown)
			{
				toolStrip.BackColor = ColorTable.DropDownBackground;
			}
			else if(toolStrip is StatusStrip)
			{
				toolStrip.BackColor = ColorTable.StatusStripBackground;
			}
			else if(toolStrip is MenuStrip)
			{
				toolStrip.BackColor = ColorTable.MenuStripBackground;
			}
			else
			{
				toolStrip.BackColor = ColorTable.ToolStripBackground;
			}
			toolStrip.ForeColor = ColorTable.Text;
		}

		protected override void InitializePanel(ToolStripPanel toolStripPanel)
		{
			toolStripPanel.BackColor = ColorTable.ToolStripBackground;
		}

		protected override void InitializeContentPanel(ToolStripContentPanel contentPanel)
		{
			contentPanel.BackColor = ColorTable.ContentPanelBackground;
		}

		protected override void InitializeItem(ToolStripItem item)
		{
			var tsTextBox = item as ToolStripTextBox;
			if(tsTextBox!= null)
			{
				tsTextBox.BorderStyle = BorderStyle.FixedSingle;
				tsTextBox.BackColor = ColorTable.TextBoxBackground;
				tsTextBox.ForeColor = ColorTable.Text;
			}
		}

		#endregion

		#region Stage 1 - Container Backgrounds

		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
		{
			var strip = e.ToolStrip;
			if(strip is ToolStripDropDown)
			{
				RenderDropDownBackground(e);
			}
			else if(strip is MenuStrip)
			{
				RenderMenuStripBackground(e);
			}
			else if(strip is StatusStrip)
			{
				RenderStatusStripBackground(e);
			}
			else
			{
				RenderToolStripBackgroundInternal(e);
			}
		}

		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
		{
		}

		protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
		{
			using(var brush = new SolidBrush(ColorTable.ContentPanelBackground))
			{
				e.Graphics.FillRectangle(brush, new Rectangle(Point.Empty, e.ToolStripContentPanel.Size));
			}
			e.Handled = true;
		}

		protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
		{
			using(var b = new SolidBrush(ColorTable.ToolStripBackground))
			{
				e.Graphics.FillRectangle(b, e.ToolStripPanel.Bounds);
			}
			e.Handled = true;
		}

		protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
		{
			int x = e.ToolStrip.Bounds.Width - 13;
			int y = e.ToolStrip.Bounds.Height - 13;
			using(var brush0 = new SolidBrush(ColorTable.ResizeGrip0))
			using(var brush1 = new SolidBrush(ColorTable.ResizeGrip1))
			{
				for(int i = 0; i < 5; ++i)
				{
					for(int j = 0; j < 5; ++j)
					{
						if(i + j >= 3)
						{
							e.Graphics.FillRectangle(brush0, new Rectangle(x + i * 3 + 0, y + j * 3 + 0, 1, 1));
							e.Graphics.FillRectangle(brush1, new Rectangle(x + i * 3 + 1, y + j * 3 + 1, 1, 1));
						}
					}
				}
			}
		}

		#endregion

		#region Stage 2 - Item Backgrounds

		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
		{
			var size = e.Item.Size;
			if(e.Vertical)
			{
				var x = size.Width / 2;
				var y = 4;
				using(var pen = new Pen(ColorTable.VerticalSeparator0))
				{
					e.Graphics.DrawLine(pen, x, y, x, y + size.Height - 8);
				}
				++x;
				using(var pen = new Pen(ColorTable.VerticalSeparator1))
				{
					e.Graphics.DrawLine(pen, x, y, x, y + size.Height - 8);
				}
			}
			else
			{
				var x = 0;
				var y = size.Height / 2;
				using(var pen = new Pen(ColorTable.MenuBorder))
				{
					e.Graphics.DrawLine(pen, x + 26, y, x + size.Width - 3, y);
				}
			}
		}

		protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
		{
			const int GripSize = 5;

			if(e.GripStyle != ToolStripGripStyle.Visible) return;

			var graphics = e.Graphics;
			var client = e.GripBounds;
			switch(e.GripDisplayStyle)
			{
				case ToolStripGripDisplayStyle.Horizontal:
					client.Y += (client.Width - GripSize) / 2;
					client.Height = 5;
					break;
				case ToolStripGripDisplayStyle.Vertical:
					client.X += (client.Width - GripSize) / 2;
					client.Width = 5;
					break;
			}
			if(client.Width <= 0 || client.Height <= 0) return;
			using(var brush = new HatchBrush(HatchStyle.Percent20, ColorTable.Grip, ColorTable.ToolStripBackground))
			{
				var ro = default(Point);
				try
				{
					ro = graphics.RenderingOrigin;
					graphics.RenderingOrigin = new Point(client.X % 4, client.Y % 4);
				}
				catch(NotImplementedException)
				{
				}
				graphics.FillRectangle(brush, client);
				try
				{
					graphics.RenderingOrigin = ro;
				}
				catch(NotImplementedException)
				{
				}
			}
		}

		protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
		{
			var item = (ToolStripButton)e.Item;
			RenderItemBackgroundInternal(e.Graphics, item.Width, item.Height, item.Pressed, item.Selected || item.Checked);
			if(item.Checked)
			{
				using(var pen = new Pen(ColorTable.CheckedBorder))
				{
					e.Graphics.DrawRectangle(pen, 0, 0, item.Width - 1, item.Height - 1);
				}
			}
		}

		protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
		{
			var item = e.Item as ToolStripDropDownButton;
			RenderItemBackgroundInternal(e.Graphics, item.Width, item.Height, item.Pressed, item.Selected && item.Enabled);
			var arrowBounds = new Rectangle(item.Width - 16, 0, 16, item.Height);
			DrawArrow(new ToolStripArrowRenderEventArgs(
				e.Graphics, e.Item,
				arrowBounds,
				Color.Black, ArrowDirection.Down));
		}

		protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
		{
			RenderItemBackgroundInternal(e.Graphics, e.Item.Width, e.Item.Height, e.Item.Pressed, e.Item.Selected);
		}

		protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
		{
			base.OnRenderLabelBackground(e);
		}

		protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
		{
			base.OnRenderOverflowButtonBackground(e);
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			var item = e.Item as ToolStripMenuItem;
			RenderMenuItemBackgroundInternal(e.Graphics, 0, 0, item.Width, item.Height, item.Pressed, item.Selected && item.Enabled, e.ToolStrip is MenuStrip);
		}

		protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
		{
			var splitButton = e.Item as ToolStripSplitButton;
			if(splitButton.DropDownButtonPressed)
			{
				RenderItemBackgroundInternal(e.Graphics, e.Item.Width, e.Item.Height, true, e.Item.Selected);
			}
			else
			{
				if(splitButton.ButtonPressed)
				{
					RenderItemBackgroundInternal(e.Graphics,
						splitButton.ButtonBounds.Width + 1,
						splitButton.ButtonBounds.Height,
						true, false);

					RenderItemBackgroundInternal(e.Graphics,
						splitButton.DropDownButtonBounds.X - 1, 0,
						splitButton.DropDownButtonBounds.Width + 1,
						splitButton.DropDownButtonBounds.Height,
						false, true);
				}
				else if(splitButton.Selected)
				{
					RenderItemBackgroundInternal(e.Graphics, e.Item.Width, e.Item.Height, false, true);
					var x = splitButton.ButtonBounds.Right;
					using(var pen = new Pen(ColorTable.ToolStripBackground))
					{
						e.Graphics.DrawLine(pen, x, 0, x, splitButton.Height - 1);
					}
				}
			}

			DrawArrow(new ToolStripArrowRenderEventArgs(
				e.Graphics, e.Item,
				splitButton.DropDownButtonBounds,
				Color.Black, ArrowDirection.Down));
		}

		protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
		{
			using(var b = new SolidBrush(ColorTable.StatusLabelBackground))
			{
				e.Graphics.FillRectangle(b, e.Item.Bounds);
			}
		}

		#endregion

		#region Stage 3 - Item Foreground Effects

		protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
		{
			if(e.Image != null)
			{
				var item = e.Item as ToolStripMenuItem;
				if(item != null && item.Checked)
				{
					RenderItemBackgroundInternal(e.Graphics,
						e.ImageRectangle.X - 2,
						e.ImageRectangle.Y - 2,
						e.ImageRectangle.Width + 4,
						e.ImageRectangle.Height + 4,
						true, false);
				}
				if(!e.Item.Enabled)
				{
					base.OnRenderItemImage(e);
				}
				else
				{
					e.Graphics.DrawImage(e.Image, e.ImageRectangle);
				}
			}
		}

		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
		{
			Color checkboxBackground;
			Color checkboxForeground;
			if(e.Item.Selected)
			{
				checkboxBackground = ColorTable.SelectedCheckboxBackground;
				checkboxForeground = ColorTable.SelectedCheckboxForeground;
			}
			else
			{
				checkboxBackground = ColorTable.NormalCheckboxBackground;
				checkboxForeground = ColorTable.NormalCheckboxForeground;
			}
			var graphics = e.Graphics;
			var rect = e.ImageRectangle;
			using(var brush = new SolidBrush(checkboxBackground))
			{
				graphics.FillRectangle(brush, rect);
			}
			var rc1 = rect;
			rc1.Width -= 1;
			rc1.Height -= 1;
			var rc2 = rc1;
			rc2.X += 1;
			rc2.Y += 1;
			rc2.Width -= 2;
			rc2.Height -= 2;
			using(var pen = new Pen(checkboxForeground))
			{
				graphics.DrawRectangle(pen, rc1);
				graphics.DrawRectangle(pen, rc2);
			}
			using(var pen = new Pen(checkboxForeground, 1.7f))
			{
				var path = new Point[]
					{
						new Point(rc2.X + 3,  6 + rc2.Y),
						new Point(rc2.X + 5,  9 + rc2.Y),
						new Point(rc2.X + 10, 2 + rc2.Y),
					};
				var oldMode = graphics.SmoothingMode;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.DrawLines(pen, path);
				graphics.SmoothingMode = oldMode;
			}
		}

		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
		{
			if(e.Item.Enabled)
			{
				if(e.Item.Selected)
				{
					e.ArrowColor = ColorTable.ArrowHighlight;
				}
				else
				{
					e.ArrowColor = ColorTable.ArrowNormal;
				}
			}
			else
			{
				e.ArrowColor = SystemColors.GrayText;
			}
			base.OnRenderArrow(e);
		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			e.TextColor = ColorTable.Text;
			base.OnRenderItemText(e);
		}
		
		#endregion

		#region Stage 4 - Paint the borders on the toolstrip if necessary

		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			if(e.ToolStrip is ToolStripDropDown)
			{
				RenderDropDownBorder(e);
			}
			else
			{
				base.OnRenderToolStripBorder(e);
			}
		}

		#endregion

		private void RenderMenuStripBackground(ToolStripRenderEventArgs e)
		{
			var strip = e.ToolStrip;
			var rc = new Rectangle(0, 0, strip.Width, strip.Height);
			using(var brush = new SolidBrush(ColorTable.MenuStripBackground))
			{
				e.Graphics.FillRectangle(brush, e.AffectedBounds);
			}
		}

		private void RenderStatusStripBackground(ToolStripRenderEventArgs e)
		{
			using(var brush = new SolidBrush(ColorTable.StatusStripBackground))
			{
				e.Graphics.FillRectangle(brush, e.AffectedBounds);
			}
		}

		private void RenderDropDownBackground(ToolStripRenderEventArgs e)
		{
			var strip = e.ToolStrip;
			var rc = new Rectangle(1, 1, strip.Width - 2, strip.Height - 2);
			using(var brush = new SolidBrush(ColorTable.DropDownBackground))
			{
				e.Graphics.FillRectangle(brush, rc);
			}
		}

		private void RenderToolStripBackgroundInternal(ToolStripRenderEventArgs e)
		{
			using(var brush = new SolidBrush(ColorTable.MenuStripBackground))
			{
				e.Graphics.FillRectangle(brush, e.AffectedBounds);
			}
		}

		private void RenderDropDownBorder(ToolStripRenderEventArgs e)
		{
			var strip = e.ToolStrip as ToolStripDropDown;
			var rc = new Rectangle(0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
			using(var pen = new Pen(ColorTable.MenuBorder))
			{
				e.Graphics.DrawRectangle(pen, rc);
			}
			using(var brush = new SolidBrush(ColorTable.DropDownBackground))
			{
				e.Graphics.FillRectangle(brush, e.ConnectedArea);
			}
		}

		private void RenderMenuItemBackgroundInternal(Graphics graphics, int x, int y, int width, int height, bool isPressed, bool isSelected, bool isRoot)
		{
			var rc = new Rectangle(x, y, width - 1, height - 1);
			if(isPressed)
			{
				if(isRoot)
				{
					using(var brush = new SolidBrush(ColorTable.DropDownBackground))
					{
						graphics.FillRectangle(brush, rc);
					}
					rc.Height += 5;
					using(var pen = new Pen(ColorTable.MenuBorder))
					{
						graphics.DrawRectangle(pen, rc);
					}
				}
				else
				{
					rc.Offset(2, 1);
					rc.Width -= 2;
					rc.Height -= 1;
					using(var brush = new SolidBrush(ColorTable.Highlight))
					{
						graphics.FillRectangle(brush, rc);
					}
				}
			}
			else if(isSelected)
			{
				if(isRoot)
				{
					rc.Offset(1, 1);
					rc.Width -= 1;
				}
				else
				{
					rc.Offset(2, 1);
					rc.Width -= 2;
				}
				rc.Height -= 1;
				using(var brush = new SolidBrush(ColorTable.Highlight))
				{
					graphics.FillRectangle(brush, rc);
				}
			}
		}

		private void RenderItemBackgroundInternal(Graphics graphics, int x, int y, int width, int height, bool pressed, bool selected)
		{
			if(pressed)
			{
				using(var brush = new SolidBrush(ColorTable.Pressed))
				{
					graphics.FillRectangle(brush, x, y, width, height);
				}
			}
			else if(selected)
			{
				using(var brush = new SolidBrush(ColorTable.Highlight))
				{
					graphics.FillRectangle(brush, x, y, width, height);
				}
			}
		}

		private void RenderItemBackgroundInternal(Graphics graphics, int width, int height, bool pressed, bool selected)
		{
			RenderItemBackgroundInternal(graphics, 0, 0, width, height, pressed, selected);
		}
	}
}
