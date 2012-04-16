namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Object which can perform various operations on git index.</summary>
	public interface IIndexAccessor
	{
		/// <summary>Get working directory status information.</summary>
		/// <param name="parameters"><see cref="QueryStatusParameters"/>.</param>
		/// <returns>Working directory status information.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		StatusData QueryStatus(QueryStatusParameters parameters);

		/// <summary>Apply patches to working directory and/or index.</summary>
		/// <param name="parameters"><see cref="ApplyPatchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void ApplyPatch(ApplyPatchParameters parameters);

		/// <summary>Commit changes.</summary>
		/// <param name="parameters"><see cref="CommitParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void Commit(CommitParameters parameters);

		/// <summary>Get the list of files that can be added.</summary>
		/// <param name="parameters"><see cref="AddFilesParameters"/>.</param>
		/// <returns>List of files which will be added by call to <see cref="AddFiles"/>(<paramref name="parameters"/>).</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<TreeFileData> QueryFilesToAdd(AddFilesParameters parameters);

		/// <summary>Add files to index.</summary>
		/// <param name="parameters"><see cref="AddFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void AddFiles(AddFilesParameters parameters);

		/// <summary>
		/// Get list of files which will be removed by a <see cref="IIndexAccessor.RemoveFiles"/>(<paramref name="parameters"/>) call.
		/// </summary>
		/// <param name="parameters"><see cref="RemoveFilesParameters"/>.</param>
		/// <returns>List of files which will be removed by a <see cref="IIndexAccessor.RemoveFiles"/>(<paramref name="parameters"/>) call.</returns>
		IList<string> QueryFilesToRemove(RemoveFilesParameters parameters);

		/// <summary>Remove files from index or from index and working directory.</summary>
		/// <param name="parameters"><see cref="RemoveFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void RemoveFiles(RemoveFilesParameters parameters);

		/// <summary>
		/// Get list of files and directories which will be removed
		/// by <see cref="IIndexAccessor.CleanFiles"/>(<paramref name="parameters"/>) call.
		/// </summary>
		/// <param name="parameters"><see cref="CleanFilesParameters"/>.</param>
		/// <returns>List of files and directories which will be removed by <see cref="IIndexAccessor.CleanFiles"/>(<paramref name="parameters"/>) call.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<string> QueryFilesToClean(CleanFilesParameters parameters);

		/// <summary>Remove untracked files from the working tree.</summary>
		/// <param name="parameters"><see cref="CleanFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void CleanFiles(CleanFilesParameters parameters);

		/// <summary>Resets files.</summary>
		/// <param name="parameters"><see cref="ResetFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void ResetFiles(ResetFilesParameters parameters);

		/// <summary>Run merge tool to resolve conflicts.</summary>
		/// <param name="parameters"><see cref="RunMergeToolParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void RunMergeTool(RunMergeToolParameters parameters);
	}
}
