﻿#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2018  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
public class PlainTextNotificationContent : NotificationContent
{
	public PlainTextNotificationContent(string message)
	{
		Message = message;

		Height = Math.Min(GitterApplication.TextRenderer.MeasureText(GraphicsUtility.MeasurementGraphics, message, Font, new Size(Dpi.System.X * ViewConstants.PopupWidth / 96, 0)).Height, 400 * Dpi.System.Y / 96);
	}

	public string Message { get; }

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Assert.IsNotNull(e);

		e.Graphics.GdiFill(BackColor, e.ClipRectangle);
		var text = Message;
		if(!string.IsNullOrWhiteSpace(text))
		{
			GitterApplication.TextRenderer.DrawText(
				e.Graphics, text,
				Font, ForeColor, ClientRectangle);
		}
	}
}
