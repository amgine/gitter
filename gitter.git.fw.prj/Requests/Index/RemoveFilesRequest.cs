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

using gitter.Framework;

/// <summary>Parameters for RemoveFiles operation.</summary>
public sealed class RemoveFilesRequest
{
	/// <summary>Create <see cref="RemoveFilesRequest"/>.</summary>
	public RemoveFilesRequest()
	{
	}

	/// <summary>Create <see cref="RemoveFilesRequest"/>.</summary>
	public RemoveFilesRequest(string path)
	{
		Paths = path;
	}

	/// <summary>Create <see cref="RemoveFilesRequest"/>.</summary>
	public RemoveFilesRequest(Many<string> paths)
	{
		Paths = paths;
	}

	/// <summary>Paths to remove.</summary>
	public Many<string> Paths { get; set; }

	/// <summary>Remove file irrespective its modified status.</summary>
	public bool Force { get; set; }

	/// <summary>Recursively remove files in subdirectories.</summary>
	public bool Recursive { get; set; }

	/// <summary>Remove file from index only.</summary>
	public bool Cached { get; set; }

	/// <summary>Don't fail if no files match paths.</summary>
	public bool IgnoreUnmatch { get; set; }
}
