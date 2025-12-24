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

/// <summary>Working tree status information.</summary>
public sealed class StatusData(
	Dictionary<string, TreeFileData> stagedFiles,
	Dictionary<string, TreeFileData> unstagedFiles,
	int unstagedUntrackedCount,
	int unstagedRemovedCount,
	int unstagedModifiedCount,
	int unmergedCount,
	int stagedAddedCount,
	int stagedModifiedCount,
	int stagedRemovedCount)
{
	public Dictionary<string, TreeFileData> StagedFiles { get; } = stagedFiles;

	public Dictionary<string, TreeFileData> UnstagedFiles { get; } = unstagedFiles;

	public int UnstagedUntrackedCount { get; } = unstagedUntrackedCount;

	public int UnstagedModifiedCount { get; } = unstagedModifiedCount;

	public int UnstagedRemovedCount { get; } = unstagedRemovedCount;

	public int UnmergedCount { get; } = unmergedCount;

	public int StagedAddedCount { get; } = stagedAddedCount;

	public int StagedModifiedCount { get; } = stagedModifiedCount;

	public int StagedRemovedCount { get; } = stagedRemovedCount;
}
