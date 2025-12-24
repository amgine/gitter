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

public sealed class SystemCheckBoxAdapter : WidgetAdapter<CheckBox>, ICheckBoxWidget
{
	public event EventHandler? IsCheckedChanged;

	public event EventHandler? CheckStateChanged;

	private void OnIsCheckedChanged(EventArgs e)
		=> IsCheckedChanged?.Invoke(this, e);

	private void OnCheckStateChanged(EventArgs e)
		=> CheckStateChanged?.Invoke(this, e);

	public SystemCheckBoxAdapter() : base(new())
	{
		_control.FlatStyle = FlatStyle.System;
		_control.CheckedChanged    += OnCheckBoxCheckedChanged;
		_control.CheckStateChanged += OnCheckBoxCheckStateChanged;
	}

	private void OnCheckBoxCheckedChanged(object? sender, EventArgs e)
		=> OnIsCheckedChanged(e);

	private void OnCheckBoxCheckStateChanged(object? sender, EventArgs e)
		=> OnCheckStateChanged(e);

	public Image? Image
	{
		get => _control.Image;
		set
		{
			if(value != _control.Image)
			{
				_control.Image = value;
				if(value is null)
				{
					_control.FlatStyle = FlatStyle.System;
				}
				else
				{
					_control.FlatStyle = FlatStyle.Standard;
					_control.TextImageRelation = TextImageRelation.ImageBeforeText;
					_control.ImageAlign = ContentAlignment.MiddleLeft;
				}
			}
		}
	}

	public bool IsChecked
	{
		get => _control.Checked;
		set => _control.Checked = value;
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
