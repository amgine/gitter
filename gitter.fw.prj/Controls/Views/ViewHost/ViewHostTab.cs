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
using System.Windows.Forms;

public sealed class ViewHostTab : ViewTabBase
{
	private readonly ViewHost _viewHost;
	private bool _buttonsHovered;

	public ViewHostTab(ViewHostTabs tabs, ViewBase view)
		: base(view, tabs.Side)
	{
		Verify.Argument.IsNotNull(tabs);

		Tabs = tabs;
		_viewHost = tabs.ViewHost;
		Buttons = new ViewButtons(tabs);
		if(_viewHost.IsDocumentWell)
		{
			Buttons.SetAvailableButtons(ViewButtonType.Close);
		}
		var dpi = Dpi.FromControl(tabs);
		Buttons.Height = Renderer.TabHeight.GetValue(dpi) + Renderer.TabFooterHeight.GetValue(dpi);
		Buttons.ButtonClick += OnButtonClick;
	}

	private void OnButtonClick(object sender, ViewButtonClickEventArgs e)
	{
		Assert.IsNotNull(e);

		switch(e.Button)
		{
			case ViewButtonType.Close:
				View.Close();
				break;
		}
	}

	public void EnsureVisible() => Tabs.EnsureVisible(this);

	public ViewHostTabs Tabs { get; }

	public override ViewHost ViewHost => Tabs.ViewHost;

	public ViewButtons Buttons { get; }

	/// <inheritdoc/>
	protected override int Measure(Graphics graphics)
		=> ViewManager.Renderer.MeasureViewHostTabLength(this, graphics);

	/// <inheritdoc/>
	internal override void OnPaint(Graphics graphics, Rectangle bounds)
	{
		Assert.IsNotNull(graphics);

		if(bounds is { Width: > 0, Height: > 0 })
		{
			ViewManager.Renderer.RenderViewHostTabBackground(this, graphics, bounds);
			ViewManager.Renderer.RenderViewHostTabContent   (this, graphics, bounds);
		}
	}

	/// <inheritdoc/>
	public override void InvalidateLayout()
	{
		base.InvalidateLayout();
	}

	/// <inheritdoc/>
	protected internal override void OnMouseLeave()
	{
		base.OnMouseLeave();
		Buttons.OnMouseLeave();
		Tabs.Invalidate();
	}

	/// <inheritdoc/>
	protected internal override void OnMouseEnter()
	{
		base.OnMouseEnter();
		Tabs.Invalidate();
	}

	/// <inheritdoc/>
	public override void OnMouseDown(int x, int y, MouseButtons button)
	{
		base.OnMouseDown(x, y, button);
		switch(button)
		{
			case MouseButtons.Middle:
				View.Close();
				return;
			case MouseButtons.Right:
				return;
		}
		var dpi = Dpi.FromControl(Tabs);
		var buttonsBounds = new Rectangle(Length - Buttons.Width - 2, 0, Buttons.Width, ViewManager.Renderer.TabHeight.GetValue(dpi));
		if(buttonsBounds.Contains(x, y))
		{
			x -= buttonsBounds.X;
			y -= buttonsBounds.Y;
			Buttons.OnMouseDown(x, y, button);
		}
		if(Buttons.PressedButton is null)
		{
			View.Activate();
		}
	}

	/// <inheritdoc/>
	public override void OnMouseMove(int x, int y, MouseButtons button)
	{
		base.OnMouseMove(x, y, button);
		var dpi = Dpi.FromControl(Tabs);
		var buttonsBounds = new Rectangle(Length - Buttons.Width - 2, 0, Buttons.Width, ViewManager.Renderer.TabHeight.GetValue(dpi));
		if(buttonsBounds.Contains(x, y))
		{
			_buttonsHovered = true;
			x -= buttonsBounds.X;
			y -= buttonsBounds.Y;
			Buttons.OnMouseMove(x, y, button);
		}
		else
		{
			if(_buttonsHovered)
			{
				Buttons.OnMouseLeave();
				_buttonsHovered = false;
			}
		}
	}

	/// <inheritdoc/>
	public override void OnMouseUp(int x, int y, MouseButtons button)
	{
		base.OnMouseUp(x, y, button);
		var dpi = Dpi.FromControl(Tabs);
		var buttonsBounds = new Rectangle(Length - Buttons.Width - 2, 0, Buttons.Width, ViewManager.Renderer.TabHeight.GetValue(dpi));
		if(Buttons.PressedButton != null || buttonsBounds.Contains(x, y))
		{
			x -= buttonsBounds.X;
			y -= buttonsBounds.Y;
			Buttons.OnMouseUp(x, y, button);
		}
	}
}
