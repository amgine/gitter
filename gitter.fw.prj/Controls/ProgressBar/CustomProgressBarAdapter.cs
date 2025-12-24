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

public sealed class CustomProgressBarAdapter
	: WidgetAdapter<CustomProgressBar>
	, IProgressBarWidget
{
	public CustomProgressBarAdapter(CustomProgressBarRenderer renderer)
		: base(new())
	{
		_control.Renderer = renderer;
	}

	public bool IsIndeterminate
	{
		get => _control.IsIndeterminate;
		set => _control.IsIndeterminate = value;
	}

	public int Minimum
	{
		get => _control.Minimum;
		set => _control.Minimum = value;
	}

	public int Maximum
	{
		get => _control.Maximum;
		set => _control.Maximum = value;
	}

	public int Value
	{
		get => _control.Value;
		set => _control.Value = value;
	}
}
