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

/// <summary>Parameters for <see cref="IRepositoryAccessor.RemoveRemoteReferences"/> operation.</summary>
public sealed class RemoveRemoteReferencesRequest
{
	/// <summary>Create <see cref="RemoveRemoteReferencesRequest"/>.</summary>
	public RemoveRemoteReferencesRequest()
	{
	}

	/// <summary>Create <see cref="RemoveRemoteReferencesRequest"/>.</summary>
	/// <param name="remoteName">Affected remote.</param>
	/// <param name="reference">Reference to remove.</param>
	public RemoveRemoteReferencesRequest(string remoteName, string reference)
	{
		RemoteName = remoteName;
		References = reference;
	}

	/// <summary>Create <see cref="RemoveRemoteReferencesRequest"/>.</summary>
	/// <param name="remoteName">Affected remote.</param>
	/// <param name="references">References to remove.</param>
	public RemoveRemoteReferencesRequest(string remoteName, Many<string> references)
	{
		RemoteName = remoteName;
		References = references;
	}

	/// <summary>Affected remote.</summary>
	public string RemoteName { get; set; } = default!;

	/// <summary>References to remove.</summary>
	public Many<string> References { get; set; }
}
