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

namespace gitter.Git;

public sealed class DiffOptions
{
	public static readonly DiffOptions Default = CreateDefault();

	private static DiffOptions CreateDefault()
	{
		var options = new DiffOptions();
		options.Freeze();
		return options;
	}

	public DiffOptions()
	{
		Context = 3;
	}

	private void Set<T>(ref T field, T value)
	{
		Verify.State.IsFalse(IsFrozen, $"This {nameof(DiffOptions)} instance is frozen.");

		field = value;
	}

	public int Context
	{
		get; set => Set(ref field, value);
	}

	public bool UsePatienceAlgorithm
	{
		get; set => Set(ref field, value);
	}

	public bool IgnoreWhitespace
	{
		get; set => Set(ref field, value);
	}

	public bool Binary
	{
		get; set => Set(ref field, value);
	}

	public bool IsFrozen { get; private set; }

	public void Freeze() => IsFrozen = true;
}
