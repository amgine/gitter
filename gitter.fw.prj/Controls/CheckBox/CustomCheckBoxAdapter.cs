﻿#region Copyright Notice
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

public sealed class CustomCheckBoxAdapter : ICheckBoxWidget
{
	#region Data

	private readonly CustomCheckBox _checkBox;

	#endregion

	#region Events

	public event EventHandler IsCheckedChanged;

	public event EventHandler CheckStateChanged;

	private void OnIsCheckedChanged(EventArgs e)
		=> IsCheckedChanged?.Invoke(this, e);

	private void OnCheckStateChanged(EventArgs e)
		=> CheckStateChanged?.Invoke(this, e);

	#endregion

	#region .ctor

	public CustomCheckBoxAdapter(CustomCheckBoxRenderer renderer)
	{
		Verify.Argument.IsNotNull(renderer);

		_checkBox = new CustomCheckBox()
		{
			Renderer = renderer,
		};
		_checkBox.IsCheckedChanged += OnCheckBoxIsCheckedChanged;
		_checkBox.CheckStateChanged += OnCheckBoxCheckStateChanged;
	}

	#endregion

	#region Event Handlers

	private void OnCheckBoxIsCheckedChanged(object sender, EventArgs e)
		=> OnIsCheckedChanged(e);

	private void OnCheckBoxCheckStateChanged(object sender, EventArgs e)
		=> OnCheckStateChanged(e);

	#endregion

	#region Properties

	public Control Control => _checkBox;

	public string Text
	{
		get => _checkBox.Text;
		set => _checkBox.Text = value;
	}

	public Image Image
	{
		get => _checkBox.Image;
		set => _checkBox.Image = value;
	}

	public bool IsChecked
	{
		get => _checkBox.IsChecked;
		set => _checkBox.IsChecked = value;
	}

	public CheckState CheckState
	{
		get => _checkBox.CheckState;
		set => _checkBox.CheckState = value;
	}

	public bool ThreeState
	{
		get => _checkBox.ThreeState;
		set => _checkBox.ThreeState = value;
	}

	#endregion

	#region IDisposable

	public void Dispose()
	{
		_checkBox.IsCheckedChanged  -= OnCheckBoxIsCheckedChanged;
		_checkBox.CheckStateChanged -= OnCheckBoxCheckStateChanged;
		_checkBox.Dispose();
	}

	#endregion
}
