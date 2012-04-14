namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Defines repository-independent git operations.</summary>
	public interface IGitAccessor : IConfigAccessor
	{
		/// <summary>Create <see cref="IRepositoryAccessor"/> for specified <paramref name="repository"/>.</summary>
		/// <param name="repository">git repository to get accessor for.</param>
		/// <returns>git repository accessor.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		IRepositoryAccessor CreateRepositoryAccessor(Repository repository);

		/// <summary>Checks if specified path is a valid repository.</summary>
		/// <param name="path">Path to check.</param>
		/// <returns><c>true</c> if specified path is a valid repository, <c>false</c> otherwise.</returns>
		bool IsValidRepository(string path);

		/// <summary>Returns git version.</summary>
		/// <returns>git version.</returns>
		Version QueryVersion();

		/// <summary>Create an empty git repository or reinitialize an existing one.</summary>
		/// <param name="parameters"><see cref="InitRepositoryParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void InitRepository(InitRepositoryParameters parameters);

		/// <summary>Clone existing repository. </summary>
		/// <param name="parameters"><see cref="CloneRepositoryParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void CloneRepository(CloneRepositoryParameters parameters);
	}
}
