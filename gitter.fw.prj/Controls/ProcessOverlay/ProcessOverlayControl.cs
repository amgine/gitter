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

namespace gitter.Framework.Controls;

using System;
using System.ComponentModel;
using System.Windows.Forms;

[DesignerCategory("")]
public partial class ProcessOverlayControl : UserControl
{
	private readonly ProcessOverlay _overlay;

	public ProcessOverlayControl()
	{
		AutoScaleMode = AutoScaleMode.Dpi;
		_overlay = new ProcessOverlay(this);
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_overlay.Dispose();
		}
		base.Dispose(disposing);
	}

	/// <inheritdoc/>
	protected override void OnPaintBackground(PaintEventArgs e)
	{
	}

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		e.Graphics.Clear(BackColor);
		e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
		_overlay.DrawMessage(e.Graphics, ClientRectangle, "Test status");
	}
}
