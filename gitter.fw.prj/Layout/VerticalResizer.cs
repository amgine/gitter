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

namespace gitter.Framework.Layout;

using System.Drawing;
using System.Windows.Forms;

public sealed class VerticalResizer
{
	private readonly Control _grip;
	private readonly GridRow _row;
	private readonly int _sign;
	private readonly IDpiBoundValue<int>? _minHeight;
	private readonly IDpiBoundValue<int>? _maxHeight;
	private bool _isMoving;
	private int _iH;
	private int _mY;

	sealed class SizeSpecImpl(int size, Dpi originalDpi) : ISizeSpec
	{
		public int Priority => 1;

		public int GetSize(int available, Dpi dpi)
			=> size * dpi.Y / originalDpi.Y;
	}

	public VerticalResizer(Control grip, GridRow row, int sign,
		IDpiBoundValue<int>? minHeight = default,
		IDpiBoundValue<int>? maxHeight = default)
	{
		_grip = grip;
		_row = row;
		_sign = sign;
		_minHeight = minHeight;
		_maxHeight = maxHeight;

		_grip.MouseDown += OnGripMouseDown;
		_grip.MouseMove += OnGripMouseMove;
		_grip.MouseUp   += OnGripMouseUp;
	}

	private void OnGripMouseDown(object? sender, MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Left && !_isMoving)
		{
			_iH = _row.Height;
			_mY = e.Y;
			_isMoving = true;
			_grip.BackColor = SystemColors.ControlDark;
		}
	}

	private void OnGripMouseMove(object? sender, MouseEventArgs e)
	{
		if(_isMoving && e.Button == MouseButtons.Left)
		{
			var dpi       = Dpi.FromControl(_grip);
			var height    = _iH + _sign * (e.Y - _mY);
			var minHeight = _minHeight?.GetValue(dpi) ?? 0;
			var maxHeight = _maxHeight?.GetValue(dpi) ?? int.MaxValue;
			if(height > maxHeight) height = maxHeight;
			if(height < minHeight) height = minHeight;
			if(_row.Height != height)
			{
				_row.SizeSpec = new SizeSpecImpl(height, dpi);
				_grip.TopLevelControl?.Update();
			}
			_iH = height;
		}
	}

	private void OnGripMouseUp(object? sender, MouseEventArgs e)
	{
		if(_isMoving)
		{
			_isMoving = false;
			_grip.BackColor = GitterApplication.Style.Colors.Window;
		}
	}
}
