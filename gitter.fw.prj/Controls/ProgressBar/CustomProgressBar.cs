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
using System.Drawing;
using System.Windows.Forms;

[System.ComponentModel.DesignerCategory("")]
public sealed class CustomProgressBar : Control
{
	private CustomProgressBarRenderer? _renderer;
	private int _minimum;
	private int _maximum;
	private int _value;
	private bool _isIndeterminate;
	private int _animationTimestamp;
	private Timer? _animationTimer;

	public CustomProgressBar()
	{
		SetStyle(
			ControlStyles.AllPaintingInWmPaint |
			ControlStyles.OptimizedDoubleBuffer |
			ControlStyles.Opaque |
			ControlStyles.ResizeRedraw |
			ControlStyles.UserPaint, true);
		SetStyle(
			ControlStyles.ContainerControl |
			ControlStyles.Selectable |
			ControlStyles.SupportsTransparentBackColor, false);
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_animationTimer is not null)
			{
				_animationTimer.Dispose();
				_animationTimer = default;
			}
		}
		base.Dispose(disposing);
	}

	private void EnableAnimationTimer()
	{
		if(_animationTimer is null)
		{
			_animationTimer = new() { Interval = 1000 / 100 };
			_animationTimer.Tick += OnAnimationTimerTick;
		}
		_animationTimer.Enabled = true;
	}

	private void OnAnimationTimerTick(object? sender, EventArgs e)
	{
		if(IsDisposed) return;
		Invalidate();
	}

	private void DisableAnimationTimer()
	{
		if(_animationTimer is null) return;
		_animationTimer.Enabled = false;
	}

	private void UpdateAnimationTimer()
	{
		if(IsIndeterminate)
		{
			EnableAnimationTimer();
		}
		else
		{
			DisableAnimationTimer();
		}
	}

	public CustomProgressBarRenderer Renderer
	{
		get => _renderer ?? CustomProgressBarRenderer.Default;
		set
		{
			if(_renderer == value) return;

			bool needsInvalidate =
				!(_renderer is null && value == CustomProgressBarRenderer.Default) &&
				!(_renderer == CustomProgressBarRenderer.Default && value is null);
			_renderer = value;
			if(needsInvalidate)
			{
				Invalidate();
			}
		}
	}

	public int AnimationTimestamp => _animationTimestamp;

	public bool IsIndeterminate
	{
		get => _isIndeterminate;
		set
		{
			if(_isIndeterminate == value) return;
			if(value)
			{
				_animationTimestamp = Environment.TickCount & int.MaxValue;
			}
			_isIndeterminate = value;
			UpdateAnimationTimer();
			Invalidate();
		}
	}

	public int Minimum
	{
		get => _minimum;
		set
		{
			if(_minimum == value) return;
			_minimum = value;
			Invalidate();
		}
	}

	public int Maximum
	{
		get => _maximum;
		set
		{
			if(_maximum == value) return;
			_maximum = value;
			Invalidate();
		}
	}

	public int Value
	{
		get => _value;
		set
		{
			if(_value == value) return;
			_value = value;
			Invalidate();
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintBackground(PaintEventArgs pevent)
	{
	}

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
		=> Renderer.Render(e.Graphics, e.ClipRectangle, this);
}
