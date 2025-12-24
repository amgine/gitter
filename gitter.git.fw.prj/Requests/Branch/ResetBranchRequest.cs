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

/// <summary>Parameters for <see cref="IRepositoryAccessor.ResetBranch"/> operation.</summary>
public sealed class ResetBranchRequest
{
	/// <summary>Create <see cref="ResetBranchRequest"/>.</summary>
	public ResetBranchRequest()
	{
	}

	/// <summary>Create <see cref="ResetBranchRequest"/>.</summary>
	/// <param name="branchName">Branch to reset.</param>
	/// <param name="revision">New branch position.</param>
	public ResetBranchRequest(string branchName, string revision)
	{
		BranchName = branchName;
		Revision = revision;
	}

	/// <summary>Branch to reset.</summary>
	public string BranchName { get; set; } = default!;

	/// <summary>New branch position.</summary>
	public string Revision { get; set; } = default!;
}
