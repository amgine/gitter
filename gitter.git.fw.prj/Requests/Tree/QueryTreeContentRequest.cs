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

/// <summary>Parameters for <see cref="IRepositoryAccessor.QueryTreeContent"/> operation.</summary>
public sealed class QueryTreeContentRequest
{
	/// <summary>Create <see cref="QueryTreeContentRequest"/>.</summary>
	public QueryTreeContentRequest()
	{
	}

	/// <summary>Create <see cref="QueryTreeContentRequest"/>.</summary>
	/// <param name="treeId">>Tree-ish.</param>
	public QueryTreeContentRequest(string treeId)
	{
		TreeId = treeId;
	}

	/// <summary>Create <see cref="QueryTreeContentRequest"/>.</summary>
	/// <param name="treeId">>Tree-ish.</param>
	/// <param name="recurse">Recurse into sub-trees.</param>
	public QueryTreeContentRequest(string treeId, bool recurse)
	{
		TreeId = treeId;
		Recurse = recurse;
	}

	/// <summary>Create <see cref="QueryTreeContentRequest"/>.</summary>
	/// <param name="treeId">>Tree-ish.</param>
	/// <param name="recurse">Recurse into sub-trees.</param>
	/// <param name="onlyTrees">Exclude files (blobs) from output.</param>
	public QueryTreeContentRequest(string treeId, bool recurse, bool onlyTrees)
	{
		TreeId = treeId;
		Recurse = recurse;
		OnlyTrees = onlyTrees;
	}

	/// <summary>Exclude files (blobs) from output.</summary>
	public bool OnlyTrees { get; set; }

	/// <summary>Recurse into sub-trees.</summary>
	public bool Recurse { get; set; }

	/// <summary>Tree-ish.</summary>
	public string TreeId { get; set; } = default!;

	/// <summary>Limiting paths.</summary>
	public Many<string> Paths { get; set; }
}
