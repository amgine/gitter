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

public sealed class HorizontalResizer
{
	private readonly Control _grip;
	private readonly GridColumn _column;
	private readonly int _sign;
	private readonly IDpiBoundValue<int>? _minWidth;
	private readonly IDpiBoundValue<int>? _maxWidth;
	private bool _isMoving;
	private int _iW;
	private int _mX;

	sealed class SizeSpecImpl(int size, Dpi originalDpi) : ISizeSpec
	{
		public int Priority => 0;

		public int GetSize(int available, Dpi dpi)
			=> size * dpi.X / originalDpi.X;
	}

	public HorizontalResizer(Control grip, GridColumn column, int sign,
		IDpiBoundValue<int>? minWidth = default,
		IDpiBoundValue<int>? maxWidth = default)
	{
		_grip     = grip;
		_column   = column;
		_sign     = sign;
		_minWidth = minWidth;
		_maxWidth = maxWidth;

		_grip.MouseDown += OnGripMouseDown;
		_grip.MouseMove += OnGripMouseMove;
		_grip.MouseUp   += OnGripMouseUp;
	}

	private void OnGripMouseDown(object? sender, MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Left && !_isMoving)
		{
			_iW = _column.Width;
			_mX = e.X;
			_isMoving = true;
			_grip.BackColor = SystemColors.ControlDark;
		}
	}

	private void OnGripMouseMove(object? sender, MouseEventArgs e)
	{
		if(_isMoving && e.Button == MouseButtons.Left)
		{
			var dpi      = Dpi.FromControl(_grip);
			var width    = _iW + _sign * (e.X - _mX);
			var minWidth = _minWidth?.GetValue(dpi) ?? 0;
			var maxWidth = _maxWidth?.GetValue(dpi) ?? int.MaxValue;
			if(width > maxWidth) width = maxWidth;
			if(width < minWidth) width = minWidth;
			if(_column.Width != width)
			{
				_column.SizeSpec = new SizeSpecImpl(width, dpi);
				_grip.TopLevelControl?.Update();
			}
			_iW = width;
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
