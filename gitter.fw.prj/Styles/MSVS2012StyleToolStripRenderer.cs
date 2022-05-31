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

		public Color Grip => GRIP;
		public Color ResizeGrip0 => RESIZE_GRIP0;
		public Color ResizeGrip1 => RESIZE_GRIP1;
		public Color Highlight => HIGHLIGHT;
		public Color MenuBorder => MENU_BORDER;
		public Color VerticalSeparator0 => VERTICAL_SEPARATOR0;
		public Color VerticalSeparator1 => VERTICAL_SEPARATOR1;
		public Color Pressed => PRESSED;
		public Color CheckedBorder => CHECKED_BORDER;

		public Color Text => TEXT;
		public Color ArrowNormal => ARROW_NORMAL;
		public Color ArrowHighlight => ARROW_HIGHLIGHT;

		public Color ToolStripBackground => TOOL_STRIP_BACKGROUND;
		public Color MenuStripBackground => MENU_STRIP_BACKGROUND;
		public Color StatusStripBackground => STATUS_STRIP_BACKGROUND;
		public Color DropDownBackground => DROP_DOWN_BACKGROUND;
		public Color StatusLabelBackground => STATUS_STRIP_BACKGROUND;
		public Color ContentPanelBackground => CONTENT_PANEL_BACKGROUND;
		public Color TextBoxBackground => TEXT_BOX_BACKGROUND;

		public Color SelectedCheckboxBackground => SELECTED_CHECKBOX_BACKGROUND;
		public Color SelectedCheckboxForeground => SELECTED_CHECKBOX_FOREGROUND;
		public Color NormalCheckboxBackground => NORMAL_CHECKBOX_BACKGROUND;
		public Color NormalCheckboxForeground => NORMAL_CHECKBOX_FOREGROUND;

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

		public Color Grip => GRIP;
		public Color ResizeGrip0 => RESIZE_GRIP0;
		public Color ResizeGrip1 => RESIZE_GRIP1;
		public Color Highlight => HIGHLIGHT;
		public Color MenuBorder => MENU_BORDER;
		public Color VerticalSeparator0 => VERTICAL_SEPARATOR0;
		public Color VerticalSeparator1 => VERTICAL_SEPARATOR1;
		public Color Pressed => PRESSED;
		public Color CheckedBorder => CHECKED_BORDER;

		public Color Text => TEXT;
		public Color ArrowNormal => ARROW_NORMAL;
		public Color ArrowHighlight => ARROW_HIGHLIGHT;

		public Color ToolStripBackground => TOOL_STRIP_BACKGROUND;
		public Color MenuStripBackground => MENU_STRIP_BACKGROUND;
		public Color StatusStripBackground => STATUS_STRIP_BACKGROUND;
		public Color DropDownBackground => DROP_DOWN_BACKGROUND;
		public Color StatusLabelBackground => STATUS_STRIP_BACKGROUND;
		public Color ContentPanelBackground => CONTENT_PANEL_BACKGROUND;
		public Color TextBoxBackground => TEXT_BOX_BACKGROUND;

		public Color SelectedCheckboxBackground => SELECTED_CHECKBOX_BACKGROUND;
		public Color SelectedCheckboxForeground => SELECTED_CHECKBOX_FOREGROUND;
		public Color NormalCheckboxBackground => NORMAL_CHECKBOX_BACKGROUND;
		public Color NormalCheckboxForeground => NORMAL_CHECKBOX_FOREGROUND;

		#endregion
	}

	private static IColorTable _darkColors;
	private static IColorTable _lightColors;

	public static IColorTable DarkColors => _darkColors ??= new DarkColorTable();

	public static IColorTable LightColors => _lightColors ??= new LightColorTable();

	#endregion

	#region Data

	private readonly IColorTable _colorTable;

	#endregion

	#region .ctor

	public MSVS2012StyleToolStripRenderer(IColorTable colorTable)
	{
		Verify.Argument.IsNotNull(colorTable);

		_colorTable = colorTable;
	}

	#endregion

	#region Properties

	private IColorTable ColorTable => _colorTable;

	#endregion

	#region Stage 0 - Initialization

	protected override void Initialize(ToolStrip toolStrip)
	{
		Assert.IsNotNull(toolStrip);

		toolStrip.BackColor = toolStrip switch
		{
			ToolStripDropDown => ColorTable.DropDownBackground,
			StatusStrip       => ColorTable.StatusStripBackground,
			MenuStrip         => ColorTable.MenuStripBackground,
			_                 => ColorTable.ToolStripBackground,
		};
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
		Assert.IsNotNull(item);

		switch(item)
		{
			case ToolStripTextBox tsTextBox:
				tsTextBox.BorderStyle = BorderStyle.FixedSingle;
				tsTextBox.BackColor = ColorTable.TextBoxBackground;
				tsTextBox.ForeColor = ColorTable.Text;
				break;
		}
	}

	#endregion

	#region Stage 1 - Container Backgrounds

	protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		switch(e.ToolStrip)
		{
			case ToolStripDropDown:
				RenderDropDownBackground(e);
				break;
			case MenuStrip:
				RenderMenuStripBackground(e);
				break;
			case StatusStrip:
				RenderStatusStripBackground(e);
				break;
			default:
				RenderToolStripBackgroundInternal(e);
				break;
		}
	}

	protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
	{
	}

	protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		using(var brush = new SolidBrush(ColorTable.ContentPanelBackground))
		{
			e.Graphics.FillRectangle(brush, new Rectangle(Point.Empty, e.ToolStripContentPanel.Size));
		}
		e.Handled = true;
	}

	protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		using(var b = new SolidBrush(ColorTable.ToolStripBackground))
		{
			e.Graphics.FillRectangle(b, e.ToolStripPanel.Bounds);
		}
		e.Handled = true;
	}

	protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		int x = e.ToolStrip.Bounds.Width - 13;
		int y = e.ToolStrip.Bounds.Height - 13;
		using var brush0 = new SolidBrush(ColorTable.ResizeGrip0);
		using var brush1 = new SolidBrush(ColorTable.ResizeGrip1);
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

	#endregion

	#region Stage 2 - Item Backgrounds

	protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
	{
		Assert.IsNotNull(e);

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
		Assert.IsNotNull(e);

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
		using var brush = new HatchBrush(HatchStyle.Percent20, ColorTable.Grip, ColorTable.ToolStripBackground);
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

	protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		var item = (ToolStripButton)e.Item;
		RenderItemBackgroundInternal(e.Graphics, item.Width, item.Height, item.Pressed, item.Selected || item.Checked);
		if(item.Checked)
		{
			using var pen = new Pen(ColorTable.CheckedBorder);
			e.Graphics.DrawRectangle(pen, 0, 0, item.Width - 1, item.Height - 1);
		}
	}

	protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		if(e.Item is not ToolStripDropDownButton item) return;
		RenderItemBackgroundInternal(e.Graphics, item.Width, item.Height, item.Pressed, item.Selected && item.Enabled);
		var arrowBounds = new Rectangle(item.Width - 16, 0, 16, item.Height);
		DrawArrow(new ToolStripArrowRenderEventArgs(
			e.Graphics, e.Item,
			arrowBounds,
			Color.Black, ArrowDirection.Down));
	}

	protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		RenderItemBackgroundInternal(e.Graphics, e.Item.Width, e.Item.Height, e.Item.Pressed, e.Item.Selected);
	}

	protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		base.OnRenderLabelBackground(e);
	}

	protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		base.OnRenderOverflowButtonBackground(e);
	}

	protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		if(e.Item is not ToolStripMenuItem item) return;
		RenderMenuItemBackgroundInternal(e.Graphics, 0, 0, item.Width, item.Height, item.Pressed, item.Selected && item.Enabled, e.ToolStrip is MenuStrip);
	}

	protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		if(e.Item is not ToolStripSplitButton splitButton) return;
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
				using var pen = new Pen(ColorTable.ToolStripBackground);
				e.Graphics.DrawLine(pen, x, 0, x, splitButton.Height - 1);
			}
		}

		DrawArrow(new ToolStripArrowRenderEventArgs(
			e.Graphics, e.Item,
			splitButton.DropDownButtonBounds,
			Color.Black, ArrowDirection.Down));
	}

	protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		using var b = new SolidBrush(ColorTable.StatusLabelBackground);
		e.Graphics.FillRectangle(b, e.Item.Bounds);
	}

	#endregion

	#region Stage 3 - Item Foreground Effects

	protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		if(e.Image is null) return;

		var bounds = e.ImageRectangle;

		if(e.Item is ToolStripMenuItem { Checked: true } item)
		{
			var conv = new DpiConverter(e.ToolStrip);
			var dx = conv.ConvertX(2);
			var dy = conv.ConvertY(2);
			RenderItemBackgroundInternal(e.Graphics,
				bounds.X - dx,
				bounds.Y - dy,
				bounds.Width  + dx * 2,
				bounds.Height + dy * 2,
				true, false);
		}
		if(!e.Item.Enabled)
		{
			using var image = CreateDisabledImage(e.Image);
			e.Graphics.DrawImage(image, bounds);
		}
		else
		{
			e.Graphics.DrawImage(e.Image, bounds);
		}
	}

	protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
	{
		Assert.IsNotNull(e);

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
		var conv = new DpiConverter(e.ToolStrip);
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
		using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
		using(var pen = new Pen(checkboxForeground, conv.ConvertX(1.7f)))
		{
			var path = new Point[]
				{
					new Point(rc2.X + conv.ConvertX( 3), conv.ConvertY(6) + rc2.Y),
					new Point(rc2.X + conv.ConvertX( 5), conv.ConvertY(9) + rc2.Y),
					new Point(rc2.X + conv.ConvertX(10), conv.ConvertY(2) + rc2.Y),
				};
			graphics.DrawLines(pen, path);
		}
	}

	protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		if(e.Item.Enabled)
		{
			e.ArrowColor = e.Item.Selected
				? ColorTable.ArrowHighlight
				: ColorTable.ArrowNormal;
		}
		else
		{
			e.ArrowColor = SystemColors.GrayText;
		}
		base.OnRenderArrow(e);
	}

	protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		e.TextColor = ColorTable.Text;
		base.OnRenderItemText(e);
	}
		
	#endregion

	#region Stage 4 - Paint the borders on the toolstrip if necessary

	protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
	{
		Assert.IsNotNull(e);

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
		Assert.IsNotNull(e);

		using var brush = new SolidBrush(ColorTable.MenuStripBackground);
		e.Graphics.FillRectangle(brush, e.AffectedBounds);
	}

	private void RenderStatusStripBackground(ToolStripRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		using var brush = new SolidBrush(ColorTable.StatusStripBackground);
		e.Graphics.FillRectangle(brush, e.AffectedBounds);
	}

	private void RenderDropDownBackground(ToolStripRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		var strip = e.ToolStrip;
		var rc = new Rectangle(1, 1, strip.Width - 2, strip.Height - 2);
		using var brush = new SolidBrush(ColorTable.DropDownBackground);
		e.Graphics.FillRectangle(brush, rc);
	}

	private void RenderToolStripBackgroundInternal(ToolStripRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		using var brush = new SolidBrush(ColorTable.MenuStripBackground);
		e.Graphics.FillRectangle(brush, e.AffectedBounds);
	}

	private void RenderDropDownBorder(ToolStripRenderEventArgs e)
	{
		Assert.IsNotNull(e);

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
		Assert.IsNotNull(graphics);

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
				graphics.GdiFill(ColorTable.Highlight, rc);
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
			graphics.GdiFill(ColorTable.Highlight, rc);
		}
	}

	private void RenderItemBackgroundInternal(Graphics graphics, int x, int y, int width, int height, bool pressed, bool selected)
	{
		Assert.IsNotNull(graphics);

		if(pressed)
		{
			graphics.GdiFill(ColorTable.Pressed, new Rectangle(x, y, width, height));
		}
		else if(selected)
		{
			graphics.GdiFill(ColorTable.Highlight, new Rectangle(x, y, width, height));
		}
	}

	private void RenderItemBackgroundInternal(Graphics graphics, int width, int height, bool pressed, bool selected)
	{
		Assert.IsNotNull(graphics);

		RenderItemBackgroundInternal(graphics, 0, 0, width, height, pressed, selected);
	}
}
