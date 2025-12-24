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
using System.Windows.Forms;

[DesignerCategory("")]
public class LabelControl : Control
{
	public LabelControl()
	{
		SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
		SetStyle(ControlStyles.Selectable | ControlStyles.ContainerControl, false);
	}

	public bool WordBreak { get; set; }

	protected override void OnPaint(PaintEventArgs e)
	{
		var bc = BackColor;
		e.Graphics.GdiFill(bc, e.ClipRectangle);
		var text = Text;
		if(!string.IsNullOrEmpty(text))
		{
			var color = Enabled ? ForeColor : GitterApplication.Style.Colors.GrayText;
			var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis;
			if(WordBreak) flags |= TextFormatFlags.WordBreak;
			TextRenderer.DrawText(e.Graphics, text, Font, ClientRectangle, color, bc, flags);
		}
	}

	protected sealed override void OnPaintBackground(PaintEventArgs pevent) { }

	protected override void OnTextChanged(EventArgs e)
		=> Invalidate();
}
