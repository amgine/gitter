namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework.Configuration;

	/// <summary>Defines repository-independent git operations.</summary>
	public interface IGitAccessor : IConfigAccessor
	{
		/// <summary>Returns provider of this accessor.</summary>
		/// <value>Provider of this accessor</value>
		IGitAccessorProvider Provider { get; }

		/// <summary>Returns git version.</summary>
		/// <value>git version.</value>
		Version GitVersion { get; }

		/// <summary>Forces re-check of git version.</summary>
		void RefreshGitVersion();

		/// <summary>Save parameters to the specified <paramref name="section"/>.</summary>
		/// <param name="section">Section to store parameters.</param>
		void SaveTo(Section section);

		/// <summary>Load parameters from the specified <paramref name="section"/>.</summary>
		/// <param name="section">Section to look for parameters.</param>
		void LoadFrom(Section section);

		/// <summary>Create <see cref="IRepositoryAccessor"/> for specified <paramref name="repository"/>.</summary>
		/// <param name="repository">git repository to get accessor for.</param>
		/// <returns>git repository accessor.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		IRepositoryAccessor CreateRepositoryAccessor(IGitRepository repository);

		/// <summary>Checks if specified path is a valid repository.</summary>
		/// <param name="path">Path to check.</param>
		/// <returns><c>true</c> if specified path is a valid repository, <c>false</c> otherwise.</returns>
		bool IsValidRepository(string path);

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
