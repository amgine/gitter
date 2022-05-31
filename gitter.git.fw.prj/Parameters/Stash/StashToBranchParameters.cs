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

/// <summary>Parameters for <see cref="IRepositoryAccessor.StashToBranch"/> operation.</summary>
public sealed class StashToBranchParameters
{
	/// <summary>Create <see cref="StashToBranchParameters"/>.</summary>
	public StashToBranchParameters()
	{
	}

	/// <summary>Create <see cref="StashToBranchParameters"/>.</summary>
	/// <param name="stashName">Stash to convert.</param>
	/// <param name="branchName">Branch name.</param>
	public StashToBranchParameters(string stashName, string branchName)
	{
		StashName = stashName;
		BranchName = branchName;
	}

	/// <summary>Stash to convert.</summary>
	public string StashName { get; set; }

	/// <summary>Branch name.</summary>
	public string BranchName { get; set; }
}
