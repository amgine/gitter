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

namespace gitter.Git.AccessLayer;

using System;

/// <summary>Parameters for <see cref="IConfigAccessor.SetConfigValue"/> operation.</summary>
public sealed class SetConfigValueParameters : BaseConfigParameters
{
	/// <summary>Create <see cref="SetConfigValueParameters"/>.</summary>
	public SetConfigValueParameters()
	{
	}

	/// <summary>Create <see cref="SetConfigValueParameters"/>.</summary>
	/// <param name="parameterName">Parameter name.</param>
	/// <param name="parameterValue">Parameter value.</param>
	public SetConfigValueParameters(string parameterName, string parameterValue)
	{
		ParameterName  = parameterName;
		ParameterValue = parameterValue;
	}

	/// <summary>Parameter name.</summary>
	public string ParameterName { get; set; }

	/// <summary>Parameter value.</summary>
	public string ParameterValue { get; set; }
}
