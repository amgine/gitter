#region Copyright Notice
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

namespace gitter.Framework.Controls;

using System.Collections.Generic;
using System.Windows.Forms;

/// <summary>Dock system elements.</summary>
/// <typeparam name="T">Element type.</typeparam>
public sealed class DockElements<T> where T : ContainerControl
{
	private static readonly List<T> _instances = new();

	/// <summary>Returns the list of all active instances of type <typeparamref name="T"/> elements.</summary>
	/// <value>List of all active instances of type <typeparamref name="T"/> elements.</value>
	public static IReadOnlyList<T> Instances => _instances;

	private DockElements() { }

	internal static void Add(T instance)
	{
		_instances.Add(instance);
	}

	internal static void Remove(T instance)
	{
		_instances.Remove(instance);
	}
}
