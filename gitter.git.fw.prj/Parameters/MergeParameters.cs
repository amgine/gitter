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

/// <summary>Parameters for <see cref="IRepositoryAccessor.Merge"/> operation.</summary>
public sealed class MergeParameters
{
	/// <summary>Create <see cref="MergeParameters"/>.</summary>
	public MergeParameters()
	{
	}

	/// <summary>Create <see cref="MergeParameters"/>.</summary>
	/// <param name="revisions">Branch to merge with.</param>
	public MergeParameters(string revisions)
	{
		Revisions = new[] { revisions };
	}

	/// <summary>Create <see cref="MergeParameters"/>.</summary>
	/// <param name="revisions">Branches to merge with.</param>
	public MergeParameters(IReadOnlyList<string> revisions)
	{
		Revisions = revisions;
	}

	/// <summary>Branches to merge with.</summary>
	public IReadOnlyList<string> Revisions { get; set; }

	/// <summary>Commit message.</summary>
	public string Message { get; set; }

	/// <summary>Create merge commit even if merge can be performed by fast-forwarding current branch.</summary>
	public bool NoFastForward { get; set; }

	/// <summary>Prepare index but do not commit.</summary>
	public bool NoCommit { get; set; }

	/// <summary>Merge strategy.</summary>
	public MergeStrategy Strategy { get; set; }

	/// <summary>Strategy option.</summary>
	public string StrategyOption { get; set; }

	/// <summary>Perform merge but do not consider this a merge.</summary>
	public bool Squash { get; set; }
}
