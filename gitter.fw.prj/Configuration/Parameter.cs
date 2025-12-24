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

namespace gitter.Framework.Configuration;

using System;

/// <summary>Configuration parameter.</summary>
/// <param name="name">Parameter name.</param>
/// <param name="type">Parameter type.</param>
/// <param name="value">Parameter value.</param>
public sealed class Parameter(string name, Type type, object? value) : INamedObject
{
	/// <summary>Parameter value.</summary>
	private object? _value = value;

	/// <summary>Gets parameter name.</summary>
	/// <value>Parameter name.</value>
	public string Name { get; } = name;

	/// <summary>Gets parameter type.</summary>
	/// <value>Parameter type.</value>
	public Type Type { get; } = type;

	/// <summary>Gets or sets parameter value.</summary>
	/// <value>Parameter value.</value>
	public object? Value
	{
		get => _value;
		set
		{
			_value = value;
			IsModified = true;
		}
	}

	/// <summary>Gets a value indicating whether this parameter is modified.</summary>
	/// <value><c>true</c> if this parameter is modified; otherwise, <c>false</c>.</value>
	public bool IsModified { get; private set; }

	/// <inheritdoc/>
	public override string ToString() => $"{Name} = {Value}";
}
