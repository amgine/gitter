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

namespace gitter.Git.AccessLayer;

using System;
using System.Collections.Generic;

/// <summary>Parameters for <see cref="IRepositoryAccessor.QueryObjects"/> operation.</summary>
public sealed class QueryObjectsParameters
{
	/// <summary>Create <see cref="QueryObjectsParameters"/>.</summary>
	public QueryObjectsParameters()
	{
	}

	/// <summary>Create <see cref="QueryObjectsParameters"/>.</summary>
	/// <param name="object">Object to query.</param>
	public QueryObjectsParameters(string @object)
	{
		Objects = new[] { @object };
	}

	/// <summary>Create <see cref="QueryObjectsParameters"/>.</summary>
	/// <param name="objects">Objects to query.</param>
	public QueryObjectsParameters(IReadOnlyList<string> objects)
	{
		Objects = objects;
	}

	/// <summary>List of requested objects.</summary>
	public IReadOnlyList<string> Objects { get; set; }
}
