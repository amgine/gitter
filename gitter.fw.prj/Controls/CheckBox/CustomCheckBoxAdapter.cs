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

public sealed class CustomCheckBoxAdapter : WidgetAdapter<CustomCheckBox>, ICheckBoxWidget
{
	public event EventHandler? IsCheckedChanged;

	public event EventHandler? CheckStateChanged;

	private void OnIsCheckedChanged(EventArgs e)
		=> IsCheckedChanged?.Invoke(this, e);

	private void OnCheckStateChanged(EventArgs e)
		=> CheckStateChanged?.Invoke(this, e);

	public CustomCheckBoxAdapter(CustomCheckBoxRenderer renderer)
		: base(new() { Renderer = renderer })
	{
		_control.IsCheckedChanged  += OnCheckBoxIsCheckedChanged;
		_control.CheckStateChanged += OnCheckBoxCheckStateChanged;
	}

	private void OnCheckBoxIsCheckedChanged(object? sender, EventArgs e)
		=> OnIsCheckedChanged(e);

	private void OnCheckBoxCheckStateChanged(object? sender, EventArgs e)
		=> OnCheckStateChanged(e);

	public Image? Image
	{
		get => _control.Image;
		set => _control.Image = value;
	}

	public bool IsChecked
	{
		get => _control.IsChecked;
		set => _control.IsChecked = value;
	}

	public CheckState CheckState
	{
		get => _control.CheckState;
		set => _control.CheckState = value;
	}

	public bool ThreeState
	{
		get => _control.ThreeState;
		set => _control.ThreeState = value;
	}
}
