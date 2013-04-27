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

namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Object which can perform various operations on git stash.</summary>
	public interface IStashAccessor
	{
		/// <summary>Query most recent stashed state.</summary>
		/// <param name="parameters"><see cref="QueryStashTopParameters"/>.</param>
		/// <returns>Most recent stashed state or null if stash is empty.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		RevisionData QueryStashTop(QueryStashTopParameters parameters);

		/// <summary>Query all stashed states.</summary>
		/// <param name="parameters"><see cref="QueryStashParameters"/>.</param>
		/// <returns>List of all stashed states.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<StashedStateData> QueryStash(QueryStashParameters parameters);

		/// <summary>Get patch representing stashed changes.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns>Patch, representing specified stashed changes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		string QueryStashPatch(QueryRevisionDiffParameters parameters);

		/// <summary>Get patch representing stashed changes.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns><see cref="Diff"/>, representing specified stashed changes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Diff QueryStashDiff(QueryRevisionDiffParameters parameters);

		/// <summary>Stash changes in working directory.</summary>
		/// <param name="parameters"><see cref="StashSaveParameters"/>.</param>
		/// <returns>true if something was stashed, false otherwise.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		bool StashSave(StashSaveParameters parameters);

		/// <summary>Apply stashed changes and remove stashed state.</summary>
		/// <param name="parameters"><see cref="StashPopParameters"/></param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void StashPop(StashPopParameters parameters);

		/// <summary>Apply stashed changes and do not remove stashed state.</summary>
		/// <param name="parameters"><see cref="StashApplyParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void StashApply(StashApplyParameters parameters);

		/// <summary>Create new branch, checkout that branch and pop stashed state.</summary>
		/// <param name="parameters"><see cref="StashToBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void StashToBranch(StashToBranchParameters parameters);

		/// <summary>Remove stashed state.</summary>
		/// <param name="parameters"><see cref="StashDropParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void StashDrop(StashDropParameters parameters);

		/// <summary>Clear stash.</summary>
		/// <param name="parameters"><see cref="StashClearParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void StashClear(StashClearParameters parameters);
	}
}
