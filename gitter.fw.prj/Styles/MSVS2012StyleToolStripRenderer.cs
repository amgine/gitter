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

sealed class MSVS2012StyleToolStripRenderer(MSVS2012StyleToolStripRenderer.ColorTable colorTable) : ToolStripRenderer
{
	public record class ColorTable(
		Color Grip,
		Color ResizeGrip0,
		Color ResizeGrip1,
		Color Highlight,
		Color MenuBorder,
		Color VerticalSeparator0,
		Color VerticalSeparator1,
		Color Pressed,
		Color CheckedBorder,

		Color Text,
		Color ArrowNormal,
		Color ArrowHighlight,

		Color ToolStripBackground,
		Color MenuStripBackground,
		Color StatusStripBackground,
		Color DropDownBackground,
		Color StatusLabelBackground,
		Color ContentPanelBackground,
		Color TextBoxBackground,

		Color SelectedCheckboxBackground,
		Color SelectedCheckboxForeground,
		Color NormalCheckboxBackground,
		Color NormalCheckboxForeground)
	{
		public static ColorTable Dark { get; } = new(
			Grip:                       Color.FromArgb( 70,  70,  74),
			ResizeGrip0:                Color.FromArgb(  0,  92, 153),
			ResizeGrip1:                Color.FromArgb(127, 188, 229),
			Highlight:                  Color.FromArgb( 62,  62,  64),
			MenuBorder:                 Color.FromArgb( 51,  51,  55),
			VerticalSeparator0:         Color.FromArgb( 34,  34,  34),
			VerticalSeparator1:         Color.FromArgb( 70,  70,  74),
			Pressed:                    Color.FromArgb(  0, 122, 204),
			CheckedBorder:              Color.FromArgb( 51, 153, 255),
			Text:                       MSVS2012DarkColors.WINDOW_TEXT,
			ArrowNormal:                Color.FromArgb(153, 153, 153),
			ArrowHighlight:             Color.FromArgb(  0, 122, 204),
			ToolStripBackground:        MSVS2012DarkColors.WORK_AREA,
			MenuStripBackground:        MSVS2012DarkColors.WORK_AREA,
			StatusStripBackground:      MSVS2012DarkColors.WORK_AREA,
			DropDownBackground:         Color.FromArgb( 27,  27,  28),
			StatusLabelBackground:      MSVS2012DarkColors.WORK_AREA,
			ContentPanelBackground:     MSVS2012DarkColors.WORK_AREA,
			TextBoxBackground:          MSVS2012DarkColors.WINDOW,
			SelectedCheckboxBackground: Color.FromArgb( 62,  62,  64),
			SelectedCheckboxForeground: Color.FromArgb(241, 241, 241),
			NormalCheckboxBackground:   Color.FromArgb( 45,  45,  48),
			NormalCheckboxForeground:   Color.FromArgb(153, 153, 153));

		public static ColorTable Light { get; } = new(
			Grip:                       Color.FromArgb( 70,  70,  74),
			ResizeGrip0:                Color.FromArgb(  0,  92, 153),
			ResizeGrip1:                Color.FromArgb(127, 188, 229),
			Highlight:                  Color.FromArgb(248, 249, 250),
			MenuBorder:                 Color.FromArgb(204, 206, 219),
			VerticalSeparator0:         Color.FromArgb( 34,  34,  34),
			VerticalSeparator1:         Color.FromArgb( 70,  70,  74),
			Pressed:                    Color.FromArgb(  0, 122, 204),
			CheckedBorder:              Color.FromArgb( 51, 153, 255),
			Text:                       MSVS2012LightColors.WINDOW_TEXT,
			ArrowNormal:                Color.FromArgb(113, 113, 113),
			ArrowHighlight:             Color.FromArgb(  0, 122, 204),
			ToolStripBackground:        MSVS2012LightColors.WORK_AREA,
			MenuStripBackground:        MSVS2012LightColors.WORK_AREA,
			StatusStripBackground:      MSVS2012LightColors.WORK_AREA,
			DropDownBackground:         Color.FromArgb(231, 232, 236),
			StatusLabelBackground:      MSVS2012LightColors.WORK_AREA,
			ContentPanelBackground:     MSVS2012LightColors.WORK_AREA,
			TextBoxBackground:          MSVS2012LightColors.WINDOW,
			SelectedCheckboxBackground: Color.FromArgb( 62,  62,  64),
			SelectedCheckboxForeground: Color.FromArgb(241, 241, 241),
			NormalCheckboxBackground:   Color.FromArgb( 45,  45,  48),
			NormalCheckboxForeground:   Color.FromArgb(153, 153, 153));
	}

