﻿#region Copyright Notice
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

using System.Drawing;
using System.Windows.Forms;

/// <summary>Button which indicates specific docking position on dock marker.</summary>
public sealed class DockMarkerButton
{
	/// <summary>Initializes a new instance of the <see cref="DockMarkerButton"/> class.</summary>
	/// <param name="bounds">Button bounds.</param>
	/// <param name="type">Button type.</param>
	internal DockMarkerButton(Rectangle bounds, DockResult type)
	{
		Bounds = bounds;
		Type   = type;
	}

	private ViewRenderer Renderer => ViewManager.Renderer;

	/// <summary>Gets the bounds of this <see cref="DockMarkerButton"/>.</summary>
	/// <value>Bounds of this <see cref="DockMarkerButton"/>.</value>
	public Rectangle Bounds { get; }

	/// <summary>Gets the docking position associated with this button.</summary>
	/// <value>Docking position associated with this button.</value>
	public DockResult Type { get; }

	/// <summary>Paints this <see cref="DockMarkerButton"/>.</summary>
	/// <param name="host">Control that hosts the button.</param>
	/// <param name="graphics">The graphics surface to draw on.</param>
	/// <param name="hover">Indicates whether this button is hovered.</param>
	internal void OnPaint(Control host, Graphics graphics, bool hover)
		=> Renderer.RenderDockMarkerButton(this, host, graphics, hover);
}
