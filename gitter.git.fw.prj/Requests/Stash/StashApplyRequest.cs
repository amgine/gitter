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

/// <summary>Parameters for <see cref="IRepositoryAccessor.StashApply"/> operation.</summary>
public sealed class StashApplyRequest
{
	/// <summary>Create <see cref="StashPopRequest"/>.</summary>
	public StashApplyRequest()
	{
	}

	/// <summary>Create <see cref="StashPopRequest"/>.</summary>
	/// <param name="stashName">Stash to apply.</param>
	/// <param name="restoreIndex">Restore index state.</param>
	public StashApplyRequest(string stashName, bool restoreIndex)
	{
		StashName = stashName;
		RestoreIndex = restoreIndex;
	}

	/// <summary>Create <see cref="StashPopRequest"/>.</summary>
	/// <param name="stashName">Stash to apply.</param>
	public StashApplyRequest(string stashName)
	{
		StashName = stashName;
	}

	/// <summary>Create <see cref="StashPopRequest"/>.</summary>
	/// <param name="restoreIndex">Restore index state.</param>
	public StashApplyRequest(bool restoreIndex)
	{
		RestoreIndex = restoreIndex;
	}

	/// <summary>Stash to apply.</summary>
	public string StashName { get; set; } = default!;

	/// <summary>Restore index state.</summary>
	public bool RestoreIndex { get; set; }
}