	#region Stage 0 - Initialization

	/// <inheritdoc/>
	protected override void Initialize(ToolStrip toolStrip)
	{
		Assert.IsNotNull(toolStrip);

		toolStrip.BackColor = toolStrip switch
		{
			ToolStripDropDown => colorTable.DropDownBackground,
			StatusStrip       => colorTable.StatusStripBackground,
			MenuStrip         => colorTable.MenuStripBackground,
			_                 => colorTable.ToolStripBackground,
		};
		toolStrip.ForeColor = colorTable.Text;

		if(colorTable == ColorTable.Dark)
		{
			OverrideToolTip(toolStrip);
		}
		ForceScaledFont(toolStrip);
	}

	private static Font GetFont(Dpi dpi)
		=> GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);

	private static Font GetFont(ToolStrip toolStrip)
		=> GetFont(Dpi.FromControl(toolStrip));

	private void ForceScaledFont(ToolStrip toolStrip)
	{
		toolStrip.Font = GetFont(toolStrip);
		toolStrip.DpiChangedAfterParent += OnDpiChangedAfterParent;
	}

	private void OnDpiChangedAfterParent(object? sender, EventArgs e)
	{
		if(sender is not ToolStrip ts) return;
		if(ts.Renderer != this)
		{
			ts.DpiChangedAfterParent -= OnDpiChangedAfterParent;
			return;
		}
		ts.Font = GetFont(ts);
	}

	private static void OverrideToolTip(ToolStrip toolStrip)
	{
		var prop = typeof(ToolStrip).GetProperty("ToolTip", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		if(prop is null) return;
		var toolTip = prop.GetValue(toolStrip) as ToolTip;
		if(toolTip is null) return;

		toolTip.BackColor = Color.FromArgb(46, 47, 47);
		toolTip.ForeColor = Color.FromArgb(255, 255, 255);
		toolTip.Draw += OnToolTipDraw;
		toolTip.OwnerDraw = true;
	}

	private static void OnToolTipDraw(object? sender, DrawToolTipEventArgs e)
	{
		if(sender is not ToolTip toolTip) return;
		e.DrawBackground();
		const TextFormatFlags textFlags =
			TextFormatFlags.VerticalCenter |
			TextFormatFlags.HidePrefix |
			TextFormatFlags.Left;
		TextRenderer.DrawText(e.Graphics, e.ToolTipText, e.Font, e.Bounds, toolTip.ForeColor, textFlags);
		e.DrawBorder();
	}

	/// <inheritdoc/>
	protected override void InitializePanel(ToolStripPanel toolStripPanel)
	{
		Assert.IsNotNull(toolStripPanel);

		toolStripPanel.BackColor = colorTable.ToolStripBackground;
	}

	/// <inheritdoc/>
	protected override void InitializeContentPanel(ToolStripContentPanel contentPanel)
	{
		Assert.IsNotNull(contentPanel);

		contentPanel.BackColor = colorTable.ContentPanelBackground;
	}

	/// <inheritdoc/>
	protected override void InitializeItem(ToolStripItem item)
	{
		Assert.IsNotNull(item);

		switch(item)
		{
			case ToolStripTextBox tsTextBox:
				tsTextBox.BorderStyle = BorderStyle.FixedSingle;
				tsTextBox.BackColor = colorTable.TextBoxBackground;
				tsTextBox.ForeColor = colorTable.Text;
				break;
		}
	}

	#endregion

	#region Stage 1 - Container Backgrounds

	/// <inheritdoc/>
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

	/// <inheritdoc/>
	protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
	{
	}

	/// <inheritdoc/>
	protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		e.Graphics.GdiFill(colorTable.ContentPanelBackground, new Rectangle(Point.Empty, e.ToolStripContentPanel.Size));
		e.Handled = true;
	}

	/// <inheritdoc/>
	protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		e.Graphics.GdiFill(colorTable.ToolStripBackground, e.ToolStripPanel.Bounds);
		e.Handled = true;
	}

	/// <inheritdoc/>
	protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		var dpi = Dpi.FromControl(e.ToolStrip);
		int x = e.ToolStrip.Bounds.Width  - 13 * dpi.X / 96;
		int y = e.ToolStrip.Bounds.Height - 13 * dpi.X / 96;
		var brush0 = colorTable.ResizeGrip0;
		var brush1 = colorTable.ResizeGrip1;
		using var gdi = e.Graphics.AsGdi();
		for(int i = 0; i < 5; ++i)
		{
			for(int j = 0; j < 5; ++j)
			{
				if(i + j >= 3)
				{
					gdi.Fill(brush0, new Rectangle(x + i * 3 + 0, y + j * 3 + 0, 1, 1));
					gdi.Fill(brush1, new Rectangle(x + i * 3 + 1, y + j * 3 + 1, 1, 1));
				}
			}
		}
	}

	#endregion

	#region Stage 2 - Item Backgrounds

	/// <inheritdoc/>
	protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		var size = e.Item.Size;
		if(e.Vertical)
		{
			var conv = e.ToolStrip is not null
				? DpiConverter.FromDefaultTo(e.ToolStrip)
				: DpiConverter.Identity;
			var w = conv.ConvertX(1);
			var x = (size.Width - w) / 2;
			var y = conv.ConvertY(4);
			using var brush = SolidBrushCache.Get(colorTable.VerticalSeparator1);
			e.Graphics.FillRectangle(brush, new Rectangle(x, y, w, size.Height - y * 2));
		}
		else
		{
			var x = 0;
			var y = size.Height / 2;
			using var pen = new Pen(colorTable.MenuBorder);
			e.Graphics.DrawLine(pen, x + 26, y, x + size.Width - 3, y);
		}
	}

	/// <inheritdoc/>
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
		using var brush = new HatchBrush(HatchStyle.Percent20, colorTable.Grip, colorTable.ToolStripBackground);
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

	/// <inheritdoc/>
	protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		var item = (ToolStripButton)e.Item;
		RenderItemBackgroundInternal(e.Graphics, item.Width, item.Height, item.Pressed, item.Selected || item.Checked);
		if(item.Checked)
		{
			using var pen = new Pen(colorTable.CheckedBorder);
			e.Graphics.DrawRectangle(pen, 0, 0, item.Width - 1, item.Height - 1);
		}
	}

	/// <inheritdoc/>
	protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		if(e.Item is not ToolStripDropDownButton item) return;
		RenderItemBackgroundInternal(e.Graphics, item.Width, item.Height, item.Pressed, item.Selected && item.Enabled);
		//var arrowBounds = new Rectangle(item.Width - 16, 0, 16, item.Height);
		//DrawArrow(new ToolStripArrowRenderEventArgs(
		//	e.Graphics, e.Item,
		//	arrowBounds,
		//	Color.Black, ArrowDirection.Down));
	}

	/// <inheritdoc/>
	protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		RenderItemBackgroundInternal(e.Graphics, e.Item.Width, e.Item.Height, e.Item.Pressed, e.Item.Selected);
	}

	/// <inheritdoc/>
	protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		base.OnRenderLabelBackground(e);
	}

	/// <inheritdoc/>
	protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		base.OnRenderOverflowButtonBackground(e);
	}

	/// <inheritdoc/>
	protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		if(e.Item is not ToolStripMenuItem item) return;
		RenderMenuItemBackgroundInternal(e.Graphics, 0, 0, item.Width, item.Height, item.Pressed, item.Selected && item.Enabled, e.ToolStrip is MenuStrip);
	}

	/// <inheritdoc/>
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
				using var pen = new Pen(colorTable.ToolStripBackground);
				e.Graphics.DrawLine(pen, x, 0, x, splitButton.Height - 1);
			}
		}

		DrawArrow(new ToolStripArrowRenderEventArgs(
			e.Graphics, e.Item,
			splitButton.DropDownButtonBounds,
			Color.Black, ArrowDirection.Down));
	}

	/// <inheritdoc/>
	protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		e.Graphics.GdiFill(colorTable.StatusLabelBackground, e.Item.Bounds);
	}

	#endregion

	#region Stage 3 - Item Foreground Effects

	/// <inheritdoc/>
	protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		if(e.Image is null) return;

		var bounds = e.ImageRectangle;

		if(e.Item is ToolStripMenuItem { Checked: true })
		{
			var conv = e.ToolStrip is not null
				? new DpiConverter(e.ToolStrip)
				: DpiConverter.Identity;
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

	/// <inheritdoc/>
	protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		Color checkboxBackground;
		Color checkboxForeground;
		if(e.Item.Selected)
		{
			checkboxBackground = colorTable.SelectedCheckboxBackground;
			checkboxForeground = colorTable.SelectedCheckboxForeground;
		}
		else
		{
			checkboxBackground = colorTable.NormalCheckboxBackground;
			checkboxForeground = colorTable.NormalCheckboxForeground;
		}
		var graphics = e.Graphics;
		var rect = e.ImageRectangle;
		var conv = e.ToolStrip is not null
			? new DpiConverter(e.ToolStrip)
			: DpiConverter.Identity;
		graphics.GdiFill(checkboxBackground, rect);
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
			graphics.DrawLines(pen,
				[
					new Point(rc2.X + conv.ConvertX( 3), conv.ConvertY(6) + rc2.Y),
					new Point(rc2.X + conv.ConvertX( 5), conv.ConvertY(9) + rc2.Y),
					new Point(rc2.X + conv.ConvertX(10), conv.ConvertY(2) + rc2.Y),
				]);
		}
	}

	/// <inheritdoc/>
	protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		if(e.Item is { Enabled: true })
		{
			e.ArrowColor = e.Item.Selected
				? colorTable.ArrowHighlight
				: colorTable.ArrowNormal;
		}
		else
		{
			e.ArrowColor = SystemColors.GrayText;
		}
		base.OnRenderArrow(e);
	}

	/// <inheritdoc/>
	protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		e.TextColor = colorTable.Text;
		base.OnRenderItemText(e);
	}

	#endregion

	#region Stage 4 - Paint the borders on the toolstrip if necessary

	/// <inheritdoc/>
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

		e.Graphics.GdiFill(colorTable.MenuStripBackground, e.AffectedBounds);
	}

	private void RenderStatusStripBackground(ToolStripRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		e.Graphics.GdiFill(colorTable.StatusStripBackground, e.AffectedBounds);
	}

	private void RenderDropDownBackground(ToolStripRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		var strip = e.ToolStrip;
		var rc = new Rectangle(1, 1, strip.Width - 2, strip.Height - 2);
		e.Graphics.GdiFill(colorTable.DropDownBackground, rc);
	}

	private void RenderToolStripBackgroundInternal(ToolStripRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		e.Graphics.GdiFill(colorTable.MenuStripBackground, e.AffectedBounds);
	}

	private void RenderDropDownBorder(ToolStripRenderEventArgs e)
	{
		Assert.IsNotNull(e);

		var rc = new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);
		using var gdi = e.Graphics.AsGdi();
		gdi.Rectangle(colorTable.MenuBorder, rc);
		gdi.Fill(colorTable.DropDownBackground, e.ConnectedArea);
	}

	private void RenderMenuItemBackgroundInternal(Graphics graphics, int x, int y, int width, int height, bool isPressed, bool isSelected, bool isRoot)
	{
		Assert.IsNotNull(graphics);

		var rc = new Rectangle(x, y, width - 1, height - 1);
		if(isPressed)
		{
			using var gdi = graphics.AsGdi();
			if(isRoot)
			{
				gdi.Fill(colorTable.DropDownBackground, rc);
				rc.Height += 5;
				gdi.Rectangle(colorTable.MenuBorder, rc);
			}
			else
			{
				rc.Offset(2, 1);
				rc.Width -= 2;
				rc.Height -= 1;
				gdi.Fill(colorTable.Highlight, rc);
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
			graphics.GdiFill(colorTable.Highlight, rc);
		}
	}

	private void RenderItemBackgroundInternal(Graphics graphics, int x, int y, int width, int height, bool pressed, bool selected)
	{
		Assert.IsNotNull(graphics);

		if(pressed)
		{
			graphics.GdiFill(colorTable.Pressed, new Rectangle(x, y, width, height));
		}
		else if(selected)
		{
			graphics.GdiFill(colorTable.Highlight, new Rectangle(x, y, width, height));
		}
	}

	private void RenderItemBackgroundInternal(Graphics graphics, int width, int height, bool pressed, bool selected)
	{
		Assert.IsNotNull(graphics);

		RenderItemBackgroundInternal(graphics, 0, 0, width, height, pressed, selected);
	}
}
