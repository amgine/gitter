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
