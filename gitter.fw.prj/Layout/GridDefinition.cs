﻿#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using System;

#nullable enable

public abstract class GridDefinition
{
	private ISizeSpec _sizeSpec;

	public event EventHandler? SizeSpecChanged;

	protected virtual void OnSizeSpecChanged(EventArgs e)
		=> SizeSpecChanged?.Invoke(this, e);

	private protected GridDefinition(Grid grid, ISizeSpec sizeSpec)
	{
		Verify.Argument.IsNotNull(sizeSpec);

		Grid      = grid;
		_sizeSpec = sizeSpec;
	}

	public Grid Grid { get; }

	public ISizeSpec SizeSpec
	{
		get => _sizeSpec;
		set
		{
			Verify.Argument.IsNotNull(value);

			if(_sizeSpec != value)
			{
				_sizeSpec = value;
				OnSizeSpecChanged(EventArgs.Empty);
			}
		}
	}
}
