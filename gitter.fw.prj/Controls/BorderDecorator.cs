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
public abstract class BorderDecorator<T> : BorderControl
	where T : Control
{
	protected BorderDecorator(T decorated)
	{
		Decorated = decorated;

		decorated.MouseEnter     += OnDecoratedMouseEnter;
		decorated.MouseLeave     += OnDecoratedMouseLeave;
		decorated.GotFocus       += OnDecoratedGotFocus;
		decorated.LostFocus      += OnDecoratedLostFocus;
		decorated.EnabledChanged += OnDecoratedEnabledChanged;
		decorated.Parent = this;

		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
		SetStyle(ControlStyles.Selectable | ControlStyles.ContainerControl, false);
	}

    public T Decorated { get; }

    protected override Colors GetColors()
	{
		var table = Style.Type == GitterStyleType.DarkBackground
			? ColorTable.Dark
			: ColorTable.Light;
		if(!Enabled)           return table.Disabled;
		if(!Decorated.Enabled) return table.Disabled;
		if(Decorated.Focused)  return table.Focused;
		if(IsMouseOver)        return table.Hover;
		return table.Normal;
	}

    protected virtual Rectangle ModifyDecoratedBounds(Rectangle bounds) => bounds;

    protected virtual void UpdateDecoratedBounds()
    {
        var conv = DpiConverter.FromDefaultTo(new Dpi(DeviceDpi));
        var paddingX = conv.ConvertX(3);

        var bounds = ClientRectangle;
        var th = Decorated.Height;
        bounds.Y += (bounds.Height - th) / 2;
        bounds.Height = th;
        bounds.X += paddingX;
        bounds.Width -= 2 * paddingX;

        Decorated.Bounds = ModifyDecoratedBounds(bounds);
    }

    /// <inheritdoc/>
    protected override void OnBackColorChanged(EventArgs e)
	{
		Decorated.BackColor = BackColor;
		base.OnBackColorChanged(e);
	}

	/// <inheritdoc/>
	protected override void OnForeColorChanged(EventArgs e)
	{
		Decorated.ForeColor = ForeColor;
		base.OnForeColorChanged(e);
	}

    /// <inheritdoc/>
    protected override void OnHandleCreated(EventArgs e)
    {
        UpdateDecoratedBounds();
        base.OnHandleCreated(e);
    }

    /// <inheritdoc/>
    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        UpdateDecoratedBounds();
        base.OnDpiChangedAfterParent(e);
    }

    /// <inheritdoc/>
    protected override void OnSizeChanged(EventArgs e)
    {
        UpdateDecoratedBounds();
        base.OnSizeChanged(e);
    }

	protected virtual void OnDecoratedMouseEnter(object? sender, EventArgs e)
	{
		IncreaseMouseOver();
	}

	protected virtual void OnDecoratedMouseLeave(object? sender, EventArgs e)
	{
		DecreaseMouseOver();
	}

	protected virtual void OnDecoratedGotFocus(object? sender, EventArgs e)
	{
		UpdateColors();
		Invalidate();
	}

	protected virtual void OnDecoratedLostFocus(object? sender, EventArgs e)
	{
		UpdateColors();
		Invalidate();
	}

	protected virtual void OnDecoratedEnabledChanged(object? sender, EventArgs e)
	{
		Invalidate();
	}
}
