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

/// <summary>Parameters for <see cref="IRepositoryAccessor.QueryRemoteReferences"/> operation.</summary>
public sealed class QueryRemoteReferencesRequest
{
	/// <summary>Create <see cref="QueryRemoteReferencesRequest"/>.</summary>
	public QueryRemoteReferencesRequest()
	{
	}

	/// <summary>Create <see cref="QueryRemoteReferencesRequest"/>.</summary>
	/// <param name="remoteName">Remote to query.</param>
	public QueryRemoteReferencesRequest(string remoteName)
	{
		RemoteName = remoteName;
	}

	/// <summary>Create <see cref="QueryRemoteReferencesRequest"/>.</summary>
	/// <param name="remoteName">Remote to query.</param>
	///	<param name="heads">Query /refs/heads.</param>
	///	<param name="tags">Query /refs/tags.</param>
	public QueryRemoteReferencesRequest(string remoteName, bool heads, bool tags)
	{
		RemoteName = remoteName;
		Heads = heads;
		Tags = tags;
	}

	/// <summary>Remote to query.</summary>
	public string RemoteName { get; set; } = default!;

	/// <summary>Query /refs/heads.</summary>
	public bool Heads { get; set; }

	/// <summary>Query /refs/tags.</summary>
	public bool Tags { get; set; }

	/// <summary>Ref name pattern.</summary>
	public string? Pattern { get; set; }
}
