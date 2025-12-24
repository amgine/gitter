#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

[DefaultProperty(nameof(Text))]
[DesignerCategory("")]
public partial class GroupSeparator : Control
{
	private Color? _separatorColor;

	/// <summary>Create <see cref="GroupSeparator"/>.</summary>
	public GroupSeparator()
	{
		Name = nameof(GroupSeparator);
		Size = new(407, 19);

		SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
		SetStyle(ControlStyles.Selectable | ControlStyles.ContainerControl, false);
	}

	public IGitterStyle Style { get; set; } = GitterApplication.Style;

	public Color? SeparatorColor
	{
		get => _separatorColor;
		set
		{
			if(_separatorColor == value) return;
			_separatorColor = value;
			Invalidate();
		}
	}

	protected override bool ScaleChildren => false;

	private void PaintBackground(Graphics graphics, Rectangle bounds, Rectangle rcText, Rectangle clip)
	{
		var conv = DpiConverter.FromDefaultTo(this);
		using var gdi = graphics.AsGdi();
		gdi.Fill(BackColor, clip);
		var h = conv.ConvertY(1);
		var t = conv.ConvertY(2);
		var rcLine = new Rectangle(0, t + (bounds.Height - h) / 2, bounds.Width, h);
		if(rcText.Width > 0)
		{
			var s = conv.ConvertX(4);
			var dx = rcText.Right + s;
			rcLine.X     += dx;
			rcText.Width -= dx;
		}
		var color = SeparatorColor ?? Style.Colors.GrayText;
		if(Rectangle.Intersect(rcLine, clip) is { Width: > 0, Height: > 0 } clippedLine)
		{
			gdi.Fill(color, clippedLine);
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		Assert.IsNotNull(e);

		const TextFormatFlags baseFlags    = TextFormatFlags.SingleLine | TextFormatFlags.NoPadding;
		const TextFormatFlags measureFlags = baseFlags;
		const TextFormatFlags drawFlags    = baseFlags | TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis;

		var rc   = ClientRectangle;
		var text = Text;
		if(string.IsNullOrEmpty(text))
		{
			PaintBackground(e.Graphics, rc, Rectangle.Empty, e.ClipRectangle);
			return;
		}
		var font = Font;
		var rcText = new Rectangle(rc.X, rc.Y, 0, rc.Height);
		var textSize = TextRenderer.MeasureText(text, font, new(short.MaxValue, rc.Height), measureFlags);
		rcText.Width  = Math.Min(textSize.Width, rc.Width);
		rcText.Y += (rcText.Height - textSize.Height) / 2;
		rcText.Height = textSize.Height;
		PaintBackground(e.Graphics, rc, rcText, e.ClipRectangle);
		TextRenderer.DrawText(e.Graphics, text, font, rcText, ForeColor, BackColor, drawFlags);
	}

	protected override void OnPaintBackground(PaintEventArgs pevent)
	{
	}
}
