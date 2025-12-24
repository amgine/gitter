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

using gitter.Framework;

/// <summary>Parameters for <see cref="IRepositoryAccessor.ResetFiles"/> operation.</summary>
public sealed class ResetFilesRequest
{
	/// <summary>Create <see cref="ResetFilesRequest"/>.</summary>
	public ResetFilesRequest()
	{
	}

	/// <summary>Create <see cref="ResetFilesRequest"/>.</summary>
	/// <param name="revision">Commit containing desired file states.</param>
	/// <param name="path">Path to reset.</param>
	public ResetFilesRequest(string revision, string path)
	{
		Revision = revision;
		Paths = path;
	}

	/// <summary>Create <see cref="ResetFilesRequest"/>.</summary>
	/// <param name="path">Path to reset.</param>
	public ResetFilesRequest(string path)
	{
		Paths = path;
	}

	/// <summary>Create <see cref="ResetFilesRequest"/>.</summary>
	/// <param name="revision">Commit containing desired file states.</param>
	/// <param name="paths">Paths to reset.</param>
	public ResetFilesRequest(string revision, Many<string> paths)
	{
		Revision = revision;
		Paths    = paths;
	}

	/// <summary>Create <see cref="ResetFilesRequest"/>.</summary>
	/// <param name="paths">Paths to reset.</param>
	public ResetFilesRequest(Many<string> paths)
	{
		Paths = paths;
	}

	/// <summary>Commit containing desired file states.</summary>
	public string? Revision { get; set; }

	/// <summary>Paths to reset.</summary>
	public Many<string> Paths { get; set; }
}
