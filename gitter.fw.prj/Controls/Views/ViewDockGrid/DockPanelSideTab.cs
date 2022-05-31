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
using System.Drawing;

public sealed class DockPanelSideTab : ViewTabBase
{
	public DockPanelSideTab(DockPanelSide side, ViewHost viewHost, ViewBase view)
		: base(view, Utility.InvertAnchor(side.Side))
	{
		Verify.Argument.IsNotNull(side);
		Verify.Argument.IsNotNull(viewHost);

		Side = side;
		ViewHost = viewHost;
	}

	public DockPanelSide Side { get; }

	public override ViewHost ViewHost { get; }

	/// <inheritdoc/>
	public override bool IsActive => false;

	/// <inheritdoc/>
	protected override int Measure(Graphics graphics)
	{
		return Renderer.MeasureViewDockSideTabLength(this, graphics);
	}

	/// <inheritdoc/>
	internal override void OnPaint(Graphics graphics, Rectangle bounds)
	{
		Assert.IsNotNull(graphics);

		if(bounds is { Width: > 0, Height: > 0 })
		{
			Renderer.RenderViewDockSideTabBackground(this, graphics, bounds);
			Renderer.RenderViewDockSideTabContent   (this, graphics, bounds);
		}
	}
}
