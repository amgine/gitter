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

namespace gitter.Git
{
	/// <summary>Merge strategy.</summary>
	public enum MergeStrategy
	{
		/// <summary>Default strategy.</summary>
		/// <remarks>
		/// <see cref="MergeStrategy.Recursive"/> if merging single branch,
		/// <see cref="MergeStrategy.Octopus"/> if merging multiple branches.
		/// </remarks>
		Default,

		/// <summary>
		/// This can only resolve two heads (i.e. the current branch and another branch you pulled from) using a 3-way merge algorithm.
		/// It tries to carefully detect criss-cross merge ambiguities and is considered generally safe and fast. 
		/// </summary>
		Resolve,
		/// <summary>
		/// This can only resolve two heads using a 3-way merge algorithm. When there is more than one common ancestor that can be
		/// used for 3-way merge, it creates a merged tree of the common ancestors and uses that as the reference tree for the
		/// 3-way merge. This has been reported to result in fewer merge conflicts without causing mis-merges by tests done on
		/// actual merge commits taken from Linux 2.6 kernel development history. Additionally this can detect and handle merges
		/// involving renames. This is the default merge strategy when pulling or merging one branch. 
		/// </summary>
		Recursive,
		/// <summary>
		/// This resolves cases with more than two heads, but refuses to do a complex merge that needs manual resolution.
		/// It is primarily meant to be used for bundling topic branch heads together. This is the default merge strategy
		/// when pulling or merging more than one branch.
		/// </summary>
		Octopus,
		/// <summary>
		/// This resolves any number of heads, but the resulting tree of the merge is always that of the current branch head,
		/// effectively ignoring all changes from all other branches. It is meant to be used to supersede old development history
		/// of side branches. Note that this is different from the -Xours option to the recursive merge strategy.
		/// </summary>
		Ours,
		/// <summary>
		/// This is a modified recursive strategy. When merging trees A and B, if B corresponds to a subtree of A, B is first
		/// adjusted to match the tree structure of A, instead of reading the trees at the same level. This adjustment is also done
		/// to the common ancestor tree.
		/// </summary>
		Subtree,
	}
}
