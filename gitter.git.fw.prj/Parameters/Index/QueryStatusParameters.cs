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

/// <summary>Parameters for <see cref="IRepositoryAccessor.QueryStatus"/> operation.</summary>
public sealed class QueryStatusParameters
{
	/// <summary>Create <see cref="QueryStatusParameters"/>.</summary>
	public QueryStatusParameters()
	{
		UntrackedFilesMode = StatusUntrackedFilesMode.All;
	}

	/// <summary>Create <see cref="QueryStatusParameters"/>.</summary>
	/// <param name="path">Path to check for changes.</param>
	public QueryStatusParameters(string path)
	{
		Path = path;
		UntrackedFilesMode = StatusUntrackedFilesMode.All;
	}

	/// <summary>Path to check for changes.</summary>
	public string Path { get; set; }

	/// <summary>Determines how untracked files are represented in status.</summary>
	public StatusUntrackedFilesMode UntrackedFilesMode { get; set; }

	/// <summary>Method of ignoring submodule changes.</summary>
	public StatusIgnoreSubmodulesMode IgnoreSubmodulesMode { get; set; }
}
