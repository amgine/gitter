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

namespace gitter.Git;

using System;

/// <summary>Reset modes.</summary>
[Flags]
public enum ResetMode
{
	/// <summary>
	/// Resets the index but not the working tree (i.e., the changed files are preserved but not marked for commit)
	/// and reports what has not been updated. This is the default action.
	/// </summary>
	Mixed = (1 << 1),
	/// <summary>
	/// Does not touch the index file nor the working tree at all, but requires them to be in a good order.
	/// This leaves all your changed files "Changes to be committed", as git-status would put it
	/// </summary>
	Soft = (1 << 2),
	/// <summary>
	/// Matches the working tree and index to that of the tree being switched to. Any changes to
	/// tracked files in the working tree since "commit" are lost.
	/// </summary>
	Hard = (1 << 3),
	/// <summary>
	/// Resets the index to match the tree recorded by the named commit, and updates the
	/// files that are different between the named commit and the current commit in the working tree.
	/// </summary>
	Merge = (1 << 4),
	/// <summary>
	/// Resets index entries and updates files in the working tree that are different between &lt;commit&gt; and <c>HEAD</c>.<br/>
	/// If a file that is different between &lt;commit&gt; and <c>HEAD</c> has local changes, reset is aborted.
	/// </summary>
	Keep = (1 << 5),
}
