#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Controls;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

[DesignerCategory("")]
public class BorderControl : Control
{
	protected readonly record struct Colors(
		Color BackColor,
		Color ForeColor,
		Color BorderColor);

	protected sealed record class ColorTable(
		Colors Normal,
		Colors Hover,
		Colors Focused,
		Colors Disabled)
	{
		public static ColorTable Dark { get; } = new(
			Normal: new(
				BackColor:   Color.FromArgb( 45,  45,  45),
				ForeColor:   Color.White,
				BorderColor: Color.FromArgb(154, 154, 154)),
			Hover: new(
				BackColor:   Color.FromArgb( 50,  50,  50),
				ForeColor:   Color.White,
				BorderColor: Color.FromArgb(200, 200, 200)),
			Focused: new(
				BackColor:   Color.FromArgb( 31,  31,  31),
				ForeColor:   Color.White,
				BorderColor: SystemColors.Highlight),
			Disabled: new(
				BackColor:   Color.FromArgb( 42,  42,  42),
				ForeColor:   Color.FromArgb(120, 120, 120),
				BorderColor: Color.FromArgb( 90,  90,  90)));

		public static ColorTable Light { get; } = new(
			Normal: new(
				BackColor:   Color.White,
				ForeColor:   Color.Black,
				BorderColor: Color.FromArgb(125, 125, 125)),
			Hover: new(
				BackColor:   Color.White,
				ForeColor:   Color.Black,
				BorderColor: Color.FromArgb(100, 100, 100)),
			Focused: new(
				BackColor:   Color.White,
				ForeColor:   Color.Black,
				BorderColor: SystemColors.Highlight),
			Disabled: new(
				BackColor:   Color.White,
				ForeColor:   Color.Black,
				BorderColor: Color.FromArgb(125, 125, 125)));
	}

	protected virtual void OnIsMouseOverChanged(EventArgs e)
	{
	}

	protected BorderControl()
	{
		SetStyle(
			ControlStyles.UserPaint |
			ControlStyles.AllPaintingInWmPaint |
			ControlStyles.OptimizedDoubleBuffer |
			ControlStyles.ResizeRedraw, true);
	}

	private int _mouseOverCounter;

	protected static IDpiBoundValue<int> BorderThickness { get; } = DpiBoundValue.ScaleY(1);

    protected IGitterStyle Style { get; } = GitterApplication.Style;

	public bool IsMouseOver => _mouseOverCounter > 0;

	protected virtual bool IsFocused => Focused;

    /// <inheritdoc/>
    protected override bool ScaleChildren => false;

	protected virtual ColorTable GetColorTable()
		=> Style.Type == GitterStyleType.DarkBackground
			? ColorTable.Dark
			: ColorTable.Light;

	protected virtual Colors GetColors()
	{
		var table = GetColorTable();
		if(!Enabled)    return table.Disabled;
		if(IsFocused)   return table.Focused;
		if(IsMouseOver) return table.Hover;
		return table.Normal;
	}

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Assert.IsNotNull(e);

		var bounds          = ClientRectangle;
		var colors          = GetColors();
		var borderThickness = BorderThickness.GetValue(new Dpi(DeviceDpi));

		using(var hdc = e.Graphics.AsGdi())
		{
			hdc.DrawBorder(bounds, colors.BorderColor, colors.BackColor, borderThickness);
		}
		bounds.Inflate(-borderThickness, -borderThickness);
		PaintContent(e.Graphics, bounds, e.ClipRectangle, colors);
	}

    /// <inheritdoc/>
    protected override void OnPaintBackground(PaintEventArgs pevent) { }

	protected virtual void PaintContent(Graphics graphics, Rectangle bounds, Rectangle clip, Colors colors)
	{
	}

	/// <inheritdoc/>
	protected override void OnMouseEnter(EventArgs e)
	{
		IncreaseMouseOver();
		base.OnMouseEnter(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		DecreaseMouseOver();
		base.OnMouseLeave(e);
	}

	protected virtual void SetColors(Colors colors)
	{
		BackColor = colors.BackColor;
		ForeColor = colors.ForeColor;
	}

	protected void UpdateColors()
		=> SetColors(GetColors());

	protected void DecreaseMouseOver()
	{
		if(--_mouseOverCounter == 0)
		{
			UpdateColors();
			Invalidate();
			OnIsMouseOverChanged(EventArgs.Empty);
		}
	}

	protected void IncreaseMouseOver()
	{
		if(++_mouseOverCounter == 1)
		{
			UpdateColors();
			Invalidate();
			OnIsMouseOverChanged(EventArgs.Empty);
		}
	}
}
